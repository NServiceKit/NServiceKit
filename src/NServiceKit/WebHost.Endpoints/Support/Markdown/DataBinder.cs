using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>A data binder.</summary>
	public static class DataBinder
	{
        /// <summary>Compile to string.</summary>
        ///
        /// <param name="type">The type.</param>
        /// <param name="expr">The expression.</param>
        ///
        /// <returns>A Func&lt;object,string&gt;</returns>
		public static Func<object, string> CompileToString(Type type, string expr)
		{
			var param = Expression.Parameter(typeof(object), "model");
			Expression body = Expression.Convert(param, type);

			var members = expr.Split('.');
			for (int i = 0; i < members.Length; i++)
			{
				body = Expression.PropertyOrField(body, members[i]);
			}

			body = CallConvertToString(body);

			return Expression.Lambda<Func<object, string>>(body, param).Compile();
		}

		private static Expression CallConvertToString(Expression body)
		{
			var method = typeof(Convert).GetMethod("ToString",
				BindingFlags.Static | BindingFlags.Public,
				null, new[] { body.Type }, null);
			
			if (method == null)
			{
				method = typeof(Convert).GetMethod("ToString",
					BindingFlags.Static | BindingFlags.Public,
					null, new[] { typeof(object) }, null);

				body = Expression.Call(method, Expression.Convert(body, typeof(object)));
			}
			else
			{
				body = Expression.Call(method, body);
			}

			return body;
		}

		private static Type GetStaticType(string typeName)
		{
			var type = Type.GetType("System." + typeName);
			return type;
		}

        /// <summary>Compile static access to string.</summary>
        ///
        /// <param name="expr">The expression.</param>
        ///
        /// <returns>A Func&lt;string&gt;</returns>
		public static Func<string> CompileStaticAccessToString(string expr)
		{
			if (string.IsNullOrEmpty(expr)) return null;

			var members = expr.Split('.');

			var type = GetStaticType(members[0]);
			if (type == null || members.Length <= 1) return null;

			var firstPropertyAccess = members[1];
			Expression body = Expression.Property(null, type.GetProperty(firstPropertyAccess));
			for (var i = 2; i < members.Length; i++)
			{
				body = Expression.PropertyOrField(body, members[i]);
			}
			
			body = Expression.Convert(body, typeof(object));
			body = CallConvertToString(body);
			
			var fn = Expression.Lambda<Func<string>>(body).Compile();
			return fn;
		}

        /// <summary>Compiles.</summary>
        ///
        /// <param name="type">The type.</param>
        /// <param name="expr">The expression.</param>
        ///
        /// <returns>A Func&lt;object,object&gt;</returns>
		public static Func<object, object> Compile(Type type, string expr)
		{
			var param = Expression.Parameter(typeof(object), "model");
			Expression body = Expression.Convert(param, type);

			var members = expr.Split('.');
			for (var i = 1; i < members.Length; i++)
			{
				body = Expression.PropertyOrField(body, members[i]);
			}

			body = Expression.Convert(body, typeof (object));

			return Expression.Lambda<Func<object, object>>(body, param).Compile();
		}

        /// <summary>Compiles.</summary>
        ///
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <typeparam name="TProp"> Type of the property.</typeparam>
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>A Func&lt;TModel,TProp&gt;</returns>
		public static Func<TModel, TProp> Compile<TModel, TProp>(string expression)
		{
			var propNames = expression.Split('.');

			var model = Expression.Parameter(typeof(TModel), "model");

			Expression body = model;
			foreach (string propName in propNames.Skip(1))
				body = Expression.Property(body, propName);
			//Debug.WriteLine(prop);

			if (body.Type != typeof(TProp))
				body = Expression.Convert(body, typeof(TProp));

			Func<TModel, TProp> func = Expression.Lambda<Func<TModel, TProp>>(body, model).Compile();
			//TODO: cache funcs
			return func;
		}
	}
}