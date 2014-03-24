SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe

REM SET BUILD=Debug
SET BUILD=Release

REM %MSBUILD% build.msbuild

MD ..\NuGet\NServiceKit\lib\net35
MD ..\NuGet\NServiceKit.Api.Swagger\lib\net35
MD ..\NuGet\NServiceKit.Common\lib\net35
MD ..\NuGet\NServiceKit.Mvc\lib\net40
MD ..\NuGet\NServiceKit.Razor\lib\net40
MD ..\NuGet\NServiceKit.Authentication.OpenId\lib\net35
MD ..\NuGet\NServiceKit.Authentication.OAuth2\lib\net35
MD ..\NuGet\NServiceKit.Plugins.ProtoBuf\lib\net35
MD ..\NuGet\NServiceKit.Plugins.MsgPack\lib\net40

COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.dll ..\NuGet\NServiceKit\lib\net35
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.pdb ..\NuGet\NServiceKit\lib\net35
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.xml ..\NuGet\NServiceKit\lib\net35
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.ServiceInterface.* ..\NuGet\NServiceKit\lib\net35

REM COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.dll ..\NuGet\NServiceKit\lib\net40
REM COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.pdb ..\NuGet\NServiceKit\lib\net40
REM COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.xml ..\NuGet\NServiceKit\lib\net40
REM COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.ServiceInterface.* ..\NuGet\NServiceKit\lib\net40

COPY ..\src\NServiceKit.Razor\bin\%BUILD%\NServiceKit.Razor.* ..\NuGet\NServiceKit.Razor\lib\net40

COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.Common.* ..\NuGet\NServiceKit.Common\lib\net35
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.Interfaces.* ..\NuGet\NServiceKit.Common\lib\net35

COPY ..\src\NServiceKit.FluentValidation.Mvc3\bin\%BUILD%\NServiceKit.FluentValidation.Mvc3.* ..\NuGet\NServiceKit.Mvc\lib\net40
COPY ..\src\NServiceKit.FluentValidation.Mvc3\bin\%BUILD%\NServiceKit.FluentValidation.Mvc3.* ..\NuGet\NServiceKit.Mvc\lib\net40

COPY ..\src\NServiceKit.Authentication.OpenId\bin\%BUILD%\NServiceKit.Authentication.OpenId.* ..\NuGet\NServiceKit.Authentication.OpenId\lib\net35

COPY ..\src\NServiceKit.Authentication.OAuth2\bin\%BUILD%\NServiceKit.Authentication.OAuth2.* ..\NuGet\NServiceKit.Authentication.OAuth2\lib\net35

COPY ..\src\NServiceKit.Plugins.ProtoBuf\bin\%BUILD%\NServiceKit.Plugins.ProtoBuf.* ..\NuGet\NServiceKit.Plugins.ProtoBuf\lib\net35

COPY ..\lib\MsgPack.dll ..\NuGet\NServiceKit.Plugins.MsgPack\lib\net40
COPY ..\src\NServiceKit.Plugins.MsgPack\bin\%BUILD%\NServiceKit.Plugins.MsgPack.* ..\NuGet\NServiceKit.Plugins.MsgPack\lib\net40

RMDIR ..\NuGet\NServiceKit.Api.Swagger\content\swagger-ui /s /q
MD ..\NuGet\NServiceKit.Api.Swagger\content\swagger-ui
COPY ..\src\NServiceKit.Api.Swagger\bin\%BUILD%\NServiceKit.Api.Swagger.* ..\NuGet\NServiceKit.Api.Swagger\lib\net35
XCOPY /E ..\src\NServiceKit.Api.Swagger\swagger-ui ..\NuGet\NServiceKit.Api.Swagger\content\swagger-ui

COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\*.* ..\..\chaweet\api\lib

COPY ..\src\NServiceKit.Razor\bin\%BUILD%\*.* ..\..\NServiceKit.Examples\lib
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\*.* ..\..\NServiceKit.Examples\lib
COPY ..\src\NServiceKit\bin\%BUILD%\*.* ..\..\NServiceKit.Contrib\lib
COPY ..\src\NServiceKit\bin\%BUILD%\*.* ..\..\NServiceKit.RedisWebServices\lib
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.ServiceInterface.* ..\..\NServiceKit.RedisWebServices\lib

COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Interfaces.dll ..\..\NServiceKit.Redis\lib
COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Text.* ..\..\NServiceKit.Redis\lib
COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Common.* ..\..\NServiceKit.Redis\lib
COPY ..\src\NServiceKit.ServiceInterface\bin\%BUILD%\NServiceKit.* ..\..\NServiceKit.Redis\lib\tests
COPY ..\tests\NServiceKit.Common.Tests\bin\%BUILD%\NServiceKit.Common.Tests.* ..\..\NServiceKit.Redis\lib\tests
COPY ..\tests\NServiceKit.Messaging.Tests\bin\%BUILD%\NServiceKit.Messaging.Tests.* ..\..\NServiceKit.Redis\lib\tests

COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Interfaces.dll ..\..\NServiceKit.OrmLite\lib
COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Text.dll ..\..\NServiceKit.OrmLite\lib
COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Text.pdb ..\..\NServiceKit.OrmLite\lib
COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Common.dll ..\..\NServiceKit.OrmLite\lib
COPY ..\src\NServiceKit\bin\%BUILD%\NServiceKit.Common.pdb ..\..\NServiceKit.OrmLite\lib
COPY ..\tests\NServiceKit.Common.Tests\bin\%BUILD%\NServiceKit.Common.Tests.* ..\..\NServiceKit.OrmLite\lib\tests
