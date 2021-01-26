using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonCacheMsalV4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            DoIt(args.Contains("-disable-legacy-cache")).Wait();

            // Unless ran in a batch, let the user press return to continue
            if (args.Length == 0)
            {
                Console.ReadLine();
            }
            Console.WriteLine("--------------------------------------------------------------------------");
        }

        static async Task DoIt(bool disableLegacyCache)
        {
            AppCoordinates.AppCoordinates v1App = AppCoordinates.PreRegisteredApps.GetV1App(useInMsal: true);
            string resource = AppCoordinates.PreRegisteredApps.MsGraph;
            string[] scopes = new string[] { resource + "/user.read" };

            string cacheFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..");
            string adalV3cacheFileName = Path.Combine(cacheFolder, "cacheAdalV3.bin");
            string msalCacheFileName = Path.Combine(cacheFolder, "cacheMsal.bin");

            AuthenticationResult result;


            PublicClientApplicationBuilder builder = PublicClientApplicationBuilder.Create(v1App.ClientId)
                                                .WithAuthority(v1App.Authority);
             
            if (disableLegacyCache)
            {
                Console.WriteLine("Disabled legacy cache.");
                builder.WithLegacyCacheCompatibility(false);
            }
                                                
            IPublicClientApplication app = builder.Build();
            FilesBasedTokenCacheHelper.EnableSerialization(app.UserTokenCache,
                                                           msalCacheFileName,
                                                           adalV3cacheFileName);
            var accounts = await app.GetAccountsAsync();
            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Using MSALV4.x got token for '{result.Account.Username}' from the cache");
                Console.ResetColor();
            }
            catch (MsalUiRequiredException ex)
            {
                result = await app.AcquireTokenInteractive(scopes)
                                  .ExecuteAsync();
                Console.WriteLine($"Using MSALV4.x got token for '{result.Account.Username}' without the cache");
            }
        }
    }
}
