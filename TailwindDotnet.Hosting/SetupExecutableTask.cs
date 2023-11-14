using Microsoft.Build.Framework;
using TailwindDotnet.Core;

namespace TailwindDotnet.Hosting;

public class SetupExecutableTask : Microsoft.Build.Utilities.Task
{
    [Required]
    public required string TailwindExecutableFolder { get; set; }

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

        if (Path.Exists(TailwindExecutablePath))
        {
            Log.LogMessage(
                importance: MessageImportance.High,
                message: $"Found local tailwindcss executable at {TailwindExecutablePath}"
            );

            return true;
        }

        var downloadTask = TailwindManager.Download(
            tailwindExecutableUrl,
            TailwindExecutablePath
        );

        Log.LogMessage(
            importance: MessageImportance.High,
            message: $"Getting Tailwindcss from {tailwindExecutableUrl}"
        );

        downloadTask.Wait();

        Log.LogMessage(
            importance: MessageImportance.High,
            message: $"Saving Tailwindcss to {TailwindExecutablePath}"
        );

        if (downloadTask.IsFaulted)
        {
            Log.LogErrorFromException(downloadTask.Exception);
        }

        TailwindManager.AddExecutablePermissions(TailwindExecutablePath);

        return downloadTask.Result is not null;
    }
}
