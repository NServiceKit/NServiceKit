using System.Collections.Generic;
using System.Linq;
using Funq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.WebHost.Endpoints;

namespace NewApi.Todos
{
    /// <summary>An application host.</summary>
    public class AppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NewApi.Todos.AppHost class.</summary>
        public AppHost() : base("TODOs Tests", typeof(Todo).Assembly) {}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            container.Register(new TodoRepository());
        }
    }
    
    //REST Resource DTO
    [Route("/todos")]
    [Route("/todos/{Ids}")]
    public class Todos : IReturn<List<Todo>>
    {
        /// <summary>Gets or sets the identifiers.</summary>
        ///
        /// <value>The identifiers.</value>
        public long[] Ids { get; set; }

        /// <summary>Initializes a new instance of the NewApi.Todos.Todos class.</summary>
        ///
        /// <param name="ids">A variable-length parameters list containing identifiers.</param>
        public Todos(params long[] ids)
        {
            this.Ids = ids;
        }
    }

    /// <summary>A todo.</summary>
    [Route("/todos", "POST")]
    [Route("/todos/{Id}", "PUT")]
    public class Todo : IReturn<Todo>
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public long Id { get; set; }

        /// <summary>Gets or sets the content.</summary>
        ///
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>Gets or sets the order.</summary>
        ///
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>Gets or sets a value indicating whether the done.</summary>
        ///
        /// <value>true if done, false if not.</value>
        public bool Done { get; set; }
    }

    /// <summary>The todos service.</summary>
    public class TodosService : Service
    {
        /// <summary>Gets or sets the repository.</summary>
        ///
        /// <value>The repository.</value>
        public TodoRepository Repository { get; set; }  //Injected by IOC

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(Todos request)
        {
            return request.Ids.IsEmpty()
                ? Repository.GetAll()
                : Repository.GetByIds(request.Ids);
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="todo">The todo to put.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(Todo todo)
        {
            return Repository.Store(todo);
        }

        /// <summary>Puts the given todo.</summary>
        ///
        /// <param name="todo">The todo to put.</param>
        ///
        /// <returns>An object.</returns>
        public object Put(Todo todo)
        {
            return Repository.Store(todo);
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        public void Delete(Todos request)
        {
            Repository.DeleteByIds(request.Ids);
        }
    }
    
    /// <summary>A todo repository.</summary>
    public class TodoRepository
    {
        List<Todo> todos = new List<Todo>();

        /// <summary>Gets by identifiers.</summary>
        ///
        /// <param name="ids">A variable-length parameters list containing identifiers.</param>
        ///
        /// <returns>The by identifiers.</returns>
        public List<Todo> GetByIds(long[] ids)
        {
            return todos.Where(x => ids.Contains(x.Id)).ToList();
        }

        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
        public List<Todo> GetAll()
        {
            return todos;
        }

        /// <summary>Stores the given todo.</summary>
        ///
        /// <param name="todo">The todo to store.</param>
        ///
        /// <returns>A Todo.</returns>
        public Todo Store(Todo todo)
        {
            var existing = todos.FirstOrDefault(x => x.Id == todo.Id);
            if (existing == null)
            {
                var newId = todos.Count > 0 ? todos.Max(x => x.Id) + 1 : 1;
                todo.Id = newId;
            }
            todos.Add(todo);
            return todo;
        }

        /// <summary>Deletes the by identifiers described by ids.</summary>
        ///
        /// <param name="ids">A variable-length parameters list containing identifiers.</param>
        public void DeleteByIds(params long[] ids)
        {
            todos.RemoveAll(x => ids.Contains(x.Id));
        }
    }

    /// <summary>A new API todos tests.</summary>
    [TestFixture]
    public class NewApiTodosTests
    {
        const string BaseUri = "http://localhost:82/";

        AppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new AppHost();
            appHost.Init();
            appHost.Start(BaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        /// <summary>Runs this object.</summary>
        [Test]
        public void Run()
        {
            var restClient = new JsonServiceClient(BaseUri);
            List<Todo> all = restClient.Get(new Todos());
            Assert.That(all.Count, Is.EqualTo(0));

            var todo = restClient.Post(new Todo { Content = "New TODO", Order = 1 });
            Assert.That(todo.Id, Is.EqualTo(1));
            all = restClient.Get(new Todos());
            Assert.That(all.Count, Is.EqualTo(1));

            todo.Content = "Updated TODO";
            todo = restClient.Put(todo);
            Assert.That(todo.Content, Is.EqualTo("Updated TODO"));

            restClient.Delete(new Todos(todo.Id));
            all = restClient.Get(new Todos());
            Assert.That(all.Count, Is.EqualTo(0));
        }

    }
}