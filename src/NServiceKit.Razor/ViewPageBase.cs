using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using NServiceKit.CacheAccess;
using NServiceKit.Common.Web;
using NServiceKit.Html;
using NServiceKit.Messaging;
using NServiceKit.MiniProfiler;
using NServiceKit.OrmLite;
using NServiceKit.Redis;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using IHtmlString = System.Web.IHtmlString;

namespace NServiceKit.Razor
{
    /// <summary>
    /// Class to represent attribute values and, more importantly, 
    /// decipher them from tuple madness slightly.
    /// </summary>
    public class AttributeValue
    {
        /// <summary>Gets the prefix.</summary>
        ///
        /// <value>The prefix.</value>
        public Tuple<string, int> Prefix { get; private set; }

        /// <summary>Gets the value.</summary>
        ///
        /// <value>The value.</value>
        public Tuple<object, int> Value { get; private set; }

        /// <summary>Gets a value indicating whether this object is literal.</summary>
        ///
        /// <value>true if this object is literal, false if not.</value>
        public bool IsLiteral { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.AttributeValue class.</summary>
        ///
        /// <param name="prefix">   The prefix.</param>
        /// <param name="value">    The value.</param>
        /// <param name="isLiteral">true if this object is literal.</param>
        public AttributeValue(Tuple<string, int> prefix, Tuple<object, int> value, bool isLiteral)
        {
            this.Prefix = prefix;
            this.Value = value;
            this.IsLiteral = isLiteral;
        }

        /// <summary>AttributeValue casting operator.</summary>
        ///
        /// <param name="value">The value.</param>
        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value)
        {
            return new AttributeValue(value.Item1, value.Item2, value.Item3);
        }

