using System;

namespace NServiceKit.ServiceHost
{
    /// <summary>Bitfield of flags for specifying Feature.</summary>
	[Flags]
	public enum Feature : int
	{
        /// <summary>A binary constant representing the none flag.</summary>
		None         = 0,

        /// <summary>A binary constant representing all flag.</summary>
		All          = int.MaxValue,

        /// <summary>A binary constant representing the SOAP flag.</summary>
		Soap         = Soap11 | Soap12,


        /// <summary>A binary constant representing the metadata flag.</summary>
        Metadata         = 1 << 0,

        /// <summary>A binary constant representing the predefined routes flag.</summary>
        PredefinedRoutes = 1 << 1,

        /// <summary>A binary constant representing the request information flag.</summary>
        RequestInfo      = 1 << 2,
        

        /// <summary>A binary constant representing the JSON flag.</summary>
        Json         = 1 << 3,

        /// <summary>A binary constant representing the XML flag.</summary>
		Xml          = 1 << 4,

        /// <summary>A binary constant representing the jsv flag.</summary>
		Jsv          = 1 << 5,

        /// <summary>A binary constant representing the SOAP 11 flag.</summary>
		Soap11       = 1 << 6,

        /// <summary>A binary constant representing the SOAP 12 flag.</summary>
		Soap12       = 1 << 7,

        /// <summary>A binary constant representing the CSV flag.</summary>
		Csv          = 1 << 8,

        /// <summary>A binary constant representing the HTML flag.</summary>
		Html         = 1 << 9,

        /// <summary>A binary constant representing the custom format flag.</summary>
		CustomFormat = 1 << 10,

        /// <summary>A binary constant representing the markdown flag.</summary>
		Markdown     = 1 << 11,

        /// <summary>A binary constant representing the razor flag.</summary>
		Razor        = 1 << 12,

        /// <summary>A binary constant representing the prototype buffer flag.</summary>
		ProtoBuf     = 1 << 13,

        /// <summary>A binary constant representing the message pack flag.</summary>
		MsgPack      = 1 << 14,
	}
}