using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tailwind.Hosting.Cli;

[assembly: HostingStartup(typeof(Tailwind.Hosting.TailwindHostingStartup))]

namespace Tailwind.Hosting
{
    internal sealed class TailwindHostingStartup : IHostingStartup
    {
#if NET6_0_OR_GREATER
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026",
            Justification = "Configuration object's public properties are preserved."
        )]
        static void ConfigureOptions<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T
        >(IServiceCollection services, IConfigurationSection section) where T : class
        {
            services.Configure<T>(section);
        }
#else
        static void ConfigureOptions<T>(IServiceCollection services, IConfigurationSection section)
            where T : class
        {
            services.Configure<T>(section);
        }
#endif

        public void Configure(IWebHostBuilder builder)
        {
            var tailwindPropsFile = Path.Combine(AppContext.BaseDirectory, "tailwind.props.json");
            if (!File.Exists(tailwindPropsFile))
            {
                return;
            }

            builder.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(tailwindPropsFile)
                    .Build();

                ConfigureOptions<TailwindOptions>(
                    services,
                    configuration.GetSection("TailwindProps")
                );

                services.AddSingleton<TailwindManager>();
                services.AddSingleton<IStartupFilter, TailwindStartupFilter>();
            });
        }
    }
}
