using System;

namespace NServiceKit.ServiceHost
{
    /// <summary>Bitfield of flags for specifying EndpointAttributes.</summary>
    [Flags]
    public enum EndpointAttributes : long
    {
        /// <summary>A binary constant representing the none flag.</summary>
        None = 0,


        /// <summary>A binary constant representing any flag.</summary>
        Any = AnyNetworkAccessType | AnySecurityMode | AnyHttpMethod | AnyCallStyle | AnyFormat,

        /// <summary>A binary constant representing any network access type flag.</summary>
        AnyNetworkAccessType = External | Localhost | LocalSubnet,

        /// <summary>A binary constant representing any security mode flag.</summary>
        AnySecurityMode = Secure | InSecure,

        /// <summary>A binary constant representing any HTTP method flag.</summary>
        AnyHttpMethod = HttpHead | HttpGet | HttpPost | HttpPut | HttpDelete | HttpOther,

        /// <summary>A binary constant representing any call style flag.</summary>
        AnyCallStyle = OneWay | Reply,

        /// <summary>A binary constant representing any format flag.</summary>
        AnyFormat = Soap11 | Soap12 | Xml | Json | Jsv | Html | ProtoBuf | Csv | MsgPack | Yaml | FormatOther,

        /// <summary>A binary constant representing any endpoint flag.</summary>
        AnyEndpoint = Http | MessageQueue | Tcp | EndpointOther,

        /// <summary>A binary constant representing the internal network access flag.</summary>
        InternalNetworkAccess = Localhost | LocalSubnet,


        /// <summary>Whether it came from an Internal or External address</summary>
        Localhost = 1 << 0,

        /// <summary>A binary constant representing the local subnet flag.</summary>
        LocalSubnet = 1 << 1,

        /// <summary>A binary constant representing the external flag.</summary>
        External = 1 << 2,


        /// <summary>A binary constant representing the secure flag.</summary>
        Secure = 1 << 3,

        /// <summary>A binary constant representing the in secure flag.</summary>
        InSecure = 1 << 4,

        //HTTP request type
        /// <summary>A binary constant representing the HTTP head flag.</summary>
        HttpHead = 1 << 5,

        /// <summary>A binary constant representing the HTTP get flag.</summary>
        HttpGet = 1 << 6,

        /// <summary>A binary constant representing the HTTP post flag.</summary>
        HttpPost = 1 << 7,

        /// <summary>A binary constant representing the HTTP put flag.</summary>
        HttpPut = 1 << 8,

        /// <summary>A binary constant representing the HTTP delete flag.</summary>
        HttpDelete = 1 << 9,

        /// <summary>A binary constant representing the HTTP patch flag.</summary>
        HttpPatch = 1 << 10,

        /// <summary>A binary constant representing the HTTP options flag.</summary>
        HttpOptions = 1 << 11,

        /// <summary>A binary constant representing the HTTP other flag.</summary>
        HttpOther = 1 << 12,

        //Call Styles
        /// <summary>An enum constant representing the one way option.</summary>
        OneWay = 1 << 13,

        /// <summary>A binary constant representing the reply flag.</summary>
        Reply = 1 << 14,


        /// <summary>An enum constant representing the SOAP 11 option.</summary>
        Soap11 = 1 << 15,

        /// <summary>A binary constant representing the SOAP 12 flag.</summary>
        Soap12 = 1 << 16,
        
        /// <summary>An enum constant representing the XML option.</summary>
        Xml = 1 << 17,

        /// <summary>An enum constant representing the JSON option.</summary>
        Json = 1 << 18,

        /// <summary>An enum constant representing the jsv option.</summary>
        Jsv = 1 << 19,

        /// <summary>An enum constant representing the prototype buffer option.</summary>
        ProtoBuf = 1 << 20,

        /// <summary>An enum constant representing the CSV option.</summary>
        Csv = 1 << 21,

        /// <summary>An enum constant representing the HTML option.</summary>
        Html = 1 << 22,

        /// <summary>An enum constant representing the yaml option.</summary>
        Yaml = 1 << 23,

        /// <summary>An enum constant representing the message pack option.</summary>
        MsgPack = 1 << 24,

