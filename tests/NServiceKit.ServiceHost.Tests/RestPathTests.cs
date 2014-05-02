using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A rest path tests.</summary>
    [TestFixture]
    public class RestPathTests
    {
        /// <summary>A simple type.</summary>
        public class SimpleType
        {
            /// <summary>Gets or sets the name.</summary>
            ///
            /// <value>The name.</value>
            public string Name { get; set; }
        }

        /// <summary>Can deserialize simple type path.</summary>
        [Test]
        public void Can_deserialize_SimpleType_path()
        {
            var restPath = new RestPath(typeof(SimpleType), "/simple/{Name}");
            var request = restPath.CreateRequest("/simple/HelloWorld!") as SimpleType;

            Assert.That(request, Is.Not.Null);
            Assert.That(request.Name, Is.EqualTo("HelloWorld!"));
        }

        /// <summary>Can deserialize simple type in middle of path.</summary>
        [Test]
        public void Can_deserialize_SimpleType_in_middle_of_path()
        {
            var restPath = new RestPath(typeof(SimpleType), "/simple/{Name}/some-other-literal");
            var request = restPath.CreateRequest("/simple/HelloWorld!/some-other-literal") as SimpleType;

            Assert.That(request, Is.Not.Null);
            Assert.That(request.Name, Is.EqualTo("HelloWorld!"));
        }

        /// <summary>Shows the allow.</summary>
        [Test]
        public void ShowAllow()
        {
            var config = EndpointHostConfig.Instance;

            const string fileName = "/path/to/image.GIF";
            var fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1);
            Console.WriteLine(fileExt);
            Assert.That(config.AllowFileExtensions.Contains(fileExt));
        }


        /// <summary>A complex type.</summary>
        public class ComplexType
        {
            /// <summary>Gets or sets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            ///
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets a unique identifier.</summary>
            ///
            /// <value>The identifier of the unique.</value>
            public Guid UniqueId { get; set; }
        }

        /// <summary>Can deserialize complex type path.</summary>
        [Test]
        public void Can_deserialize_ComplexType_path()
        {

            var restPath = new RestPath(typeof(ComplexType),
                "/Complex/{Id}/{Name}/Unique/{UniqueId}");
            var request = restPath.CreateRequest(
                "/complex/5/Is Alive/unique/4583B364-BBDC-427F-A289-C2923DEBD547") as ComplexType;

            Assert.That(request, Is.Not.Null);
            Assert.That(request.Id, Is.EqualTo(5));
            Assert.That(request.Name, Is.EqualTo("Is Alive"));
            Assert.That(request.UniqueId, Is.EqualTo(new Guid("4583B364-BBDC-427F-A289-C2923DEBD547")));
        }

        /// <summary>A complex type with fields.</summary>
        public class ComplexTypeWithFields
        {
            /// <summary>The identifier.</summary>
            public readonly int Id;

            /// <summary>The name.</summary>
            public readonly string Name;

            /// <summary>Unique identifier.</summary>
            public readonly Guid UniqueId;

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.RestPathTests.ComplexTypeWithFields class.</summary>
            ///
            /// <param name="id">      The identifier.</param>
            /// <param name="name">    The name.</param>
            /// <param name="uniqueId">Unique identifier.</param>
            public ComplexTypeWithFields(int id, string name, Guid uniqueId)
            {
                Id = id;
                Name = name;
                UniqueId = uniqueId;
            }
        }

        /// <summary>Can deserialize complex type with fields path.</summary>
        [Test]
        public void Can_deserialize_ComplexTypeWithFields_path()
        {
            using (JsConfig.With(includePublicFields: true))
            {
                var restPath = new RestPath(typeof(ComplexTypeWithFields),
                    "/Complex/{Id}/{Name}/Unique/{UniqueId}");
                var request = restPath.CreateRequest(
                    "/complex/5/Is Alive/unique/4583B364-BBDC-427F-A289-C2923DEBD547") as ComplexTypeWithFields;

                Assert.That(request, Is.Not.Null);
                Assert.That(request.Id, Is.EqualTo(5));
                Assert.That(request.Name, Is.EqualTo("Is Alive"));
                Assert.That(request.UniqueId, Is.EqualTo(new Guid("4583B364-BBDC-427F-A289-C2923DEBD547")));
            }
        }


        /// <summary>A bbc music request.</summary>
        public class BbcMusicRequest
        {
            /// <summary>Gets or sets a unique identifier of the mbz.</summary>
            ///
            /// <value>Unique identifier of the mbz.</value>
            public Guid mbz_guid { get; set; }

            /// <summary>Gets or sets the type of the release.</summary>
            ///
            /// <value>The type of the release.</value>
            public string release_type { get; set; }

            /// <summary>Gets or sets the type of the content.</summary>
            ///
            /// <value>The type of the content.</value>
            public string content_type { get; set; }
        }

        private static void AssertMatch(string definitionPath, string requestPath,
            string firstMatchHashKey, BbcMusicRequest expectedRequest)
        {
            var restPath = new RestPath(typeof(BbcMusicRequest), definitionPath);

            var reqestTestPath = RestPath.GetPathPartsForMatching(requestPath);
            Assert.That(restPath.IsMatch("GET", reqestTestPath), Is.True);

            Assert.That(firstMatchHashKey, Is.EqualTo(restPath.FirstMatchHashKey));

            var actualRequest = restPath.CreateRequest(requestPath) as BbcMusicRequest;

            Assert.That(actualRequest, Is.Not.Null);
            Assert.That(actualRequest.mbz_guid, Is.EqualTo(expectedRequest.mbz_guid));
            Assert.That(actualRequest.release_type, Is.EqualTo(expectedRequest.release_type));
            Assert.That(actualRequest.content_type, Is.EqualTo(expectedRequest.content_type));
        }

        /// <summary>Can support bbc rest apis.</summary>
        [Test]
        public void Can_support_BBC_REST_Apis()
        {
            /*
                /music/artists/:mbz_guid.[xml|yaml|json]
                /music/artists/:mbz_guid/promotions.[json]
                /music/artists/:mbz_guid/releases.[xml|yaml|json]
                /music/artists/:mbz_guid/releases/[albums|singles|eps|...].[xml|yaml|json]
            */
            var mbz = new Guid("E0A387F5-48F0-40E0-AAEA-483DD7EE7484");

            AssertMatch("/music/artists/{mbz_guid}.{content_type}",
                "/music/artists/E0A387F5-48F0-40E0-AAEA-483DD7EE7484.xml",
                "3/music",
                new BbcMusicRequest { mbz_guid = mbz, content_type = "xml" });

            AssertMatch("/music/artists/{mbz_guid}/promotions.{content_type}",
                "/music/artists/E0A387F5-48F0-40E0-AAEA-483DD7EE7484/promotions.json",
                "4/music",
                new BbcMusicRequest { mbz_guid = mbz, content_type = "json" });

            AssertMatch("/music/artists/{mbz_guid}/releases.{content_type}",
                "/music/artists/E0A387F5-48F0-40E0-AAEA-483DD7EE7484/releases.yaml",
                "4/music",
                new BbcMusicRequest { mbz_guid = mbz, content_type = "yaml" });

            AssertMatch("/music/artists/{mbz_guid}/releases/{release_type}.{content_type}",
                "/music/artists/E0A387F5-48F0-40E0-AAEA-483DD7EE7484/releases/albums.json",
                "5/music",
                new BbcMusicRequest { mbz_guid = mbz, release_type = "albums", content_type = "json" });
        }

        /// <summary>A rack space request.</summary>
        public class RackSpaceRequest
        {
            /// <summary>Gets or sets the version.</summary>
            ///
            /// <value>The version.</value>
            public string version { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public string id { get; set; }

            /// <summary>Gets or sets the type of the resource.</summary>
            ///
            /// <value>The type of the resource.</value>
            public string resource_type { get; set; }

            /// <summary>Gets or sets the action.</summary>
            ///
            /// <value>The action.</value>
            public string action { get; set; }

            /// <summary>Gets or sets the type of the content.</summary>
            ///
            /// <value>The type of the content.</value>
            public string content_type { get; set; }
        }


        private static void AssertMatch(string definitionPath, string requestPath,
            string firstMatchHashKey, RackSpaceRequest expectedRequest)
        {
            var restPath = new RestPath(typeof(RackSpaceRequest), definitionPath);

            var reqestTestPath = RestPath.GetPathPartsForMatching(requestPath);
            Assert.That(restPath.IsMatch("GET", reqestTestPath), Is.True);

            Assert.That(firstMatchHashKey, Is.EqualTo(restPath.FirstMatchHashKey));

            var actualRequest = restPath.CreateRequest(requestPath) as RackSpaceRequest;

            Assert.That(actualRequest, Is.Not.Null);
            Assert.That(actualRequest.version, Is.EqualTo(expectedRequest.version));
            Assert.That(actualRequest.id, Is.EqualTo(expectedRequest.id));
            Assert.That(actualRequest.resource_type, Is.EqualTo(expectedRequest.resource_type));
            Assert.That(actualRequest.action, Is.EqualTo(expectedRequest.action));
        }

        /// <summary>Can support rackspace rest apis.</summary>
        [Test]
        public void Can_support_Rackspace_REST_Apis()
        {
            /*
             * /v1.0/214412/images
             * /v1.0/214412/images.xml
             * /servers/id/action
             * /images/detail 
             */

            AssertMatch("/{version}/{id}/images", "/v1.0/214412/images",
                "3/images",
                new RackSpaceRequest { version = "v1.0", id = "214412" });

            AssertMatch("/{version}/{id}/images.{content_type}", "/v1.0/214412/images.xml",
                "3/images",
                new RackSpaceRequest { version = "v1.0", id = "214412", content_type = "xml" });

            AssertMatch("/servers/{id}/{action}", "/servers/214412/delete",
                "3/servers",
                new RackSpaceRequest { id = "214412", action = "delete" });

            AssertMatch("/images/{action}", "/images/detail",
                "2/images",
                new RackSpaceRequest { action = "detail" });

            AssertMatch("/images/detail", "/images/detail",
                "2/images",
                new RackSpaceRequest { });
        }

        /// <summary>A slug request.</summary>
        public class SlugRequest
        {
            /// <summary>Gets or sets the slug.</summary>
            ///
            /// <value>The slug.</value>
            public string Slug { get; set; }

            /// <summary>Gets or sets the version.</summary>
            ///
            /// <value>The version.</value>
            public int Version { get; set; }

            /// <summary>Gets or sets options for controlling the operation.</summary>
            ///
            /// <value>The options.</value>
            public string Options { get; set; }
        }

        private static void AssertMatch(string definitionPath, string requestPath, string firstMatchHashKey,
                                        SlugRequest expectedRequest, int expectedScore)
        {
            var restPath = new RestPath(typeof(SlugRequest), definitionPath);
            var requestTestPath = RestPath.GetPathPartsForMatching(requestPath);
            Assert.That(restPath.IsMatch("GET", requestTestPath), Is.True);

            Assert.That(firstMatchHashKey, Is.EqualTo(restPath.FirstMatchHashKey));

            var actualRequest = restPath.CreateRequest(requestPath) as SlugRequest;
            Assert.That(actualRequest, Is.Not.Null);
            Assert.That(actualRequest.Slug, Is.EqualTo(expectedRequest.Slug));
            Assert.That(actualRequest.Version, Is.EqualTo(expectedRequest.Version));
            Assert.That(actualRequest.Options, Is.EqualTo(expectedRequest.Options));
            Assert.That(restPath.MatchScore("GET", requestTestPath), Is.EqualTo(expectedScore));
        }

        private static void AssertNoMatch(string definitionPath, string requestPath)
        {
            var restPath = new RestPath(typeof(SlugRequest), definitionPath);
            var requestTestPath = RestPath.GetPathPartsForMatching(requestPath);
            Assert.That(restPath.IsMatch("GET", requestTestPath), Is.False);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Cannot_have_variable_after_wildcard()
        {
            AssertMatch("/content/{Slug*}/{Version}",
                "/content/wildcard/slug/path/1", "*/content", new SlugRequest(), -1);
        /// <summary>Can support internal wildcard.</summary>
        /// <summary>Can support internal wildcard.</summary>
        }

        [Test]
        public void Can_support_internal_wildcard()
        {
            AssertMatch("/content/{Slug*}/literal",
                        "/content/wildcard/slug/path/literal",
                        "*/content",
                        new SlugRequest { Slug = "wildcard/slug/path" },
                        7901);

            AssertMatch("/content/{Slug*}/version/{Version}",
                        "/content/wildcard/slug/path/version/1",
                        "*/content",
                        new SlugRequest { Slug = "wildcard/slug/path", Version = 1 },
                        7801);

            AssertMatch("/content/{Slug*}/with/{Options*}",
                        "/content/wildcard/slug/path/with/optionA/optionB",
                        "*/content",
                        new SlugRequest { Slug = "wildcard/slug/path", Options = "optionA/optionB" },
                        5801);

            AssertMatch("/{Slug*}/content", "/content", "*/content", new SlugRequest(), 10901);

            AssertMatch("/content/{Slug*}/literal", "/content/literal", "*/content", new SlugRequest(), 10901);

            AssertNoMatch("/content/{Slug*}/literal", "/content/wildcard/slug/path");

            AssertNoMatch("/content/{Slug*}/literal", "/content/literal/literal");
        }

        /// <summary>Routes have expected precedence.</summary>
        [Test]
        public void Routes_have_expected_precedence()
        {
            AssertPrecedence("GET /content",
                "GET /content",
                "ANY /content",
                "GET /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("PUT /content",
                "PUT /content",
                "ANY /content",
                "PUT /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("GET /content/literal",
                "GET /content/literal",
                "ANY /content/literal",
                "GET /content/{Version}",
                "ANY /content/{Version}",
                "GET /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("PUT /content/literal",
                "PUT /content/literal",
                "ANY /content/literal",
                "PUT /content/{Version}",
                "ANY /content/{Version}",
                "PUT /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("GET /content/v1",
                "GET /content/{Version}",
                "ANY /content/{Version}",
                "GET /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("PUT /content/v1",
                "PUT /content/{Version}",
                "ANY /content/{Version}",
                "PUT /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("GET /content/v1/literal-after",
                "GET /content/v1/literal-after",
                "ANY /content/v1/literal-after",
                "GET /content/{Version}/literal-after",
                "ANY /content/{Version}/literal-after",
                "GET /content/{Version}/{Slug}",
                "ANY /content/{Version}/{Slug}",
                "GET /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("PUT /content/v1/literal-after",
                "ANY /content/v1/literal-after",
                "ANY /content/{Version}/literal-after",
                "ANY /content/{Version}/{Slug}",
                "PUT /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("GET /content/literal-before/v1",
                "GET /content/literal-before/v1",
                "ANY /content/literal-before/v1",
                "GET /content/literal-before/{Version}",
                "ANY /content/literal-before/{Version}",
                "GET /content/{Version}/{Slug}",
                "ANY /content/{Version}/{Slug}",
                "GET /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("PUT /content/literal-before/v1",
                "ANY /content/literal-before/v1",
                "ANY /content/literal-before/{Version}",
                "ANY /content/{Version}/{Slug}",
                "PUT /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("GET /content/v1/literal/slug",
                "GET /content/v1/literal/slug",
                "ANY /content/v1/literal/slug",
                "GET /content/v1/literal/{ignore}",
                "GET /content/{ignore}/literal/{ignore}",
                "GET /content/{Version*}/literal/{Slug*}",
                "ANY /content/{Version*}/literal/{Slug*}",
                "GET /content/{Slug*}",
                "ANY /content/{Slug*}");

            AssertPrecedence("PUT /content/v1/literal/slug",
                "ANY /content/v1/literal/slug",
                "ANY /content/{Version*}/literal/{Slug*}",
                "PUT /content/{Slug*}",
                "ANY /content/{Slug*}");
        }

        class SlugRoute
        {
            /// <summary>The definitions.</summary>
            public static SlugRoute[] Definitions = new[]{
                new SlugRoute("GET /content"),
                new SlugRoute("PUT /content"),
                new SlugRoute("ANY /content"),

                new SlugRoute("GET /content/literal"),
                new SlugRoute("PUT /content/literal"),
                new SlugRoute("ANY /content/literal"),

                new SlugRoute("GET /content/{Version}"),
                new SlugRoute("PUT /content/{Version}"),
                new SlugRoute("ANY /content/{Version}"),

                new SlugRoute("GET /content/{Slug*}"),
                new SlugRoute("PUT /content/{Slug*}"),
                new SlugRoute("ANY /content/{Slug*}"),

                new SlugRoute("GET /content/v1/literal-after"),
                new SlugRoute("ANY /content/v1/literal-after"),
                new SlugRoute("GET /content/{Version}/literal-after"),
                new SlugRoute("ANY /content/{Version}/literal-after"),
                new SlugRoute("GET /content/{Version}/{Slug}"),
                new SlugRoute("ANY /content/{Version}/{Slug}"),

                new SlugRoute("GET /content/literal-before/v1"),
                new SlugRoute("ANY /content/literal-before/v1"),
                new SlugRoute("GET /content/literal-before/{Version}"),
                new SlugRoute("ANY /content/literal-before/{Version}"),

                new SlugRoute("GET /content/v1/literal/slug"),
                new SlugRoute("ANY /content/v1/literal/slug"),                
                new SlugRoute("GET /content/v1/literal/{ignore}"),
                new SlugRoute("GET /content/{ignore}/literal/{ignore}"),
                new SlugRoute("GET /content/{Version*}/literal/{Slug*}"),
                new SlugRoute("ANY /content/{Version*}/literal/{Slug*}"),

            };

            /// <summary>Gets or sets the definition.</summary>
            ///
            /// <value>The definition.</value>
            public string Definition { get; set; }

            /// <summary>Gets or sets the full pathname of the rest file.</summary>
            ///
            /// <value>The full pathname of the rest file.</value>
            public RestPath RestPath { get; set; }

            /// <summary>Gets or sets the score.</summary>
            ///
            /// <value>The score.</value>
            public int Score { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.RestPathTests.SlugRoute class.</summary>
            ///
            /// <param name="definition">The definition.</param>
            public SlugRoute(string definition)
            {
                this.Definition = definition;
                var parts = definition.SplitOnFirst(' ');
                RestPath = new RestPath(typeof(SlugRequest), path: parts[1], verbs: parts[0] == "ANY" ? null : parts[0]);
            }

            /// <summary>Gets ordered matching rules.</summary>
            ///
            /// <param name="withVerb">The with verb.</param>
            /// <param name="forPath"> Full pathname of for file.</param>
            ///
            /// <returns>The ordered matching rules.</returns>
            public static List<SlugRoute> GetOrderedMatchingRules(string withVerb, string forPath)
            {
                var matchingRoutes = new List<SlugRoute>();

                foreach (var definition in Definitions)
                {
                    var pathComponents = RestPath.GetPathPartsForMatching(forPath);
                    definition.Score = definition.RestPath.MatchScore(withVerb, pathComponents);
                    if (definition.Score > 0)
                    {
                        matchingRoutes.Add(definition);
                    }
                }

                var orderedRoutes = matchingRoutes.OrderByDescending(x => x.Score).ToList();
                return orderedRoutes;
            }
        }

        /// <summary>Assert precedence.</summary>
        ///
        /// <param name="requestedDefinition">The requested definition.</param>
        /// <param name="expected">           A variable-length parameters list containing expected.</param>
        public void AssertPrecedence(string requestedDefinition, params string[] expected)
        {
            var parts = requestedDefinition.SplitOnFirst(' ');
            var orderedRoutes = SlugRoute.GetOrderedMatchingRules(parts[0], parts[1]);
            var matchingDefinitions = orderedRoutes.ConvertAll(x => x.Definition);

            var isMatch = matchingDefinitions.EquivalentTo(expected);

            Assert.That(isMatch, "Expected:\n{0}\n  Actual:\n{1}".Fmt(expected.Join("\n"), matchingDefinitions.Join("\n")));
        }

        /// <summary>Can match lowercase HTTP method.</summary>
        [Test]
        public void Can_match_lowercase_http_method()
        {
            var restPath = new RestPath(typeof (ComplexType), "/Complex/{Id}/{Name}/Unique/{UniqueId}", "PUT");
            var withPathInfoParts = RestPath.GetPathPartsForMatching("/complex/5/Is Alive/unique/4583B364-BBDC-427F-A289-C2923DEBD547");
            Assert.That(restPath.IsMatch("put", withPathInfoParts));
        }
    }
}