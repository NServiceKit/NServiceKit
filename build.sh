export EnableNuGetPackageRestore=true
mono ./src/.nuget/NuGet.exe install NServiceKit.Text
xbuild ./src/NServiceKit.sln /p:TargetFramework=net40 /p:Configuration=Release
