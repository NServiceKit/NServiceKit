using System;

namespace NServiceKit.AuthWeb.Tests
{
    /// <summary>A global.</summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>Event handler. Called by Application for start events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            new AppHost().Init();
        }
    }
}