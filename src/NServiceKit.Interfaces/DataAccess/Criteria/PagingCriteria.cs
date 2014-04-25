namespace NServiceKit.DataAccess.Criteria
{
	/// <summary>
	/// 
	/// </summary>
	public class PagingCriteria : IPagingCriteria
	{
		/// <summary>
		/// 
		/// </summary>
		public uint ResultOffset { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public uint ResultLimit { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resultOffset"></param>
		/// <param name="resultLimit"></param>
		public PagingCriteria(uint resultOffset, uint resultLimit)
		{
			this.ResultOffset = resultOffset;
			this.ResultLimit = resultLimit;
		}
	}
}