namespace NServiceKit.ServiceHost.Tests.Formats
{
    /// <summary>A text block utilities.</summary>
	public static class TextBlockUtils
	{
        /// <summary>A string extension method that replace foreach.</summary>
        ///
        /// <param name="tempalte">   The tempalte to act on.</param>
        /// <param name="replaceWith">The replace with.</param>
        ///
        /// <returns>A string.</returns>
		public static string ReplaceForeach(this string tempalte, string replaceWith)
		{
			var startPos = tempalte.IndexOf("@foreach");
			var endPos = tempalte.IndexOf("}", startPos);

			var expected = tempalte.Substring(0, startPos)
			               + replaceWith
			               + tempalte.Substring(endPos + 1);

			return expected;
		}
	}
}