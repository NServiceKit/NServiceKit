using System.Collections.Generic;

namespace NServiceKit.Common.ServiceModel
{
    /// <summary>A metadata types configuration.</summary>
    public class MetadataTypesConfig
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.ServiceModel.MetadataTypesConfig class.</summary>
        ///
        /// <param name="baseUrl">                     The base URL.</param>
        /// <param name="makePartial">                 true if make partial, false if not.</param>
        /// <param name="makeVirtual">                 true if make virtual, false if not.</param>
        /// <param name="addReturnMarker">             true if add return marker, false if not.</param>
        /// <param name="convertDescriptionToComments">true to convert description to comments.</param>
        /// <param name="addDataContractAttributes">   true if add data contract attributes, false if not.</param>
        /// <param name="makeDataContractsExtensible"> true if make data contracts extensible, false if not.</param>
        /// <param name="addIndexesToDataMembers">     true if add indexes to data members, false if not.</param>
        /// <param name="addDefaultXmlNamespace">      The add default XML namespace.</param>
        /// <param name="initializeCollections">       true if initialize collections, false if not.</param>
        /// <param name="addResponseStatus">           true if add response status, false if not.</param>
        /// <param name="addImplicitVersion">          The add implicit version.</param>
        public MetadataTypesConfig(
            string baseUrl = null,
            bool makePartial = true,
            bool makeVirtual = true,
            bool addReturnMarker = true,
            bool convertDescriptionToComments = true,
            bool addDataContractAttributes = false,
            bool makeDataContractsExtensible = false,
            bool addIndexesToDataMembers = false,
            string addDefaultXmlNamespace = null,
            bool initializeCollections = true,
            bool addResponseStatus = false,
            int? addImplicitVersion = null)
        {
            BaseUrl = baseUrl;
            MakePartial = makePartial;
            MakeVirtual = makeVirtual;
            AddReturnMarker = addReturnMarker;
            AddDescriptionAsComments = convertDescriptionToComments;
            AddDataContractAttributes = addDataContractAttributes;
            AddDefaultXmlNamespace = addDefaultXmlNamespace;
            MakeDataContractsExtensible = makeDataContractsExtensible;
            AddIndexesToDataMembers = addIndexesToDataMembers;
            InitializeCollections = initializeCollections;
            AddResponseStatus = addResponseStatus;
            AddImplicitVersion = addImplicitVersion;

            DefaultNamespaces = new List<string> 
            {
                "System",
                "System.Collections",
                "System.ComponentModel",
                "System.Collections.Generic",
                "System.Runtime.Serialization",
                "NServiceKit.ServiceHost",
                "NServiceKit.ServiceInterface.ServiceModel",
            };
        }

        /// <summary>Gets or sets URL of the base.</summary>
        ///
        /// <value>The base URL.</value>
        public string BaseUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether the make partial.</summary>
        ///
        /// <value>true if make partial, false if not.</value>
        public bool MakePartial { get; set; }

        /// <summary>Gets or sets a value indicating whether the make virtual.</summary>
        ///
        /// <value>true if make virtual, false if not.</value>
        public bool MakeVirtual { get; set; }

        /// <summary>Gets or sets a value indicating whether the add return marker.</summary>
        ///
        /// <value>true if add return marker, false if not.</value>
        public bool AddReturnMarker { get; set; }

        /// <summary>Gets or sets a value indicating whether the add description as comments.</summary>
        ///
        /// <value>true if add description as comments, false if not.</value>
        public bool AddDescriptionAsComments { get; set; }

        /// <summary>Gets or sets a value indicating whether the add data contract attributes.</summary>
        ///
        /// <value>true if add data contract attributes, false if not.</value>
        public bool AddDataContractAttributes { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is make data contracts extensible.</summary>
        ///
        /// <value>true if make data contracts extensible, false if not.</value>
        public bool MakeDataContractsExtensible { get; set; }

        /// <summary>Gets or sets a value indicating whether the add indexes to data members.</summary>
        ///
        /// <value>true if add indexes to data members, false if not.</value>
        public bool AddIndexesToDataMembers { get; set; }

        /// <summary>Gets or sets a value indicating whether the initialize collections.</summary>
        ///
        /// <value>true if initialize collections, false if not.</value>
        public bool InitializeCollections { get; set; }

        /// <summary>Gets or sets the add implicit version.</summary>
        ///
        /// <value>The add implicit version.</value>
        public int? AddImplicitVersion { get; set; }

        /// <summary>Gets or sets a value indicating whether the add response status.</summary>
        ///
        /// <value>true if add response status, false if not.</value>
        public bool AddResponseStatus { get; set; }

        /// <summary>Gets or sets the add default XML namespace.</summary>
        ///
        /// <value>The add default XML namespace.</value>
        public string AddDefaultXmlNamespace { get; set; }

        /// <summary>Gets or sets the default namespaces.</summary>
        ///
        /// <value>The default namespaces.</value>
        public List<string> DefaultNamespaces { get; set; }
    }

