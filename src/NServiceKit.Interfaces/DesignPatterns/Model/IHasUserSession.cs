using System;

namespace NServiceKit.DesignPatterns.Model
{
	public interface IHasUserSession
	{
		Guid UserId { get; }

		Guid SessionId { get; }
	}
}