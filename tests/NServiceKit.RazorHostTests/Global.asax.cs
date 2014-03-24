using System;
using NServiceKit.VirtualPath;

namespace NServiceKit.RazorHostTests
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            new AppHost().Init();

            foreach(var resource in typeof(AppHost).Assembly.GetManifestResourceNames())
            {
                var tokens = resource.TokenizeResourcePath();
            }
        }
    }   
}
