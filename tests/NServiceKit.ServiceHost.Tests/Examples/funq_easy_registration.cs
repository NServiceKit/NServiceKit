using Funq;
using NUnit.Framework;
using NServiceKit.ServiceHost.Tests.TypeFactory;

namespace NServiceKit.ServiceHost.Tests.Examples
{
	/// <summary>
	/// Other examples
	///https://source.db4o.com/db4o/trunk/db4o.net/Libs/compact-3.5/System.Linq.Expressions/Test/System.Linq.Expressions/ExpressionTest_MemberInit.cs
	/// </summary>
	[TestFixture]
	public class FunqEasyRegistration
	{
        /// <summary>Interface for foo.</summary>
		public interface IFoo { }

        /// <summary>A foo.</summary>
		public class Foo : IFoo { }

        /// <summary>Interface for bar.</summary>
		public interface IBar { }

        /// <summary>A bar.</summary>
		public class Bar : IBar
		{
            /// <summary>Gets or sets the foo.</summary>
            ///
            /// <value>The foo.</value>
			public IFoo Foo { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Examples.FunqEasyRegistration.Bar class.</summary>
            ///
            /// <param name="foo">The foo.</param>
			public Bar(IFoo foo)
			{
				Foo = foo;
			}
		}

        /// <summary>Interface for baz.</summary>
		public interface IBaz { }

        /// <summary>A baz.</summary>
		public class Baz : IBaz
		{
            /// <summary>Gets or sets the bar.</summary>
            ///
            /// <value>The bar.</value>
			public IBar Bar { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Examples.FunqEasyRegistration.Baz class.</summary>
            ///
            /// <param name="bar">The bar.</param>
			public Baz(IBar bar)
			{
				Bar = bar;
			}
		}

        /// <summary>Should be able to get service implementation.</summary>
		[Test]
		public void should_be_able_to_get_service_impl()
		{
			var c = new Container();
			c.EasyRegister<IFoo, Foo>();

			Assert.IsInstanceOf<Foo>(c.Resolve<IFoo>());
		}

        /// <summary>Should be able to inject dependency.</summary>
		[Test]
		public void should_be_able_to_inject_dependency()
		{
			var c = new Container();
			c.EasyRegister<IFoo, Foo>();
			c.EasyRegister<IBar, Bar>();

			var bar = c.Resolve<IBar>() as Bar;

			Assert.IsNotNull(bar.Foo);
		}

        /// <summary>Should be able to chain dependencies.</summary>
		[Test]
		public void should_be_able_to_chain_dependencies()
		{
			var c = new Container();
			var testFoo = new Foo();
			c.Register<IFoo>(testFoo);
			c.EasyRegister<IBar, Bar>();
			c.EasyRegister<IBaz, Baz>();
			var baz = c.Resolve<IBaz>() as Baz;

			var bar = baz.Bar as Bar;
			Assert.AreSame(bar.Foo, testFoo);
		}
	}
}