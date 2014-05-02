using System.Collections.Generic;
using System.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Templates
{
    /// <summary>A wsdl template base.</summary>
	public abstract class WsdlTemplateBase
	{
        /// <summary>Gets or sets the XSD.</summary>
        ///
        /// <value>The XSD.</value>
		public string Xsd { get; set; }

        /// <summary>Gets or sets the name of the service.</summary>
        ///
        /// <value>The name of the service.</value>
		public string ServiceName { get; set; }

        /// <summary>Gets or sets a list of names of the reply operations.</summary>
        ///
        /// <value>A list of names of the reply operations.</value>
        public IList<string> ReplyOperationNames { get; set; }

        /// <summary>Gets or sets a list of names of the one way operations.</summary>
        ///
        /// <value>A list of names of the one way operations.</value>
        public IList<string> OneWayOperationNames { get; set; }

        /// <summary>Gets or sets URI of the reply endpoint.</summary>
        ///
        /// <value>The reply endpoint URI.</value>
		public string ReplyEndpointUri { get; set; }

        /// <summary>Gets or sets URI of the one way endpoint.</summary>
        ///
        /// <value>The one way endpoint URI.</value>
		public string OneWayEndpointUri { get; set; }

        /// <summary>Gets the name of the wsdl.</summary>
        ///
        /// <value>The name of the wsdl.</value>
		public abstract string WsdlName { get; }

        /// <summary>Gets the reply messages template.</summary>
        ///
        /// <value>The reply messages template.</value>
		protected virtual string ReplyMessagesTemplate
		{
			get
			{
				return
	@"<wsdl:message name=""{0}In"">
        <wsdl:part name=""par"" element=""tns:{0}"" />
    </wsdl:message>
    <wsdl:message name=""{0}Out"">
        <wsdl:part name=""par"" element=""tns:{0}Response"" />
    </wsdl:message>";
			}
		}

        /// <summary>Gets the one way messages template.</summary>
        ///
        /// <value>The one way messages template.</value>
		protected virtual string OneWayMessagesTemplate
		{
			get
			{
				return
	@"<wsdl:message name=""{0}In"">
        <wsdl:part name=""par"" element=""tns:{0}"" />
    </wsdl:message>";
			}
		}

        /// <summary>Gets the reply operations template.</summary>
        ///
        /// <value>The reply operations template.</value>
		protected virtual string ReplyOperationsTemplate
		{
			get
			{
				return
	@"<wsdl:operation name=""{0}"">
        <wsdl:input message=""svc:{0}In"" />
        <wsdl:output message=""svc:{0}Out"" />
    </wsdl:operation>";
			}
		}

        /// <summary>Gets the one way operations template.</summary>
        ///
        /// <value>The one way operations template.</value>
		protected virtual string OneWayOperationsTemplate
		{
			get
			{
				return
	@"<wsdl:operation name=""{0}"">
        <wsdl:input message=""svc:{0}In"" />
    </wsdl:operation>";
			}
		}

        /// <summary>Gets the reply actions template.</summary>
        ///
        /// <value>The reply actions template.</value>
		protected virtual string ReplyActionsTemplate
		{
			get
			{
				return
	@"<wsdl:operation name=""{1}"">
      <soap:operation soapAction=""{0}{1}"" style=""document"" />
      <wsdl:input>
        <soap:body use=""literal"" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use=""literal"" />
      </wsdl:output>
    </wsdl:operation>";
			}
		}

        /// <summary>Gets the one way actions template.</summary>
        ///
        /// <value>The one way actions template.</value>
		protected virtual string OneWayActionsTemplate
		{
			get
			{
				return
	@"<wsdl:operation name=""{1}"">
      <soap:operation soapAction=""{0}{1}"" style=""document"" />
      <wsdl:input>
        <soap:body use=""literal"" />
      </wsdl:input>
    </wsdl:operation>";
			}
		}

        /// <summary>Gets the reply binding container template.</summary>
        ///
        /// <value>The reply binding container template.</value>
		protected abstract string ReplyBindingContainerTemplate { get; }

        /// <summary>Gets the one way binding container template.</summary>
        ///
        /// <value>The one way binding container template.</value>
		protected abstract string OneWayBindingContainerTemplate { get; }

        /// <summary>Gets the reply endpoint URI template.</summary>
        ///
        /// <value>The reply endpoint URI template.</value>
		protected abstract string ReplyEndpointUriTemplate { get; }

        /// <summary>Gets the one way endpoint URI template.</summary>
        ///
        /// <value>The one way endpoint URI template.</value>
		protected abstract string OneWayEndpointUriTemplate { get; }

		private const string Template =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<wsdl:definitions name=""{0}"" 
    targetNamespace=""{10}"" 
    xmlns:svc=""{10}"" 
    xmlns:tns=""{10}"" 
    
    xmlns:wsdl=""http://schemas.xmlsoap.org/wsdl/"" 
    xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" 
    xmlns:soap12=""http://schemas.xmlsoap.org/wsdl/soap12/"" 
    xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" 
    xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"" 
    xmlns:wsam=""http://www.w3.org/2007/05/addressing/metadata"" 
    xmlns:wsa=""http://schemas.xmlsoap.org/ws/2004/08/addressing"" 
    xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" 
    xmlns:wsap=""http://schemas.xmlsoap.org/ws/2004/08/addressing/policy"" 
    xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
    xmlns:msc=""http://schemas.microsoft.com/ws/2005/12/wsdl/contract"" 
    xmlns:wsaw=""http://www.w3.org/2006/05/addressing/wsdl"" 
    xmlns:wsa10=""http://www.w3.org/2005/08/addressing"" 
    xmlns:wsx=""http://schemas.xmlsoap.org/ws/2004/09/mex"">

	<wsdl:types>
		{1}
	</wsdl:types>

	{2}

	{3}

	{4}

	{5}

	{6}
        
	{7}

	{8}

	{9}
	
</wsdl:definitions>";

        /// <summary>Repeater template.</summary>
        ///
        /// <param name="template">  The template.</param>
        /// <param name="dataSource">The data source.</param>
        ///
        /// <returns>A string.</returns>
		public string RepeaterTemplate(string template, IEnumerable<string> dataSource)
		{
			var sb = new StringBuilder();
			foreach (var item in dataSource)
			{
				sb.AppendFormat(template, item);
			}
			return sb.ToString();
		}

        /// <summary>Repeater template.</summary>
        ///
        /// <param name="template">  The template.</param>
        /// <param name="arg0">      The argument 0.</param>
        /// <param name="dataSource">The data source.</param>
        ///
        /// <returns>A string.</returns>
		public string RepeaterTemplate(string template, object arg0, IEnumerable<string> dataSource)
		{
			var sb = new StringBuilder();
			foreach (var item in dataSource)
			{
				sb.AppendFormat(template, arg0, item);
			}
			return sb.ToString();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
            var wsdlSoapActionNamespace = EndpointHost.Config.WsdlSoapActionNamespace;
            if (!wsdlSoapActionNamespace.EndsWith("/"))
                wsdlSoapActionNamespace += '/';
            
            var replyMessages = RepeaterTemplate(this.ReplyMessagesTemplate, this.ReplyOperationNames);
		    var replyOperations  = RepeaterTemplate(this.ReplyOperationsTemplate, this.ReplyOperationNames);
		    var replyServiceName = (ServiceName ?? "SyncReply");
		    replyOperations = "<wsdl:portType name=\"I" + replyServiceName + "\">" + replyOperations + "</wsdl:portType>";
            var replyActions = RepeaterTemplate(this.ReplyActionsTemplate, wsdlSoapActionNamespace, this.ReplyOperationNames);
            var replyBindings = string.Format(this.ReplyBindingContainerTemplate, replyActions, replyServiceName);
            var replyEndpointUri = string.Format(this.ReplyEndpointUriTemplate, ServiceName, this.ReplyEndpointUri, replyServiceName);

            string oneWayMessages = "";
            string oneWayOperations = "";
		    string oneWayBindings = "";
            string oneWayEndpointUri = "";
            if (OneWayOperationNames.Count > 0)
            {
                oneWayMessages = RepeaterTemplate(this.OneWayMessagesTemplate, this.OneWayOperationNames);
                oneWayOperations = RepeaterTemplate(this.OneWayOperationsTemplate, this.OneWayOperationNames);
                var oneWayServiceName = (ServiceName ?? "");
                oneWayOperations = "<wsdl:portType name=\"I" + oneWayServiceName + "OneWay\">" + oneWayOperations + "</wsdl:portType>";
                var oneWayActions=RepeaterTemplate(this.OneWayActionsTemplate, wsdlSoapActionNamespace, this.OneWayOperationNames);
                oneWayBindings = string.Format(this.OneWayBindingContainerTemplate, oneWayActions, oneWayServiceName);
                oneWayEndpointUri = string.Format(this.OneWayEndpointUriTemplate, ServiceName, this.OneWayEndpointUri, oneWayServiceName);
            }

		    var wsdl = string.Format(Template, 
				WsdlName, 
				Xsd, 
				replyMessages, 
				oneWayMessages, 
				replyOperations, 
				oneWayOperations,
				replyBindings, 
				oneWayBindings, 
				replyEndpointUri, 
				oneWayEndpointUri,
				EndpointHost.Config.WsdlServiceNamespace);
            
			return wsdl;
		}
	}
}