using System.IO;

namespace NServiceKit.Service
{
	public interface IStreamWriter
	{
		void WriteTo(Stream responseStream);
	}
}