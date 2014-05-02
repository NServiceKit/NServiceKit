using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>An evaluator execution context.</summary>
	public class EvaluatorExecutionContext
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.EvaluatorExecutionContext class.</summary>
		public EvaluatorExecutionContext()
		{
			this.Items = new List<EvaluatorItem>();
			this.TypeProperties = new Dictionary<string, Type>();
		}

        /// <summary>Gets or sets the type of the base.</summary>
        ///
        /// <value>The type of the base.</value>
		public Type BaseType { get; set; }

        /// <summary>Gets or sets the generic arguments.</summary>
        ///
        /// <value>The generic arguments.</value>
		public Type[] GenericArgs { get; set; }

        /// <summary>Gets or sets the type properties.</summary>
        ///
        /// <value>The type properties.</value>
		public IDictionary<string, Type> TypeProperties { get; set; }

        /// <summary>Gets the items.</summary>
        ///
        /// <value>The items.</value>
		public List<EvaluatorItem> Items { get; private set; }

        /// <summary>Gets the build.</summary>
        ///
        /// <returns>An Evaluator.</returns>
		public Evaluator Build()
		{
			return new Evaluator(Items, BaseType, GenericArgs, TypeProperties);
		}
	}

    /// <summary>An evaluator.</summary>
	public class Evaluator
	{
	    private static ILog Log = LogManager.GetLogger(typeof (Evaluator));

		const string StaticMethodName = "__tmp";
		Assembly compiledAssembly;
		Type compiledType = null;
		object compiled = null;
		EmptyCtorDelegate compiledTypeCtorFn;

		private Type BaseType { get; set; }
		private Type[] GenericArgs { get; set; }
		private IDictionary<string, Type> TypeProperties { get; set; }

	    static readonly List<Assembly> Assemblies = new List<Assembly> {
			typeof(string).Assembly,       //"system.dll",
//			typeof(XmlDocument).Assembly,  //"system.xml.dll",
			typeof(Expression).Assembly,   //"system.core.dll",
			typeof(AppHostBase).Assembly,  //"NServiceKit.dll",
			typeof(JsConfig).Assembly,     //"NServiceKit.Text.dll",
			typeof(IService).Assembly,   //"NServiceKit.Interfaces.dll",
			typeof(Common.UrnId).Assembly, //"NServiceKit.Common.dll"
		};

	    static readonly List<string> AssemblyNames = new List<string> {
	        "System",
            "System.Text",
//            "System.Xml",
            "System.Collections",
            "System.Collections.Generic",
            "System.Linq",
            "System.Linq.Expressions",
            "NServiceKit.Html",
            "NServiceKit.Markdown"                                                                     
        };

        /// <summary>NOTE: This assumes that the assembly name will always be equal to the namespace.</summary>
        ///
        /// <param name="assemblyName">Name of the assembly.</param>
        public static void AddAssembly(string assemblyName)
        {
            if (AssemblyNames.Contains(assemblyName)) return;
            AssemblyNames.Add(assemblyName);

            try {
                var assembly = Assembly.Load(assemblyName);
                if (!Assemblies.Contains(assembly))
                    Assemblies.Add(assembly);
            } catch (System.IO.FileNotFoundException) {
                //Possibly the assembly name differs from the namespace name
                FindNamespaceInLoadedAssemblies(assemblyName);
            } catch (Exception ex) {
                Log.Error("Can't load assembly: " + assemblyName, ex);
            }
        }

        private static void FindNamespaceInLoadedAssemblies(string assemblyNamespace)
        {
            var assemblies = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.Namespace == assemblyNamespace
                             select type.Assembly;

            foreach (var a in assemblies) {
                if (!Assemblies.Contains(a))
                    Assemblies.Add(a);
            }
        }

        /// <summary>Searches for the first type.</summary>
        ///
        /// <param name="typeName">Name of the type.</param>
        ///
        /// <returns>The found type.</returns>
        public static Type FindType(string typeName)
        {
            if (typeName == null || typeName.Contains(".")) return null;
            var type = Type.GetType(typeName);
            if (type != null) return type;
            
            foreach (var assembly in Assemblies)
            {
                var searchType = assembly.GetName().Name + "." + typeName;
                type = assembly.GetType(searchType);
                if (type != null) 
                    return type;
            }

            return null;
        }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.Evaluator class.</summary>
        ///
        /// <param name="items">The items.</param>
		public Evaluator(IEnumerable<EvaluatorItem> items)
			: this(items, null, null, null)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.Evaluator class.</summary>
        ///
        /// <param name="items">         The items.</param>
        /// <param name="baseType">      Type of the base.</param>
        /// <param name="genericArgs">   The generic arguments.</param>
        /// <param name="typeProperties">The type properties.</param>
		public Evaluator(IEnumerable<EvaluatorItem> items,
			Type baseType, Type[] genericArgs, IDictionary<string, Type> typeProperties)
		{
			this.BaseType = baseType;
			this.GenericArgs = genericArgs ?? new Type[0];
			this.TypeProperties = typeProperties;

			ConstructEvaluator(items);
		}

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.Evaluator class.</summary>
        ///
        /// <param name="returnType">Type of the return.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="name">      The name.</param>
		public Evaluator(Type returnType, string expression, string name)
			: this(returnType, expression, name, null) { }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.Evaluator class.</summary>
        ///
        /// <param name="returnType">Type of the return.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="name">      The name.</param>
        /// <param name="exprParams">Options for controlling the expression.</param>
		public Evaluator(Type returnType, string expression, string name, IDictionary<string, Type> exprParams)
		{
			EvaluatorItem[] items = 
			{
				new EvaluatorItem {
	                ReturnType  = returnType, 
					Expression = expression, 
					Name = name,
					Params = exprParams ?? new Dictionary<string, Type>(),
				}
			};
			ConstructEvaluator(items);
		}

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.Evaluator class.</summary>
        ///
        /// <param name="item">The item.</param>
		public Evaluator(EvaluatorItem item)
		{
			EvaluatorItem[] items = { item };
			ConstructEvaluator(items);
		}

        /// <summary>Gets type name.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The type name.</returns>
		public string GetTypeName(Type type)
		{
			try
			{
				//Inner classes?
				var typeName = type == null
					//|| type.FullName == null
					? null
					: type.FullName.Replace('+', '.').SplitOnFirst('`')[0];

				if (typeName == null) return null;

				if (type.HasGenericType()
					//TODO: support GenericTypeDefinition properly
					&& !type.IsGenericTypeDefinition
				)
				{
					var genericArgs = type.GetGenericArguments();

					typeName += "<";
					var i = 0;
					foreach (var genericArg in genericArgs)
					{
						if (i++ > 0)
							typeName += ", ";
						typeName += GetTypeName(genericArg);
					}
					typeName += ">";
				}

				return typeName;
			}
			catch (Exception)
			{
				//Console.WriteLine(ex);
				throw;
			}
		}

		private static readonly bool IsVersion4AndUp = Type.GetType("System.Collections.Concurrent.Partitioner") != null;

		private static void AddAssembly(CompilerParameters cp, string location)
		{
			//Error if trying to re-add ref to mscorlib or System.Core for .NET 4.0
			if (IsVersion4AndUp && 
				(location == typeof(string).Assembly.Location
				|| location == typeof(Expression).Assembly.Location))
				return;

			cp.ReferencedAssemblies.Add(location);
		}

		private void ConstructEvaluator(IEnumerable<EvaluatorItem> items)
		{
			//var codeCompiler = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
			var codeCompiler = CodeDomProvider.CreateProvider("CSharp");

			var cp = new CompilerParameters  //(new[] { "mscorlib.dll", "system.core.dll" })
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
			};
			Assemblies.ForEach(x => AddAssembly(cp, x.Location));
            
			var code = new StringBuilder();

            AssemblyNames.ForEach(x => 
                code.AppendFormat("using {0};\n", x));

			code.Append(
@"

namespace CSharpEval 
{
  public class _Expr
");

			if (this.BaseType != null)
			{
				code.Append("   : " + GetTypeName(this.BaseType));

				if (GenericArgs.Length > 0)
				{
					code.Append("<");
					var i = 0;
					foreach (var genericArg in GenericArgs)
					{
						if (i++ > 0) code.Append(", ");

						code.Append(GetTypeName(genericArg));
						ReferenceTypesIfNotExist(cp, Assemblies, genericArg);
					}
					code.AppendLine(">");
				}
				
				ReferenceTypesIfNotExist(cp, Assemblies, this.BaseType);
			}

			code.AppendLine("  {");

			AddPropertiesToTypeIfAny(code);

			foreach (var item in items)
			{
				var sbParams = new StringBuilder();
				foreach (var param in item.Params)
				{
					if (sbParams.Length > 0)
						sbParams.Append(", ");

					var typeName = GetTypeName(param.Value);
					sbParams.AppendFormat("{0} {1}", typeName, param.Key);

					var paramType = param.Value;

					ReferenceAssembliesIfNotExists(cp, paramType, Assemblies);
				}

				var isVoid = item.ReturnType == typeof(void);

				var returnType = isVoid ? "void" : GetTypeName(item.ReturnType);
				code.AppendFormat("    public {0} {1}({2})",
					returnType, item.Name, sbParams);

				code.AppendLine("    {");
				if (isVoid)
				{
					code.AppendFormat("      {0}; \n", item.Expression);
				}
				else
				{
					code.AppendFormat("      return ({0}); \n", item.Expression);
				}
				code.AppendLine("    }");
			}

			code.AppendLine("  }");
			code.AppendLine("}");

			if (IsVersion4AndUp)
			{
				//var type = Type.GetType("System.Collections.Concurrent.Partitioner");
				//if (type != null)
				//    cp.ReferencedAssemblies.Add(type.Assembly.Location);
				if (!Env.IsMono)
				{
					//cp.ReferencedAssemblies.Add(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll");
					cp.ReferencedAssemblies.Add(@"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.5\System.Core.dll");
				}
			}

			var src = code.ToString();
			var compilerResults = codeCompiler.CompileAssemblyFromSource(cp, src);
			if (compilerResults.Errors.HasErrors)
			{
				var error = new StringBuilder();
				error.Append("Error Compiling Expression: ");
				foreach (CompilerError err in compilerResults.Errors)
				{
					error.AppendFormat("{0}\n", err.ErrorText);
				}
				throw new Exception("Error Compiling Expression: " + error);
			}

			compiledAssembly = compilerResults.CompiledAssembly;
			compiled = compiledAssembly.CreateInstance("CSharpEval._Expr");
			compiledType = compiled.GetType();
			compiledTypeCtorFn = Text.ReflectionExtensions.GetConstructorMethodToCache(compiledType);
		}

		private static void ReferenceTypesIfNotExist(CompilerParameters cp, List<Assembly> assemblies, params Type[] paramTypes)
		{
			foreach (var paramType in paramTypes)
			{
				if (!assemblies.Contains(paramType.Assembly))
				{
					assemblies.Add(paramType.Assembly);
					AddAssembly(cp, paramType.Assembly.Location);
				}
			}
		}

		private static void ReferenceAssembliesIfNotExists(CompilerParameters cp, Type paramType, List<Assembly> assemblies)
		{
			var typeAssemblies = new List<Assembly>();

			var typeAssembly = paramType.Assembly;
			if (!assemblies.Contains(typeAssembly))
				typeAssemblies.Add(typeAssembly);

			if (paramType.IsGenericType)
			{
				var genericArgs = paramType.GetGenericArguments();
				typeAssemblies.AddRange(genericArgs.Select(x => x.Assembly));
			}

			foreach (var assembly in typeAssemblies)
			{
				assemblies.Add(assembly);

                if (assembly.IsDynamic()) continue;
				AddAssembly(cp, assembly.Location);
			}
		}

		private void AddPropertiesToTypeIfAny(StringBuilder code)
		{
			if (this.TypeProperties != null)
			{
				foreach (var typeProperty in TypeProperties)
				{
					var name = typeProperty.Key;
					var type = typeProperty.Value;
					var typeName = GetTypeName(type);

					var mi = type.GetMember("Instance", BindingFlags.Static | BindingFlags.Public);
					var hasSingleton = mi.Length > 0;

					var returnExpr = hasSingleton
									 ? typeName + ".Instance"
									 : "new " + typeName + "()";

					code.AppendFormat("    public {0} {1} = {2};\n", typeName, name, returnExpr);
				}
			}
		}

        /// <summary>Gets the instance.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>The instance.</returns>
		public T GetInstance<T>()
		{
			return (T)compiled;
		}

        /// <summary>Creates the instance.</summary>
        ///
        /// <returns>The new instance.</returns>
		public object CreateInstance()
		{
			return compiledTypeCtorFn();
		}

        /// <summary>Gets compiled method information.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The compiled method information.</returns>
		public MethodInfo GetCompiledMethodInfo(string name)
		{
			return compiledType.GetMethod(name);
		}

        /// <summary>Evaluates.</summary>
        ///
        /// <param name="name">      The name.</param>
        /// <param name="exprParams">Options for controlling the expression.</param>
        ///
        /// <returns>An object.</returns>
		public object Evaluate(string name, params object[] exprParams)
		{
			return Evaluate(compiled, name, exprParams);
		}

        /// <summary>Evaluates.</summary>
        ///
        /// <exception>Thrown when an Inner error condition occurs.</exception>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="name">      The name.</param>
        /// <param name="exprParams">Options for controlling the expression.</param>
        ///
        /// <returns>An object.</returns>
		public object Evaluate(object instance, string name, params object[] exprParams)
		{
			try
			{
				var mi = compiledType.GetMethod(name);
				return mi.Invoke(instance, exprParams);
			}
			catch (TargetInvocationException ex)
			{
				Console.WriteLine(ex.InnerException);
				throw ex.InnerException;
			}
		}

        /// <summary>Evals.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="name">      The name.</param>
        /// <param name="exprParams">Options for controlling the expression.</param>
        ///
        /// <returns>A T.</returns>
		public T Eval<T>(string name, params object[] exprParams)
		{
			return (T)Evaluate(name, exprParams);
		}

        /// <summary>Evals.</summary>
        ///
        /// <param name="code">The code.</param>
        ///
        /// <returns>An object.</returns>
		public static object Eval(string code)
		{
			var eval = new Evaluator(typeof(object), code, StaticMethodName);
			return eval.Evaluate(StaticMethodName);
		}

        /// <summary>Evals.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="code">The code.</param>
        ///
        /// <returns>A T.</returns>
		public static T Eval<T>(string code)
		{
			var eval = new Evaluator(typeof(T), code, StaticMethodName);
			return (T)eval.Evaluate(StaticMethodName);
		}

	}

    /// <summary>An evaluator item.</summary>
	public class EvaluatorItem
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.EvaluatorItem class.</summary>
		public EvaluatorItem() { }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Markdown.EvaluatorItem class.</summary>
        ///
        /// <param name="returnType">The type of the return.</param>
        /// <param name="name">      The name.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="exprParams">Options for controlling the expression.</param>
		public EvaluatorItem(Type returnType, string name, string expression, IDictionary<string, Type> exprParams)
		{
			ReturnType = returnType;
			Name = name;
			Expression = expression;
			Params = exprParams;
		}

        /// <summary>Gets or sets the type of the return.</summary>
        ///
        /// <value>The type of the return.</value>
		public Type ReturnType { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the expression.</summary>
        ///
        /// <value>The expression.</value>
		public string Expression { get; set; }

        /// <summary>Gets or sets options for controlling the operation.</summary>
        ///
        /// <value>The parameters.</value>
		public IDictionary<string, Type> Params { get; set; }
	}
}
