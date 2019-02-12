@echo off

Rem -- Adding test app for sharing between MSALv3 and MSALv3, in new format
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "MSALv3: No Caches exists. Expecting Interactive Auth (allowing also adalv3)"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

copy cacheAdalV3.bin cacheAdalV3.bak
copy unifiedCache.bin unifiedCache.bak
copy unifiedCacheV2.bin unifiedCacheV2.bak

echo "MSALv3: ADALv3 Cache exists. Expecting load from cache (allowing also adalv3)"
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy cacheAdalV3.bak cacheAdalV3.bin
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

echo "MSALv3: MSALv3 Cache exists. Expecting load from cache (allowing also adalv3)"
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCacheV2.bak unifiedCacheV2.bin
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

echo "MSALv3: Msalv2 Cache exists. Expecting load from cache (allowing also adalv3)"
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCache.bak unifiedCache.bin
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCache.bak unifiedCache.bin
echo "MSALv3: Msalv2 Cache exists. Expecting Interactive Auth"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe

pause

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv3 sharing with MSALv3 using new format and adalv3"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv3 sharing with MSALv3 using new format and adalv3"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv3 sharing with MSALv3 using new format"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe 
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

pause