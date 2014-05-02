using System;
using System.IO;
using NServiceKit.Html;
using NServiceKit.ServiceHost;

namespace NServiceKit.Razor
{
    /// <summary>A view page.</summary>
    public abstract class ViewPage : ViewPageBase<dynamic>, IRazorView
	{
        /// <summary>The HTML.</summary>
		public HtmlHelper Html = new HtmlHelper();

        /// <summary>Gets the type of the model.</summary>
        ///
        /// <value>The type of the model.</value>
		public override Type ModelType
		{
			get { return typeof(DynamicRequestObject); }
		}

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="httpReq">   The HTTP request.</param>
        /// <param name="httpRes">   The HTTP resource.</param>
        public void Init(IViewEngine viewEngine, IHttpRequest httpReq, IHttpResponse httpRes)
        {
            base.Request = httpReq;
            base.Response = httpRes;

            Html.Init(viewEngine: viewEngine, httpReq: httpReq, httpRes: httpRes, razorPage:this);
        }

        /// <summary>Sets a model.</summary>
        ///
        /// <param name="o">The object to process.</param>
        public override void SetModel(object o)
        {
            base.SetModel(o);
            Html.SetModel(o);
        }

        /// <summary>Writes to.</summary>
        ///
        /// <param name="writer">The writer.</param>
        public void WriteTo(StreamWriter writer)
        {
            this.Output = Html.Writer = writer;
            this.Execute();
            this.Output.Flush();
        }
	}

    /// <summary>A view page.</summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
    public abstract class ViewPage<TModel> : ViewPageBase<TModel>, IRazorView where TModel : class
    {
        /// <summary>The HTML.</summary>
        public HtmlHelper<TModel> Html = new HtmlHelper<TModel>();

        /// <summary>Gets or sets the counter.</summary>
        ///
        /// <value>The counter.</value>
        public int Counter { get; set; }

        /// <summary>Gets the HTML helper.</summary>
        ///
        /// <value>The HTML helper.</value>
        public HtmlHelper HtmlHelper
        {
            get { return Html; }
        }

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="httpReq">   The HTTP request.</param>
        /// <param name="httpRes">   The HTTP resource.</param>
        public void Init(IViewEngine viewEngine, IHttpRequest httpReq, IHttpResponse httpRes)
        {
            base.Request = httpReq;
            base.Response = httpRes;

            Html.Init(viewEngine: viewEngine, httpReq: httpReq, httpRes: httpRes, razorPage: this);
        }

        /// <summary>Sets a model.</summary>
        ///
        /// <param name="o">The object to process.</param>
        public override void SetModel(object o)
        {
            base.SetModel(o);
            Html.SetModel(o);
        }

        /// <summary>Writes to.</summary>
        ///
        /// <param name="writer">The writer.</param>
        public void WriteTo(StreamWriter writer)
        {
            this.Output = Html.Writer = writer;
            this.Execute();
            this.Output.Flush();
        }

        /// <summary>Gets the type of the model.</summary>
        ///
        /// <value>The type of the model.</value>
        public override Type ModelType
        {
            get { return typeof(TModel); }
        }

        //public override void Init(IRazorViewEngine viewEngine, ViewDataDictionary viewData, IHttpRequest httpReq, IHttpResponse httpRes)
        //{
        //    this.Request = httpReq;
        //    this.Response = httpRes;
        //    Html = new HtmlHelper<TModel>();
        //    Html.Init(httpReq, httpRes, viewEngine, viewData, null);
        //    if (viewData.Model is TModel)
        //        this.Model = (TModel)viewData.Model;
        //    else
        //        this.ModelError = viewData.Model;
        //}
    }
}