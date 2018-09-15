using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using N17Solutions.Semaphore.Data.Context;

namespace N17Solutions.Semaphore.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<SemaphoreContext>(options => options.UseNpgsql(connectionString));
        }
    }
}