namespace NServiceKit.Common.Web
{
    /// <summary>An end point.</summary>
    public class EndPoint
    {
        /// <summary>Gets the host.</summary>
        ///
        /// <value>The host.</value>
        public string Host { get; private set; }

        /// <summary>Gets the port.</summary>
        ///
        /// <value>The port.</value>
        public int Port { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.EndPoint class.</summary>
        ///
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public EndPoint(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}