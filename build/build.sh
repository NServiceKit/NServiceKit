#!/bin/bash

MODO=Debug
LIB_DIR=lib
MONO_DIR=lib
#MODO=Release

CURRENT_DIR=`pwd`
cd ../..

function makeMonoDir {
    echo "$1/$MONO_DIR"
	if [ ! -d "$1/$MONO_DIR" ]
	then
		mkdir -p "$1/$MONO_DIR"/tests
	fi
}  

if [ ! -d "$LIB_DIR" ]
then
	mkdir "$LIB_DIR"
fi

function buildComponentAndCopyToNServiceKit {
	xbuild /p:Configuration="$MODO" "$1"/src/"$2"/"$2".csproj
	cp "$1"/src/"$2"/bin/"$MODO"/"$2".dll "$LIB_DIR"
	cp "$LIB_DIR"/"$2".dll NServiceKit/"$MONO_DIR"
}

function buildComponent {
	xbuild /p:Configuration="$MODO" "$1"/src/"$2"/"$2".csproj
	cp "$1"/src/"$2"/bin/"$MODO"/"$2".dll "$LIB_DIR"
}

function buildTestComponent {
	xbuild /p:Configuration="$MODO" "$1"/tests/"$2"/"$2".csproj
	cp "$1"/tests/"$2"/bin/"$MODO"/"$2".dll "$LIB_DIR"
}

function buildNServiceKitBenchmarks {
	xbuild /p:Configuration="$MODO" "$1"/src/"$2"/"$3"/"$3".csproj
	cp "$1"/src/"$2"/"$3"/bin/"$MODO"/"$3".dll "$LIB_DIR"
}

makeMonoDir NServiceKit
makeMonoDir NServiceKit.OrmLite
makeMonoDir NServiceKit.Redis
makeMonoDir NServiceKit.Text


buildComponentAndCopyToNServiceKit NServiceKit.Text NServiceKit.Text
cp "$LIB_DIR"/NServiceKit.Text.dll NServiceKit.OrmLite/"$MONO_DIR"
cp "$LIB_DIR"/NServiceKit.Text.dll NServiceKit.Redis/"$MONO_DIR"
cp "$LIB_DIR"/NServiceKit.Text.dll NServiceKit.Redis/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.Text.dll NServiceKit.Text/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.Text.dll NServiceKit.Logging/"$MONO_DIR"

buildComponentAndCopyToNServiceKit NServiceKit NServiceKit.Interfaces
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.OrmLite/"$MONO_DIR"
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.Redis/"$MONO_DIR"
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.Redis/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.Text/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.Logging/"$MONO_DIR"

buildComponentAndCopyToNServiceKit NServiceKit NServiceKit.Common
cp "$LIB_DIR"/NServiceKit.Common.dll NServiceKit.OrmLite/"$MONO_DIR"
cp "$LIB_DIR"/NServiceKit.Common.dll NServiceKit.Redis/"$MONO_DIR"
cp "$LIB_DIR"/NServiceKit.Common.dll NServiceKit.Redis/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.Common.dll NServiceKit.Text/"$MONO_DIR"/tests/
cp "$LIB_DIR"/NServiceKit.Common.dll NServiceKit.Logging/"$MONO_DIR"

buildComponent NServiceKit NServiceKit
cp "$LIB_DIR"/NServiceKit.dll NServiceKit.Redis/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.dll NServiceKit.Text/"$MONO_DIR"/tests

buildComponentAndCopyToNServiceKit NServiceKit.OrmLite NServiceKit.OrmLite
buildComponentAndCopyToNServiceKit NServiceKit.OrmLite NServiceKit.OrmLite.Sqlite
buildComponentAndCopyToNServiceKit NServiceKit.OrmLite NServiceKit.OrmLite.SqlServer
buildComponent NServiceKit.OrmLite NServiceKit.OrmLite.MySql
buildComponent NServiceKit.OrmLite NServiceKit.OrmLite.PostgreSQL
buildComponent NServiceKit.OrmLite NServiceKit.OrmLite.Firebird
buildComponent NServiceKit.OrmLite NServiceKit.OrmLite.Oracle

cp "$LIB_DIR"/NServiceKit.OrmLite.dll NServiceKit.Text/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.OrmLite.SqlServer.dll NServiceKit.Text/"$MONO_DIR"/tests
cp "$LIB_DIR"/NServiceKit.OrmLite.Sqlite.dll NServiceKit.Text/"$MONO_DIR"/tests

#NServiceKit again 
buildComponent NServiceKit NServiceKit.Authentication.OpenId
buildComponent NServiceKit NServiceKit.Plugins.MsgPack
buildComponent NServiceKit NServiceKit.ServiceInterface
buildComponent NServiceKit NServiceKit.Plugins.ProtoBuf
buildComponent NServiceKit NServiceKit.FluentValidation.Mvc3
buildComponent NServiceKit NServiceKit.Razor2
cp "$LIB_DIR"/NServiceKit.Razor2.dll NServiceKit.Redis/"$MONO_DIR"/tests/
cp "$LIB_DIR"/NServiceKit.ServiceInterface.dll NServiceKit.Redis/"$MONO_DIR"/tests/
cp "$LIB_DIR"/NServiceKit.ServiceInterface.dll NServiceKit.Text/"$MONO_DIR"/tests/


