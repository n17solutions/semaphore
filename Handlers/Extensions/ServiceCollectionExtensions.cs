using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace N17Solutions.Semaphore.Handlers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            return services.AddMediatR();
        }
    }
}