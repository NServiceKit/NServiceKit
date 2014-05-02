using System;
using System.Linq.Expressions;
using System.Web;

namespace NServiceKit.Html
{
    /// <summary>A mvc HTML string.</summary>
	public class MvcHtmlString
	{
		private delegate MvcHtmlString MvcHtmlStringCreator(string value);
		private static readonly MvcHtmlStringCreator _creator = GetCreator();

        /// <summary>imporant: this declaration must occur after the _creator declaration.</summary>
		public static readonly MvcHtmlString Empty = Create(String.Empty);

		private readonly string _value;

        /// <summary>Initializes a new instance of the NServiceKit.Html.MvcHtmlString class.</summary>
        ///
        /// <param name="value">The value.</param>
		protected MvcHtmlString(string value)
		{
			_value = value ?? String.Empty;
		}

        /// <summary>Creates a new MvcHtmlString.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString Create(string value)
		{
			return _creator(value);
		}

		// in .NET 4, we dynamically create a type that subclasses MvcHtmlString and implements IHtmlString
		private static MvcHtmlStringCreator GetCreator()
		{
			var iHtmlStringType = typeof(HttpContext).Assembly.GetType("System.Web.IHtmlString");
			if (iHtmlStringType != null)
			{
				// first, create the dynamic type
				var dynamicType = DynamicTypeGenerator.GenerateType("DynamicMvcHtmlString", typeof(MvcHtmlString), new[] { iHtmlStringType });

				// then, create the delegate to instantiate the dynamic type
				var valueParamExpr = Expression.Parameter(typeof(string), "value");
				var newObjExpr = Expression.New(dynamicType.GetConstructor(new[] { typeof(string) }), valueParamExpr);
				var lambdaExpr = Expression.Lambda<MvcHtmlStringCreator>(newObjExpr, valueParamExpr);
				return lambdaExpr.Compile();
			}
			else
			{
				// disabling 0618 allows us to call the MvcHtmlString() constructor
#pragma warning disable 0618
				return value => new MvcHtmlString(value);
#pragma warning restore 0618
			}
		}

        /// <summary>Queries if a null or is empty.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if a null or is empty, false if not.</returns>
		public static bool IsNullOrEmpty(MvcHtmlString value)
		{
			return (value == null || value._value.Length == 0);
		}

        /// <summary>IHtmlString.ToHtmlString()</summary>
        ///
        /// <returns>This object as a string.</returns>
		public string ToHtmlString()
		{
			return _value;
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return _value;
		}
	}
}
