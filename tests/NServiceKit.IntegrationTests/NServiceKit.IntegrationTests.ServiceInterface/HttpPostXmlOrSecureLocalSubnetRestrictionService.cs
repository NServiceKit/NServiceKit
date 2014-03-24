using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.IntegrationTests.ServiceModel;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class HttpPostXmlOrSecureLocalSubnetRestrictionService
		: TestServiceBase<HttpPostXmlOrSecureLocalSubnetRestriction>
	{
		protected override object Run(HttpPostXmlOrSecureLocalSubnetRestriction request)
		{
			return new HttpPostXmlOrSecureLocalSubnetRestrictionResponse();
		}
	}

}
