using System;

namespace NServiceKit.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVirtualNode
    {
        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>
        /// The directory.
        /// </value>
        IVirtualDirectory Directory { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <value>
        /// The virtual path.
        /// </value>
        string VirtualPath { get; }
        /// <summary>
        /// Gets the real path.
        /// </summary>
        /// <value>
        /// The real path.
        /// </value>
        string RealPath { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is directory.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directory; otherwise, <c>false</c>.
        /// </value>
        bool IsDirectory { get; }
        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        DateTime LastModified { get; }
    }
}
