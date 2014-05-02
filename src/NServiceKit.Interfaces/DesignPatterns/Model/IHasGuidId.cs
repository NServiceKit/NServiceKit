using System;

namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has unique identifier.</summary>
	public interface IHasGuidId : IHasId<Guid>
	{
	}
}