using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonCacheMsal2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            DoIt().Wait();
            Console.ReadLine();
        }

        static async Task DoIt()
        {
            AppCoordinates.AppCoordinates v1App = AppCoordinates.PreRegisteredApps.GetV1App(useInMsal: true);
            string resource = AppCoordinates.PreRegisteredApps.MsGraph;
            string[] scopes = new string[] { resource + "/user.read" };

            string cacheFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..");
            string adalV3cacheFileName = Path.Combine(cacheFolder, "cacheAdalV3.bin");
            string unifiedCacheFileName = Path.Combine(cacheFolder, "unifiedCache.bin");
            TokenCache tokenCache = FilesBasedTokenCacheHelper.GetUserCache(unifiedCacheFileName, adalV3cacheFileName);

            AuthenticationResult result;
            PublicClientApplication app = new PublicClientApplication(v1App.ClientId, v1App.Authority, tokenCache);
            var accounts = await app.GetAccountsAsync();
            try
            {
                result = await app.AcquireTokenSilentAsync(scopes, accounts.FirstOrDefault(), app.Authority, false);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"got token for '{result.Account.Username}' from the cache");
                Console.ResetColor();
            }
            catch (MsalUiRequiredException ex)
            {
                result = await app.AcquireTokenAsync(scopes);
                Console.WriteLine($"got token for '{result.Account.Username}' without the cache");
            }
        }
        }
}
