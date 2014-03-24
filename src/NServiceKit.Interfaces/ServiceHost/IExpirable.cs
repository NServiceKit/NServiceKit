using System;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
	public interface IExpirable
	{
		DateTime? LastModified { get; }
	}
}