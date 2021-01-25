del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe -batch
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe -batch

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe -batch
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe -batch

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe -batch
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe -batch

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe -batch
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe -batch

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe -batch
CommonCacheAdalV3\bin\Debug\CommonCacheAdalV3.exe -batch

del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheMsalV3\bin\Debug\CommonCacheMsalV3.exe -batch
CommonCacheAdalV5\bin\Debug\CommonCacheAdalV5.exe -batch

rem Demonstrates disabling legacy ADAL cache in MSAL V4.
del cacheAdalV3.bin
del cacheMsal.bin
CommonCacheMsalV4\bin\Debug\CommonCacheMsalV4.exe -batch

del cacheMsal.bin
rem When legacy cache is enabled in MSAL, MSAL will use cacheAdalV3.bin to retrieve token.
CommonCacheMsalV4\bin\Debug\CommonCacheMsalV4.exe -batch

del cacheMsal.bin
rem When legacy cache is disabled in MSAL and cacheMsal.bin doesn't exist, login UI will be shown.
CommonCacheMsalV4\bin\Debug\CommonCacheMsalV4.exe -batch -disable-legacy-cache

pause