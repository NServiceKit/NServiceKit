using System;

namespace NServiceKit.ServiceHost
{
    /// <summary>Attribute for API allowable values.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ApiAllowableValuesAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAllowableValuesAttribute class.</summary>
        ///
        /// <param name="name">Parameter name with which allowable values will be associated.</param>
		public ApiAllowableValuesAttribute(string name)
		{
			this.Name = name;
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAllowableValuesAttribute class.</summary>
        ///
        /// <param name="name">Parameter name with which allowable values will be associated.</param>
        /// <param name="min"> The minimum value.</param>
        /// <param name="max"> The maximum value.</param>
		public ApiAllowableValuesAttribute(string name, int min, int max) : this(name)
		{
			Type = "RANGE";
			Min = min;
			Max = max;
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAllowableValuesAttribute class.</summary>
        ///
        /// <param name="name">  Parameter name with which allowable values will be associated.</param>
        /// <param name="values">The values.</param>
		public ApiAllowableValuesAttribute(string name, params string[] values)
			: this(name)
		{
			Type = "LIST";
			Values = values;
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAllowableValuesAttribute class.</summary>
        ///
        /// <param name="name">    Parameter name with which allowable values will be associated.</param>
        /// <param name="enumType">Type of the enum.</param>
		public ApiAllowableValuesAttribute(string name, Type enumType)
			: this(name)
		{
			if (enumType.IsEnum)
			{
				Type = "LIST";
				Values = System.Enum.GetNames(enumType);
			}
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAllowableValuesAttribute class.</summary>
        ///
        /// <param name="name">      Parameter name with which allowable values will be associated.</param>
        /// <param name="listAction">The list action.</param>
		public ApiAllowableValuesAttribute(string name, Func<string[]> listAction)
			: this(name)
		{
			if (listAction != null)
			{
				Type = "LIST";
				Values = listAction();
			}
		}
        /// <summary>
        /// Gets or sets parameter name with which allowable values will be associated.
        /// </summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the type.</summary>
        ///
        /// <value>The type.</value>
		public string Type { get; set; }

        /// <summary>Gets or sets the minimum.</summary>
        ///
        /// <value>The minimum value.</value>
		public int? Min { get; set; }

        /// <summary>Gets or sets the maximum.</summary>
        ///
        /// <value>The maximum value.</value>
		public int? Max { get; set; }

        /// <summary>Gets or sets the values.</summary>
        ///
        /// <value>The values.</value>
		public String[] Values { get; set; }

        //TODO: should be implemented according to:
        //https://github.com/wordnik/swagger-core/wiki/datatypes
    }
}