buildComponentAndCopyToNServiceKit NServiceKit.Redis NServiceKit.Redis
cp  "$LIB_DIR"/NServiceKit.Redis.dll NServiceKit.Redis/"$LIB_DIR"/tests
cp  "$LIB_DIR"/NServiceKit.Redis.dll NServiceKit.Text/"$LIB_DIR"/tests

#NServiceKit.Benchmarks
cp "$LIB_DIR"/NServiceKit.dll NServiceKit.Benchmarks/lib
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.Benchmarks/lib

cp "$LIB_DIR"/NServiceKit.Text.dll NServiceKit.Benchmarks/src/Northwind.Benchmarks/Lib
cp "$LIB_DIR"/NServiceKit.Interfaces.dll NServiceKit.Benchmarks/src/Northwind.Benchmarks/Lib

buildNServiceKitBenchmarks NServiceKit.Benchmarks Northwind.Benchmarks Northwind.Common
buildNServiceKitBenchmarks NServiceKit.Benchmarks Northwind.Benchmarks Northwind.Perf
buildNServiceKitBenchmarks NServiceKit.Benchmarks Northwind.Benchmarks Northwind.Benchmarks
buildNServiceKitBenchmarks NServiceKit.Benchmarks Northwind.Benchmarks Northwind.Benchmarks.Console

cp "$LIB_DIR"/Northwind.Common.dll NServiceKit/"$MONO_DIR"/tests/
cp "$LIB_DIR"/Northwind.Common.dll NServiceKit.OrmLite/"$MONO_DIR"/tests/
cp "$LIB_DIR"/Northwind.Common.dll NServiceKit.Redis/"$MONO_DIR"/tests/
cp "$LIB_DIR"/Northwind.Common.dll NServiceKit.Text/"$MONO_DIR"/tests/
cp "$LIB_DIR"/Northwind.Perf.dll NServiceKit.OrmLite/"$MONO_DIR"/tests/


buildTestComponent NServiceKit NServiceKit.Common.Tests
cp  "$LIB_DIR"/NServiceKit.Common.Tests.dll NServiceKit.OrmLite/"$MONO_DIR"/tests
cp  "$LIB_DIR"/NServiceKit.Common.Tests.dll NServiceKit.Redis/"$MONO_DIR"/tests
cp  "$LIB_DIR"/NServiceKit.Common.Tests.dll NServiceKit.Text/"$MONO_DIR"/tests
buildTestComponent NServiceKit NServiceKit.Messaging.Tests
cp  "$LIB_DIR"/NServiceKit.Messaging.Tests.dll NServiceKit.Redis/"$MONO_DIR"/tests
cp  "$LIB_DIR"/NServiceKit.Messaging.Tests.dll NServiceKit.Text/"$MONO_DIR"/tests


#NServiceKit.RazorHostTests : xbuild can not  build it, but monodevelop does it!

#xbuild  NServiceKit/tests/NServiceKit.RazorHostTests/NServiceKit.RazorHostTests.csproj

#Imported project: 
#"/usr/local/lib/mono/xbuild/Microsoft/VisualStudio/v10.0/WebApplications/Microsoft.WebApplication.targets" does not exist.
# there is v9.0 

#NServiceKit.RazorNancyTests : xbuild can not  build it, but monodevelop does it!
#xbuild  NServiceKit/tests/NServiceKit.RazorNancyTests/NServiceKit.RazorNancyTests.csproj

buildTestComponent NServiceKit NServiceKit.ServiceHost.Tests
buildTestComponent NServiceKit NServiceKit.ServiceModel.Tests
#NServiceKit.WebHost.Endpoints.Tests:  warning as error
buildTestComponent NServiceKit NServiceKit.WebHost.Endpoints.Tests
#NServiceKit.WebHost.IntegrationTests
# execute once :
#sudo ln -s /usr/local/lib/mono/xbuild/Microsoft/VisualStudio/v9.0 /usr/local/lib/mono/xbuild/Microsoft/VisualStudio/v10.0
buildTestComponent NServiceKit NServiceKit.WebHost.IntegrationTests
cp  NServiceKit/tests/NServiceKit.WebHost.IntegrationTests/bin/NServiceKit.WebHost.IntegrationTests.dll "$LIB_DIR"


buildTestComponent NServiceKit.Redis NServiceKit.Redis.Tests
buildTestComponent NServiceKit.Text NServiceKit.Text.Tests
#NServiceKit.Text.Tests  // comment public void #Can_Serialize_User_OAuthSession_list()  and public void #Doesnt_serialize_TypeInfo_when_set()


#NServiceKit.Logging.EventLog

#fail: ../../src//.nuget/nuget.targets: Project file could not be imported
#use monodevelop
#xbuild  NServiceKit.Logging/src/NServiceKit.Logging.EventLog/NServiceKit.Logging.EventLog.csproj

#NServiceKit.Logging.Log4Net
#fail: ../../src//.nuget/nuget.targets: Project file could not be imported
#use monodevelop
#xbuild  #NServiceKit.Logging/src/NServiceKit.Logging.Log4Net/NServiceKit.Logging.Log4Net.csproj

cd "$CURRENT_DIR"
