using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleRabbit.Cunsuming
{
    public static class DependencyInjection
    {
        private const string DefaultSection = "rabbitMq";

        public static IServiceCollection AddSimleConsuming(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultSection)
        {
            return services.AddScoped<IConsumeManager, ConsumeManager>(q => new ConsumeManager(configuration, sectionName));
        }
    }
}
