@echo off

Rem -- Adding test app for sharing between MSALv2 and MSALv3
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv3 sharing with MSALv2"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe
CommonCacheMsal2\bin\Debug\CommonCacheMsal2.exe

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv2 sharing with MSALv3"
CommonCacheMsal2\bin\Debug\CommonCacheMsal2.exe
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe

Rem -- Adding test app for sharing between ADALv3 and MSALv3
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating ADALv3 sharing with MSALv3"
CommonCacheAdalv3\bin\Debug\CommonCacheAdalV3.exe
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv3 sharing with ADALv3"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe
CommonCacheAdalv3\bin\Debug\CommonCacheAdalV3.exe

Rem -- Adding test app for sharing between ADALv4 and MSALv3
del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating ADALv4 sharing with MSALv3"
CommonCacheAdalV4\bin\Debug\CommonCacheAdalV4.exe
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe

del cacheAdalV3.bin
del unifiedCache.bin
del unifiedCacheV2.bin
echo "validating MSALv3 sharing with ADALv4"
CommonCacheMsal3\bin\Debug\CommonCacheMsal3.exe
CommonCacheAdalV4\bin\Debug\CommonCacheAdalV4.exe

pause