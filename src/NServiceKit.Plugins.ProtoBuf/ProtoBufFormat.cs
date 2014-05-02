using System;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Plugins.ProtoBuf
{
    /// <summary>A prototype buffer format.</summary>
	public class ProtoBufFormat : IPlugin, IProtoBufPlugin
	{
        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
		public void Register(IAppHost appHost)
		{
		    appHost.ContentTypeFilters.Register(ContentType.ProtoBuf, Serialize, Deserialize);
		}

	    private static RuntimeTypeModel model;

        /// <summary>Gets the model.</summary>
        ///
        /// <value>The model.</value>
        public static RuntimeTypeModel Model
        {
            get { return model ?? (model = TypeModel.Create()); }
        }

        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="dto">           The dto.</param>
        /// <param name="outputStream">  Stream to write data to.</param>
        public static void Serialize(IRequestContext requestContext, object dto, Stream outputStream)
        {
            Model.Serialize(outputStream, dto);
        }

        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="type">      The type.</param>
        /// <param name="fromStream">from stream.</param>
        ///
        /// <returns>An object.</returns>
	    public static object Deserialize(Type type, Stream fromStream)
	    {
	        var obj = Model.Deserialize(fromStream, null, type);
	        return obj;
	    }
	}
}
