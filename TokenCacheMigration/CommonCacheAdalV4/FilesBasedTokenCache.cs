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

using Microsoft.Identity.Core.Cache;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Security.Cryptography;

namespace CommonCacheADAL
{
    /// <summary>
    /// Simple file based persistent cache implementation for a desktop application (from ADAL 4.x)
    /// </summary>
    class FilesBasedTokenCache : TokenCache
    {
        public string AdalV3CacheFilePath { get; }
        public string UnifiedCacheFilePath { get; }
        private static readonly object FileLock = new object();

        // Initializes the cache against a local file.
        // If the file is already rpesent, it loads its content in the ADAL cache
        public FilesBasedTokenCache(string adalV3FilePath, string unifiedCacheFilePath)
        {
            AdalV3CacheFilePath = adalV3FilePath;
            this.AfterAccess = AfterAccessNotification;
            this.UnifiedCacheFilePath = unifiedCacheFilePath;
            this.BeforeAccess = BeforeAccessNotification;
            lock (FileLock)
            {
                CacheData cacheData = new CacheData();
                cacheData.AdalV3State = ReadFromFileIfExists(AdalV3CacheFilePath);
                cacheData.UnifiedState = ReadFromFileIfExists(UnifiedCacheFilePath);
                this.DeserializeAdalAndUnifiedCache(cacheData);
            }
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            File.Delete(AdalV3CacheFilePath);
            File.Delete(UnifiedCacheFilePath);
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                CacheData cacheData = new CacheData();
                cacheData.AdalV3State = ReadFromFileIfExists(AdalV3CacheFilePath);
                cacheData.UnifiedState = ReadFromFileIfExists(UnifiedCacheFilePath);
                this.DeserializeAdalAndUnifiedCache(cacheData);
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
                    WriteToFileIfNotNull(AdalV3CacheFilePath, cacheData.AdalV3State);
                    WriteToFileIfNotNull(UnifiedCacheFilePath, cacheData.UnifiedState);
                    // once the write operation took place, restore the HasStateChanged bit to false
                    this.HasStateChanged = false;
                }
            }
        }

        /// <summary>
        /// Read the content of a file if it exists
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Content of the file (in bytes)</returns>
        private byte[] ReadFromFileIfExists(string path)
        {
 //           byte[] protectedBytes = (!string.IsNullOrEmpty(path) && File.Exists(path)) ? File.ReadAllBytes(path) : null;
 //           byte[] unprotectedBytes = (protectedBytes != null) ? ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser) : null;
            byte[] unprotectedBytes = (!string.IsNullOrEmpty(path) && File.Exists(path)) ? File.ReadAllBytes(path) : null;
            return unprotectedBytes;
        }

        /// <summary>
        /// Writes a blob of bytes to a file. If the blob is <c>null</c>, deletes the file
        /// </summary>
        /// <param name="path">path to the file to write</param>
        /// <param name="blob">Blob of bytes to write</param>
        private static void WriteToFileIfNotNull(string path, byte[] blob)
        {
            if (blob != null)
            {
                //byte[] protectedBytes = ProtectedData.Protect(blob, null, DataProtectionScope.CurrentUser);
                //File.WriteAllBytes(path, protectedBytes);
                File.WriteAllBytes(path, blob);
            }
            else
            {
                File.Delete(path);
            }
        }
    }

}
