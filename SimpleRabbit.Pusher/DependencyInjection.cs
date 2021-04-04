using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleRabbit.Publisher
{
    public static class DependencyInjection
    {
        private const string DefaultSection = "rabbitMq";

        public static IServiceCollection AddSimplePusher(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = DefaultSection)
        {
            return services.AddScoped<IPublishService, PublishService>(q => new PublishService(configuration, sectionName));
        }
    }
}
