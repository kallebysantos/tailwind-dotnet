using Microsoft.Build.Framework;
using TailwindDotnet.Core;

namespace TailwindDotnet.Hosting.Build.Tasks;

public class SetupExecutableTask : Microsoft.Build.Utilities.Task
{
    public string Version { get; set; } = "latest";

    public string BaseDownloadUrl { get; set; } =
    "https://github.com/tailwindlabs/tailwindcss/releases";

    public override bool Execute()
    {

        var tailwindExecutableUrl =
            TailwindManager.GetTailwindExecutableUrl(
                baseUrl: BaseDownloadUrl,
                version: Version
            )
            ?? throw new PlatformNotSupportedException(
                message: "Tailwindcss CLI is not available for your operating system."
            );

        Log.LogMessage(
            importance: MessageImportance.High,
            message: $"Downloading Tailwindcss from {tailwindExecutableUrl}"
        );

        var executableFilename = TailwindManager.Download(tailwindExecutableUrl).Result;

        Log.LogMessage(
            importance: MessageImportance.High,
            message: $"tailwindcss executable saved at {executableFilename}"
        );

        return executableFilename is not null;
    }
}
