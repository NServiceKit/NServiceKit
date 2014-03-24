using System;

namespace NServiceKit
{
	public interface IViewPage
	{
		bool IsCompiled { get; }

		void Compile(bool force=false);
	}
}

