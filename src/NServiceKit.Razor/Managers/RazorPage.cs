using System;
using System.Web;
using NServiceKit.IO;
using NServiceKit.Razor.Compilation;
using NServiceKit.Text;

namespace NServiceKit.Razor.Managers
{
    /// <summary>A razor page.</summary>
    public class RazorPage
    {
        private readonly object syncRoot = new object();

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Managers.RazorPage class.</summary>
        public RazorPage()
        {
            this.IsValid = false;
        }

        /// <summary>Gets the synchronise root.</summary>
        ///
        /// <value>The synchronise root.</value>
        public object SyncRoot { get { return syncRoot; } }

        /// <summary>Gets or sets the page host.</summary>
        ///
        /// <value>The page host.</value>
        public RazorPageHost PageHost { get; set; }

        /// <summary>Gets or sets a value indicating whether the marked for compilation.</summary>
        ///
        /// <value>true if marked for compilation, false if not.</value>
        public bool MarkedForCompilation { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is compiling.</summary>
        ///
        /// <value>true if this object is compiling, false if not.</value>
        public bool IsCompiling { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is valid.</summary>
        ///
        /// <value>true if this object is valid, false if not.</value>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets the file.</summary>
        ///
        /// <value>The file.</value>
        public IVirtualFile File { get; set; }

        /// <summary>Gets or sets the type of the page.</summary>
        ///
        /// <value>The type of the page.</value>
        public Type PageType { get; set; }

        /// <summary>Gets or sets the type of the model.</summary>
        ///
        /// <value>The type of the model.</value>
        public Type ModelType { get; set; }

        /// <summary>Gets or sets details of the exception.</summary>
        ///
        /// <value>The compile exception.</value>
        public virtual HttpCompileException CompileException { get; set; }

        /// <summary>Activates the instance.</summary>
        ///
        /// <returns>A RenderingPage.</returns>
        public RenderingPage ActivateInstance()
        {
            return this.PageType.CreateInstance() as RenderingPage;
        }
    }
}