using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using N17Solutions.Semaphore.Data.Context;

namespace N17Solutions.Semaphore.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString)
        {
            Migrate(connectionString);
            return services.AddDbContext<SemaphoreContext>(options => options.UseNpgsql(connectionString));
        }

        private static void Migrate(string connectionString)
        {
            var currentAssemblyName = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;

            try
            {
                var migrationOptions = new DbContextOptionsBuilder<SemaphoreContext>().UseNpgsql(
                        connectionString,
                        db => { db.MigrationsAssembly(currentAssemblyName); })
                    .Options;

                var migrationContext = new SemaphoreContext(migrationOptions);
                var pendingMigrations = migrationContext.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                    migrationContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred migrating the database.", ex);
            }
        }
    }
}