using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonCacheMsalV3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            DoIt().Wait();
        }

        static async Task DoIt()
        {
            AppCoordinates.AppCoordinates v1App = AppCoordinates.PreRegisteredApps.GetV1App(useInMsal: true);
            string resource = AppCoordinates.PreRegisteredApps.MsGraph;
            string[] scopes = new string[] { resource + "/user.read" };

            string cacheFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..");
            string adalV3cacheFileName = Path.Combine(cacheFolder, "cacheAdalV3.bin");
            string unifiedCacheFileName = Path.Combine(cacheFolder, "cacheMsal.bin");

            AuthenticationResult result;

            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create(v1App.ClientId)
                                                .WithAuthority(v1App.Authority)
                                                .Build();
            FilesBasedTokenCacheHelper.EnableSerialization(app.UserTokenCache,
                                                           unifiedCacheFileName,
                                                           adalV3cacheFileName);
            var accounts = await app.GetAccountsAsync();
            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"got token for '{result.Account.Username}' from the cache");
                Console.ResetColor();
            }
            catch (MsalUiRequiredException ex)
            {
                result = await app.AcquireTokenInteractive(scopes)
                                  .ExecuteAsync();
                Console.WriteLine($"got token for '{result.Account.Username}' without the cache");
            }
        }
    }
}