        /// <summary>An enum constant representing the format other option.</summary>
        FormatOther = 1 << 25,

        /// <summary>An enum constant representing the HTTP option.</summary>
        Http = 1 << 26,

        /// <summary>An enum constant representing the message queue option.</summary>
        MessageQueue = 1 << 27,

        /// <summary>An enum constant representing the TCP option.</summary>
        Tcp = 1 << 28,

        /// <summary>An enum constant representing the endpoint other option.</summary>
        EndpointOther = 1 << 29,
    }

    /// <summary>Values that represent Network.</summary>
    public enum Network : long
    {
        /// <summary>An enum constant representing the localhost option.</summary>
        Localhost = 1 << 0,

        /// <summary>An enum constant representing the local subnet option.</summary>
        LocalSubnet = 1 << 1,

        /// <summary>An enum constant representing the external option.</summary>
        External = 1 << 2,
    }

    /// <summary>Values that represent Security.</summary>
    public enum Security : long
    {
        /// <summary>An enum constant representing the secure option.</summary>
        Secure = 1 << 3,

        /// <summary>An enum constant representing the in secure option.</summary>
        InSecure = 1 << 4,
    }

    /// <summary>Values that represent Http.</summary>
    public enum Http : long
    {
        /// <summary>An enum constant representing the head option.</summary>
        Head = 1 << 5,

        /// <summary>An enum constant representing the get option.</summary>
        Get = 1 << 6,

        /// <summary>An enum constant representing the post option.</summary>
        Post = 1 << 7,

        /// <summary>An enum constant representing the put option.</summary>
        Put = 1 << 8,

        /// <summary>An enum constant representing the delete option.</summary>
        Delete = 1 << 9,

        /// <summary>An enum constant representing the patch option.</summary>
        Patch = 1 << 10,

        /// <summary>An enum constant representing the options option.</summary>
        Options = 1 << 11,

        /// <summary>An enum constant representing the other option.</summary>
        Other = 1 << 12,
    }

    /// <summary>Values that represent CallStyle.</summary>
    public enum CallStyle : long
    {
        /// <summary>An enum constant representing the one way option.</summary>
        OneWay = 1 << 13,

        /// <summary>An enum constant representing the reply option.</summary>
        Reply = 1 << 14,
    }

    /// <summary>Values that represent Format.</summary>
    public enum Format : long
    {
        /// <summary>An enum constant representing the SOAP 11 option.</summary>
        Soap11 = 1 << 15,

        /// <summary>An enum constant representing the SOAP 12 option.</summary>
        Soap12 = 1 << 16,

        /// <summary>An enum constant representing the XML option.</summary>
        Xml = 1 << 17,

        /// <summary>An enum constant representing the JSON option.</summary>
        Json = 1 << 18,

        /// <summary>An enum constant representing the jsv option.</summary>
        Jsv = 1 << 19,

        /// <summary>An enum constant representing the prototype buffer option.</summary>
        ProtoBuf = 1 << 20,

        /// <summary>An enum constant representing the CSV option.</summary>
        Csv = 1 << 21,

        /// <summary>An enum constant representing the HTML option.</summary>
        Html = 1 << 22,

        /// <summary>An enum constant representing the yaml option.</summary>
        Yaml = 1 << 23,

        /// <summary>An enum constant representing the message pack option.</summary>
        MsgPack = 1 << 24,

        /// <summary>An enum constant representing the other option.</summary>
        Other = 1 << 25,
    }

    /// <summary>Values that represent Endpoint.</summary>
    public enum Endpoint : long
    {
        /// <summary>An enum constant representing the HTTP option.</summary>
        Http = 1 << 26,

        /// <summary>An enum constant representing the message queue option.</summary>
        MessageQueue = 1 << 27,

        /// <summary>An enum constant representing the TCP option.</summary>
        Tcp = 1 << 28,

        /// <summary>An enum constant representing the other option.</summary>
        Other = 1 << 29,
    }

