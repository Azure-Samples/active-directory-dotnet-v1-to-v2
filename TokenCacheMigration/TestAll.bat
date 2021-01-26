del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe

rem Demonstrates disabling legacy ADAL cache in MSAL V4.
del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheMsalV4\bin\Debug\CommonCacheMsalV4.exe

del cacheMsal.bin
rem When legacy cache is enabled in MSAL, MSAL will use cacheAdalV3.bin to retrieve token.
CommonCacheMsalV4\bin\Debug\CommonCacheMsalV4.exe

del cacheMsal.bin
rem When legacy cache is disabled in MSAL and cacheMsal.bin doesn't exist, login UI will be shown.
CommonCacheMsalV4\bin\Debug\CommonCacheMsalV4.exe -disable-legacy-cache

pause