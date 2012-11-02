@echo off
echo Updating submodules...
git submodule update --init

:BUILD_WITH_NETFX
echo Trying to build with .NET...
msbuild backend\backend.csproj /maxcpucount /toolsversion:4.0 /verbosity:m /nologo
if %ERRORLEVEL% NEQ 0 (
	echo Checking if .NET 4.0 installed in standard location...
	dir /B %windir%\Microsoft.NET\Framework\v4.0.* > ~
	set /p path_=< ~
	del ~
	if "%path_%"=="" goto BUILD_WITH_MONO
	echo .NET 4.0 installed in %windir%\Microsoft.NET\Framework\%path_%
	set path=%path%;%windir%\Microsoft.NET\Framework\%path_%
) else if %ERRORLEVEL% EQU 0 exit /B 0
msbuild backend\backend.csproj /maxcpucount /toolsversion:4.0 /verbosity:m /nologo
if %ERRORLEVEL% NEQ 0 goto BUILD_WITH_MONO
exit /B 0

:BUILD_WITH_MONO
echo Trying to build with Mono...
xbuild /maxcpucount /p:TargetFrameworkProfile="" /verbosity:minimal backend/backend.csproj
if %ERRORLEVEL% NEQ 0 goto BUILD_NOT
exit /B

:BUILD_NOT
echo Could not find msbuild or xbuild.
echo Make sure .NET Framework 4.0 and/or Mono 2.1x.x are installed.
echo If this message appears with .NET Framework 4.0 being installed,
echo try running this script from Visual Studio Command Line Tools.
exit /B
