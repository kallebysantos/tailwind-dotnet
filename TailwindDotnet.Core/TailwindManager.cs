using System.Runtime.InteropServices;

namespace TailwindDotnet.Core;

public class TailwindManager
{
    private const string TAILWIND_EXECUTABLE_URL =
        "https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss";

    public static async Task<string?> Download()
    {
        string? tailwindExecutableUrl =
            GetTailwindExecutableUrl()
            ?? throw new PlatformNotSupportedException(
                message: "Tailwind CSS CLI is not available for your operating system."
            );

        var (_, ext) = GetTailwindExecutablePlatform();

        var executableFilePath = Path.Combine(AppContext.BaseDirectory, "tailwindcss");
        var executableFilename = executableFilePath + ext;

        using var httpClient = new HttpClient();

        Console.WriteLine($"Downloading tailwindcss: {tailwindExecutableUrl}");

        var response = (await httpClient.GetAsync(tailwindExecutableUrl)).EnsureSuccessStatusCode();

        using var fileStream = File.Create(path: $"{executableFilename}");
        await response.Content.CopyToAsync(fileStream);

        Console.WriteLine($"Download finished: {executableFilename}");

        return executableFilename;
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

    static string? GetTailwindExecutableUrl()
    {
        var (platform, ext) = GetTailwindExecutablePlatform();
        var arch = GetTailwindExecutableArchitecture();

        if (platform is null || arch is null)
        {
            return null;
        }

        return $"{TAILWIND_EXECUTABLE_URL}-{platform}-{arch}{ext}";
    }
}
