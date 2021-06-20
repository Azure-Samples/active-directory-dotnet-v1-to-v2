using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using System;

namespace ConfidentialClientTokenCache
{
    /// <summary>
    /// Extension methods to bring a simplified developer experience for
    /// Adding token caches in ASP.NET, or .NET Core, or .NET FW
    /// </summary>
    static class TokenCacheConfidentialClientApplicationExtension
    {
        /// <summary>
        /// Use a token cache of you choice
        /// </summary>
        /// <returns>The confidential client application</returns>
        /// <param name="initializeCache">Action that you'll use to add a cache serialization
        /// to the service collection passed as an argument</param>
        /// <returns>The application for chaining</returns>
        /// <example>
        /// 
        /// The following code adds a distributed in-memory token cache.
        /// 
        /// <code>
        ///  app.UseTokenCaches(services =>
        ///  {
        ///      // In memory distributed token cache
        ///      // In net472, requires to reference Microsoft.Extensions.Caching.Memory
        ///      services.AddDistributedTokenCaches();
        ///      services.AddDistributedMemoryCache();
        ///  });
        /// </code>
        /// 
        /// 
        /// </example>
        public static IConfidentialClientApplication UseTokenCaches(
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

        /// <summary>
        /// Add an in-memory token cache.
        /// In net462 and net472, you'll need to reference Microsoft.Extensions.Caching.Memory
        /// </summary>
        /// <param name="app">Application</param>
        /// <returns>The application for chaining</returns>
        public static IConfidentialClientApplication UseInMemoryTokenCaches(
         this IConfidentialClientApplication app)
        {
            // In memory token cache
            // In net472, requires to reference Microsoft.Extensions.Caching.Memory
            app.UseTokenCaches(services => 
            { 
                services.AddInMemoryTokenCaches();
            });
            return app;
        }

        /// <summary>
        /// Add a distributed token cache.
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="initializeDistributedCache">Action taking a <see cref="IServiceCollection"/>
        /// and by which you initialize your distributed cache</param>
        /// <returns>The application for chaining</returns>
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

            app.UseTokenCaches(services =>
            {
                services.AddDistributedTokenCaches();
                initializeDistributedCache(services);
            });
            return app;
        }


    }
}
