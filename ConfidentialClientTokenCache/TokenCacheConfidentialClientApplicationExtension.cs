﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.TokenCacheProviders;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using System;

namespace ConfidentialClientTokenCache
{
    /// <summary>
    /// Extension methods to expose a simplified developer experience for
    /// Adding token caches to MSAL.NET confidential client applications
    /// in ASP.NET, or .NET Core, or .NET FW
    /// </summary>
    static class TokenCacheConfidentialClientApplicationExtension
    {
        /// <summary>
        /// Use a token cache and choose the serialization part by adding it to
        /// the services collection and configuring its options
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
        /// The following code adds a token cache based on REDIS and initializes
        /// its configuration.
        /// 
        /// <code>
        ///  app.UseTokenCaches(services =>
        ///  {
        ///       services.AddDistributedTokenCaches();
        ///       // Redis token cache
        ///       // Requires to reference Microsoft.Extensions.Caching.StackExchangeRedis
        ///       services.AddStackExchangeRedisCache(options =>
        ///       {
        ///           options.Configuration = "localhost";
        ///           options.InstanceName = "Redis";
        ///       });
        ///  });
        /// </code>
        /// 
        /// </example>
        /// <remarks>Don't use this method in ASP.NET Core. Just add use the ConfigureServices method
        /// instead.</remarks>
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
        /// Add an in-memory well partitionned token cache to MSAL.NET confidential client 
        /// application. Don't use this method in ASP.NET Core: rather use 
        /// <code>services.AddInMemoryTokenCaches()</code> in ConfigureServices.
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
