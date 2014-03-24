using System;

namespace NServiceKit.DesignPatterns.Model
{
	public interface IHasUserId
	{
		Guid UserId { get; }
	}
}