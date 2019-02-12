
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
    }
}