        /// <summary>AttributeValue casting operator.</summary>
        ///
        /// <param name="value">The value.</param>
        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value)
        {
            return new AttributeValue(
                value.Item1, new Tuple<object, int>(value.Item2.Item1, value.Item2.Item2), value.Item3);
        }
    }

    //Should handle all razor rendering functionality
    public abstract class RenderingPage
    {
        /// <summary>Gets or sets the request.</summary>
        ///
        /// <value>The request.</value>
        public IHttpRequest Request { get; set; }

        /// <summary>Gets or sets the response.</summary>
        ///
        /// <value>The response.</value>
        public IHttpResponse Response { get; set; }

        /// <summary>Gets or sets the output.</summary>
        ///
        /// <value>The output.</value>
        public StreamWriter Output { get; set; }

        /// <summary>Gets or sets the view bag.</summary>
        ///
        /// <value>The view bag.</value>
        public dynamic ViewBag { get; set; }

        /// <summary>Gets the typed view bag.</summary>
        ///
        /// <value>The typed view bag.</value>
        public IViewBag TypedViewBag
        {
            get { return (IViewBag)ViewBag; }
        }

        /// <summary>Gets or sets the parent page.</summary>
        ///
        /// <value>The parent page.</value>
        public IRazorView ParentPage { get; set; }

        /// <summary>Gets or sets the child page.</summary>
        ///
        /// <value>The child page.</value>
        public IRazorView ChildPage { get; set; }

        /// <summary>Gets or sets the child body.</summary>
        ///
        /// <value>The child body.</value>
        public string ChildBody { get; set; }

        /// <summary>The child sections.</summary>
        public Dictionary<string, Action> childSections = new Dictionary<string, Action>();

        /// <summary>Initializes a new instance of the NServiceKit.Razor.RenderingPage class.</summary>
        protected RenderingPage()
        {
            this.ViewBag = new DynamicDictionary(this);
        }

        //overridden by the RazorEngine when razor generates code.
        public abstract void Execute();

        //No HTML encoding
        public virtual void WriteLiteral(string str)
        {
            this.Output.Write(str);
        }

        //With HTML encoding
        public virtual void Write(object obj)
        {
            this.Output.Write(HtmlEncode(obj));
        }

        //With HTML encoding
        public virtual void WriteTo(TextWriter writer, HelperResult value)
        {
            if (value != null)
            {
                value.WriteTo(writer);
            }
        }

        /// <summary>Writes a literal to.</summary>
        ///
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public virtual void WriteLiteralTo(TextWriter writer, HelperResult value)
        {
            if (value != null)
            {
                value.WriteTo(writer);
            }
        }

        /// <summary>Writes a literal to.</summary>
        ///
        /// <param name="writer"> The writer.</param>
        /// <param name="literal">The literal.</param>
        public void WriteLiteralTo(TextWriter writer, string literal)
        {
            if (literal == null)
                return;

            writer.Write(literal);
        }

        private static string HtmlEncode(object value)
        {
            if (value == null)
            {
                return null;
            }

            var str = value as System.Web.IHtmlString;

            return str != null ? str.ToHtmlString() : HttpUtility.HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));
        }

        /// <summary>Writes an attribute.</summary>
        ///
        /// <param name="name">  The name.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        public virtual void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
        {
            var attributeValue = this.BuildAttribute(name, prefix, suffix, values);
            this.WriteLiteral(attributeValue);
        }

        /// <summary>Writes an attribute to.</summary>
        ///
        /// <param name="writer">The writer.</param>
        /// <param name="name">  The name.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        public virtual void WriteAttributeTo(TextWriter writer, string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
        {
            var attributeValue = this.BuildAttribute(name, prefix, suffix, values);
            WriteLiteralTo(writer, attributeValue);
        }

        private string BuildAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix,
                                      params AttributeValue[] values)
        {
            var writtenAttribute = false;
            var attributeBuilder = new StringBuilder(prefix.Item1);

            foreach (var value in values)
            {
                if (this.ShouldWriteValue(value.Value.Item1))
                {
                    var stringValue = this.GetStringValue(value);
                    var valuePrefix = value.Prefix.Item1;

                    if (!string.IsNullOrEmpty(valuePrefix))
                    {
                        attributeBuilder.Append(valuePrefix);
                    }

                    attributeBuilder.Append(stringValue);
                    writtenAttribute = true;
                }
            }

            attributeBuilder.Append(suffix.Item1);

            var renderAttribute = writtenAttribute || values.Length == 0;

            if (renderAttribute)
            {
                return attributeBuilder.ToString();
            }

            return string.Empty;
        }
        
        private string GetStringValue(AttributeValue value)
        {
            if (value.IsLiteral)
            {
                return (string)value.Value.Item1;
            }

            var htmlString = value.Value.Item1 as IHtmlString;
            if (htmlString != null)
                return htmlString.ToHtmlString();

            //if (value.Value.Item1 is DynamicDictionaryValue) {
            //    var dynamicValue = (DynamicDictionaryValue)value.Value.Item1;
            //    return dynamicValue.HasValue ? dynamicValue.Value.ToString() : string.Empty;
            //}

            return value.Value.Item1.ToString();
        }


        private bool ShouldWriteValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                var boolValue = (bool)value;

                return boolValue;
            }

            return true;
        }

        /// <summary>Sets child page.</summary>
        ///
        /// <param name="childPage">The child page.</param>
        /// <param name="childBody">The child body.</param>
        public void SetChildPage(IRazorView childPage, string childBody)
        {
            this.ChildPage = childPage;
            this.ChildBody = childBody;
        }

        /// <summary>Renders the body.</summary>
        ///
        /// <returns>An object.</returns>
        public object RenderBody()
        {
            if (ChildBody != null)
            {
                Output.Write(ChildBody);
            }

            return null;
        }

        /// <summary>Query if 'sectionName' is section defined.</summary>
        ///
        /// <param name="sectionName">Name of the section.</param>
        ///
        /// <returns>true if section defined, false if not.</returns>
        public virtual bool IsSectionDefined(string sectionName)
        {
            return this.childSections.ContainsKey(sectionName);
        }

        /// <summary>Define section.</summary>
        ///
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="action">     The action.</param>
        public virtual void DefineSection(string sectionName, Action action)
        {
            this.childSections.Add(sectionName, action);
        }

        /// <summary>Renders the section.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="required">   true if required.</param>
        ///
        /// <returns>An object.</returns>
        public object RenderSection(string sectionName, bool required)
        {
            if (required && !IsSectionDefined(sectionName) && !ChildPage.IsSectionDefined(sectionName))
                throw new Exception("Required Section {0} is not defined".Fmt(sectionName));

            return RenderSection(sectionName);
        }

        /// <summary>Renders the section.</summary>
        ///
        /// <param name="sectionName">Name of the section.</param>
        ///
        /// <returns>An object.</returns>
        public object RenderSection(string sectionName)
        {
            Action section;
            if (childSections.TryGetValue(sectionName, out section))
            {
                section();
            }
            else if (this.ChildPage != null)
            {
                this.ChildPage.RenderSection(sectionName, Output);
            }
            return null;
        }

        /// <summary>Renders the section.</summary>
        ///
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="writer">     The writer.</param>
        public void RenderSection(string sectionName, StreamWriter writer)
        {
            Action section;
            if (childSections.TryGetValue(sectionName, out section))
            {
                var hold = Output;
                try
                {
                    Output = writer;
                    section();
                    Output.Flush();
                }
                finally
                {
                    Output = hold;
                }
            }
        }
    }

    /// <summary>Interface for has model.</summary>
    public interface IHasModel
    {
        /// <summary>Gets the type of the model.</summary>
        ///
        /// <value>The type of the model.</value>
        Type ModelType { get; }

        /// <summary>Sets a model.</summary>
        ///
        /// <param name="o">The object to process.</param>
        void SetModel(object o);
    }

    /// <summary>A view page base.</summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
    public abstract class ViewPageBase<TModel> : RenderingPage, IHasModel where TModel : class
    {
        /// <summary>Gets or sets the layout.</summary>
        ///
        /// <value>The layout.</value>
        public string Layout
        {
            get
            {
                return layout;
            }
            set
            {
                layout = value != null ? value.Trim(' ', '"') : null;
            }
        }

        /// <summary>Gets or sets the model.</summary>
        ///
        /// <value>The model.</value>
        public TModel Model { get; set; }

        /// <summary>Gets the type of the model.</summary>
        ///
        /// <value>The type of the model.</value>
        public abstract Type ModelType { get; }

        /// <summary>Sets a model.</summary>
        ///
        /// <param name="o">The object to process.</param>
        public virtual void SetModel(object o)
        {
            var viewModel = o as TModel;
            this.Model = viewModel;

            if (viewModel == null)
            {
                this.ModelError = o;
            }
        }

        /// <summary>URL of the document.</summary>
        public UrlHelper Url = new UrlHelper();

        private IAppHost appHost;

        /// <summary>Gets or sets the view engine.</summary>
        ///
        /// <value>The view engine.</value>
        public virtual IViewEngine ViewEngine { get; set; }

        /// <summary>Gets or sets the application host.</summary>
        ///
        /// <value>The application host.</value>
        public IAppHost AppHost
        {
            get { return appHost ?? EndpointHost.AppHost; }
            set { appHost = value; }
        }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T Get<T>()
        {
            return this.AppHost.TryResolve<T>();
        }

        /// <summary>Gets or sets the model error.</summary>
        ///
        /// <value>The model error.</value>
        public object ModelError { get; set; }

        /// <summary>Gets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus
        {
            get
            {
                return ToResponseStatus(ModelError) ?? ToResponseStatus(Model);
            }
        }

        private ResponseStatus ToResponseStatus<T>(T modelError)
        {
            var ret = modelError.ToResponseStatus();
            if (ret != null) return ret;

            if (modelError is DynamicObject)
            {
                var dynError = modelError as dynamic;
                return (ResponseStatus)dynError.ResponseStatus;
            }

            return null;
        }

        private ICacheClient cache;

        /// <summary>Gets the cache.</summary>
        ///
        /// <value>The cache.</value>
        public ICacheClient Cache
        {
            get { return cache ?? (cache = Get<ICacheClient>()); }
        }

        private IDbConnection db;

        /// <summary>Gets the database.</summary>
        ///
        /// <value>The database.</value>
        public IDbConnection Db
        {
            get { return db ?? (db = Get<IDbConnectionFactory>().OpenDbConnection()); }
        }

        private IRedisClient redis;

        /// <summary>Gets the redis.</summary>
        ///
        /// <value>The redis.</value>
        public IRedisClient Redis
        {
            get { return redis ?? (redis = Get<IRedisClientsManager>().GetClient()); }
        }

        private IMessageProducer messageProducer;

        /// <summary>Gets the message producer.</summary>
        ///
        /// <value>The message producer.</value>
        public virtual IMessageProducer MessageProducer
        {
            get { return messageProducer ?? (messageProducer = Get<IMessageFactory>().CreateMessageProducer()); }
        }

        private ISessionFactory sessionFactory;
        private ISession session;

        /// <summary>Gets the session.</summary>
        ///
        /// <value>The session.</value>
        public virtual ISession Session
        {
            get
            {
                if (sessionFactory == null)
                    sessionFactory = new SessionFactory(Cache);

                return session ?? (session = sessionFactory.GetOrCreateSession(Request, Response));
            }
        }

        private IAuthSession userSession;
        private string layout;

        /// <summary>Gets the session.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>The session.</returns>
        public virtual T GetSession<T>() where T : class, IAuthSession
        {
            if (userSession != null) return (T)userSession;
            return (T)(userSession = SessionFeature.GetOrCreateSession<T>(Cache, Request, Response));
        }

        /// <summary>Gets the session key.</summary>
        ///
        /// <value>The session key.</value>
        public string SessionKey
        {
            get
            {
                return SessionFeature.GetSessionKey();
            }
        }

        /// <summary>Clears the session.</summary>
        public void ClearSession()
        {
            userSession = null;
            this.Cache.Remove(SessionKey);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            try
            {
                if (cache != null) cache.Dispose();
                cache = null;
            }
            catch { }
            try
            {
                if (db != null) db.Dispose();
                db = null;
            }
            catch { }
            try
            {
                if (redis != null) redis.Dispose();
                redis = null;
            }
            catch { }
            try
            {
                if (messageProducer != null) messageProducer.Dispose();
                messageProducer = null;
            }
            catch { }
        }

        /// <summary>Hrefs the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        ///
        /// <returns>A string.</returns>
        public string Href(string url)
        {
            var replacedUrl = Url.Content(url);
            return replacedUrl;
        }

        /// <summary>Prepends the contents.</summary>
        ///
        /// <param name="contents">The contents.</param>
        public void Prepend(string contents)
        {
            if (contents == null) return;
            //Builder.Insert(0, contents);
        }
    }
}