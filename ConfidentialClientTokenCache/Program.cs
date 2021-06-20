// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfidentialClientTokenCache
{
    /// <summary>
    /// Choice of token cache serialization implementation
    /// (this is for the sample)
    /// </summary>
    public enum CacheImplementationDemo
    {
        InMemory,
        DistributedMemory,
        DistributedSqlServer,
        StackExchangeRedis,
        CosmosDb
    }

    /// <summary>
    /// The goal of this little sample is to show how you can use MSAL token cache adapters
    /// in confidential client applications both in .NET Core, or .NET Framework 4.7.2
    /// Note that if you write a .NET Core web app or web API, Microsoft recommends that you use the
    /// IDownstreamApi or ITokenAcquisition interfaces directly (instead of MSAL)
    /// </summary>
    static class Program
    {
        static async Task Main(string[] args)
        {
            string clientId = "6af093f3-b445-4b7a-beae-046864468ad6";
            string tenant = "msidentitysamplestesting.onmicrosoft.com";
            string[] scopes = new[] { "api://8206b76f-586e-4098-b1e5-598c1aa3e2a1/.default" };

            // Certificate
            string keyVaultContainer = "https://WebAppsApisTests.vault.azure.net";
            string keyVaultReference = "MsIdWebScenarioTestCert";
            CertificateDescription certDescription = CertificateDescription.FromKeyVault(keyVaultContainer, keyVaultReference);
            ICertificateLoader certificateLoader = new DefaultCertificateLoader();
            certificateLoader.LoadIfNeeded(certDescription);

            // Create the confidential client application
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(clientId)
                // Alternatively to the certificate you can use .WithClientSecret(clientSecret)
                .WithCertificate(certDescription.Certificate)
                .WithTenantId(tenant)
                .Build();


            app.UseTokenCache(services =>
            {
                ConfigureCache(CacheImplementationDemo.InMemory, services);
            });
 
            // Acquire a token (twice)
            var result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
            Console.WriteLine(result.AuthenticationResultMetadata.TokenSource);

            result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
            Console.WriteLine(result.AuthenticationResultMetadata.TokenSource);
        }

        /// <summary>
        /// Creates a token cache (implementation of your choice)
        /// </summary>
        /// <param name="cacheImplementation">implementation for the token cache</param>
        /// <returns>An MSAL Token cache provider</returns>
        private static void ConfigureCache(
            CacheImplementationDemo cacheImplementation,
            IServiceCollection services)
        {
            // (Simulates the configuration, could be a IConfiguration or anything)
            Dictionary<string, string> Configuration = new Dictionary<string, string>();

            switch (cacheImplementation)
            {
                case CacheImplementationDemo.InMemory:
                    // In memory token cache
                    // In net472, requires to reference Microsoft.Extensions.Caching.Memory
                    services.AddInMemoryTokenCaches();
                    break;

                case CacheImplementationDemo.DistributedMemory:
                    // In memory distributed token cache
                    // In net472, requires to reference Microsoft.Extensions.Caching.Memory
                    services.AddDistributedTokenCaches();
                    services.AddDistributedMemoryCache();
                    break;

                case CacheImplementationDemo.DistributedSqlServer:
                    // SQL Server token cache
                    // Requires to reference Microsoft.Extensions.Caching.SqlServer
                    services.AddDistributedTokenCaches();
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestCache;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                        options.SchemaName = "dbo";
                        options.TableName = "TestCache";

                        // You don't want the SQL token cache to be purged before the access token has expired. Usually
                        // access tokens expire after 1 hour (but this can be changed by token lifetime policies), whereas
                        // the default sliding expiration for the distributed SQL database is 20 mins. 
                        // Use a value which is above 60 mins (or the lifetime of a token in case of longer lived tokens)
                        options.DefaultSlidingExpiration = TimeSpan.FromMinutes(90);
                    });
                    break;

                case CacheImplementationDemo.StackExchangeRedis:
                    // Redis token cache
                    // Requires to reference Microsoft.Extensions.Caching.StackExchangeRedis
                    services.AddDistributedTokenCaches();
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = "localhost";
                        options.InstanceName = "Redis";
                    });
                    break;

                case CacheImplementationDemo.CosmosDb:
                    // Redis token cache
                    // Requires to reference Microsoft.Extensions.Caching.Cosmos (preview)
                    services.AddDistributedTokenCaches();
                    services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
                    {
                        cacheOptions.ContainerName = Configuration["CosmosCacheContainer"];
                        cacheOptions.DatabaseName = Configuration["CosmosCacheDatabase"];
                        cacheOptions.ClientBuilder = new CosmosClientBuilder(Configuration["CosmosConnectionString"]);
                        cacheOptions.CreateIfNotExists = true;
                    });
                    break;

                default:
                    break;
            }
        }
    }
}
