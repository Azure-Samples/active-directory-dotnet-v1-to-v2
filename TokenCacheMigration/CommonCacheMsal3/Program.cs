//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonCacheMsal3
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
            AppCoordinates.AppCoordinates v1App = AppCoordinates.PreRegisteredApps.GetV1App(useInMsal: true);
            string[] scopes = AppCoordinates.PreRegisteredApps.MsGraphWithUserReadScope;

            string cacheFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..");
            var fileBasedTokenCache = new FilesBasedTokenCache(cacheFolder);
            TokenCache tokenCache = fileBasedTokenCache.GetUserCache();

            AuthenticationResult result;
            PublicClientApplication app = new PublicClientApplication(v1App.ClientId, v1App.Authority, tokenCache);
            var accounts = await app.GetAccountsAsync();
            try
            {
                result = await app.AcquireTokenSilentAsync(scopes, accounts.FirstOrDefault());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"got token for '{result.Account.Username}' from the cache");
                Console.ResetColor();
            }
            catch (MsalUiRequiredException)
            {
                result = await app.AcquireTokenAsync(scopes);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"got token for '{result.Account.Username}' using Interactive Auth");
                Console.ResetColor();
            }
        }
    }
}
