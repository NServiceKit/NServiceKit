using System.IO;
using NServiceKit.ServiceHost;

namespace NServiceKit.Html
{
    /// <summary>Interface for view bag.</summary>
    public interface IViewBag
    {
        /// <summary>Attempts to get item from the given data.</summary>
        ///
        /// <param name="name">  The name.</param>
        /// <param name="result">The result.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool TryGetItem(string name, out object result);
    }

    /// <summary>Interface for razor view.</summary>
    public interface IRazorView
    {
        /// <summary>Gets the typed view bag.</summary>
        ///
        /// <value>The typed view bag.</value>
        IViewBag TypedViewBag { get; }

        /// <summary>Gets or sets the layout.</summary>
        ///
        /// <value>The layout.</value>
        string Layout { get; set; }

        /// <summary>Sets child page.</summary>
        ///
        /// <param name="childPage">The child page.</param>
        /// <param name="childBody">The child body.</param>
        void SetChildPage(IRazorView childPage, string childBody);

        /// <summary>Gets the child page.</summary>
        ///
        /// <value>The child page.</value>
        IRazorView ChildPage { get; }

        /// <summary>Gets or sets the parent page.</summary>
        ///
        /// <value>The parent page.</value>
        IRazorView ParentPage { get; set; }

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="httpReq">   The HTTP request.</param>
        /// <param name="httpRes">   The HTTP resource.</param>
        void Init(IViewEngine viewEngine, IHttpRequest httpReq, IHttpResponse httpRes);

        /// <summary>Writes to.</summary>
        ///
        /// <param name="writer">The writer.</param>
        void WriteTo(StreamWriter writer);

        /// <summary>Query if 'sectionName' is section defined.</summary>
        ///
        /// <param name="sectionName">Name of the section.</param>
        ///
        /// <returns>true if section defined, false if not.</returns>
        bool IsSectionDefined(string sectionName);

        /// <summary>Renders the section.</summary>
        ///
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="writer">     The writer.</param>
        void RenderSection(string sectionName, StreamWriter writer);
    }
}