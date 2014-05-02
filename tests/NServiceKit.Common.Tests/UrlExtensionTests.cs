using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests
{
    /// <summary>A just identifier.</summary>
    [Route("/route/{Id}")]
    public class JustId : IReturn
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public long Id { get; set; }
    }

    /// <summary>A field identifier.</summary>
    [Route("/route/{Id}")]
	public class FieldId : IReturn
	{
        /// <summary>The identifier.</summary>
		public readonly long Id;

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.FieldId class.</summary>
        ///
        /// <param name="id">The identifier.</param>
		public FieldId(long id)
		{
			Id = id;
		}
	}

    /// <summary>An array identifiers.</summary>
    [Route("/route/{Ids}")]
    public class ArrayIds : IReturn
    {
        /// <summary>Gets or sets the identifiers.</summary>
        ///
        /// <value>The identifiers.</value>
        public long[] Ids { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.ArrayIds class.</summary>
        ///
        /// <param name="ids">A variable-length parameters list containing identifiers.</param>
        public ArrayIds(params long[] ids)
        {
            this.Ids = ids;
        }
    }

    /// <summary>A request with ignored data members.</summary>
    [Route("/route/{Id}")]
    public class RequestWithIgnoredDataMembers : IReturn
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public long Id { get; set; }

        /// <summary>Gets or sets the included.</summary>
        ///
        /// <value>The included.</value>
        public string Included { get; set; }

        /// <summary>Gets or sets the excluded.</summary>
        ///
        /// <value>The excluded.</value>
        [IgnoreDataMember]
        public string Excluded { get; set; }
    }

    /// <summary>A request with data members.</summary>
    [DataContract]
    [Route("/route/{Id}")]
    public class RequestWithDataMembers : IReturn
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember]
        public long Id { get; set; }

        /// <summary>Gets or sets the included.</summary>
        ///
        /// <value>The included.</value>
        [DataMember]
        public string Included { get; set; }

        /// <summary>Gets or sets the excluded.</summary>
        ///
        /// <value>The excluded.</value>
        public string Excluded { get; set; }
    }

    /// <summary>A request with named data members.</summary>
    [DataContract]
    [Route("/route/{Key}")]
    public class RequestWithNamedDataMembers : IReturn
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember(Name = "Key")]
        public long Id { get; set; }

        /// <summary>Gets or sets the included.</summary>
        ///
        /// <value>The included.</value>
        [DataMember(Name = "Inc")]
        public string Included { get; set; }

        /// <summary>Gets or sets the excluded.</summary>
        ///
        /// <value>The excluded.</value>
        public string Excluded { get; set; }
    }

    /// <summary>Values that represent Gender.</summary>
	public enum Gender
	{
        /// <summary>An enum constant representing the none option.</summary>
		None = 0,

        /// <summary>An enum constant representing the male option.</summary>
		Male,

        /// <summary>An enum constant representing the female option.</summary>
		Female
	}

    /// <summary>A request with value types.</summary>
	[Route("/route/{Id}")]
	public class RequestWithValueTypes : IReturn
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public long Id { get; set; }

        /// <summary>Gets or sets the gender 1.</summary>
        ///
        /// <value>The gender 1.</value>
		public Gender Gender1 { get; set; }

        /// <summary>Gets or sets the gender 2.</summary>
        ///
        /// <value>The gender 2.</value>
		public Gender? Gender2 { get; set; }
	}

    /// <summary>An URL extension tests.</summary>
    [TestFixture]
    public class UrlExtensionTests
    {
        /// <summary>Can create URL with just identifier.</summary>
        [Test]
        public void Can_create_url_with_JustId()
        {
            var url = new JustId { Id = 1 }.ToUrl("GET");
            Assert.That(url, Is.EqualTo("/route/1"));
        }

        /// <summary>Can create URL with field identifier.</summary>
		[Test]
		public void Can_create_url_with_FieldId()
		{
			using (JsConfig.BeginScope())
			{
				JsConfig.IncludePublicFields = true;
				var url = new FieldId(1).ToUrl("GET");
				Assert.That(url, Is.EqualTo("/route/1"));

			}
		}


        /// <summary>Can create URL with array identifiers.</summary>
        [Test]
        public void Can_create_url_with_ArrayIds()
        {
            var url = new ArrayIds(1, 2, 3).ToUrl("GET");
            Assert.That(url, Is.EqualTo("/route/1%2C2%2C3"));
        }

        /// <summary>Cannot include ignored data members on querystring.</summary>
        [Test]
        public void Cannot_include_ignored_data_members_on_querystring()
        {
            var url = new RequestWithIgnoredDataMembers { Id = 1, Included = "Yes", Excluded = "No" }.ToUrl("GET");
            Assert.That(url, Is.EqualTo("/route/1?included=Yes"));
        }

        /// <summary>Can include only data members on querystring.</summary>
        [Test]
        public void Can_include_only_data_members_on_querystring()
        {
            var url = new RequestWithDataMembers { Id = 1, Included = "Yes", Excluded = "No" }.ToUrl("GET");
            Assert.That(url, Is.EqualTo("/route/1?included=Yes"));
        }

        /// <summary>Use data member names on querystring.</summary>
        [Test]
        public void Use_data_member_names_on_querystring()
        {
            var url = new RequestWithNamedDataMembers { Id = 1, Included = "Yes", Excluded = "No" }.ToUrl("GET");
            Assert.That(url, Is.EqualTo("/route/1?inc=Yes"));
        }

        /// <summary>Cannot use default for non nullable value types on querystring.</summary>
		[Test]
		public void Cannot_use_default_for_non_nullable_value_types_on_querystring()
		{
			var url = new RequestWithValueTypes {Id = 1, Gender1 = Gender.None}.ToUrl("GET");
			Assert.That(url, Is.EqualTo("/route/1"));
		}

        /// <summary>Can use non default for non nullable value types on querystring.</summary>
		[Test]
		public void Can_use_non_default_for_non_nullable_value_types_on_querystring()
		{
			var url = new RequestWithValueTypes { Id = 1, Gender1 = Gender.Male }.ToUrl("GET");
			Assert.That(url, Is.EqualTo("/route/1?gender1=Male"));
		}

        /// <summary>Can use default for nullable value types on querystring.</summary>
		[Test]
		public void Can_use_default_for_nullable_value_types_on_querystring()
		{
			var url = new RequestWithValueTypes { Id = 1, Gender2 = Gender.None }.ToUrl("GET");
			Assert.That(url, Is.EqualTo("/route/1?gender2=None"));
		}

        /// <summary>Cannot use null for nullable value types on querystring.</summary>
		[Test]
		public void Cannot_use_null_for_nullable_value_types_on_querystring()
		{
			var url = new RequestWithValueTypes { Id = 1, Gender2 = null }.ToUrl("GET");
			Assert.That(url, Is.EqualTo("/route/1"));
		}

        /// <summary>Can use non default for nullable value types on querystring.</summary>
		[Test]
		public void Can_use_non_default_for_nullable_value_types_on_querystring()
		{
			var url = new RequestWithValueTypes { Id = 1, Gender2 = Gender.Male }.ToUrl("GET");
			Assert.That(url, Is.EqualTo("/route/1?gender2=Male"));
		}

        /// <summary>Can combine uris with to URL.</summary>
        [Test]
        public void Can_combine_Uris_with_toUrl()
        {
            var serviceEndpoint = new Uri("http://localhost/api/", UriKind.Absolute);
            var actionUrl = new Uri(new JustId { Id = 1 }.ToUrl("GET").Substring(1), UriKind.Relative);

            Assert.That(new Uri(serviceEndpoint, actionUrl).ToString(), Is.EqualTo("http://localhost/api/route/1"));
        }

        /// <summary>Can use default for non nullable value types on path.</summary>
		[Test]
		public void Can_use_default_for_non_nullable_value_types_on_path()
		{
			var url = new RequestWithValueTypes { Id = 0 }.ToUrl("GET");
			Assert.That(url, Is.EqualTo("/route/0"));
		}

        /// <summary>A wild card path.</summary>
        [Route("/images/{ImagePath*}")]
        public class WildCardPath : IReturn<object>
        {
            /// <summary>Gets or sets the full pathname of the image file.</summary>
            ///
            /// <value>The full pathname of the image file.</value>
            public string ImagePath { get; set; }
        }

        /// <summary>Can generate route with wild card path.</summary>
        [Test]
        public void Can_generate_route_with_WildCard_path()
        {
            var request = new WildCardPath { ImagePath = "this/that/theother.jpg" };
            var url = request.ToUrl("GET");
            Assert.That(url, Is.EqualTo("/images/" + Uri.EscapeDataString(request.ImagePath)));
        }

        /// <summary>Can generate empty route with wild card path.</summary>
        [Test]
        public void Can_generate_empty_route_with_WildCard_path()
        {
            var request = new WildCardPath();
            var url = request.ToUrl("GET");
            Assert.That(url, Is.EqualTo("/images/"));
        }

        /// <summary>Shows the what URI escaping encodes.</summary>
        [Test]
        public void Show_what_uri_escaping_encodes()
        {
            //INFO on what needs to be url-encoded
            //http://stackoverflow.com/a/10385414/85785

            var data = "amp&mod%space comma,dot.colon:semicolon;slash/";

            Assert.That(Uri.EscapeUriString(data), Is.EqualTo("amp&mod%25space%20comma,dot.colon:semicolon;slash/"));
            Assert.That(Uri.EscapeDataString(data), Is.EqualTo("amp%26mod%25space%20comma%2Cdot.colon%3Asemicolon%3Bslash%2F"));
        }

    }
}