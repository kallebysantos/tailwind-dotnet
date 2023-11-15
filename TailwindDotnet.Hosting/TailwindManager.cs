using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TailwindDotnet.Hosting
{
    public sealed class TailwindManager : IDisposable
    {
        private Process? _tailwindProcess;

        private bool _disposedValue;

        private readonly TailwindOptions _options;

        private readonly ILogger<TailwindManager> _logger;

        public TailwindManager(IOptions<TailwindOptions> options, ILogger<TailwindManager> logger)
        {
            _options = options.Value;
            _logger = logger;
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

                #if NET7_0_OR_GREATER
                Architecture.Armv6 => "armv7",
                #endif
                
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

        public void LaunchTailwindProcess()
        {
            var executableFilename = _options.ExecutablePath;
            var commandArguments = new string[]
            {
                $"--input {_options.InputCssFile}",
                $"--output {_options.OutputCssFile}",
                $"--config {_options.ConfigFile}",
                _options.IsWatchEnabled ? "--watch" : string.Empty,
            };

            var arguments = string.Join(' ', commandArguments);

            try
            {
                var info = new ProcessStartInfo(executableFilename, arguments)
                {
                    // Linux and Mac OS don't have the concept of launching a terminal process in a new window. On those cases the process will be launched in the same terminal window and will just print some output during the start phase of the app.
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, _options.WorkingDirectory)
                };

                _tailwindProcess = Process.Start(info);

                if (_tailwindProcess is null)
                {
                    throw new Exception("");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    exception: ex,
                    message: $"Failed to launch the Tailwind process tailwindcss '{arguments}'."
                );
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
                try
                {
                    if (_tailwindProcess != null && !_tailwindProcess.HasExited)
                    {
                        // Review: Whether or not to do this at all. Turns out that if we try to kill the
                        // tailwind process that we start, even with this option we only stop this process
                        // and the service keeps running.
                        // Compared to performing Ctrl+C on the window or closing the window for the newly spawned
                        // process which seems to do the right thing.
                        if (!_tailwindProcess.CloseMainWindow())
                        {   
                            #if NET6_0_OR_GREATER
                            _tailwindProcess.Kill(entireProcessTree: true);
                            #else
                            _tailwindProcess.Kill();
                            #endif

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
}