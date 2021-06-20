using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using System;

namespace ConfidentialClientTokenCache
{
    static class TokenCacheConfidentialClientApplicationExtension
    {
        // In memory token cache
        // In net472, requires to reference Microsoft.Extensions.Caching.Memory
        public static IConfidentialClientApplication UseInMemoryTokenCache(
         this IConfidentialClientApplication app,
         MsalMemoryTokenCacheOptions options = null)
        {
            // In memory token cache
            // In net472, requires to reference Microsoft.Extensions.Caching.Memory
            app.UseTokenCache(services => 
            { 
                services.AddInMemoryTokenCaches();
            });
            return app;
        }

        public static IConfidentialClientApplication UseDistributedTokenCache(
         this IConfidentialClientApplication app,
         Action<IServiceCollection> initializeDistributedCache)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (initializeDistributedCache is null)
            {
                throw new ArgumentNullException(nameof(initializeDistributedCache));
            }

            app.UseTokenCache(services =>
            {
                services.AddDistributedTokenCaches();
                initializeDistributedCache(services);
            });
            return app;
        }

        public static IConfidentialClientApplication UseTokenCache(
         this IConfidentialClientApplication app,
         Action<IServiceCollection> initializeCache)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
              .ConfigureLogging(l => { })
              .ConfigureServices(services =>
              {
                  initializeCache(services);
              });

            IServiceProvider serviceProvider = hostBuilder.Build().Services;
            IMsalTokenCacheProvider msalTokenCacheProvider = serviceProvider.GetRequiredService<IMsalTokenCacheProvider>();
            msalTokenCacheProvider.Initialize(app.UserTokenCache);
            msalTokenCacheProvider.Initialize(app.AppTokenCache);
            return app;
        }
    }
}
