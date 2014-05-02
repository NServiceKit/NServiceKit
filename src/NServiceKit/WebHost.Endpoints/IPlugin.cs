namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>Interface for plugin.</summary>
	public interface IPlugin
	{
        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
		void Register(IAppHost appHost);
	}

    /// <summary>Interface for pre initialise plugin.</summary>
    public interface IPreInitPlugin
    {
        /// <summary>Configures the given application host.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        void Configure(IAppHost appHost);
    }

    /// <summary>Interface for prototype buffer plugin.</summary>
	public interface IProtoBufPlugin { } //Marker for ProtoBuf plugin
    /// <summary>Interface for message pack plugin.</summary>
    public interface IMsgPackPlugin { }  //Marker for MsgPack plugin
    /// <summary>Interface for razor plugin.</summary>
    public interface IRazorPlugin { }    //Marker for MVC Razor plugin
}