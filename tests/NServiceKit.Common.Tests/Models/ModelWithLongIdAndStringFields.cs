namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with long identifier and string fields.</summary>
	public class ModelWithLongIdAndStringFields
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public long Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the identifier of the album.</summary>
        ///
        /// <value>The identifier of the album.</value>
		public string AlbumId { get; set; }

        /// <summary>Gets or sets the name of the album.</summary>
        ///
        /// <value>The name of the album.</value>
		public string AlbumName { get; set; }
	}
}