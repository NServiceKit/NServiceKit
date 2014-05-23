using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.Api.Swagger
{
    using NServiceKit.ServiceHost;
    using NServiceKit.ServiceInterface;

    using NServiceKit.Api.Swagger.Models;

    /// <summary>
    /// The swagger ui service. Renders the swagger-ui if <see cref="SwaggerFeature.SwaggerUiEnabled"/> is set to true.
    /// </summary>
    [DefaultRequest(typeof(SwaggerUiRequest))]
    internal class SwaggerUiService : Service
    {
        public SwaggerUiResponse Get(SwaggerUiRequest request)
        {
            return new SwaggerUiResponse();
        }
    }
}
