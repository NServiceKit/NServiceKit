using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Attribute for automatic increment.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AutoIncrementAttribute : Attribute
	{
	}
}