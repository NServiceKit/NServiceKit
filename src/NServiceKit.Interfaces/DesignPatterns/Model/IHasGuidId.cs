using System;

namespace NServiceKit.DesignPatterns.Model
{
	public interface IHasGuidId : IHasId<Guid>
	{
	}
}