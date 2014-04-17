SET NUGET=..\src\.nuget\nuget
%NUGET% pack NServiceKit\NServiceKit.nuspec -symbols
%NUGET% pack NServiceKit.Common\NServiceKit.common.nuspec -symbols
%NUGET% pack NServiceKit.Mvc\NServiceKit.mvc.nuspec -symbols
%NUGET% pack NServiceKit.Api.Swagger\NServiceKit.api.swagger.nuspec -symbols
%NUGET% pack NServiceKit.Razor\NServiceKit.razor.nuspec -symbols

%NUGET% pack NServiceKit.Host.AspNet\NServiceKit.host.aspnet.nuspec
%NUGET% pack NServiceKit.Host.Mvc\NServiceKit.host.mvc.nuspec
%NUGET% pack NServiceKit.Client.Silverlight\NServiceKit.client.silverlight.nuspec

%NUGET% pack NServiceKit.Authentication.OpenId\NServiceKit.authentication.openid.nuspec -symbols
%NUGET% pack NServiceKit.Authentication.OAuth2\NServiceKit.authentication.oauth2.nuspec -symbols
%NUGET% pack NServiceKit.Plugins.ProtoBuf\NServiceKit.plugins.protobuf.nuspec -symbols
%NUGET% pack NServiceKit.Plugins.MsgPack\NServiceKit.plugins.msgpack.nuspec -symbols

