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

using System;
using System.IO;
using System.Security.Cryptography;
using AppCoordinates;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;

namespace CommonCacheMsal3
{
    /// <summary>
    /// Simple persistent cache implementation of the dual cache serialization (ADAL V3 legacy
    /// and unified cache format) for a desktop applications (from MSAL 2.x)
    /// </summary>
    public class FilesBasedTokenCache
    {
        private static readonly object FileLock = new object();

        /// <summary>
        /// Token cache
        /// </summary>
        private TokenCache usertokenCache;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheFolder">Folder which is the base of the caches.</param>
        public FilesBasedTokenCache(string cacheFolder)
        {
            UnifiedCacheFileName = Path.Combine(cacheFolder, "unifiedCache.bin");
            UnifiedV2CacheFileName = Path.Combine(cacheFolder, "unifiedCacheV2.bin"); 
            AdalV3CacheFileName = Path.Combine(cacheFolder, "cacheAdalV3.bin");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("File 'AdalV3': " + File.Exists(AdalV3CacheFileName));
            Console.WriteLine("File 'MsalV2': " + File.Exists(UnifiedCacheFileName));
            Console.WriteLine("File 'MsalV3': " + File.Exists(UnifiedV2CacheFileName));
            Console.ResetColor();
        }

        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <returns></returns>
        public TokenCache GetUserCache()
        {
            if (usertokenCache == null)
            {
                usertokenCache = new TokenCache();
                usertokenCache.SetBeforeAccess(BeforeAccessNotification);
                usertokenCache.SetAfterAccess(AfterAccessNotification);
            }
            return usertokenCache;
        }

        public bool AllowAdalV3Format { get; set; } = false;

        /// <summary>
        /// File path where the token cache is serialiazed with the unified cache format (ADAL.NET V5, MSAL.NET V3)
        /// </summary>
        public string UnifiedV2CacheFileName { get; private set; }

        /// <summary>
        /// File path where the token cache is serialiazed with the unified cache format (ADAL.NET V4, MSAL.NET V2 and MSAL.NET v3)
        /// </summary>
        public string UnifiedCacheFileName { get; private set; }

        /// <summary>
        /// File path where the token cache is serialiazed with the legacy ADAL V3 format
        /// </summary>
        public string AdalV3CacheFileName { get; private set; }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                // We will need a CacheData which takes the new format allowing to read both from ADAL as well as from MSAL.
                // Perhaps the easiest is to simply expand the CacheData class to include the new format and deprecate the old one?
                // Unfortunately there might be people who are already using the Java Stuff.

                byte[] v2UnifiedState = FileStorage.ReadFromFileIfExists(UnifiedV2CacheFileName);
                byte[] adalV3State = FileStorage.ReadFromFileIfExists(AdalV3CacheFileName);
                byte[] unifiedState = FileStorage.ReadFromFileIfExists(UnifiedCacheFileName);

                if (!AllowAdalV3Format)
                {
                    adalV3State = null;
                }

                // Using new format if it exists.
                if (v2UnifiedState != null)
                {
                    // Fallback using old unified format and adalv3 format
                    CacheData cacheData = new CacheData
                    {
                        UnifiedState = null,
                        AdalV3State = adalV3State
                    };
                    args.TokenCache.DeserializeUnifiedAndAdalCache(cacheData);
                    args.TokenCache.DeserializeV3(v2UnifiedState);
                }
                else
                {
                    // Fallback using old unified format and adalv3 format
                    CacheData cacheData = new CacheData
                    {
                        UnifiedState = unifiedState,
                        AdalV3State = adalV3State
                    };
                    args.TokenCache.DeserializeUnifiedAndAdalCache(cacheData);
                }
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            // initially when starting up, the args.HasStateChanged is not correct! 
            // ... actually the byte array returns something but it's actually empty
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    // TODO: Figure out if we should continue having a CacheData or if we should go with
                    // SerializeAdalV3 and SerializeMsalV2, unfortunately we already used unified... 
                    // Perhaps it should be called SerializeUnifiedFormat or SerializeSharedFormat
                    // The main reason for not being able to stick to the names... would be that ADAL has to be updates 
                    // and that one shipped as GA ... we risk developers not figuring out that the format has changed.
                    // Need logging and exceptions allowing developers to handle this gracefully.
                    // 
                    // CommonCache { AdalV3State, CommonState/SharedState }
                    // 
                    // ... Miration Aid will not work with-out this.
                    //
                    // TODO: Add support for testing sharing with ADAL with-out using the ADALv3 format

                    CacheData cacheData = args.TokenCache.SerializeUnifiedAndAdalCache();
                    byte[] v2UnifiedState = args.TokenCache.SerializeV3();

                    // Write all formats
                    FileStorage.WriteToFileIfNotNull(UnifiedV2CacheFileName, v2UnifiedState);
                    FileStorage.WriteToFileIfNotNull(AdalV3CacheFileName, cacheData.AdalV3State);
                    FileStorage.WriteToFileIfNotNull(UnifiedCacheFileName, cacheData.UnifiedState);
                }
            }
        }
    }
}
