using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
    /// <summary>An automatic wire.</summary>
	[DataContract]
	public class AutoWire { }

    /// <summary>An automatic wire response.</summary>
	[DataContract]
	public class AutoWireResponse
	{
        /// <summary>Gets or sets the foo.</summary>
        ///
        /// <value>The foo.</value>
		public IFoo Foo { get; set; }

        /// <summary>Gets or sets the bar.</summary>
        ///
        /// <value>The bar.</value>
		public IBar Bar { get; set; }
	}

    /// <summary>An automatic wire service.</summary>
	public class AutoWireService : ServiceInterface.Service
	{
		private readonly IFoo foo;

        /// <summary>Gets the foo.</summary>
        ///
        /// <value>The foo.</value>
		public IFoo Foo
		{
			get { return foo; }
		}

        /// <summary>Gets or sets the bar.</summary>
        ///
        /// <value>The bar.</value>
		public IBar Bar { get; set; }

        /// <summary>Gets or sets the number of. </summary>
        ///
        /// <value>The count.</value>
		public int Count { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Support.AutoWireService class.</summary>
        ///
        /// <param name="foo">The foo.</param>
		public AutoWireService(IFoo foo)
		{
			this.foo = foo;
		}

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An AutoWireResponse.</returns>
        public AutoWireResponse Any(AutoWire request)
		{
			return new AutoWireResponse { Foo = foo, Bar = Bar };
		}
	}

    /// <summary>A foo.</summary>
	public class Foo : IFoo
	{
	}

    /// <summary>A foo 2.</summary>
	public class Foo2 : IFoo
	{
	}

    /// <summary>Interface for foo.</summary>
	public interface IFoo
	{
	}

    /// <summary>A bar.</summary>
	public class Bar : IBar
	{
	}

    /// <summary>A bar 2.</summary>
	public class Bar2 : IBar
	{
	}

    /// <summary>Interface for bar.</summary>
	public interface IBar
	{
	}
}