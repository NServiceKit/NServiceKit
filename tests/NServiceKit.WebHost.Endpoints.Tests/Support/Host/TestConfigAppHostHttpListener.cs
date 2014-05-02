using System.Runtime.Serialization;
using Funq;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Host
{
    /// <summary>A bcl dto.</summary>
	[DataContract]
	[Route("/login/{UserName}/{Password}")]
	public class BclDto
	{
        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
		[DataMember(Name = "uname")]
		public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
		[DataMember(Name = "pwd")]
		public string Password { get; set; }
	}

    /// <summary>A bcl dto response.</summary>
	[DataContract]
	public class BclDtoResponse
	{
        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
		[DataMember(Name = "uname")]
		public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
		[DataMember(Name = "pwd")]
		public string Password { get; set; }
	}

    /// <summary>A bcl dto service.</summary>
	public class BclDtoService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
	    public object Any(BclDto request)
		{
			return new BclDtoResponse
			{
				UserName = request.UserName,
				Password = request.Password
			};
		}
	}

    /// <summary>A test configuration application host HTTP listener.</summary>
	public class TestConfigAppHostHttpListener
		: AppHostHttpListenerBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.TestConfigAppHostHttpListener class.</summary>
		public TestConfigAppHostHttpListener()
			: base("TestConfigAppHost Service", typeof(BclDto).Assembly)
		{
		}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		public override void Configure(Container container)
		{
			SetConfig(new EndpointHostConfig
			{
				UseBclJsonSerializers = true,
			});
		}
	}
}