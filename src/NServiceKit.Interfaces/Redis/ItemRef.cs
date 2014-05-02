namespace NServiceKit.Redis
{
    /// <summary>An item reference.</summary>
    public class ItemRef
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the item.</summary>
        ///
        /// <value>The item.</value>
        public string Item { get; set; }
    }
}