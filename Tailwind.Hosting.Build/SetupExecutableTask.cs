using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Tailwind.Hosting.Cli;

namespace Tailwind.Hosting.Build;

public class SetupExecutableTask : Microsoft.Build.Utilities.Task, ITask
{
    private static readonly TimeSpan MutexTimeout = TimeSpan.FromMinutes(5);

    [Required]
    public string TailwindExecutableFolder { get; set; } = default!;

    [Output]
    public string TailwindExecutablePath { get; set; } = default!;

    public string? TailwindVersion { get; set; }

    public string TailwindExecutableDownloadUrl { get; set; } =
        "https://github.com/tailwindlabs/tailwindcss/releases";

    public override bool Execute()
    {
        if (TailwindVersion is null)
        {
            Log.LogError("Tailwind version not provided");
            return false;
        }

        var tailwindExecutableUrl =
            TailwindManager.GetTailwindExecutableUrl(
                baseUrl: TailwindExecutableDownloadUrl,
                version: TailwindVersion
            )
            ?? throw new PlatformNotSupportedException(
                message: "Tailwindcss CLI is not available for your operating system."
            );

        var tailwindExecutableFilename = tailwindExecutableUrl.Split('/').Last();

        TailwindExecutablePath = Path.Combine(TailwindExecutableFolder, tailwindExecutableFilename);

        if (File.Exists(TailwindExecutablePath))
        {
            Log.LogMessage(
                importance: MessageImportance.High,
                message: $"Found local tailwindcss executable at {TailwindExecutablePath}"
            );

            return true;
        }

        var mutexName = CreateMutexName(TailwindExecutablePath);
        using var mutex = new Mutex(false, mutexName, out var createdNew);

        if (!createdNew)
        {
            Log.LogMessage(
                importance: MessageImportance.High,
                message: "Another process is downloading the Tailwind CLI executable. Waiting..."
            );
        }

        bool acquired;
        try
        {
            acquired = mutex.WaitOne(MutexTimeout);
        }
        catch (AbandonedMutexException)
        {
            // Previous owner crashed — we now own the mutex, proceed normally
            acquired = true;
        }

        if (!acquired)
        {
            Log.LogError(
                "Timed out waiting for another process to finish downloading the Tailwind CLI executable."
            );
            return false;
        }

        if (!createdNew)
        {
            Log.LogMessage(
                importance: MessageImportance.High,
                message: "Finished waiting. Resuming Tailwind CLI setup."
            );
        }

        var tempFilePath = TailwindExecutablePath + ".downloading";

        try
        {
            // Double-check: another process may have completed the download while we waited
            if (File.Exists(TailwindExecutablePath))
            {
                Log.LogMessage(
                    importance: MessageImportance.High,
                    message: $"Found local tailwindcss executable at {TailwindExecutablePath}"
                );

                return true;
            }

            Log.LogMessage(
                importance: MessageImportance.High,
                message: $"Getting Tailwindcss from {tailwindExecutableUrl}"
            );

            var downloadTask = TailwindManager.Download(tailwindExecutableUrl, tempFilePath);
            downloadTask.Wait();

            if (downloadTask.IsFaulted)
            {
                Log.LogErrorFromException(downloadTask.Exception);
                return false;
            }

            if (downloadTask.Result == null)
            {
                Log.LogError("Tailwind CLI download returned no result");
                return false;
            }

            File.Move(tempFilePath, TailwindExecutablePath);

            Log.LogMessage(
                importance: MessageImportance.High,
                message: $"Saved Tailwindcss to {TailwindExecutablePath}"
            );

            TailwindManager.AddExecutablePermissions(TailwindExecutablePath);

            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex);
            return false;
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private static string CreateMutexName(string executablePath)
    {
        var normalizedPath = Path.GetFullPath(executablePath).ToUpperInvariant();
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(normalizedPath));
        var hashString = BitConverter.ToString(hash).Replace("-", "");
        return $"Global\\TailwindCli_{hashString}";
    }
}
