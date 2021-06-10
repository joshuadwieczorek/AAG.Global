using AAG.Global.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AAG.Global.ExtensionMethods
{
    public static class InstallServices
    {
        /// <summary>
        /// Install services from assembly.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void InstallServicesFromAssembly<T>(
              this IServiceCollection services
            , IConfiguration configuration)
        {
            List<IInstaller> installers = typeof(T).Assembly.ExportedTypes.Where(t => typeof(IInstaller).IsAssignableFrom(t)
                                                            && !t.IsInterface
                                                            && !t.IsAbstract)
                                                            .Select(Activator.CreateInstance)
                                                            .Cast<IInstaller>()
                                                            .ToList();

            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}