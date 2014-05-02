using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;

namespace NServiceKit.ServiceHost.Tests.Formats
{
    /// <summary>A page.</summary>
	public class Page
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Formats.Page class.</summary>
		public Page()
		{
			this.Tags = new List<string>();
		}

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the slug.</summary>
        ///
        /// <value>The slug.</value>
		public string Slug { get; set; }

        /// <summary>Gets or sets the source for the.</summary>
        ///
        /// <value>The source.</value>
		public string Src { get; set; }

        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
		public string FilePath { get; set; }

        /// <summary>Gets or sets the category.</summary>
        ///
        /// <value>The category.</value>
		public string Category { get; set; }

        /// <summary>Gets or sets the content.</summary>
        ///
        /// <value>The content.</value>
		public string Content { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime? CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		public DateTime? ModifiedDate { get; set; }

        /// <summary>Gets or sets the tags.</summary>
        ///
        /// <value>The tags.</value>
		public List<string> Tags { get; set; }

        /// <summary>Gets URL of the absolute.</summary>
        ///
        /// <value>The absolute URL.</value>
		public string AbsoluteUrl
		{
			get { return "http://path.com/to/" + this.Slug; }
		}
	}

    /// <summary>A search response.</summary>
	public class SearchResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Formats.SearchResponse class.</summary>
		public SearchResponse()
		{
			this.Results = new List<Page>();
		}

        /// <summary>Gets or sets the query.</summary>
        ///
        /// <value>The query.</value>
		public string Query { get; set; }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
		public List<Page> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}


    /// <summary>An use case tests.</summary>
	[TestFixture]
	public class UseCaseTests : MarkdownTestBase
	{
		private List<Page> Pages;
		private SearchResponse SearchResponse;

		string websiteTemplate = 
@"<!DOCTYPE html>
<html>
    <head>
        <title>Simple Site</title>
    </head>
    <body>
        <div id=""header"">
            <a href=""/"">Home</a>
        </div>
        
        <div id=""body"">
            <!--@Body-->
        </div>
    </body>
</html>".NormalizeNewLines();

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			var jsonPages = File.ReadAllText("~/AppData/Pages.json".MapProjectPath());
	
			Pages = JsonSerializer.DeserializeFromString<List<Page>>(jsonPages);

			SearchResponse = new SearchResponse {
				Query = "OrmLite",
				Results = Pages.Take(5).ToList(),
			};
		}

        /// <summary>Can display search results basic.</summary>
		[Test]
		public void Can_display_search_results_basic()
		{
			var pageTemplate = @"@var Title = ""Search results for "" + Model.Query

@if (Model.Results.Count == 0) {

#### Your search did not match any documents.

## Suggestions:

  - Make sure all words are spelled correctly.
  - Try different keywords.
  - Try more general keywords.
  - Try fewer keywords.
}

@if (Model.Results.Count > 0) {
#### Showing Results 1 - @Model.Results.Count
}

<div id=""searchresults"">
@foreach page in Model.Results {
### @page.Category &gt; [@page.Name](@page.AbsoluteUrl)
@page.Content
}
</div>";
			var expectedHtml = @"<!DOCTYPE html>
<html>
    <head>
        <title>Simple Site</title>
    </head>
    <body>
        <div id=""header"">
            <a href=""/"">Home</a>
        </div>
        
        <div id=""body"">
            <h4>Showing Results 1 - 5</h4>
<div id=""searchresults"">
<h3>Markdown &gt; <a href=""http://path.com/to/about"">About Docs</a></h3>
<h3>Markdown &gt; <a href=""http://path.com/to/markdown-features"">Markdown Features</a></h3>
<h3>Markdown &gt; <a href=""http://path.com/to/markdown-razor"">Markdown Razor</a></h3>
<h3>Framework &gt; <a href=""http://path.com/to/home"">Home</a></h3>
<h3>Framework &gt; <a href=""http://path.com/to/overview"">Overview</a></h3>
</div>
        </div>
    </body>
</html>".NormalizeNewLines();

			var markdownFormat = Create(websiteTemplate, pageTemplate);

			var html = markdownFormat.RenderDynamicPageHtml(PageName, SearchResponse);

			Console.WriteLine(html);

			Assert.That(html.NormalizeNewLines(), Is.EqualTo(expectedHtml));
		}


        /// <summary>Can display search results.</summary>
		[Test]
		public void Can_display_search_results()
		{
			var pageTemplate = @"@var Title = ""Search results for "" + Model.Query

@if (Model.Results.Count == 0) {

#### Your search did not match any documents.

## Suggestions:

  - Make sure all words are spelled correctly.
  - Try different keywords.
  - Try more general keywords.
  - Try fewer keywords.

} else {

#### Showing Results 1 - @Model.Results.Count

^<div id=""searchresults"">

@foreach page in Model.Results {

### @page.Category &gt; [@page.Name](@page.AbsoluteUrl)
@page.Content

}

^</div>

}";
			var expectedHtml = @"<!DOCTYPE html>
<html>
    <head>
        <title>Simple Site</title>
    </head>
    <body>
        <div id=""header"">
            <a href=""/"">Home</a>
        </div>
        
        <div id=""body"">
            <h4>Showing Results 1 - 5</h4>
<div id=""searchresults"">
<h3>Markdown &gt; <a href=""http://path.com/to/about"">About Docs</a></h3>
<h3>Markdown &gt; <a href=""http://path.com/to/markdown-features"">Markdown Features</a></h3>
<h3>Markdown &gt; <a href=""http://path.com/to/markdown-razor"">Markdown Razor</a></h3>
<h3>Framework &gt; <a href=""http://path.com/to/home"">Home</a></h3>
<h3>Framework &gt; <a href=""http://path.com/to/overview"">Overview</a></h3>
</div>

        </div>
    </body>
</html>".NormalizeNewLines();

			var markdownFormat = Create(websiteTemplate, pageTemplate);

			var html = markdownFormat.RenderDynamicPageHtml(PageName, SearchResponse);

			Console.WriteLine(html);

			Assert.That(html, Is.EqualTo(expectedHtml));
		}
	}

}


