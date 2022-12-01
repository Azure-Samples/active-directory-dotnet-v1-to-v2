# Token cache migration between ADAL.NET 3.x, ADAL.NET 4.x, and MSAL.NET 2.x

This solution shows how to migrate the token cache from ADAL to MSAL, so that users do not have to be prompted. ADAL v3 code is very similar.

For a more complex sample see [this branch](https://github.com/Azure-Samples/active-directory-dotnet-v1-to-v2/tree/side_by_side_adal_msal/TokenCacheMigration). This shows:

- ADAL + MSAL side by side (e.g. a web service where one machine uses ADAL and one machine uses MSAL). 
- ADAL v3 to MSAL migration
- Acquiring a token by refresh token (ADAL v2 to MSAL migration)

## Features

This solution provides a .NET desktop console application that uses both ADAL and MSAL.

### Disabling legacy token cache
MSAL has some internal code specifically to enable the ability to interact with legacy ADAL cache. When MSAL and ADAL are not used side-by-side (therefore the legacy cache is not used), the related legacy cache code is unnecessary. MSAL [4.25.0](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/releases/tag/4.25.0) adds the ability to disable legacy ADAL cache code and improve cache usage performance. See pull request [#2309](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/pull/2309) for performance comparison before and after disabling the legacy cache. Call `.WithLegacyCacheCompatibility(false)` on an application builder like below.

```csharp
var app = ConfidentialClientApplicationBuilder
	.Create(clientId)
	.WithClientSecret(clientSecret)
	.WithLegacyCacheCompatibility(false)
	.Build();
```

## Getting Started

1. Clone this repository

   ```PowerShell
   git clone https://github.com/Azure-Samples/active-directory-dotnet-v1-to-v2
   cd active-directory-dotnet-v1-to-v2\TokenCacheMigration
   ```

2. Build the solution.
3. [Optional] Update the class Settings with your own client id, redirect uri etc.
3. Run the console app ADAL2MSAL

## Resources

- [Token cache serialization](https://aka.ms/adal-net-token-cache-serialization) in ADAL.NET:
  - [legacy](https://aka.ms/adal-net-token-cache-serialization-legacy) token cache serialization (ADAL V3 format)
  - [unified cache](https://aka.ms/adal-net-token-cache-serialization-unified) token cache serialization
- [Token cache serialization](https://aka.ms/msal-net-token-cache-serialization) in MSAL.NET

### About ADAL.NET and MSAL.NET

- [ADAL.NET](https://aka.ms/adalnet) conceptual documentation
- [MSAL.NET](https://aka.ms/msalnet) conceptual documentation
- Changes in ADAL.NET [between 3.x and 4.x](https://aka.ms/adal-net-4-released)
- Changes in MSAL.NET [Between 1.x and 2.x](https://aka.ms/msal-net-2-released)
- Changes in MSAL.NET [Between 2.x and 3.x](https://aka.ms/msal-net-2x)
