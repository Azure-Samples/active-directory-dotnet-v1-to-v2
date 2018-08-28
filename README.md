# Migrating from an application using ADAL.NET to an application using MSAL.NET

This sample aims at gathering a number of Visual Studio solutions focusing on certain aspects of the Azure AD v1.0 applications, leveraging ADAL.NET to Azure AD v2.0 applications (also named converged applications), leverating MSAL.NET

## Features

This repository contains the following solutions

Solution | Description
-------- | -----------
[Token Cache Migration](TokenCacheMigration/README.md) | shows the token cache customization for .NET desktop applications to share the Single Sign On (SSO) state between, [ADAL.NET](https://aka.ms/adalnet) V3.x applications, [ADAL.NET V4.x](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki/changes-adalnet-4.0-preview) applications and [MSAL.NET](https://aka.ms/msalnet) 2.x applications.

## Getting Started

. Clone this repository

   ```PowerShell
   git clone https://github.com/Azure-Samples/active-directory-dotnet-v1-to-v2
   cd active-directory-dotnet-v1-to-v2
   ```

   Then go to each sub folder to see a particular aspect of the migration

## Resources

- [ADAL.NET](https://aka.ms/adalnet) conceptual documentation
- [MSAL.NET](https://aka.ms/msalnet) conceptual documentation
- |From ADAL.NET to MSAL.NET](https://aka.ms/adal-net-to-msal-net)
- Changes in ADAL.NET [between 3.x and 4.x](https://aka.ms/adal-net-4-released)
- Changes in MSAL.NET [Between 1.x and 2.x](https://aka.ms/msal-net-2-released)
