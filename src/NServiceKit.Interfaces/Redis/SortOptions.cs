namespace NServiceKit.Redis
{
    /// <summary>
    /// 
    /// </summary>
	public class SortOptions
	{
		/// <summary>
		/// 
		/// </summary>
		public string SortPattern { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? Skip { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? Take { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string GetPattern { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool SortAlpha { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool SortDesc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string StoreAtKey { get; set; }
	}
}