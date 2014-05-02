using System;

namespace NServiceKit.Messaging
{
    /// <summary>Bitfield of flags for specifying MessageOption.</summary>
    [Flags]
    public enum MessageOption : int
    {
        /// <summary>A binary constant representing the none flag.</summary>
        None = 0,

        /// <summary>A binary constant representing all flag.</summary>
        All = int.MaxValue,


        /// <summary>A binary constant representing the notify one way flag.</summary>
        NotifyOneWay = 1 << 0,
    }
}