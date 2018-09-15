using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace N17Solutions.Semaphore.Requests.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequests(this IServiceCollection services)
        {
            return services.AddMediatR();
        } 
    }
}