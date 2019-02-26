/*
 The MIT License (MIT)

Copyright (c) 2015 Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using AppCoordinates;
using Microsoft.Identity.Core.Cache;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Security.Cryptography;

namespace CommonCacheADAL
{
    /// <summary>
    /// Simple file based persistent cache implementation for a desktop application (from ADAL 4.x)
    /// </summary>
    public class FilesBasedTokenCache : TokenCache
    {
        public string AdalV3CacheFilePath { get; }
        public string UnifiedCacheFilePath { get; }
        public string UnifiedCacheV2FilePath { get; }

        private static readonly object FileLock = new object();

        // Initializes the cache against a local file.
        // If the file is already rpesent, it loads its content in the ADAL cache
        public FilesBasedTokenCache(string cacheFolder)
        {
            AdalV3CacheFilePath = Path.Combine(cacheFolder, "cacheAdalV3.bin");
            UnifiedCacheFilePath = Path.Combine(cacheFolder, "unifiedCache.bin");
            UnifiedCacheV2FilePath = Path.Combine(cacheFolder, "unifiedCacheV2.bin");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("File 'AdalV3': " + File.Exists(AdalV3CacheFilePath));
            Console.WriteLine("File 'MsalV2': " + File.Exists(UnifiedCacheFilePath));
            Console.WriteLine("File 'MsalV3': " + File.Exists(UnifiedCacheV2FilePath));
            Console.ResetColor();

            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;

            //lock (FileLock)
            //{
            //    CacheData cacheData = new CacheData();
            //    cacheData.AdalV3State = ReadFromFileIfExists(AdalV3CacheFilePath);
            //    cacheData.UnifiedState = ReadFromFileIfExists(UnifiedCacheFilePath);
            //    this.DeserializeAdalAndUnifiedCache(cacheData);
            //}
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            File.Delete(AdalV3CacheFilePath);
            File.Delete(UnifiedCacheFilePath);
            File.Delete(UnifiedCacheV2FilePath);
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                CacheData cacheData = new CacheData();
                cacheData.AdalV3State = FileStorage.ReadFromFileIfExists(AdalV3CacheFilePath);
                cacheData.UnifiedState = FileStorage.ReadFromFileIfExists(UnifiedCacheFilePath);

                // new format
                var v3 = FileStorage.ReadFromFileIfExists(UnifiedCacheV2FilePath);
                if (v3 != null)
                {
                    this.DeserializeV3(v3);

                    // This seems to overwrite the v3 state set. Thus should ideally only be done if no tokens exists
                    if (cacheData.AdalV3State != null)
                    {
                        cacheData.AdalV3State = FileStorage.ReadFromFileIfExists(AdalV3CacheFilePath);
                        cacheData.UnifiedState = null;
                        this.DeserializeAdalAndUnifiedCache(cacheData);
                    }
                }
                //else if (v3 == null && cacheData.AdalV3State != null)
                //{
                //    cacheData.AdalV3State = FileStorage.ReadFromFileIfExists(AdalV3CacheFilePath);
                //    cacheData.UnifiedState = null;
                //    this.DeserializeAdalAndUnifiedCache(cacheData);
                //}
                else
                {
                    cacheData.AdalV3State = FileStorage.ReadFromFileIfExists(AdalV3CacheFilePath);
                    cacheData.UnifiedState = FileStorage.ReadFromFileIfExists(UnifiedCacheFilePath);
                    this.DeserializeAdalAndUnifiedCache(cacheData);
                }
            }
        }

        // Triggered right after ADAL accessed the cache.
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (this.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    CacheData cacheData = this.SerializeAdalAndUnifiedCache();
                    FileStorage.WriteToFileIfNotNull(AdalV3CacheFilePath, cacheData.AdalV3State);
                    FileStorage.WriteToFileIfNotNull(UnifiedCacheFilePath, cacheData.UnifiedState);

                    var v3 = this.SerializeV3(); // Seems to be missing AT?
                    FileStorage.WriteToFileIfNotNull(UnifiedCacheV2FilePath, v3);
                    // once the write operation took place, restore the HasStateChanged bit to false
                    this.HasStateChanged = false;
                }
            }
        }
    }
}
