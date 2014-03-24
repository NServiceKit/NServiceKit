using System;

namespace NServiceKit.Configuration
{
	public interface ITypeFactory
	{
		object CreateInstance(Type type);
	}
}