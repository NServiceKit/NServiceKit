using System;
using System.IO;
using System.Net;
using Funq;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.FluentValidation;
using NServiceKit.FluentValidation.Results;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Validation;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A custom validation application host.</summary>
    public class CustomValidationAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.CustomValidationAppHost class.</summary>
        public CustomValidationAppHost() : base("Custom Error", typeof(CustomValidationAppHost).Assembly) {}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            Plugins.Add(new ValidationFeature { ErrorResponseFilter = CustomValidationError });
            container.RegisterValidators(typeof(MyValidator).Assembly);           
        }

        /// <summary>Custom validation error.</summary>
        ///
        /// <param name="validationResult">The validation result.</param>
        /// <param name="errorDto">        The error dto.</param>
        ///
        /// <returns>An object.</returns>
        public static object CustomValidationError(ValidationResult validationResult, object errorDto)
        {
            var firstError = validationResult.Errors[0];
            var dto = new MyCustomErrorDto { code = firstError.ErrorCode, error = firstError.ErrorMessage };
            return new HttpError(dto, HttpStatusCode.BadRequest, dto.code, dto.error);
        }
    }

    /// <summary>my custom error dto.</summary>
    public class MyCustomErrorDto
    {
        /// <summary>Gets or sets the code.</summary>
        ///
        /// <value>The code.</value>
        public string code { get; set; }

        /// <summary>Gets or sets the error.</summary>
        ///
        /// <value>The error.</value>
        public string error { get; set; }
    }

    /// <summary>A custom error.</summary>
    [Route("/customerror")]
    public class CustomError
    {
        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        public int Age { get; set; }

        /// <summary>Gets or sets the company.</summary>
        ///
        /// <value>The company.</value>
        public string Company { get; set; }
    }

    /// <summary>my validator.</summary>
    public class MyValidator : AbstractValidator<CustomError>
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.MyValidator class.</summary>
        public MyValidator()
        {
            RuleFor(x => x.Age).GreaterThan(0);
            RuleFor(x => x.Company).NotEmpty();
        }
    }

    /// <summary>A custom validation service.</summary>
    public class CustomValidationService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(CustomError request)
        {
            return request;
        }
    }

    /// <summary>A custom validation error tests.</summary>
    [TestFixture]
    public class CustomValidationErrorTests
    {
        private CustomValidationAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new CustomValidationAppHost();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can create custom validation error.</summary>
        ///
        /// <exception cref="HTTP">Thrown when a HTTP error condition occurs.</exception>
        [Test]
        public void Can_create_custom_validation_error()
        {
            try
            {
                var response = "{0}/customerror".Fmt(Config.NServiceKitBaseUri).GetJsonFromUrl();
                Assert.Fail("Should throw HTTP Error");
            }
            catch (Exception ex)
            {
                var body = ex.GetResponseBody();
                Assert.That(body, Is.EqualTo("{\"code\":\"GreaterThan\",\"error\":\"'Age' must be greater than '0'.\"}"));
            }
        }
    }

    /// <summary>A web request utilities.</summary>
    public static class WebRequestUtils
    {
        /// <summary>An Exception extension method that gets response body.</summary>
        ///
        /// <param name="ex">The ex to act on.</param>
        ///
        /// <returns>The response body.</returns>
        public static string GetResponseBody(this Exception ex)
        {
            var webEx = ex as WebException;
            if (webEx == null || webEx.Status != WebExceptionStatus.ProtocolError) return null;

            var errorResponse = ((HttpWebResponse)webEx.Response);
            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}