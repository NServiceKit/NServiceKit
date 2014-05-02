
namespace NServiceKit.Html
{
	internal interface IResolver<T>
	{
        /// <summary>Gets the current.</summary>
        ///
        /// <value>The current.</value>
		T Current { get; }
	}
}
