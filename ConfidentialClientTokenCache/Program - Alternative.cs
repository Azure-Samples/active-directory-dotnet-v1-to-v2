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
    //public enum CacheImplementationDemo
    //{
    //    InMemory,
    //    DistributedMemory,
    //    DistributedSqlServer,
    //    StackExchangeRedis,
    //    CosmosDb
    //}

    /// <summary>
    /// The goal of this little sample is to show how you can use MSAL token cache adapters
    /// in confidential client applications both in .NET Core, or .NET Framework 4.7.2
    /// Note that if you write a .NET Core web app or web API, Microsoft recommends that you use the
    /// IDownstreamApi or ITokenAcquisition interfaces directly (instead of MSAL)
    /// </summary>
    static class Program2
    {
        static async Task Main2(string[] args)
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

            app.UseInMemoryTokenCaches();

            // Or

            app.UseDistributedTokenCache(services =>
            {
                // In memory distributed token cache
                // In net472, requires to reference Microsoft.Extensions.Caching.Memory
                services.AddDistributedMemoryCache();
            });

            // Or

            app.UseTokenCaches(services =>
            {
                // In memory distributed token cache
                // In net472, requires to reference Microsoft.Extensions.Caching.Memory
                services.AddDistributedTokenCaches();
                services.AddDistributedMemoryCache();
            });

            // Or

            app.UseDistributedTokenCache(services =>
            {
                services.AddDistributedSqlServerCache(options =>
                {
                    // SQL Server token cache
                    // Requires to reference Microsoft.Extensions.Caching.SqlServer
                    options.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestCache;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    options.SchemaName = "dbo";
                    options.TableName = "TestCache";

                    // You don't want the SQL token cache to be purged before the access token has expired. Usually
                    // access tokens expire after 1 hour (but this can be changed by token lifetime policies), whereas
                    // the default sliding expiration for the distributed SQL database is 20 mins. 
                    // Use a value which is above 60 mins (or the lifetime of a token in case of longer lived tokens)
                    options.DefaultSlidingExpiration = TimeSpan.FromMinutes(90);
                });
            });

            // Or

            app.UseTokenCaches(services =>
            {
                services.AddDistributedTokenCaches();
                services.AddDistributedSqlServerCache(options =>
                {
                    // In net472, requires to reference Microsoft.Extensions.Caching.Memory
                    // SQL Server token cache
                    // Requires to reference Microsoft.Extensions.Caching.SqlServer
                    options.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestCache;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    options.SchemaName = "dbo";
                    options.TableName = "TestCache";

                    // You don't want the SQL token cache to be purged before the access token has expired. Usually
                    // access tokens expire after 1 hour (but this can be changed by token lifetime policies), whereas
                    // the default sliding expiration for the distributed SQL database is 20 mins. 
                    // Use a value which is above 60 mins (or the lifetime of a token in case of longer lived tokens)
                    options.DefaultSlidingExpiration = TimeSpan.FromMinutes(90);
                });
            });

            // Or 

            app.UseTokenCaches(services =>
            {
                services.AddDistributedTokenCaches();
                // Redis token cache
                // Requires to reference Microsoft.Extensions.Caching.StackExchangeRedis
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = "localhost";
                    options.InstanceName = "Redis";
                });
            });

            // Or

            app.UseTokenCaches(services =>
            {
                services.AddDistributedTokenCaches();

                // (Simulates the configuration, could be a IConfiguration or anything)
                Dictionary<string, string> Configuration = new Dictionary<string, string>();

                // Redis token cache
                // Requires to reference Microsoft.Extensions.Caching.Cosmos (preview)
                services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
                {
                    cacheOptions.ContainerName = Configuration["CosmosCacheContainer"];
                    cacheOptions.DatabaseName = Configuration["CosmosCacheDatabase"];
                    cacheOptions.ClientBuilder = new CosmosClientBuilder(Configuration["CosmosConnectionString"]);
                    cacheOptions.CreateIfNotExists = true;
                });
            });

            // Acquire a token (twice)
            var result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
            Console.WriteLine(result.AuthenticationResultMetadata.TokenSource);

            result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
            Console.WriteLine(result.AuthenticationResultMetadata.TokenSource);
        }
    }
}
