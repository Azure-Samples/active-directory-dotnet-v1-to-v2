
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonCacheADAL
{
    class Program
    {
        static void Main(string[] args)
        {
            //ADAL v5 
             Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Running: " + Assembly.GetEntryAssembly().GetName());
            Console.ResetColor();
            DoIt().Wait();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Hit any key to continue ...");
            Console.ResetColor();
            Console.ReadLine();
        }

        static async Task DoIt()
        {
            AppCoordinates.AppCoordinates app = AppCoordinates.PreRegisteredApps.GetV1App(useInMsal: false);
            string resource = AppCoordinates.PreRegisteredApps.MsGraph;

            string cacheFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..");
            FilesBasedTokenCache tokenCache = new FilesBasedTokenCache(cacheFolder);

            ShowCacheContent(tokenCache);

            AuthenticationContext authenticationContext = new AuthenticationContext(app.Authority, tokenCache);

            AuthenticationResult result;
            try
            {
                result = await authenticationContext.AcquireTokenSilentAsync(resource, app.ClientId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"got token for '{result.UserInfo.DisplayableId}' from the cache");
                Console.ResetColor();
            }
            catch (AdalSilentTokenAcquisitionException)
            {
                result = await authenticationContext.AcquireTokenAsync(resource, app.ClientId, app.RedirectUri, new PlatformParameters(PromptBehavior.SelectAccount));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"got token for '{result.UserInfo.DisplayableId}' using Interactive Auth");
                Console.ResetColor();
            }

        }

        private static void ShowCacheContent(FilesBasedTokenCache tokenCache)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("TokenCache Content");

            var tokenCacheItems = tokenCache.ReadItems();
            foreach (TokenCacheItem tokenCacheItem in tokenCacheItems)
            {
                Console.WriteLine("A: {0}, C: {1}, D: {2}, F: {3}, G: {4}, IdP: {5}, R: {6}, T: {7}, Uid: {8}",
                    tokenCacheItem.Authority,
                    tokenCacheItem.ClientId,
                    tokenCacheItem.DisplayableId,
                    tokenCacheItem.FamilyName,
                    tokenCacheItem.GivenName,
                    tokenCacheItem.IdentityProvider,
                    tokenCacheItem.Resource,
                    tokenCacheItem.TenantId,
                    tokenCacheItem.UniqueId);
            }
            Console.ResetColor();
        }
    }
}
