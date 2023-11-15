using System;
using System.IO;

namespace TailwindDotnet.Hosting
{
    public class TailwindOptions
    {
        public string InputCssFile { get; set; } = "input.css";

        public string OutputCssFile { get; set; } = "output.css";

        public string ConfigFile { get; set; } = "tailwind.config.js";

        public bool IsWatchEnabled { get; set; } = false;

        public string WorkingDirectory { get; set; } = string.Empty;

        public string ExecutablePath { get; set; } =
            Path.Combine(AppContext.BaseDirectory, "tailwindcss");
    }
}