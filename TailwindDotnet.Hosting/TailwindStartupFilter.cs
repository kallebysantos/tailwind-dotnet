using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TailwindDotnet.Hosting
{
    internal sealed class TailwindStartupFilter : IStartupFilter
    {
        private readonly TailwindManager _tailwindManager;

        public TailwindStartupFilter(TailwindManager tailwindManager)
        {
            _tailwindManager = tailwindManager ?? throw new ArgumentNullException(nameof(tailwindManager));
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            _tailwindManager.LaunchTailwindProcess();

            return builder => next(builder);
        }
    }
}