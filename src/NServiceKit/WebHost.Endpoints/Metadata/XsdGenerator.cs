using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Utils;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>An XSD generator.</summary>
	public class XsdGenerator
	{
		private readonly ILog log = LogManager.GetLogger(typeof(XsdGenerator));

        /// <summary>Gets or sets a value indicating whether the optimize for flash.</summary>
        ///
        /// <value>true if optimize for flash, false if not.</value>
		public bool OptimizeForFlash { get; set; }

        /// <summary>Gets or sets a list of types of the operations.</summary>
        ///
        /// <value>A list of types of the operations.</value>
		public ICollection<Type> OperationTypes { get; set; }

		private string Filter(string xsd)
		{
			return !this.OptimizeForFlash ? xsd : xsd.Replace("ser:guid", "xs:string");
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (OperationTypes == null || OperationTypes.Count == 0) return null;

            var uniqueTypes = new List<Type>();
            var uniqueTypeNames = new List<string>();
            foreach (var type in OperationTypes)
            {
                foreach (var assemblyType in type.Assembly.GetTypes())
                {
                    if (assemblyType.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0)
                    {
                        var baseTypeWithSameName = XsdMetadata.GetBaseTypeWithTheSameName(assemblyType);
                        if (uniqueTypeNames.Contains(baseTypeWithSameName.Name))
                        {
                            log.WarnFormat("Skipping duplicate type with existing name '{0}'", baseTypeWithSameName.Name);
                        }
                        uniqueTypes.Add(baseTypeWithSameName);
                    }
                }
            }
            this.OperationTypes = uniqueTypes;

			var schemaSet = XsdUtils.GetXmlSchemaSet(OperationTypes);
			var xsd = XsdUtils.GetXsd(schemaSet);
			var filteredXsd = Filter(xsd);
			return filteredXsd;
		}
	}
}