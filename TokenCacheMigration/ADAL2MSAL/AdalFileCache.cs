using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ADAL2MSAL
{
    /// <summary>
    /// IMPORTANT: Only suitable for desktop apps (public client apps)!
    /// </summary>
    /// <remarks>
    /// In ADAL v3, some of the APIs are named slightly differently. See https://github.com/Azure-Samples/active-directory-dotnet-v1-to-v2/blob/master/TokenCacheMigration/CommonCacheAdalv3/FilesBasedAdalV3TokenCache.cs
    /// </remarks>
    class AdalFileCache : TokenCache
    {
        public string AdalV3CacheFilePath { get; }
        //public string MsalCacheFilePath { get; }

        private static readonly object FileLock = new object();

        // Initializes the cache against a local file.
        // If the file is already present, it loads its content in the ADAL cache
        public AdalFileCache(string adalV3FilePath, string msalCacheFilePath)
        {
            AdalV3CacheFilePath = adalV3FilePath;
            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
            BeforeAccessNotification(null);
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            File.Delete(AdalV3CacheFilePath);
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                DeserializeAdalV3(ReadFromFileIfExists(AdalV3CacheFilePath));
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
                    WriteToFileIfNotNull(AdalV3CacheFilePath, SerializeAdalV3());

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
            byte[] protectedBytes = (!string.IsNullOrEmpty(path) && File.Exists(path)) ? File.ReadAllBytes(path) : null;
            byte[] unprotectedBytes = (protectedBytes != null) ? ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser) : null;
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
                byte[] protectedBytes = ProtectedData.Protect(blob, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(path, protectedBytes);
            }
            else
            {
                File.Delete(path);
            }
        }
    }
}
