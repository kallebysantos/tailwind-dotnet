using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(TailwindDotnet.Hosting.TailwindHostingStartup))]

namespace TailwindDotnet.Hosting;

internal sealed class TailwindHostingStartup : IHostingStartup
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Configuration object's public properties are preserved.")]
    static void ConfigureOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(IServiceCollection services, IConfigurationSection section)
     where T : class
    {
        services.Configure<T>(section);
    }

    public void Configure(IWebHostBuilder builder)
    {
        var tailwindPropsFile = Path.Combine(AppContext.BaseDirectory, "tailwind.props.json");
        if (!File.Exists(tailwindPropsFile))
        {
            return;
        }


        builder.ConfigureServices(services =>
            {
                Console.WriteLine(tailwindPropsFile);
                var configuration = new ConfigurationBuilder().AddJsonFile(tailwindPropsFile).Build();

                ConfigureOptions<TailwindOptions>(services, configuration.GetSection("TailwindProps"));

                services.AddSingleton<TailwindManager>();
                services.AddSingleton<IStartupFilter, TailwindStartupFilter>();
            }
        );
    }
}
