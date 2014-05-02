using System;
using NServiceKit.Logging.NLogger;

namespace NServiceKit.Razor.Tests
{
    /// <summary>A global.</summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>Event handler. Called by Application for start events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_Start( object sender, EventArgs e )
        {
            Logging.LogManager.LogFactory = new NLogFactory();
            new HelloAppHost().Init();
        }

        /// <summary>Event handler. Called by Session for start events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Session_Start( object sender, EventArgs e )
        {

        }

        /// <summary>Event handler. Called by Application for begin request events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_BeginRequest( object sender, EventArgs e )
        {

        }

        /// <summary>Event handler. Called by Application for authenticate request events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_AuthenticateRequest( object sender, EventArgs e )
        {

        }

        /// <summary>Event handler. Called by Application for error events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_Error( object sender, EventArgs e )
        {

        }

        /// <summary>Event handler. Called by Session for end events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Session_End( object sender, EventArgs e )
        {

        }

        /// <summary>Event handler. Called by Application for end events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_End( object sender, EventArgs e )
        {

        }
    }
}
