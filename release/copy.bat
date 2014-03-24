MD latest\NServiceKit
MD latest\NServiceKit.OrmLite
MD latest\NServiceKit.Redis

COPY ..\NuGet\NServiceKit\lib\net35\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit\lib\net40\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit.Common\lib\net35\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit.Mvc\lib\net40\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit.Authentication.OpenId\lib\net35\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit.Plugins.ProtoBuf\lib\net35\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit.Plugins.MsgPack\lib\net40\*  latest\NServiceKit
COPY ..\NuGet\NServiceKit.Razor2\lib\net40\*  latest\NServiceKit

COPY ..\..\NServiceKit.Text\NuGet\lib\net35\*  latest\NServiceKit
COPY ..\..\NServiceKit.Redis\NuGet\lib\net35\*  latest\NServiceKit
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.SqlServer\lib\*  latest\NServiceKit

MD latest\NServiceKit.OrmLite\Firebird
MD latest\NServiceKit.OrmLite\MySql
MD latest\NServiceKit.OrmLite\PostgreSQL
MD latest\NServiceKit.OrmLite\Sqlite32
MD latest\NServiceKit.OrmLite\Sqlite64
MD latest\NServiceKit.OrmLite\SqlServer
MD latest\NServiceKit.OrmLite\Sqlite32.Mono

COPY ..\..\NServiceKit.Text\NuGet\lib\net35\*  latest\NServiceKit.OrmLite
COPY ..\NuGet\NServiceKit.Common\lib\net35\*  latest\NServiceKit.OrmLite
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Firebird\lib\*  latest\NServiceKit.OrmLite\Firebird
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.MySql\lib\*  latest\NServiceKit.OrmLite\MySql
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.PostgreSQL\lib\*  latest\NServiceKit.OrmLite\PostgreSQL
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite32\lib\*  latest\NServiceKit.OrmLite\Sqlite32
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite64\lib\*  latest\NServiceKit.OrmLite\Sqlite64
COPY ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.SqlServer\lib\*  latest\NServiceKit.OrmLite\SqlServer
COPY ..\..\NServiceKit.OrmLite\src\NServiceKit.OrmLite.Sqlite\bin\Release\NServiceKit.OrmLite.*  latest\NServiceKit.OrmLite\Sqlite32.Mono

