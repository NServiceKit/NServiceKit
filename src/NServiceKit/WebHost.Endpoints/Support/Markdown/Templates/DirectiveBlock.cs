using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceKit.Common;
using NServiceKit.Markdown;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectiveBlock : StatementExprBlock
    {
        /// <summary>
        /// Gets or sets the base type.
        /// </summary>
        /// <value>
        /// The base type
        /// </value>
        public Type BaseType { get; set; }

        /// <summary>
        /// Gets or sets the generic arguments.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public Type[] GenericArgs { get; set; }

        /// <summary>
        /// Gets or sets the helpers.
        /// </summary>
        /// <value>
        /// The helpers.
        /// </value>
        public Dictionary<string, Type> Helpers { get; set; }

        /// <summary>
        /// Gets or sets the template path.
        /// </summary>
        /// <value>
        /// The template path.
        /// </value>
        public string TemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the variable declarations.
        /// </summary>
        /// <value>
        /// The variable declarations.
        /// </value>
        protected Dictionary<string, Func<object, object>> VarDeclarations { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        /// <exception cref="System.TypeLoadException">Could not load type:  + typeName</exception>
        public Type GetType(string typeName)
        {
            var type = Evaluator.FindType(typeName)
                       ?? AssemblyUtils.FindType(typeName);
            if (type == null)
            {
                var parts = typeName.Split(new[] { '<', '>' });
                if (parts.Length > 1)
                {
                    var genericTypeName = parts[0];
                    var genericArgNames = parts[1].Split(',');
                    var genericDefinitionName = genericTypeName + "`" + genericArgNames.Length;
                    var genericDefinition = Type.GetType(genericDefinitionName);
                    var argTypes = genericArgNames.Select(AssemblyUtils.FindType).ToArray();
                    var concreteType = genericDefinition.MakeGenericType(argTypes);
                    type = concreteType;
                }
                else
                {
                    throw new TypeLoadException("Could not load type: " + typeName);
                }
            }

            return type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectiveBlock"/> class.
        /// </summary>
        /// <param name="directive">The directive.</param>
        /// <param name="line">The line.</param>
        public DirectiveBlock(string directive, string line)
            : base(directive, null)
        {
            if (directive == null)
                throw new ArgumentNullException("directive");
            if (line == null)
                throw new ArgumentNullException("line");

            directive = directive.ToLower();
            line = line.Trim();

            if (directive == "model")
            {
                this.BaseType = typeof(MarkdownViewBase<>);
                this.GenericArgs = new[] { GetType(line) };
            }
            else if (directive == "inherits")
            {
                var parts = line.Split(new[] { '<', '>' })
                    .Where(x => !x.IsNullOrEmpty()).ToArray();

                var isGenericType = parts.Length >= 2;

                this.BaseType = isGenericType ? GetType(parts[0] + "`1") : GetType(parts[0]);

                if (isGenericType)
                {
                    this.GenericArgs = new[] { GetType(parts[1]) };
                }
            }
            else if (directive == "helper")
            {
                var helpers = line.Split(',');
                this.Helpers = new Dictionary<string, Type>();

                foreach (var helper in helpers)
                {
                    var parts = helper.Split(':');
                    if (parts.Length != 2)
                        throw new InvalidDataException(
                            "Invalid helper directive, should be 'TagName: Helper.Namespace.And.Type'");

                    var tagName = parts[0].Trim();
                    var typeName = parts[1].Trim();

                    var helperType = GetType(typeName);
                    if (helperType == null)
                        throw new InvalidDataException("Unable to resolve helper type: " + typeName);

                    this.Helpers[tagName] = helperType;
                }
            }
            else if (directive == "template" || directive == "layout")
            {
                this.TemplatePath = line.Trim().Trim('"');
            }
        }

        /// <summary>Called when [first run].</summary>
        protected override void OnFirstRun()
        {
            base.OnFirstRun();

            if (this.BaseType != null)
                Page.ExecutionContext.BaseType = this.BaseType;

            Page.ExecutionContext.GenericArgs = this.GenericArgs;

            if (this.Helpers != null)
            {
                foreach (var helper in this.Helpers)
                {
                    Page.ExecutionContext.TypeProperties[helper.Key] = helper.Value;
                }
            }
        }

        /// <summary>Writes the specified instance.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs) { }
    }
}