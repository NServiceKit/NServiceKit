using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.ServiceHost.Tests.Support;
using Funq;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>An i/o c tests.</summary>
	[TestFixture]
	public class IoCTests
	{
        /// <summary>Can automatic wire types dynamically with expressions.</summary>
		[Test]
		public void Can_AutoWire_types_dynamically_with_expressions()
		{
			var serviceType = typeof(AutoWireService);

			var container = new Container();
			container.Register<IFoo>(c => new Foo());
			container.Register<IBar>(c => new Bar());
			container.Register<int>(c => 100);

			container.RegisterAutoWiredType(serviceType);

			var service = container.Resolve<AutoWireService>();

			Assert.That(service.Foo, Is.Not.Null);
			Assert.That(service.Bar, Is.Not.Null);
			Assert.That(service.Count, Is.EqualTo(0));
		}

        /// <summary>A test.</summary>
		public class Test
		{
            /// <summary>Gets or sets the foo.</summary>
            ///
            /// <value>The foo.</value>
			public IFoo Foo { get; set; }

            /// <summary>Gets or sets the bar.</summary>
            ///
            /// <value>The bar.</value>
			public IBar Bar { get; set; }

            /// <summary>Gets or sets the foo 2.</summary>
            ///
            /// <value>The foo 2.</value>
			public IFoo Foo2 { get; set; }

            /// <summary>Gets or sets the names.</summary>
            ///
            /// <value>The names.</value>
			public IEnumerable<string> Names { get; set; }

            /// <summary>Gets or sets the age.</summary>
            ///
            /// <value>The age.</value>
			public int Age { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.IoCTests.Test class.</summary>
			public Test()
			{
				this.Foo2 = new Foo2();
				this.Names = new List<string>() { "Steffen", "Demis" };
			}
		}

        /// <summary>Can automatic wire existing instance.</summary>
		[Test]
		public void Can_AutoWire_Existing_Instance()
		{
			var test = new Test();

			var container = new Container();
			container.Register<IFoo>(c => new Foo());
			container.Register<IBar>(c => new Bar());
			container.Register<int>(c => 10);

			container.AutoWire(test);

			Assert.That(test.Foo, Is.Not.Null);
			Assert.That(test.Bar, Is.Not.Null);
			Assert.That(test.Foo2 as Foo, Is.Null);
			Assert.That(test.Names, Is.Not.Null);
			Assert.That(test.Age, Is.EqualTo(0));
		}

        /// <summary>A dependency with built in types.</summary>
        public class DependencyWithBuiltInTypes
        {
            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.IoCTests.DependencyWithBuiltInTypes class.</summary>
            public DependencyWithBuiltInTypes()
            {
                this.String = "A String";
                this.Age = 27;
            }

            /// <summary>Gets or sets the foo.</summary>
            ///
            /// <value>The foo.</value>
            public IFoo Foo { get; set; }

            /// <summary>Gets or sets the bar.</summary>
            ///
            /// <value>The bar.</value>
            public IBar Bar { get; set; }

            /// <summary>Gets or sets the string.</summary>
            ///
            /// <value>The string.</value>
            public string String { get; set; }

            /// <summary>Gets or sets the age.</summary>
            ///
            /// <value>The age.</value>
            public int Age { get; set; }            
        }

        /// <summary>Does not automatic wire built in bcl and value types.</summary>
	    [Test]
	    public void Does_not_AutoWire_BuiltIn_BCL_and_ValueTypes()
	    {
            var container = new Container();
            container.Register<IFoo>(c => new Foo());
            container.Register<IBar>(c => new Bar());

            //Should not be autowired
            container.Register<string>(c => "Replaced String");
            container.Register<int>(c => 99);

            container.RegisterAutoWired<DependencyWithBuiltInTypes>();

	        var test = container.Resolve<DependencyWithBuiltInTypes>();
            Assert.That(test.Foo, Is.Not.Null);
            Assert.That(test.Bar, Is.Not.Null);
            Assert.That(test.String, Is.EqualTo("A String"));
            Assert.That(test.Age, Is.EqualTo(27));
	    }
	}
}
