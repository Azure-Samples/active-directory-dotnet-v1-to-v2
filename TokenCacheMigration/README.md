# Token cache migration between ADAL.NET 3.x, ADAL.NET 4.x, and MSAL.NET 2.x

This solution shows how to customize token cache serialization in a .NET desktop application so that it shares the Single Sign On (SSO) state between:

- [ADAL.NET](https://aka.ms/adalnet) V3.x applications
- [ADAL.NET V5.x](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki/changes-adalnet-4.0-preview) applications
- [MSAL.NET](https://aka.ms/msalnet) 2.x applications.

## Features

This solution provides three .NET desktop console applications and one common library.

- `CommonCacheAdalV3` is a desktop application referencing ADAL 3.19.8, and serializing / deserializing the token cache in the ADAL V3 legacy format
- `CommonCacheAdalV5` is a desktop application referencing ADAL 5.x, and serializing / deserializing the token cache in both the ADAL V3 legacy format and the Unified cache format
- `CommonCacheMsalV3` is a desktop application referencing MSAL 3.x, and serializing / deserializing the token cache in both the ADAL V3 legacy format and the Unified cache format
- `AppCoordinates` is a common library containing the application coordinates (really the ClientID) of an Azure AD application used in the three desktop applications.

![image](https://user-images.githubusercontent.com/13203188/45534630-a5e25200-b7b0-11e8-98ca-0e21c3df1176.png)

## Getting Started

1. Clone this repository

   ```PowerShell
   git clone https://github.com/Azure-Samples/active-directory-dotnet-v1-to-v2
   cd active-directory-dotnet-v1-to-v2\TokenCacheMigration
   ```

2. Build the solution.
3. Run any of the three console applications, and then another, you should see that the token cache is shared: you don't need to re-sign-in.
4. delete the token cache files (in the same folder as the Visual Studio solution):
   - `cacheAdalV3.bin`
   - `unifiedCache.bin`

5. Alternatively, if you want to try out all the combinations, you can run the `TestAll.bat` script. This batch script:

- deletes both token cache files if they exist
- runs one of the desktop apps (you'll need to sign-in with a work and school account)
- runs one of the other two desktop apps. you are automatically signed-in 

## Resources

- [Token cache serialization](https://aka.ms/adal-net-token-cache-serialization) in ADAL.NET:
  - [legacy](https://aka.ms/adal-net-token-cache-serialization-legacy) token cache serialization (ADAL V3 format)
  - [unified cache](https://aka.ms/adal-net-token-cache-serialization-unified) token cache serialization
- Token cache serialization in MSAL.NET

### About ADAL.NET and MSAL.NET

- [ADAL.NET](https://aka.ms/adalnet) conceptual documentation
- [MSAL.NET](https://aka.ms/msalnet) conceptual documentation
- Changes in ADAL.NET [between 3.x and 4.x](https://aka.ms/adal-net-4-released)
- Changes in MSAL.NET [Between 1.x and 2.x](https://aka.ms/msal-net-2-released)
- Changes in MSAL.NET [Between 2.x and 3.x](https://aka.ms/msal-net-2x)