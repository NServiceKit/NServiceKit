using System;

namespace NServiceKit.ServiceInterface.ServiceModel
{
	public interface ICacheByDateModified
	{
		DateTime? LastModified { get; }
	}
}