    /// <summary>An endpoint attributes extensions.</summary>
    public static class EndpointAttributesExtensions
    {
        /// <summary>The EndpointAttributes extension method that query if 'attrs' is localhost.</summary>
        ///
        /// <param name="attrs">The attrs to act on.</param>
        ///
        /// <returns>true if localhost, false if not.</returns>
        public static bool IsLocalhost(this EndpointAttributes attrs)
        {
            return (EndpointAttributes.Localhost & attrs) == EndpointAttributes.Localhost;
        }

        /// <summary>The EndpointAttributes extension method that query if 'attrs' is local subnet.</summary>
        ///
        /// <param name="attrs">The attrs to act on.</param>
        ///
        /// <returns>true if local subnet, false if not.</returns>
        public static bool IsLocalSubnet(this EndpointAttributes attrs)
        {
            return (EndpointAttributes.LocalSubnet & attrs) == EndpointAttributes.LocalSubnet;
        }

        /// <summary>The EndpointAttributes extension method that query if 'attrs' is external.</summary>
        ///
        /// <param name="attrs">The attrs to act on.</param>
        ///
        /// <returns>true if external, false if not.</returns>
        public static bool IsExternal(this EndpointAttributes attrs)
        {
            return (EndpointAttributes.External & attrs) == EndpointAttributes.External;
        }

        /// <summary>A Feature extension method that converts a feature to a format.</summary>
        ///
        /// <param name="format">The format to act on.</param>
        ///
        /// <returns>feature as a Format.</returns>
        public static Format ToFormat(this string format)
        {
            try
            {
                return (Format)Enum.Parse(typeof(Format), format.ToUpper().Replace("X-", ""), true);
            }
            catch (Exception)
            {
                return Format.Other;
            }
        }

        /// <summary>A Format extension method that initializes this object from the given from format.</summary>
        ///
        /// <param name="format">The format to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string FromFormat(this Format format)
        {
            var formatStr = format.ToString().ToLower();
            if (format == Format.ProtoBuf || format == Format.MsgPack)
                return "x-" + formatStr;
            return formatStr;
        }

        /// <summary>A Feature extension method that converts a feature to a format.</summary>
        ///
        /// <param name="feature">The feature to act on.</param>
        ///
        /// <returns>feature as a Format.</returns>
        public static Format ToFormat(this Feature feature)
        {
            switch (feature)
            {
                case Feature.Xml:
                    return Format.Xml;
                case Feature.Json:
                    return Format.Json;
                case Feature.Jsv:
                    return Format.Jsv;
                case Feature.Csv:
                    return Format.Csv;
                case Feature.Html:
                    return Format.Html;
                case Feature.MsgPack:
                    return Format.MsgPack;
                case Feature.ProtoBuf:
                    return Format.ProtoBuf;
                case Feature.Soap11:
                    return Format.Soap11;
                case Feature.Soap12:
                    return Format.Soap12;
            }
            return Format.Other;
        }

        /// <summary>A Format extension method that converts a format to a feature.</summary>
        ///
        /// <param name="format">The format to act on.</param>
        ///
        /// <returns>format as a Feature.</returns>
        public static Feature ToFeature(this Format format)
        {
            switch (format)
            {
                case Format.Xml:
                    return Feature.Xml;
                case Format.Json:
                    return Feature.Json;
                case Format.Jsv:
                    return Feature.Jsv;
                case Format.Csv:
                    return Feature.Csv;
                case Format.Html:
                    return Feature.Html;
                case Format.MsgPack:
                    return Feature.MsgPack;
                case Format.ProtoBuf:
                    return Feature.ProtoBuf;
                case Format.Soap11:
                    return Feature.Soap11;
                case Format.Soap12:
                    return Feature.Soap12;
            }
            return Feature.CustomFormat;
        }

        /// <summary>The EndpointAttributes extension method that converts the attributes to a SOAP feature.</summary>
        ///
        /// <param name="attributes">The attributes to act on.</param>
        ///
        /// <returns>attributes as a Feature.</returns>
        public static Feature ToSoapFeature(this EndpointAttributes attributes)
        {
            if ((EndpointAttributes.Soap11 & attributes) == EndpointAttributes.Soap11)
                return Feature.Soap11;
            if ((EndpointAttributes.Soap12 & attributes) == EndpointAttributes.Soap12)
                return Feature.Soap12;            
            return Feature.None;
        }
    }
}