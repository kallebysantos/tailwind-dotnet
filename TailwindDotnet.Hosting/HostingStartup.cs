using Microsoft.AspNetCore.Hosting;
using TailwindDotnet.Core;

[assembly: HostingStartup(typeof(TailwindDotnet.Hosting.SpaHostingStartup))]

namespace TailwindDotnet.Hosting;

internal sealed class SpaHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        Console.WriteLine("Downloading tailwind...");

        using var twManager = new TailwindManager(
            options: new TailwindOptions
            {
                IsWatchEnabled = true,
                WorkingDirectory = "./wwwroot/input.css"
            }
        );

        _ = twManager.LaunchTailwindProcess();

        // var executableFilename =
        //     TailwindManager.Download().Result ?? throw new Exception("Could not download tailwind");

        // TailwindManager.AddExecutablePermissions(executableFilename);

        builder.ConfigureServices(services =>
        {
            Console.WriteLine("Configure...");
        });
    }
}
