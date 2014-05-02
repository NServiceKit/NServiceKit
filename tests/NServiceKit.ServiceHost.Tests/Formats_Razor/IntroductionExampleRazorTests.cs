using NServiceKit.Razor;
using NServiceKit.ServiceHost.Tests.Formats;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.VirtualPath;
using NUnit.Framework;
using System.Collections.Generic;

namespace NServiceKit.ServiceHost.Tests.Formats_Razor
{
    /// <summary>A product.</summary>
    public class Product
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Formats_Razor.Product class.</summary>
        public Product() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Formats_Razor.Product class.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="price">The price.</param>
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        /// <summary>Gets or sets the identifier of the product.</summary>
        ///
        /// <value>The identifier of the product.</value>
        public int ProductID { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the price.</summary>
        ///
        /// <value>The price.</value>
        public decimal Price { get; set; }
    }

    /// <summary>An introduction example razor tests.</summary>
    [TestFixture]
    public class IntroductionExampleRazorTests : RazorTestBase
    {
        private List<Product> products;
        object productArgs;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.products = new List<Product> {
				new Product("Pen", 1.99m),
				new Product("Glass", 9.99m),
				new Product("Book", 14.99m),
				new Product("DVD", 11.99m),
			};
            productArgs = new { products = products };
        }

        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            RazorFormat.Instance = null;
            RazorFormat = new RazorFormat
            {
                PageBaseType = typeof(CustomRazorBasePage<>),
                VirtualPathProvider = new InMemoryVirtualPathProvider(new BasicAppHost()),
            }.Init();
        }

        /// <summary>Basic razor example.</summary>
        [Test]
        public void Basic_Razor_Example()
        {
            var template =
@"<h1>Razor Example</h1>

<h3>Hello @Model.name, the year is @DateTime.Now.Year</h3>

<p>Checkout <a href=""/Product/Details/@Model.productId"">this product</a></p>
".NormalizeNewLines();

            var expectedHtml =
@"<h1>Razor Example</h1>

<h3>Hello Demis, the year is 2014</h3>

<p>Checkout <a href=""/Product/Details/10"">this product</a></p>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, model: new { name = "Demis", productId = 10 });

            html.Print();
            Assert.That(html, Is.EqualTo(expectedHtml));
        }


        /// <summary>Simple loop.</summary>
        [Test]
        public void Simple_loop()
        {
            var template = @"<ul>
@foreach (var p in Model.products) {
	<li>@p.Name: (@p.Price)</li>
}
</ul>
".NormalizeNewLines();

            var expectedHtml =
@"<ul>
	<li>Pen: (1.99)</li>
	<li>Glass: (9.99)</li>
	<li>Book: (14.99)</li>
	<li>DVD: (11.99)</li>
</ul>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, model: productArgs);

            html.Print();
            Assert.That(html, Is.EqualTo(expectedHtml));
        }

        /// <summary>If statment.</summary>
        [Test]
        public void If_Statment()
        {
            var template = @"
@if (Model.products.Count == 0) {
<p>Sorry - no products in this category</p>
} else {
<p>We have products for you!</p>
}
".NormalizeNewLines();

            var expectedHtml = @"
<p>We have products for you!</p>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, model: productArgs);

            html.Print();
            Assert.That(html, Is.EqualTo(expectedHtml));
        }

        /// <summary>Multi variable declarations.</summary>
        [Test]
        public void Multi_variable_declarations()
        {
            var template = @"
@{ 
var number = 1; 
var message = ""Number is "" + number; 
}
<p>Your Message: @message</p>
".NormalizeNewLines();

            var expectedHtml = @"

<p>Your Message: Number is 1</p>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, model: productArgs);

            html.Print();
            Assert.That(html, Is.EqualTo(expectedHtml));
        }


        /// <summary>Integrating content and code.</summary>
        [Test]
        public void Integrating_content_and_code()
        {
            var template =
@"<p>Send mail to demis.bellot@gmail.com telling him the time: @DateTime.Now.</p>
".NormalizeNewLines();

            var expectedHtml =
@"<p>Send mail to demis.bellot@gmail.com telling him the time: 02/06/2011 06:38:34.</p>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, model: productArgs);

            html.Print();
            Assert.That(html, Is.StringMatching(expectedHtml.Substring(0, expectedHtml.Length - 25)));
        }


        /// <summary>Identifying nested content.</summary>
        [Test]
        public void Identifying_nested_content()
        {
            var template =
@"
@if (DateTime.Now.Year == 2014) {
<p>If the year is 2014 then print this 
multi-line text block and 
the date: @DateTime.Now</p>
}
".NormalizeNewLines();

            var expectedHtml =
@"<p>If the year is 2014 then print this 
multi-line text block and 
the date: 02/06/2014 06:42:45</p>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, model: productArgs);

            html.Print();
            Assert.That(html, Is.StringMatching(expectedHtml.Substring(0, expectedHtml.Length - 25)));
        }

        /// <summary>HTML encoding.</summary>
        [Test]
        public void HTML_encoding()
        {
            var template =
@"<p>Some Content @Model.stringContainingHtml</p>
".NormalizeNewLines();

            var expectedHtml =
@"<p>Some Content &lt;span&gt;html&lt;/span&gt;</p>
".NormalizeNewLines();

            var html = RazorFormat.CreateAndRenderToHtml(template, new { stringContainingHtml = "<span>html</span>" });

            html.Print();
            Assert.That(html, Is.EqualTo(expectedHtml));
        }

    }
}
