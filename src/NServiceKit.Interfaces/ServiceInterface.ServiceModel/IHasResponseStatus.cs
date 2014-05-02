namespace NServiceKit.ServiceInterface.ServiceModel
{
	/// <summary>
	/// Contract indication that the Response DTO has a ResponseStatus
	/// </summary>
	public interface IHasResponseStatus
	{
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		ResponseStatus ResponseStatus { get; set; }
	}
}