namespace NServiceKit.WebHost.Endpoints.Support.Templates
{
    /// <summary>A SOAP 11 wsdl template.</summary>
	public class Soap11WsdlTemplate : WsdlTemplateBase
	{
        /// <summary>Gets the name of the wsdl.</summary>
        ///
        /// <value>The name of the wsdl.</value>
		public override string WsdlName
		{
			get { return "Soap11"; }
		}

        /// <summary>Gets the reply actions template.</summary>
        ///
        /// <value>The reply actions template.</value>
		protected override string ReplyActionsTemplate
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
		protected override string OneWayActionsTemplate
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
		protected override string ReplyBindingContainerTemplate
		{
			get
			{
				return
    @"<wsdl:binding name=""BasicHttpBinding_I{1}"" type=""svc:I{1}"">
        <soap:binding transport=""http://schemas.xmlsoap.org/soap/http"" />
		{0}
	</wsdl:binding>";
			}
		}

        /// <summary>Gets the one way binding container template.</summary>
        ///
        /// <value>The one way binding container template.</value>
		protected override string OneWayBindingContainerTemplate
		{
			get
			{
				return
    @"<wsdl:binding name=""BasicHttpBinding_I{1}OneWay"" type=""svc:I{1}OneWay"">
        <soap:binding transport=""http://schemas.xmlsoap.org/soap/http"" />
		{0}
	</wsdl:binding>";
			}
		}

        /// <summary>Gets the reply endpoint URI template.</summary>
        ///
        /// <value>The reply endpoint URI template.</value>
		protected override string ReplyEndpointUriTemplate
		{
			get
			{
				return
    @"<wsdl:service name=""{0}SyncReply"">
		<wsdl:port name=""BasicHttpBinding_I{2}"" binding=""svc:BasicHttpBinding_I{2}"">
			<soap:address location=""{1}"" />
		</wsdl:port>
	</wsdl:service>";
			}
		}

        /// <summary>Gets the one way endpoint URI template.</summary>
        ///
        /// <value>The one way endpoint URI template.</value>
		protected override string OneWayEndpointUriTemplate
		{
			get
			{
				return
    @"<wsdl:service name=""{0}AsyncOneWay"">
		<wsdl:port name=""BasicHttpBinding_I{2}OneWay"" binding=""svc:BasicHttpBinding_I{2}OneWay"">
			<soap:address location=""{1}"" />
		</wsdl:port>
	</wsdl:service>";
			}
		}

	}
}