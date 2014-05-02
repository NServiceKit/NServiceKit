using System.Collections.Generic;

namespace NServiceKit.Tests.Html.Support.Types
{
	class Person
	{
        /// <summary>Gets or sets the first.</summary>
        ///
        /// <value>The first.</value>
		public string First { get; set; }

        /// <summary>Gets or sets the last.</summary>
        ///
        /// <value>The last.</value>
		public string Last { get; set; }

        /// <summary>Gets or sets the work.</summary>
        ///
        /// <value>The work.</value>
		public Address Work { get; set; }

        /// <summary>Gets or sets the home.</summary>
        ///
        /// <value>The home.</value>
		public Address Home { get; set; }
	}

	class Address
	{
        /// <summary>Gets or sets the street.</summary>
        ///
        /// <value>The street.</value>
		public string Street { get; set; }

        /// <summary>Gets or sets the street no.</summary>
        ///
        /// <value>The street no.</value>
		public string StreetNo { get; set; }

        /// <summary>Gets or sets the zip.</summary>
        ///
        /// <value>The zip.</value>
		public string ZIP { get; set; }

        /// <summary>Gets or sets the city.</summary>
        ///
        /// <value>The city.</value>
		public string City { get; set; }

        /// <summary>Gets or sets the state.</summary>
        ///
        /// <value>The state.</value>
		public string State { get; set; }
	}
}
