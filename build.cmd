@echo on
if "%APPVEYOR%" == "" (
   .\src\.nuget\nuget install
   SETLOCAL EnableExtensions EnableDelayedExpansion
   set _major=0&set _minor=0&set _build=0
   FOR /F "tokens=3-6 delims=." %%i IN ('dir /b nuget-packages\NServiceKit.Text.*') DO (
    if %%i GTR !_major! (set _major=%%i&set _minor=%%j&set _build=%%k)
    if %%i==!_major! if %%j GTR !_minor! (set _minor=%%j&set _build=%%k)
    if %%i==!_major! if %%j==!_minor! if %%k GTR !_build! (set _build=%%k)
    ECHO %%I
  )
   ENDLOCAL & SET NSERVICEKIT_TEXT_VERSIONED_PATH=NServiceKit.Text.%_major%.%_minor%.%_build%
   SET "NSERVICEKIT_TEXT=%~dp0nuget-packages\%NSERVICEKIT_TEXT_VERSIONED_PATH%\"
   ECHO COPY "%NSERVICEKIT_TEXT%\lib\net35\*.*" .\lib\
   ECHO COPY "%NSERVICEKIT_TEXT%\lib\sl5\*.*" .\lib\sl5
)
ENDLOCAL

if "%BUILD_NUMBER%" == "" (
   set BUILD_NUMBER=%APPVEYOR_BUILD_NUMBER%
)

set target=%1
if "%target%" == "" (
   set target=UnitTests
)

if "%target%" == "NuGetPack" (
	if "%BUILD_NUMBER%" == "" (
	 	echo BUILD_NUMBER environment variable is not set.
		exit;
	)
)

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Build\Build.proj /target:%target% /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false