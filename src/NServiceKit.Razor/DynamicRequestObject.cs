using System;
using System.Collections.Generic;
using System.Dynamic;
using NServiceKit.Html;
using NServiceKit.ServiceHost;

namespace NServiceKit.Razor
{
    /// <summary>A dynamic request object.</summary>
    public class DynamicRequestObject : DynamicDictionary
    {
        private readonly IHttpRequest httpReq;
        private readonly IDictionary<string, object> model;
        private readonly object originalModel;

        /// <summary>The extension methods.</summary>
        public static readonly Dictionary<string, Func<object, object>> ExtensionMethods = new Dictionary<string, Func<object, object>>
            {
                {"AsRaw", o => o.AsRaw()},
                {"AsRawJson", o => o.AsRawJson()},
            };

        /// <summary>Initializes a new instance of the NServiceKit.Razor.DynamicRequestObject class.</summary>
        public DynamicRequestObject() { }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.DynamicRequestObject class.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="model">  The model.</param>
        public DynamicRequestObject(IHttpRequest httpReq, object model = null)
        {
            this.httpReq = httpReq;
            this.originalModel = model;
            if (model != null)
            {
                this.model = new RouteValueDictionary(model);
            }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic
        /// behavior for operations such as getting a value for a property.
        /// </summary>
        ///
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is
        /// performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the
        /// <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        ///
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            if (model != null)
            {
                if (model.TryGetValue(name, out result))
                {
                    return true;
                }

                Func<object, object> modelFn;
                if (ExtensionMethods.TryGetValue(name, out modelFn))
                {
                    result = (Func<object>)(() => modelFn(originalModel ?? model));
                    return true;
                }
            }

            result = httpReq.GetParam(name);
            return result != null || base.TryGetMember(binder, out result);
        }
    }

    /// <summary>Dictionary of dynamics.</summary>
    public class DynamicDictionary : System.Dynamic.DynamicObject, IViewBag
    {
        /// <summary>The dictionary.</summary>
        protected readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();
        private readonly RenderingPage page;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.DynamicDictionary class.</summary>
        public DynamicDictionary() { }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.DynamicDictionary class.</summary>
        ///
        /// <param name="page">The page.</param>
        public DynamicDictionary(RenderingPage page)
        {
            this.page = page;
        }

        /// <summary>Gets the number of. </summary>
        ///
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic
        /// behavior for operations such as getting a value for a property.
        /// </summary>
        ///
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is
        /// performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the
        /// <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        ///
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = null;
            var name = binder.Name.ToLower();
            if (!dictionary.TryGetValue(name, out result))
            {
                if (page != null)
                {
                    if (page.ChildPage != null)
                    {
                        page.ChildPage.TypedViewBag.TryGetItem(name, out result);
                    }
                    else if (page.ParentPage != null)
                    {
                        page.ParentPage.TypedViewBag.TryGetItem(name, out result);
                    }
                }

            }

            return true;
        }

        /// <summary>Attempts to get item from the given data.</summary>
        ///
        /// <param name="name">  The name.</param>
        /// <param name="result">The result.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryGetItem(string name, out object result)
        {
            if (this.dictionary.TryGetValue(name, out result))
                return true;

            return page.ChildPage != null
                && page.ChildPage.TypedViewBag.TryGetItem(name, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic
        /// behavior for operations such as setting a value for a property.
        /// </summary>
        ///
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For
        /// example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name
        /// returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="value"> The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the
        /// <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".
        /// </param>
        ///
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time
        /// exception is thrown.)
        /// </returns>
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            dictionary[binder.Name.ToLower()] = value;
            return true;
        }

        /// <summary>Returns the enumeration of all dynamic member names.</summary>
        ///
        /// <returns>A sequence that contains dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dictionary.Keys;
        }
    }

    /// <summary>A dynamic utilities.</summary>
    public static class DynamicUtils
    {
        /// <summary>An object extension method that converts an anonymousObject to an expando.</summary>
        ///
        /// <param name="anonymousObject">The anonymousObject to act on.</param>
        ///
        /// <returns>anonymousObject as an ExpandoObject.</returns>
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}