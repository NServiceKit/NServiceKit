using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Razor.Tests
{
    /// <summary>A hello request.</summary>
    public class HelloRequest
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }
    /// <summary>A hello response.</summary>
    public class HelloResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }
    }

    //https://github.com/NServiceKit/NServiceKit/wiki/New-Api
    public class HelloService : ServiceInterface.Service, IAny<HelloRequest>
    {
        //public HelloResponse Any( Hello h )
        //{
        //    //return new HelloResponse { Result = "Hello, " + h.Name };
        //    return h;
        //}

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(HelloRequest request)
        {
            //return new HelloResponse { Result = "Hello, " + request.Name };
            return new { Foo = "foo", Password = "pwd", Pw2 = "222", FooMasta = new { Charisma = 10, Mula = 10000000000, Car = "911Turbo" } };
        }
    }


    /// <summary>A foo request.</summary>
    public class FooRequest
    {
        /// <summary>Gets or sets the what to say.</summary>
        ///
        /// <value>The what to say.</value>
        public string WhatToSay { get; set; }
    }

    /// <summary>A foo response.</summary>
    public class FooResponse
    {
        /// <summary>Gets or sets the foo said.</summary>
        ///
        /// <value>The foo said.</value>
        public string FooSaid { get; set; }
    }

    /// <summary>A default view foo request.</summary>
    public class DefaultViewFooRequest
    {
        /// <summary>Gets or sets the what to say.</summary>
        ///
        /// <value>The what to say.</value>
        public string WhatToSay { get; set; }
    }

    /// <summary>A default view foo response.</summary>
    public class DefaultViewFooResponse
    {
        /// <summary>Gets or sets the foo said.</summary>
        ///
        /// <value>The foo said.</value>
        public string FooSaid { get; set; }
    }

    /// <summary>A controller for handling fooes.</summary>
    public class FooController : ServiceInterface.Service, IGet<FooRequest>, IPost<FooRequest>
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(FooRequest request)
        {
            return new FooResponse { FooSaid = string.Format("GET: {0}", request.WhatToSay) };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(FooRequest request)
        {
            return new FooResponse { FooSaid = string.Format("POST: {0}", request.WhatToSay) };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        [DefaultView("DefaultViewFoo")]
        public object Get(DefaultViewFooRequest request)
        {
            if (request.WhatToSay == "redirect")
            {
                return HttpResult.Redirect("/");
            }
            return new DefaultViewFooResponse { FooSaid = string.Format("GET: {0}", request.WhatToSay) };
        }
    }
}
