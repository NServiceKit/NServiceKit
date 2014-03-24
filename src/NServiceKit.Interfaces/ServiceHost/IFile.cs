using System.IO;

namespace NServiceKit.ServiceHost
{
	public interface IFile
	{
		string FileName { get; }
		long ContentLength { get; }
		string ContentType { get; }
		Stream InputStream { get; }
	}
}