    /// <summary>A metadata types.</summary>
    public class MetadataTypes
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.ServiceModel.MetadataTypes class.</summary>
        public MetadataTypes()
        {
            Types = new List<MetadataType>();
            Operations = new List<MetadataOperationType>();
            Version = 1;
        }

        /// <summary>Gets or sets the version.</summary>
        ///
        /// <value>The version.</value>
        public int Version { get; set; }

        /// <summary>Gets or sets the configuration.</summary>
        ///
        /// <value>The configuration.</value>
        public MetadataTypesConfig Config { get; set; }

        /// <summary>Gets or sets the types.</summary>
        ///
        /// <value>The types.</value>
        public List<MetadataType> Types { get; set; }

        /// <summary>Gets or sets the operations.</summary>
        ///
        /// <value>The operations.</value>
        public List<MetadataOperationType> Operations { get; set; }
    }

    /// <summary>A metadata operation type.</summary>
    public class MetadataOperationType
    {
        /// <summary>Gets or sets the actions.</summary>
        ///
        /// <value>The actions.</value>
        public List<string> Actions { get; set; }

        /// <summary>Gets or sets the request.</summary>
        ///
        /// <value>The request.</value>
        public MetadataType Request { get; set; }

        /// <summary>Gets or sets the response.</summary>
        ///
        /// <value>The response.</value>
        public MetadataType Response { get; set; }
    }

    /// <summary>A metadata type.</summary>
    public class MetadataType
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the namespace.</summary>
        ///
        /// <value>The namespace.</value>
        public string Namespace { get; set; }

        /// <summary>Gets or sets the generic arguments.</summary>
        ///
        /// <value>The generic arguments.</value>
        public string[] GenericArgs { get; set; }

        /// <summary>Gets or sets the inherits.</summary>
        ///
        /// <value>The inherits.</value>
        public string Inherits { get; set; }

        /// <summary>Gets or sets the inherits generic arguments.</summary>
        ///
        /// <value>The inherits generic arguments.</value>
        public string[] InheritsGenericArgs { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>Gets or sets a value indicating whether the return void marker.</summary>
        ///
        /// <value>true if return void marker, false if not.</value>
        public bool ReturnVoidMarker { get; set; }

        /// <summary>Gets or sets the return marker generic arguments.</summary>
        ///
        /// <value>The return marker generic arguments.</value>
        public string[] ReturnMarkerGenericArgs { get; set; }

        /// <summary>Gets or sets the routes.</summary>
        ///
        /// <value>The routes.</value>
        public List<MetadataRoute> Routes { get; set; }

        /// <summary>Gets or sets the data contract.</summary>
        ///
        /// <value>The data contract.</value>
        public MetadataDataContract DataContract { get; set; }

        /// <summary>Gets or sets the properties.</summary>
        ///
        /// <value>The properties.</value>
        public List<MetadataPropertyType> Properties { get; set; }

        /// <summary>Gets or sets the attributes.</summary>
        ///
        /// <value>The attributes.</value>
        public List<MetadataAttribute> Attributes { get; set; }
    }

    /// <summary>A metadata route.</summary>
    public class MetadataRoute
    {
        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
        public string Path { get; set; }

        /// <summary>Gets or sets the verbs.</summary>
        ///
        /// <value>The verbs.</value>
        public string Verbs { get; set; }

        /// <summary>Gets or sets the notes.</summary>
        ///
        /// <value>The notes.</value>
        public string Notes { get; set; }

        /// <summary>Gets or sets the summary.</summary>
        ///
        /// <value>The summary.</value>
        public string Summary { get; set; }
    }

    /// <summary>A metadata contract.</summary>
    public class MetadataDataContract
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the namespace.</summary>
        ///
        /// <value>The namespace.</value>
        public string Namespace { get; set; }
    }

    /// <summary>A metadata member.</summary>
    public class MetadataDataMember
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the order.</summary>
        ///
        /// <value>The order.</value>
        public int? Order { get; set; }

        /// <summary>Gets or sets the is required.</summary>
        ///
        /// <value>The is required.</value>
        public bool? IsRequired { get; set; }

        /// <summary>Gets or sets the emit default value.</summary>
        ///
        /// <value>The emit default value.</value>
        public bool? EmitDefaultValue { get; set; }
    }

    /// <summary>A metadata property type.</summary>
    public class MetadataPropertyType
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the type.</summary>
        ///
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>Gets or sets the generic arguments.</summary>
        ///
        /// <value>The generic arguments.</value>
        public string[] GenericArgs { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>Gets or sets the data member.</summary>
        ///
        /// <value>The data member.</value>
        public MetadataDataMember DataMember { get; set; }

        /// <summary>Gets or sets the attributes.</summary>
        ///
        /// <value>The attributes.</value>
        public List<MetadataAttribute> Attributes { get; set; }
    }

    /// <summary>Attribute for metadata.</summary>
    public class MetadataAttribute
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the constructor arguments.</summary>
        ///
        /// <value>The constructor arguments.</value>
        public List<MetadataPropertyType> ConstructorArgs { get; set; }

        /// <summary>Gets or sets the arguments.</summary>
        ///
        /// <value>The arguments.</value>
        public List<MetadataPropertyType> Args { get; set; }
    }
}