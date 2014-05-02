using System;
using System.IO;
using NServiceKit.Common.Utils;

namespace NServiceKit.WebHost.Endpoints.Support.Templates
{
    /// <summary>A HTML templates.</summary>
    public static class HtmlTemplates
    {
        private static string indexOperationsTemplate;

        /// <summary>Gets or sets the index operations template.</summary>
        ///
        /// <value>The index operations template.</value>
        public static string IndexOperationsTemplate
        {
            get
            {
                return indexOperationsTemplate ??
                       (indexOperationsTemplate = LoadEmbeddedHtmlTemplate("IndexOperations.html"));
            }
            set { indexOperationsTemplate = value; }
        }

        private static string operationControlTemplate;

        /// <summary>Gets or sets the operation control template.</summary>
        ///
        /// <value>The operation control template.</value>
        public static string OperationControlTemplate
        {
            get
            {
                return operationControlTemplate ??
                       (operationControlTemplate = LoadEmbeddedHtmlTemplate("OperationControl.html"));
            }
            set { operationControlTemplate = value; }
        }

        private static string operationsControlTemplate;

        /// <summary>Gets or sets the operations control template.</summary>
        ///
        /// <value>The operations control template.</value>
        public static string OperationsControlTemplate
        {
            get
            {
                return operationsControlTemplate ??
                       (operationsControlTemplate = LoadEmbeddedHtmlTemplate("OperationsControl.html"));
            }
            set { operationsControlTemplate = value; }
        }


        static HtmlTemplates()
        {
            if (EndpointHost.Config.UseCustomMetadataTemplates)
            {
                TryLoadExternal("IndexOperations.html", ref indexOperationsTemplate);
                TryLoadExternal("OperationControl.html", ref operationControlTemplate);
                TryLoadExternal("OperationsControl.html", ref operationsControlTemplate);
            }
        }

        private static bool TryLoadExternal(string templateName, ref string template)
        {
            try
            {
                var staticFilePath = PathUtils.CombinePaths(
                    EndpointHost.AppHost.VirtualPathProvider.RootDirectory.RealPath, 
                    EndpointHost.Config.MetadataCustomPath, 
                    templateName);

                template = File.ReadAllText(staticFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string LoadEmbeddedHtmlTemplate(string templateName)
        {
            var resourceNamespace = typeof(HtmlTemplates).Namespace + ".Html.";
            var stream = typeof(HtmlTemplates).Assembly.GetManifestResourceStream(resourceNamespace + templateName);
            if (stream == null)
            {
                throw new FileNotFoundException(
                    "Could not load HTML template embedded resource " + templateName,
                    templateName);
            }
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>Formats.</summary>
        ///
        /// <param name="template">The template.</param>
        /// <param name="args">    A variable-length parameters list containing arguments.</param>
        ///
        /// <returns>The formatted value.</returns>
        public static string Format(string template, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                template = template.Replace(@"{" + i + "}", (args[i] ?? "").ToString());
            }
            return template;
        }

    }
}
