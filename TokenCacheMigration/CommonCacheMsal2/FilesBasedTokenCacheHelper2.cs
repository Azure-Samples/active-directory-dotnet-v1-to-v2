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

using System.IO;
using System.Security.Cryptography;
using AppCoordinates;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;

namespace CommonCacheMsal2
{
    /// <summary>
    /// Simple persistent cache implementation of the dual cache serialization (ADAL V3 legacy
    /// and unified cache format) for a desktop applications (from MSAL 2.x)
    /// </summary>
    static class FilesBasedTokenCacheHelper2
    {
        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <param name="adalV3CacheFileName">File name where the cache is serialized with the ADAL V3 token cache format. Can
        /// be <c>null</c> if you don't want to implement the legacy ADAL V3 token cache serialization in your MSAL 2.x+ application</param>
        /// <param name="unifiedCacheFileName">File name where the cache is serialized with the Unified cache format, common to
        /// ADAL V4 and MSAL V2 and above, and also accross ADAL/MSAL on the same platform. Should not be <c>null</c></param>
        /// <returns></returns>
        public static TokenCache GetUserCache(string unifiedCacheFileName, string adalV3CacheFileName)
        {
            UnifiedCacheFileName = unifiedCacheFileName;
            AdalV3CacheFileName = adalV3CacheFileName;
            if (usertokenCache == null)
            {
                usertokenCache = new TokenCache();
                usertokenCache.SetBeforeAccess(BeforeAccessNotification);
                usertokenCache.SetAfterAccess(AfterAccessNotification);
            }
            return usertokenCache;
        }

        /// <summary>
        /// Token cache
        /// </summary>
        static TokenCache usertokenCache;

        /// <summary>
        /// File path where the token cache is serialiazed with the unified cache format (ADAL.NET V4, MSAL.NET V3)
        /// </summary>
        public static string UnifiedCacheFileName { get; private set; }

        /// <summary>
        /// File path where the token cache is serialiazed with the legacy ADAL V3 format
        /// </summary>
        public static string AdalV3CacheFileName { get; private set; }

        private static readonly object FileLock = new object();

        public static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                CacheData cacheData = new CacheData
                {
                    UnifiedState = FileStorage.ReadFromFileIfExists(UnifiedCacheFileName),
                    AdalV3State = FileStorage.ReadFromFileIfExists(AdalV3CacheFileName)
                };
                args.TokenCache.DeserializeUnifiedAndAdalCache(cacheData);
            }
        }

        public static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    CacheData cacheData = args.TokenCache.SerializeUnifiedAndAdalCache();

                    // reflect changesgs in the persistent store
                    FileStorage.WriteToFileIfNotNull(UnifiedCacheFileName, cacheData.UnifiedState);
                    if (!string.IsNullOrWhiteSpace(AdalV3CacheFileName))
                    {
                        FileStorage.WriteToFileIfNotNull(AdalV3CacheFileName, cacheData.AdalV3State);
                    }
                }
            }
        }
    }
}
