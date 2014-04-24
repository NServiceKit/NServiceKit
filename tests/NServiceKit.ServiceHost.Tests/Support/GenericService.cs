using System.Runtime.Serialization;
using System.Reflection;
using System;

namespace NServiceKit.ServiceHost.Tests.Support
{
    public class Generic1 { }

    public class Generic1Response
    {
        public string Data { get; set; }
    }

    public class Generic2 { }

    [DataContract]
    public class Generic3<T>
    {
        public T Data { get; set; }
    }

	public class GenericService<T> : ServiceInterface.Service
	{
        public Generic1Response Any(T request)
        {
            return new Generic1Response() { Data = request.GetType().FullName };
        }
    }
}
