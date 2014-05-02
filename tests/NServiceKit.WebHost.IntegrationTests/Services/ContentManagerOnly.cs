using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.WebHost.IntegrationTests.Tests;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A content manager only.</summary>
    public class ContentManagerOnly : IReturn<ContentManagerOnlyResponse>
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }
	}

    /// <summary>A content manager only response.</summary>
	public class ContentManagerOnlyResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A content manager only service.</summary>
	[RequiredRole(ManageRolesTests.ContentManager)]
	public class ContentManagerOnlyService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ContentManagerOnly request)
		{
			return new ContentManagerOnlyResponse { Result = "Haz Access" };
		}
	}

    /// <summary>A content permission only.</summary>
    public class ContentPermissionOnly : IReturn<ContentPermissionOnlyResponse>
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }
	}

    /// <summary>A content permission only response.</summary>
	public class ContentPermissionOnlyResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A content permission only service.</summary>
	[RequiredPermission(ManageRolesTests.ContentPermission)]
	public class ContentPermissionOnlyService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ContentPermissionOnly request)
		{
			return new ContentPermissionOnlyResponse { Result = "Haz Access" };
		}
	}
}