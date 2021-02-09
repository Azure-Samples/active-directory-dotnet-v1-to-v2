// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders;
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
            string clientId = "812287fd-3ea3-49c6-b4ab-e8d41dea1f17";
            string clientSecret = "[Enter here the secret register with your application]";
            string tenant = "msidentitysamplestesting.onmicrosoft.com";
            string[] scopes = new[] { "api://2d96f90e-a1a7-4ef5-b15c-87758986eb1a/.default" };
            string keyVaultContainer = "https://buildautomation.vault.azure.net";
            string keyVaultReference = "AzureADIdentityDivisionTestAgentCert";

            CertificateDescription certDescription = CertificateDescription.FromKeyVault(keyVaultContainer, keyVaultReference);

            CacheImplementationDemo cacheImplementation = CacheImplementationDemo.InMemory;

            // Create the token cache (4 possible implementations)
            IMsalTokenCacheProvider msalTokenCacheProvider = CreateTokenCache(cacheImplementation);

            // Create the confidential client application
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(clientId)
                //.WithClientSecret(clientSecret)
                .WithCertificate(certDescription.Certificate)
                .WithTenantId(tenant)
                .Build();

            await msalTokenCacheProvider.InitializeAsync(app.UserTokenCache);
            await msalTokenCacheProvider.InitializeAsync(app.AppTokenCache);

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
        /// <returns>An Msal Token cache provider</returns>
        private static IMsalTokenCacheProvider CreateTokenCache(CacheImplementationDemo cacheImplementation=CacheImplementationDemo.InMemory)
        {
            IServiceCollection services = new ServiceCollection();

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
                        options.InstanceName = "SampleInstance";
                    });
                    break;

                case CacheImplementationDemo.CosmosDb:
                    // Redis token cache
                    // Requires to reference Microsoft.Extensions.Caching.Cosmos (preview)
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

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IMsalTokenCacheProvider msalTokenCacheProvider = serviceProvider.GetRequiredService<IMsalTokenCacheProvider>();
            return msalTokenCacheProvider;
        }
    }
}
