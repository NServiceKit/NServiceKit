using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Utils;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints.Formats;

namespace NServiceKit.ServiceHost.Tests.Formats
{
    /// <summary>A markdown format tests.</summary>
	[TestFixture]
	public class MarkdownFormatTests
	{
        /// <summary>A person.</summary>
		public class Person
		{
            /// <summary>Gets or sets the person's first name.</summary>
            ///
            /// <value>The name of the first.</value>
			public string FirstName { get; set; }

            /// <summary>Gets or sets the person's last name.</summary>
            ///
            /// <value>The name of the last.</value>
			public string LastName { get; set; }
		}

		private MarkdownFormat markdownFormat;

		string dynamicPagePath;
		string dynamicPageContent;

		readonly string[] viewPageNames = new[] {
				"Dynamic", "Customer", "CustomerDetailsResponse", "DynamicListTpl", 
				"DynamicNestedTpl", "DynamicTpl", 
			};
		readonly string[] sharedViewPageNames = new[] {
				"DynamicShared", "DynamicTplShared",
			};
		readonly string[] contentPageNames = new[] {
				"Static", "StaticTpl",
			};


        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			dynamicPagePath = "~/Views/Template/DynamicTpl.md".MapProjectPath();
			dynamicPageContent = File.ReadAllText(dynamicPagePath);
		}

        /// <summary>Executes the before each test action.</summary>
		[SetUp]
		public void OnBeforeEachTest()
		{
			markdownFormat = new MarkdownFormat {
                VirtualPathProvider = new FileSystemVirtualPathProvider(new BasicAppHost(), "~/".MapProjectPath()),
            };
		}

        /// <summary>Can load all markdown files.</summary>
		[Test]
		public void Can_load_all_markdown_files()
		{
			markdownFormat.RegisterMarkdownPages("~/".MapProjectPath());

			Assert.That(markdownFormat.ViewPages.Count, Is.EqualTo(viewPageNames.Length));
			Assert.That(markdownFormat.ViewSharedPages.Count, Is.EqualTo(sharedViewPageNames.Length));
			Assert.That(markdownFormat.ContentPages.Count, Is.EqualTo(contentPageNames.Length));
			Assert.That(markdownFormat.MasterPageTemplates.Count, Is.EqualTo(4));

			var pageNames = new List<string>();
			markdownFormat.ViewPages.ForEach((k, v) => pageNames.Add(k));

			Console.WriteLine(pageNames.Dump());
			Assert.That(pageNames.EquivalentTo(viewPageNames));
		}

        /// <summary>Can render static page.</summary>
		[Test]
		public void Can_Render_StaticPage()
		{
			markdownFormat.RegisterMarkdownPages("~/".MapProjectPath());
			var html = markdownFormat.RenderStaticPageHtml("AppData/NoTemplate/Static");

			Assert.That(html, Is.Not.Null);
			Assert.That(html, Is.StringStarting("<h1>Static Markdown template</h1>"));
		}

        /// <summary>Can render static page with template.</summary>
		[Test]
		public void Can_Render_StaticPage_WithTemplate()
		{
			markdownFormat.RegisterMarkdownPages("~/".MapProjectPath());
			var html = markdownFormat.RenderStaticPageHtml("AppData/Template/StaticTpl");

            html.Print();

			Assert.That(html, Is.Not.Null);
			Assert.That(html, Is.StringStarting("<!doctype html>"));
		}

        /// <summary>Can render dynamic page.</summary>
		[Test]
		public void Can_Render_DynamicPage()
		{
			var person = new Person { FirstName = "Demis", LastName = "Bellot" };
			markdownFormat.RegisterMarkdownPages("~/".MapProjectPath());

			var html = markdownFormat.RenderDynamicPageHtml("Dynamic", person);

            var tplPath = "~/Views/Shared/_Layout.shtml".MapProjectPath();
            var tplContent = File.ReadAllText(tplPath);

			var expectedHtml = markdownFormat.Transform(dynamicPageContent)
				.Replace("@Model.FirstName", person.FirstName)
				.Replace("@Model.LastName", person.LastName);

            expectedHtml = tplContent.Replace("<!--@Body-->", expectedHtml);

            "Template: {0}".Fmt(html).Print();
			Assert.That(html, Is.EqualTo(expectedHtml));
		}
	}
}