using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(TailwindDotnet.Hosting.SpaHostingStartup))]

namespace TailwindDotnet.Hosting;

internal sealed class SpaHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        Console.WriteLine("Starting...");

        builder.ConfigureServices(services =>
        {
            Console.WriteLine("Configure...");
        });
    }
}
