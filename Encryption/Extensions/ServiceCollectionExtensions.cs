using Microsoft.Extensions.DependencyInjection;

namespace N17Solutions.Semaphore.Encryption.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEncryption(this IServiceCollection services)
        {
            return services.AddSingleton<DataEncrypter>();
        } 
    }
}