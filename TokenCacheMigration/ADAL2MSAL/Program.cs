using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ADAL2MSAL
{
    class Settings
    {
        // Do not use in production, register your own!
        public const string ClientId = "cebb9841-cd93-4119-80f2-bcbe797ffa93";
        public const string Authority = "https://login.microsoftonline.com/common";

        public static Uri RedirectUri = new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient");

        public static readonly string[] MsalScope = new[] { "User.Read" };
        public const string AdalResource = "https://graph.microsoft.com/";
    }

    class Program
    {
        private static string AdalCacheFile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".adal_cache.bin";
        private static string MsalCacheFile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".msal_cache.bin";

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine(@"
                        -------------------------------------------------
                        1. [ADAL] Get Users
                        2. [ADAL] Acquire Token Silent with first user 
                        3. [ADAL] Acquire Token Interactive
                        4. [ADAL] Clear Cache
                        -------------------------------------------------
                        5. [MSAL] Get Accounts 
                        6. [MSAL] Acquire Token Silent with first account
                        7. [MSAL] Acquire Token Interactive
                        8. [MSAL] Clear Cache
                        -------------------------------------------------
                        x. Exit app
                        
                    Enter your Selection: "); ;

                char.TryParse(Console.ReadLine(), out var selection);

                try
                {
                    switch (selection)
                    {
                        case '1':  // [ADAL] Get Users
                            var authContext = CreateAdalContext();

                            foreach (var adalCacheItem in authContext.TokenCache.ReadItems())
                            {
                                Console.WriteLine("Cache entry for " + adalCacheItem.DisplayableId);
                            }

                            Console.WriteLine("Done getting ADAL users");
                            break;

                        case '2': // [ADAL] Acquire Token Silent with first ADAL user
                            authContext = CreateAdalContext();

                            var cacheItem = authContext.TokenCache.ReadItems().FirstOrDefault();
                            if (cacheItem == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("No ADAL users in the cache");
                                Console.ResetColor();
                                break;
                            }
                            
                            var result = await
                                authContext.AcquireTokenSilentAsync(
                                    Settings.AdalResource,
                                    Settings.ClientId, 
                                    new UserIdentifier(cacheItem?.DisplayableId, UserIdentifierType.OptionalDisplayableId));

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Got a token for: " + result.UserInfo.DisplayableId);
                            Console.ResetColor();
                            break;

                        case '3': // acquire token
                            authContext = CreateAdalContext();
                            result = await
                                authContext.AcquireTokenAsync(
                                    Settings.AdalResource,
                                    Settings.ClientId,
                                    Settings.RedirectUri,
                                    new PlatformParameters(PromptBehavior.SelectAccount));

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Got a token for: " + result.UserInfo.DisplayableId);
                            Console.ResetColor();
                            break;

                        case '4':  // [ADAL] Clear cache
                            AdalFileCache tokenCache = new AdalFileCache(AdalCacheFile);
                            tokenCache.Clear();

                            break;

                        case '5': // [MSAL] Get Accounts
                            var pca = CreateMsalApp();
                            var accounts = await pca.GetAccountsAsync();
                            foreach (var account in accounts)
                            {
                                Console.WriteLine("Cache entry for " + account.Username);
                            }

                            Console.WriteLine("Done getting MSAL accounts");


                            break;
                        case '6': // [MSAL] Acquire Token Silent with first account
                            pca = CreateMsalApp();
                            var firstAccount = (await pca.GetAccountsAsync()).FirstOrDefault();

                            var msalResult = await pca.AcquireTokenSilent(Settings.MsalScope, firstAccount).ExecuteAsync();

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Got a token for: " + msalResult.Account.Username);
                            Console.ResetColor();

                            break;
                        case '7': // [MSAL] Acquire Token Interactive
                            pca = CreateMsalApp();
                            msalResult = await pca.AcquireTokenInteractive(Settings.MsalScope).ExecuteAsync();

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Got a token for: " + msalResult.Account.Username);
                            Console.ResetColor();

                            break;
                        case '8': // [MSAL] Clear Cache
                            pca = CreateMsalApp();
                            accounts = await pca.GetAccountsAsync();
                            foreach (var account in accounts)
                            {
                                await pca.RemoveAsync(account);
                            }
                            break;
                    }       
                }           
                catch (Exception e)
                {
                    if (e is AdalSilentTokenAcquisitionException)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Cannot get ADAL token silently .. " + e.Message);                        
                        Console.ResetColor();
                    }

                    if (e is MsalUiRequiredException)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Cannot get MSAL token silently .. " + e.Message);
                        Console.ResetColor();
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ResetColor();
                }

                Console.WriteLine("\n\nHit 'ENTER' to continue...");
                Console.ReadLine();
            }
        }

        private static AuthenticationContext CreateAdalContext()
        {
            AdalFileCache tokenCache = new AdalFileCache(AdalCacheFile);
            AuthenticationContext authenticationContext = new AuthenticationContext(Settings.Authority, tokenCache);
            return authenticationContext;
        }

        private static IPublicClientApplication CreateMsalApp()
        {
            var pca = PublicClientApplicationBuilder
                            .Create(Settings.ClientId)
                            .WithAuthority(Settings.Authority)
                            .Build();

            MsalTokenCacheWithAdalSupport cache = new MsalTokenCacheWithAdalSupport(AdalCacheFile, MsalCacheFile);
            cache.BindCache(pca.UserTokenCache);
            
            return pca;
        }

    }
}
