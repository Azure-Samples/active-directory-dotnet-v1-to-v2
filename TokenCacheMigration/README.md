# Token cache migration

This solution shows the token cache customization for .NET desktop applications to share the Single Sign On (SSO) state between:

- [ADAL.NET](https://aka.ms/adalnet) V3.x applications
- [ADAL.NET V4.x](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki/changes-adalnet-4.0-preview) applications
- [MSAL.NET](https://aka.ms/msalnet) 2.x applications.

## Features

This solution provides three .NET desktop console applications and one common library.

- `CommonCacheAdalV3` is a desktop application leveraging ADAL 3.19.8, and serializing / deserializing the token cache in the ADAL V3 legacy format
- `CommonCacheAdalV4` is a desktop application leveraging ADAL 4.x, and serializing / deserializing the token cache in both the ADAL V3 legacy format and the Unified cache format
- `CommonCacheMsalV2` is a desktop application leveraging ADAL 2.x, and serializing / deserializing the token cache in both the ADAL V3 legacy format and the Unified cache format
- `AppCoordinates` is a common library containing the coordinates (ClientID) of an Azure AD application used in the three desktop applications

## Getting Started

1. Clone this repository

   ```PowerShell
   git clone https://github.com/Azure-Samples/active-directory-dotnet-v1-to-v2
   cd active-directory-dotnet-v1-to-v2\TokenCacheMigration
   ```

2. Build the solution.
3. Run any of the three console application, and then another, you should see that the token cache is shared: you don't need to re-sign-in.
4. delete the token cache files (in the same folder as the Visual Studio solution):
   - `cacheAdalV3.bin`
   - `unifiedCache.bin`

5. Alternatively, if you want to try out all the combinations, you can run the `TestAll.bat` script. This scripts:

- delete both token cache files if they exist
- runs one of the desktop apps (you'll need to sign-in with a work and school account)
- runs one of the other two desktop apps. you are automatically signed-in 

## Resources

- [Token cache serialization](adal-net-token-cache-serialization) in ADAL.NET:
  - [legacy](adal-net-token-cache-serialization-legacy) token cache serialization (ADAL V3 format)
  - [unified cache](adal-net-token-cache-serialization-unified ) token cache serialization
- Token cache serialization in MSAL.NET

### About ADAL.NET and MSAL.NET

- [ADAL.NET](https://aka.ms/adalnet) conceptual documentation
- [MSAL.NET](https://aka.ms/msalnet) conceptual documentation
- Changes in ADAL.NET [between 3.x and 4.x]()
- Changes in MSAL.NET [Between 1.x and 2.x]()

