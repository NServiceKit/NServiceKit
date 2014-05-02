using System.Collections.Generic;
using Funq;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A todo.</summary>
	public class Todo
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the content.</summary>
        ///
        /// <value>The content.</value>
		public string Content { get; set; }

        /// <summary>Gets or sets a value indicating whether the done.</summary>
        ///
        /// <value>true if done, false if not.</value>
		public bool Done { get; set; }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The todo to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(Todo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id == Id && Equals(other.Name, Name) && Equals(other.Content, Content) && other.Done.Equals(Done);
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The object to compare with the current object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Todo)) return false;
			return Equals((Todo) obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int result = Id;
				result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
				result = (result*397) ^ (Content != null ? Content.GetHashCode() : 0);
				result = (result*397) ^ Done.GetHashCode();
				return result;
			}
		}
	}

    /// <summary>List of todoes.</summary>
    [Route("/todolist")]
	public class TodoList : List<Todo>
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.TodoList class.</summary>
		public TodoList() {}

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.TodoList class.</summary>
        ///
        /// <param name="collection">The collection.</param>
		public TodoList(IEnumerable<Todo> collection) : base(collection) {}
	}

    /// <summary>A todo list response.</summary>
	public class TodoListResponse
	{
        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
		public TodoList Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A todo list service.</summary>
    [DefaultRequest(typeof(TodoList))]
	public class TodoListService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(TodoList request)
		{
			return new TodoListResponse { Results = request };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(TodoList request)
		{
			return new TodoListResponse { Results = request };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(TodoList request)
		{
			return new TodoListResponse { Results = request };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(TodoList request)
		{
			return new TodoListResponse { Results = request };
		}
	}

    /// <summary>A todo list tests.</summary>
	[TestFixture]
	public class TodoListTests
	{
		private const string ListeningOn = "http://localhost:8082/";

        /// <summary>A todo list application host HTTP listener.</summary>
		public class TodoListAppHostHttpListener
			: AppHostHttpListenerBase
		{
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.TodoListTests.TodoListAppHostHttpListener class.</summary>
			public TodoListAppHostHttpListener()
				: base("TodoList Tests", typeof(TodoList).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
			public override void Configure(Container container) {}
		}

		TodoListAppHostHttpListener appHost;

		readonly Todo[] Todos = new[] {
			new Todo { Id = 1, Name = "Todo1", Content = "Content1", Done = false},
			new Todo { Id = 2, Name = "Todo2", Content = "Content2", Done = true},
			new Todo { Id = 3, Name = "Todo3", Content = "Content3", Done = false},
		};

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			appHost = new TodoListAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
		}

        /// <summary>Can send todo list.</summary>
		[Test]
		public void Can_Send_TodoList()
		{
			var serviceClient = new JsonServiceClient(ListeningOn);
			var response = serviceClient.Send<TodoListResponse>(new TodoList(Todos));
			Assert.That(response.Results, Is.EquivalentTo(Todos));
		}

        /// <summary>Can post todo list.</summary>
		[Test]
		public void Can_Post_TodoList()
		{
			var serviceClient = new JsonServiceClient(ListeningOn);
			var response = serviceClient.Post<TodoListResponse>("/todolist", new TodoList(Todos));
			Assert.That(response.Results, Is.EquivalentTo(Todos));
		}

        /// <summary>Can get todo list.</summary>
		[Test]
		public void Can_Get_TodoList()
		{
			var serviceClient = new JsonServiceClient(ListeningOn);
			var response = serviceClient.Get<TodoListResponse>("/todolist");
			Assert.That(response.Results.Count, Is.EqualTo(0));
		}
	}
}