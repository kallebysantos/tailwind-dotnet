using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TailwindDotnet.Hosting;

public sealed class TailwindManager : IDisposable
{
    private Process? _tailwindProcess;
    private bool _disposedValue;

    private const string TAILWIND_EXECUTABLE_URL =
        "https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss";
    private readonly TailwindOptions _options;

    public TailwindManager(TailwindOptions options)
    {
        _options = options;
    }

    public static async Task<string?> Download(string tailwindExecutableUrl, string tailwindExecutablePath)
    {
        using var httpClient = new HttpClient();

        var response = (await httpClient.GetAsync(tailwindExecutableUrl)).EnsureSuccessStatusCode();

        using var fileStream = File.Create(path: tailwindExecutablePath);
        await response.Content.CopyToAsync(fileStream);

        return tailwindExecutablePath;
    }

    // Make the downloaded executable file executable (Linux/macOS).
    public static void AddExecutablePermissions(string executableFilename)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"+x {executableFilename}",
            }
        };

        process.Start();
        process.WaitForExit();
    }

    static string? GetTailwindExecutableArchitecture() =>
        RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.Arm => "armv7",
            Architecture.Arm64 => "arm64",
            Architecture.Armv6 => "armv7",
            _ => default
        };

    static (string, string) GetTailwindExecutablePlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return ("linux", string.Empty);
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return ("macos", string.Empty);
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ("windows", ".exe");
        }

        return default;
    }

    public static string? GetTailwindExecutableUrl(string baseUrl, string version)
    {
        var (platform, ext) = GetTailwindExecutablePlatform();
        var arch = GetTailwindExecutableArchitecture();

        if (platform is null || arch is null)
        {
            return null;
        }

        if (version == "latest")
        {
            baseUrl += "/latest/download";
        }
        else
        {
            baseUrl += $"/download/{version}";
        }

        return $"{baseUrl}/tailwindcss-{platform}-{arch}{ext}";
    }

    public async Task LaunchTailwindProcess()
    {
        try
        {
            var (_, ext) = GetTailwindExecutablePlatform();
            var executableFilename = _options.ExecutablePath + ext;
            var commandArguments = new string[]
            {
                $"--input {_options.InputCssFile}",
                $"--output {_options.OutputCssFile}",
                _options.IsWatchEnabled ? "--watch" : string.Empty,
                _options.IsWatchEnabled ? "--minify" : string.Empty,
            };

            var arguments = string.Join(' ', commandArguments);

            var info = new ProcessStartInfo(executableFilename, arguments)
            {
                // Linux and Mac OS don't have the concept of launching a terminal process in a new window. On those cases the process will be launched in the same terminal window and will just print some output during the start phase of the app.
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.Combine(AppContext.BaseDirectory, _options.WorkingDirectory)
            };

            _tailwindProcess = Process.Start(info);

            if (_tailwindProcess is null)
            {
                throw new Exception("");
            }

            while (!_tailwindProcess.StandardOutput.EndOfStream)
            {
                var line = await _tailwindProcess.StandardOutput.ReadLineAsync();
                Console.Write(line);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            // _logger.LogError(
            //     exception,
            //     $"Failed to launch the SPA development server '{_options.LaunchCommand}'."
            // );
        }
    }

    private void LaunchStopScriptWindows(int spaProcessId)
    {
        var stopScript =
            $@"do{{
  try
  {{
    $processId = Get-Process -PID {Environment.ProcessId} -ErrorAction Stop;
  }}catch
  {{
    $processId = $null;
  }}
  Start-Sleep -Seconds 1;
}}while($processId -ne $null);

try
{{
  taskkill /T /F /PID {spaProcessId};
}}
catch
{{
}}";
        var stopScriptInfo = new ProcessStartInfo(
            "powershell.exe",
            string.Join(" ", "-NoProfile", "-C", stopScript)
        )
        {
            CreateNoWindow = true,
            WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "")
        };

        var stopProcess = Process.Start(stopScriptInfo);
        if (stopProcess == null || stopProcess.HasExited)
        {
            // if (_logger.IsEnabled(LogLevel.Warning))
            // {
            //     _logger.LogWarning(
            //         $"The SPA process shutdown script '{stopProcess?.Id}' failed to start. The SPA proxy might"
            //             + $" remain open if the dotnet process is terminated ungracefully. Use the operating system commands to kill"
            //             + $" the process tree for {spaProcessId}"
            //     );
            // }
        }
        else
        {
            // if (_logger.IsEnabled(LogLevel.Debug))
            // {
            //     _logger.LogDebug($"Watch process '{stopProcess}' started.");
            // }
        }
    }

    private void LaunchStopScriptMacOS(int spaProcessId)
    {
        var fileName = Guid.NewGuid().ToString("N") + ".sh";
        var scriptPath = Path.Combine(AppContext.BaseDirectory, fileName);
        var stopScript =
            @$"function list_child_processes () {{
    local ppid=$1;
    local current_children=$(pgrep -P $ppid);
    local local_child;
    if [ $? -eq 0 ];
    then
        for current_child in $current_children
        do
          local_child=$current_child;
          list_child_processes $local_child;
          echo $local_child;
        done;
    else
      return 0;
    fi;
}}

ps {Environment.ProcessId};
while [ $? -eq 0 ];
do
  sleep 1;
  ps {Environment.ProcessId} > /dev/null;
done;

for child in $(list_child_processes {spaProcessId});
do
  echo killing $child;
  kill -s KILL $child;
done;
rm {scriptPath};
";
        File.WriteAllText(scriptPath, stopScript.ReplaceLineEndings());

        var stopScriptInfo = new ProcessStartInfo("/bin/bash", scriptPath)
        {
            CreateNoWindow = true,
            WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "")
        };

        var stopProcess = Process.Start(stopScriptInfo);
        if (stopProcess == null || stopProcess.HasExited)
        {
            // _logger.LogWarning(
            //     $"The SPA process shutdown script '{stopProcess?.Id}' failed to start. The SPA proxy might"
            //         + $" remain open if the dotnet process is terminated ungracefully. Use the operating system commands to kill"
            //         + $" the process tree for {spaProcessId}"
            // );
        }
    }

    public Task StopAsync()
    {
        Dispose(true);
        return Task.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Nothing to do here since ther are no managed resources
            }

            try
            {
                if (_tailwindProcess != null && !_tailwindProcess.HasExited)
                {
                    // Review: Whether or not to do this at all. Turns out that if we try to kill the
                    // npm.cmd/ps1 process that we start, even with this option we only stop this process
                    // and the service keeps running.
                    // Compared to performing Ctrl+C on the window or closing the window for the newly spawned
                    // process which seems to do the right thing.
                    // Process.CloseMainWindow seems to do the right thing in this situation and is doable since
                    // we now start a proxy every time.
                    // We can't guarantee that we stop/cleanup the proxy on every situation (for example if someone)
                    // kills this process in a "rude" way, but this gets 95% there.
                    // For cases where the proxy is left open and where there might not be a "visible" window the recommendation
                    // is to kill the process manually. (We will not fail, we will simply notify the proxy is "already" up.
                    if (!_tailwindProcess.CloseMainWindow())
                    {
                        _tailwindProcess.Kill(entireProcessTree: true);
                        _tailwindProcess = null;
                    }
                }
            }
            catch (Exception)
            {
                // Avoid throwing if we are running inside the finalizer.
                if (disposing)
                {
                    throw;
                }
            }

            _disposedValue = true;
        }
    }

    ~TailwindManager()
    {
        Dispose(disposing: false);
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
