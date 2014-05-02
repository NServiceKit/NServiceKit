using System.Linq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.DataAnnotations;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests
{

    /// <summary>A route inference tests.</summary>
	[TestFixture]
	public class RouteInferenceTests
	{
		ServiceRoutes routes = new ServiceRoutes();
        BasicAppHost loadAppHost;

        /// <summary>Infer routes.</summary>
		[TestFixtureSetUp]
		public void InferRoutes()
		{
            loadAppHost = new BasicAppHost().Init();
            
            RouteNamingConvention.PropertyNamesToMatch.Add("Key");
			RouteNamingConvention.AttributeNamesToMatch.Add(typeof(KeyAttribute).Name);
			routes.AddFromAssembly(typeof(RouteInferenceTests).Assembly);
		}

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            loadAppHost.Dispose();
        }

        /// <summary>Should infer route from request dto type.</summary>
		[Test]
		public void Should_infer_route_from_RequestDTO_type()
		{
			var restPath = (from r in routes.RestPaths
							 where r.RequestType == typeof(RequestNoMembers)
							 select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 1);

			Assert.That(restPath.AllowedVerbs.Contains("GET"));
			Assert.That(restPath.AllowedVerbs.Contains("POST"));
			Assert.That(restPath.AllowedVerbs.Contains("PUT"));
			Assert.That(restPath.AllowedVerbs.Contains("DELETE"));
			Assert.That(restPath.AllowedVerbs.Contains("PATCH"));
			Assert.That(restPath.AllowedVerbs.Contains("HEAD"));
			Assert.That(restPath.AllowedVerbs.Contains("OPTIONS"));

			Assert.IsTrue(typeof(RequestNoMembers).Name.EqualsIgnoreCase(restPath.Path.Remove(0,1)));
		}

        /// <summary>Should infer route from any public property named identifier.</summary>
		[Test]
		public void Should_infer_route_from_AnyPublicProperty_named_Id()
		{
			var restPath = (from r in routes.RestPaths
							where r.RequestType == typeof(RequestWithMemberCalledId)
							//routes without {placeholders} are tested above
							&& r.PathComponentsCount > 1 
							select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 2);
			Assert.IsTrue(restPath.Path.EndsWithInvariant("{Id}"));
		}

        /// <summary>Should infer route from any public property named identifiers.</summary>
		[Test]
		public void Should_infer_route_from_AnyPublicProperty_named_Ids()
		{
			var restPath = (from r in routes.RestPaths
							where r.RequestType == typeof(RequestWithMemberCalledIds)
							//routes without {placeholders} are tested above
							&& r.PathComponentsCount > 1
							select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 2);
			Assert.IsTrue(restPath.Path.EndsWithInvariant("{Ids}"));
		}

        /// <summary>Should infer route from any public property in matching name strategy.</summary>
		[Test]
		public void Should_infer_route_from_AnyPublicProperty_in_MatchingNameStrategy()
		{
			var restPath = (from r in routes.RestPaths
							where r.RequestType == typeof(RequestWithMemberCalledSpecialName)
							//routes without {placeholders} are tested above
							&& r.PathComponentsCount > 1
							select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 2);
			Assert.IsTrue(restPath.Path.EndsWithInvariant("{Key}"));
		}

        /// <summary>Should infer route from any public property with primary key attribute.</summary>
		[Test]
		public void Should_infer_route_from_AnyPublicProperty_with_PrimaryKeyAttribute()
		{
			var restPath = (from r in routes.RestPaths
							where r.RequestType == typeof(RequestWithPrimaryKeyAttribute)
							//routes without {placeholders} are tested above
							&& r.PathComponentsCount > 1
							select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 2);
			//it doesn't matter what the placeholder name is; only that 1 placeholer is in the path
			Assert.IsTrue(restPath.Path.Count(c => c == '}') == 1);
		}

        /// <summary>Should infer route from any public property in matching attribute strategy.</summary>
		[Test]
		public void Should_infer_route_from_AnyPublicProperty_in_MatchingAttributeStrategy()
		{
			var restPath = (from r in routes.RestPaths
							where r.RequestType == typeof(RequestWithMemberWithKeyAttribute)
							//routes without {placeholders} are tested above
							&& r.PathComponentsCount > 1
							select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 2);
			//it doesn't matter what the placeholder name is; only that 1 placeholer is in the path
			Assert.IsTrue(restPath.Path.Count(c => c == '}') == 1);
		}

        /// <summary>Should infer route from any pubic property from any strategy and composite the route.</summary>
		[Test]
		public void Should_infer_route_from_AnyPubicProperty_FromAnyStrategy_AndCompositeTheRoute()
		{
			var restPath = (from r in routes.RestPaths
							where r.RequestType == typeof(RequestWithCompositeKeys)
							//routes without {placeholders} are tested above
							&& r.PathComponentsCount > 1
							select r).FirstOrDefault();

			Assert.That(restPath, Is.Not.Null);

			Assert.That(restPath.PathComponentsCount == 3);
			//it doesn't matter what the placeholder name is; only that 1 placeholer is in the path
			Assert.IsTrue(restPath.Path.Count(c => c == '}') == 2);
		}
	}

    /// <summary>A request no members.</summary>
	public class RequestNoMembers { }

    /// <summary>A request with member called identifier.</summary>
	public class RequestWithMemberCalledId
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public object Id { get; set; }
	}

    /// <summary>A request with member called identifiers.</summary>
	public class RequestWithMemberCalledIds
	{
        /// <summary>Gets or sets the identifiers.</summary>
        ///
        /// <value>The identifiers.</value>
		public object[] Ids { get; set; }
	}

    /// <summary>A request with member called special name.</summary>
	public class RequestWithMemberCalledSpecialName
	{
        /// <summary>Gets or sets the key.</summary>
        ///
        /// <value>The key.</value>
		public string Key { get; set; }
	}

    /// <summary>Attribute for request with primary key.</summary>
	public class RequestWithPrimaryKeyAttribute
	{
        /// <summary>Gets or sets the primary key attribute property.</summary>
        ///
        /// <value>The primary key attribute property.</value>
		[PrimaryKey]
		public int PrimaryKeyAttributeProperty { get; set; }
	}	

    /// <summary>Attribute for request with member with key.</summary>
	public class RequestWithMemberWithKeyAttribute
	{
        /// <summary>Gets or sets the key attribute property.</summary>
        ///
        /// <value>The key attribute property.</value>
		[Key]
		public string KeyAttributeProperty { get; set; }
	}

    /// <summary>A request with composite keys.</summary>
	public class RequestWithCompositeKeys
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the key.</summary>
        ///
        /// <value>The key.</value>
		public int Key { get; set; }
	}

    /// <summary>A request no members service.</summary>
	public class RequestNoMembersService : TestRestService<RequestNoMembers> { }
	
    /// <summary>A request with member called identifier service.</summary>
	public class RequestWithMemberCalledIdService : TestRestService<RequestWithMemberCalledId> { }
	
    /// <summary>A request with member called identifiers service.</summary>
	public class RequestWithMemberCalledIdsService : TestRestService<RequestWithMemberCalledIds> { }
	
    /// <summary>A request with member called special name service.</summary>
	public class RequestWithMemberCalledSpecialNameService : TestRestService<RequestWithMemberCalledSpecialName> { }
	
    /// <summary>A request with primary key attribute service.</summary>
	public class RequestWithPrimaryKeyAttributeService : TestRestService<RequestWithPrimaryKeyAttribute> { }
	
    /// <summary>A request with member with key attribute service.</summary>
	public class RequestWithMemberWithKeyAttributeService : TestRestService<RequestWithMemberWithKeyAttribute> { }
	
    /// <summary>A request with composite keys service.</summary>
	public class RequestWithCompositeKeysService : TestRestService<RequestWithCompositeKeys> { }
}
