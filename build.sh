set -e # exit on first error
set -u # exit on using unset variable

nuget restore ./src/NServiceKit.sln -configfile nuget.config
nuget install NServiceKit.Text
xbuild ./src/NServiceKit.sln /p:TargetFramework=net40 /p:Configuration=Release
