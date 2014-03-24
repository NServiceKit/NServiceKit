using System;

namespace NServiceKit.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AutoIncrementAttribute : Attribute
	{
	}
}