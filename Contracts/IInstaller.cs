using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AAG.Global.Contracts
{
    public interface IInstaller
    {
        /// <summary>
        /// Install services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}