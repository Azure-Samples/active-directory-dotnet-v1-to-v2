@echo off

Rem -- Adding test app for sharing between MSALv3 and MSALv3, in new format
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
del cacheAdalV3.bak
del unifiedCache.bak
del unifiedCacheV2.bak
echo "ADALv5: No Caches exists. Expecting Interactive Auth (allowing also adalv3)"
CommonCacheAdalV4\bin\Debug\CommonCacheAdalV4.exe

copy cacheAdalV3.bin cacheAdalV3.bak
copy unifiedCache.bin unifiedCache.bak
copy unifiedCacheV2.bin unifiedCacheV2.bak

echo "MSALv3: ADALv3 Cache exists. Expecting load from cache (allowing also adalv3)"
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy cacheAdalV3.bak cacheAdalV3.bin
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

echo "ADALv5: ADALv3 Cache exists. Expecting load from cache
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy cacheAdalV3.bak cacheAdalV3.bin
CommonCacheAdalV4\bin\Debug\CommonCacheAdalV4.exe

echo "MSALv3: UnifiedCacheV2 exists. Expecting load from cache (allowing also adalv3)"
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCacheV2.bak unifiedCacheV2.bin
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

echo "ADALv5: unifiedCacheV2 exists. Expecting load from cache
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCacheV2.bak unifiedCacheV2.bin
CommonCacheAdalV4\bin\Debug\CommonCacheAdalV4.exe

echo "MSALv3: UnifiedCache exists. Expecting load from cache (allowing also adalv3)"
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCache.bak unifiedCache.bin
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe "AllowV3"

echo "ADALv5: unifiedCache exists. Expecting load from cache
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
copy unifiedCache.bak unifiedCache.bin
CommonCacheAdalV4\bin\Debug\CommonCacheAdalV4.exe

pause
