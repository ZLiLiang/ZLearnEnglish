using Microsoft.Extensions.DependencyInjection;
using Z.Commons;

namespace Z.MediaEncoder.Domain
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<MediaEncoderFactory>();
        }
    }
}
