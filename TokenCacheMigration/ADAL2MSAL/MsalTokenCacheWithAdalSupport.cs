using Microsoft.Identity.Client;
using System.IO;
using System.Security.Cryptography;

namespace ADAL2MSAL
{
    /// <summary>
    ///  IMPORTANT: encrypted files are suitable for desktop apps (public client apps), not for web sites, web services etc.
    ///  For web sites, we recommend you rely on SSO from the browser, and not rely on token cache migration.
    ///  Fop web API (AcquireTokenOnBehalfOf) and app 2 app (AcquireTokenForClient), refresh token migration is not supported.
    ///  
    ///  Assumes that both ADAL and MSAL token caches are stored on disk and encrypted with Data Protection API.
    /// 
    ///  For cross platform desktop apps (Windows, Mac, Linux), see https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet
    ///  For web sites and web services, see https://github.com/AzureAD/microsoft-identity-web/wiki/token-cache-serialization
    /// </summary>
    /// <remarks>    
    /// This cache is configured so that MSAL **reads** from the ADAL cache file 
    /// </remarks>
    class MsalTokenCacheWithAdalSupport
    {
        // For debugging purposes only!
        private const bool Encrypt = true;

        private static readonly object _fileLock = new object();
        private readonly string _adalCacheFile;
        private readonly string _msalCacheFile;

        public MsalTokenCacheWithAdalSupport(string adalCacheFile, string msalCacheFile)
        {
            _adalCacheFile = adalCacheFile;
            _msalCacheFile = msalCacheFile;
        }

        public void BindCache(ITokenCache cache)
        {          
            cache.SetBeforeAccess(BeforeAccessNotification);
            cache.SetAfterAccess(AfterAccessNotification);
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (_fileLock)
            {
                // Load up any data from ADAL
                args.TokenCache.DeserializeAdalV3(ReadFromFileIfExists(_adalCacheFile));

                // Now load up the data from MSAL. Tokens will be merged.
                args.TokenCache.DeserializeMsalV3(ReadFromFileIfExists(_msalCacheFile));
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (_fileLock)
                {
                    WriteToFileIfNotNull(_msalCacheFile, args.TokenCache.SerializeMsalV3());
                    if (!string.IsNullOrWhiteSpace(_adalCacheFile))
                    {
                        WriteToFileIfNotNull(_adalCacheFile, args.TokenCache.SerializeAdalV3()); 
                    }
                }
            }
        }

        /// <summary>
        /// Read the content of a file if it exists
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Content of the file (in bytes)</returns>
        private static byte[] ReadFromFileIfExists(string path)
        {
            byte[] protectedBytes = (!string.IsNullOrEmpty(path) && File.Exists(path)) ? File.ReadAllBytes(path) : null;
            byte[] unprotectedBytes = Encrypt ?
                ((protectedBytes != null) ? ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser) : null)
                : protectedBytes;
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
                byte[] protectedBytes = Encrypt ? ProtectedData.Protect(blob, null, DataProtectionScope.CurrentUser) : blob;
                File.WriteAllBytes(path, protectedBytes);
            }
            else
            {
                File.Delete(path);
            }
        }
    }
}
