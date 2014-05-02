using System.IO;

namespace NServiceKit.Service
{
    /// <summary>Interface for stream writer.</summary>
	public interface IStreamWriter
	{
        /// <summary>Writes to.</summary>
        ///
        /// <param name="responseStream">The response stream.</param>
		void WriteTo(Stream responseStream);
	}
}