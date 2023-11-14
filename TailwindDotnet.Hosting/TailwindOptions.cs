namespace TailwindDotnet.Hosting;

public class TailwindOptions
{
    public string InputCssFile { get; set; } = "input.css";

    public string OutputCssFile { get; set; } = "output.css";

    public bool IsWatchEnabled { get; set; } = false;

    public bool IsMinifyEnabled { get; set; } = false;

    public string WorkingDirectory { get; set; } = string.Empty;

    public string ExecutablePath { get; set; } =
        Path.Combine(AppContext.BaseDirectory, "tailwindcss");

    public string Version { get; set; } = "latest";

    public string BaseDownloadUrl { get; set; } =
        "https://github.com/tailwindlabs/tailwindcss/releases";
}
