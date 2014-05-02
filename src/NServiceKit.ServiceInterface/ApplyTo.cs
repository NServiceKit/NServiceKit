using System;
using System.Collections.Generic;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Bitfield of flags for specifying ApplyTo.</summary>
    [Flags]
    public enum ApplyTo
    {
        /// <summary>A binary constant representing the none flag.</summary>
        None = 0,

        /// <summary>A binary constant representing all flag.</summary>
        All = int.MaxValue,

        /// <summary>A binary constant representing the get flag.</summary>
        Get = 1 << 0,

        /// <summary>A binary constant representing the post flag.</summary>
        Post = 1 << 1,

        /// <summary>A binary constant representing the put flag.</summary>
        Put = 1 << 2,

        /// <summary>A binary constant representing the delete flag.</summary>
        Delete = 1 << 3,

        /// <summary>A binary constant representing the patch flag.</summary>
        Patch = 1 << 4,

        /// <summary>A binary constant representing the options flag.</summary>
        Options = 1 << 5,

        /// <summary>A binary constant representing the head flag.</summary>
        Head = 1 << 6,

        /// <summary>A binary constant representing the connect flag.</summary>
        Connect = 1 << 7,

        /// <summary>A binary constant representing the trace flag.</summary>
        Trace = 1 << 8,

        /// <summary>A binary constant representing the property find flag.</summary>
        PropFind = 1 << 9,

        /// <summary>A binary constant representing the property patch flag.</summary>
        PropPatch = 1 << 10,

        /// <summary>A binary constant representing the mk col flag.</summary>
        MkCol = 1 << 11,

        /// <summary>A binary constant representing the copy flag.</summary>
        Copy = 1 << 12,

        /// <summary>A binary constant representing the move flag.</summary>
        Move = 1 << 13,

        /// <summary>A binary constant representing the lock flag.</summary>
        Lock = 1 << 14,

        /// <summary>A binary constant representing the un lock flag.</summary>
        UnLock = 1 << 15,

        /// <summary>A binary constant representing the report flag.</summary>
        Report = 1 << 16,

        /// <summary>A binary constant representing the check out flag.</summary>
        CheckOut = 1 << 17,

        /// <summary>A binary constant representing the check in flag.</summary>
        CheckIn = 1 << 18,

        /// <summary>A binary constant representing the un check out flag.</summary>
        UnCheckOut = 1 << 19,

        /// <summary>A binary constant representing the mk work space flag.</summary>
        MkWorkSpace = 1 << 20,

        /// <summary>A binary constant representing the update flag.</summary>
        Update = 1 << 21,

        /// <summary>A binary constant representing the label flag.</summary>
        Label = 1 << 22,

        /// <summary>A binary constant representing the merge flag.</summary>
        Merge = 1 << 23,

        /// <summary>A binary constant representing the mk activity flag.</summary>
        MkActivity = 1 << 24,

        /// <summary>A binary constant representing the order patch flag.</summary>
        OrderPatch = 1 << 25,

        /// <summary>A binary constant representing the ACL flag.</summary>
        Acl = 1 << 26,

        /// <summary>A binary constant representing the search flag.</summary>
        Search = 1 << 27,

        /// <summary>A binary constant representing the version control flag.</summary>
        VersionControl = 1 << 28,

        /// <summary>A binary constant representing the base line control flag.</summary>
        BaseLineControl = 1 << 29,
    }

    /// <summary>An apply to utilities.</summary>
    public static class ApplyToUtils
    {
        static ApplyToUtils()
        {
            var map = new Dictionary<string, ApplyTo>();
            foreach (var entry in ApplyToVerbs)
            {
                map[entry.Value] = entry.Key;
            }
            VerbsApplyTo = map;
        }

        /// <summary>The verbs apply to.</summary>
        public static Dictionary<string, ApplyTo> VerbsApplyTo;

        /// <summary>The apply to verbs.</summary>
        public static readonly Dictionary<ApplyTo,string> ApplyToVerbs = new Dictionary<ApplyTo, string> {
            {ApplyTo.Get, HttpMethods.Get},
            {ApplyTo.Post, HttpMethods.Post},
            {ApplyTo.Put, HttpMethods.Put},
            {ApplyTo.Delete, HttpMethods.Delete},
            {ApplyTo.Patch, HttpMethods.Patch},
            {ApplyTo.Options, HttpMethods.Options},
            {ApplyTo.Head, HttpMethods.Head},
            {ApplyTo.Connect, "CONNECT"},
            {ApplyTo.Trace, "TRACE"},
            {ApplyTo.PropFind, "PROPFIND"},
            {ApplyTo.PropPatch, "PROPPATCH"},
            {ApplyTo.MkCol, "MKCOL"},
            {ApplyTo.Copy, "COPY"},
            {ApplyTo.Move, "MOVE"},
            {ApplyTo.Lock, "LOCK"},
            {ApplyTo.UnLock, "UNLOCK"},
            {ApplyTo.Report, "REPORT"},
            {ApplyTo.CheckOut, "CHECKOUT"},
            {ApplyTo.CheckIn, "CHECKIN"},
            {ApplyTo.UnCheckOut, "UNCHECKOUT"},
            {ApplyTo.MkWorkSpace, "MKWORKSPACE"},
            {ApplyTo.Update, "UPDATE"},
            {ApplyTo.Label, "LABEL"},
            {ApplyTo.Merge, "MERGE"},
            {ApplyTo.MkActivity, "MKACTIVITY"},
            {ApplyTo.OrderPatch, "ORDERPATCH"},
            {ApplyTo.Acl, "ACL"},
            {ApplyTo.Search, "SEARCH"},
            {ApplyTo.VersionControl, "VERSION-CONTROL"},
            {ApplyTo.BaseLineControl, "BASELINE-CONTROL"},
        };

        /// <summary>An IHttpRequest extension method that HTTP method as apply to.</summary>
        ///
        /// <param name="req">The req to act on.</param>
        ///
        /// <returns>An ApplyTo.</returns>
        public static ApplyTo HttpMethodAsApplyTo(this IHttpRequest req)
        {
            ApplyTo applyTo;
            return VerbsApplyTo.TryGetValue(req.HttpMethod, out applyTo)
                ? applyTo
                : ApplyTo.None;
        }
    }
}
