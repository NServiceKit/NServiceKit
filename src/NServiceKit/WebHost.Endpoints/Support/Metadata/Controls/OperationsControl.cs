using System.Collections.Generic;
using System.Web.UI;
using NServiceKit.WebHost.Endpoints.Support.Templates;

namespace NServiceKit.WebHost.Endpoints.Support.Metadata.Controls
{
    internal class OperationsControl : System.Web.UI.Control
    {
        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>Gets or sets a list of names of the operations.</summary>
        ///
        /// <value>A list of names of the operations.</value>
        public List<string> OperationNames { get; set; }

        /// <summary>Gets or sets the metadata operation page body HTML.</summary>
        ///
        /// <value>The metadata operation page body HTML.</value>
        public string MetadataOperationPageBodyHtml { get; set; }

        /// <summary>Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.</summary>
        ///
        /// <param name="output">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter output)
        {
            var operationsPart = new ListTemplate
            {
                ListItems = this.OperationNames,
                ListItemTemplate = @"<li><a href=""?op={0}"">{0}</a></li>"
            }.ToString();
            var renderedTemplate = HtmlTemplates.Format(HtmlTemplates.OperationsControlTemplate,
                this.Title, this.MetadataOperationPageBodyHtml, operationsPart);
            output.Write(renderedTemplate);
        }

    }
}