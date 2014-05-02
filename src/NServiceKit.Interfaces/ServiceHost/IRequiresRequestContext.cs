namespace NServiceKit.ServiceHost
{
	/// <summary>
	/// Implement on services that need access to the RequestContext
	/// </summary>
	public interface IRequiresRequestContext
	{
        /// <summary>Gets or sets a context for the request.</summary>
        ///
        /// <value>The request context.</value>
		IRequestContext RequestContext { get; set; }
	}
}