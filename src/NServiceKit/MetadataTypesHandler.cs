using System.Web;
using NServiceKit.Common;
using NServiceKit.Common.ServiceModel;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using HttpRequestWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpResponseWrapper;

namespace NServiceKit
{
    /// <summary>A metadata types handler.</summary>
    public class MetadataTypesHandler : HttpHandlerBase, INServiceKitHttpHandler
    {
        /// <summary>Gets or sets the configuration.</summary>
        ///
        /// <value>The configuration.</value>
        public MetadataTypesConfig Config { get; set; }

        /// <summary>Executes the given context.</summary>
        ///
        /// <param name="context">The context.</param>
        public override void Execute(HttpContext context)
        {
            ProcessRequest(
                new HttpRequestWrapper(GetType().Name, context.Request),
                new HttpResponseWrapper(context.Response),
                GetType().Name);
        }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            var metadata = new MetadataTypes
            {
                Config = Config,
            };
            var existingTypes = new HashSet<Type> {
                typeof(ResponseStatus),
                typeof(ErrorResponse),
            };

            var meta = EndpointHost.Metadata;
            foreach (var operation in meta.Operations)
            {
                if (!meta.IsVisible(httpReq, operation))
                    continue;

                metadata.Operations.Add(new MetadataOperationType
                {
                    Actions = operation.Actions,
                    Request = operation.RequestType.ToType(),
                    Response = operation.ResponseType.ToType(),
                });

                existingTypes.Add(operation.RequestType);
                if (operation.ResponseType != null)
                {
                    existingTypes.Add(operation.ResponseType);
                }
            }

            foreach (var type in meta.GetAllTypes())
            {
                if (existingTypes.Contains(type))
                    continue;

                metadata.Operations.Add(new MetadataOperationType
                {
                    Request = type.ToType(),
                });

                existingTypes.Add(type);
            }

            var considered = new HashSet<Type>(existingTypes);
            var queue = new Queue<Type>(existingTypes);

            while (queue.Count > 0)
            {
                var type = queue.Dequeue();
                foreach (var pi in type.GetSerializableProperties())
                {
                    if (pi.PropertyType.IsUserType())
                    {
                        if (considered.Contains(pi.PropertyType))
                            continue;

                        considered.Add(pi.PropertyType);
                        queue.Enqueue(pi.PropertyType);
                        metadata.Types.Add(pi.PropertyType.ToType());
                    }
                }

                if (type.BaseType != null
                    && type.BaseType.IsUserType()
                    && !considered.Contains(type.BaseType))
                {
                    considered.Add(type.BaseType);
                    queue.Enqueue(type.BaseType);
                    metadata.Types.Add(type.BaseType.ToType());
                }
            }

            var json = metadata.ToJson();

            //httpRes.ContentType = "application/json";
            //httpRes.Write(json);
            //return;

            httpRes.ContentType = "application/x-ssz-metatypes";
            var encJson = CryptUtils.Encrypt(EndpointHostConfig.PublicKey, json, RsaKeyLengths.Bit2048);
            httpRes.Write(encJson);
        }
    }

    /// <summary>A metadata type extensions.</summary>
    public static class MetadataTypeExtensions
    {
        /// <summary>A Type extension method that converts a type to a type.</summary>
        ///
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>type as a MetadataType.</returns>
        public static MetadataType ToType(this Type type)
        {
            if (type == null) return null;

            var metaType = new MetadataType
            {
                Name = type.Name,
                Namespace = type.Namespace,
                GenericArgs = type.IsGenericType
                    ? type.GetGenericArguments().Select(x => x.Name).ToArray()
                    : null,
                Attributes = type.ToAttributes(),
                Properties = type.ToProperties(),
            };

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                metaType.Inherits = type.BaseType.Name;
                metaType.InheritsGenericArgs = type.BaseType.IsGenericType
                    ? type.BaseType.GetGenericArguments().Select(x => x.Name).ToArray()
                    : null;
            }

            if (type.GetTypeWithInterfaceOf(typeof(IReturnVoid)) != null)
            {
                metaType.ReturnVoidMarker = true;
            }
            else
            {
                var genericMarker = type.GetTypeWithGenericTypeDefinitionOf(typeof(IReturn<>));
                if (genericMarker != null)
                {
                    metaType.ReturnMarkerGenericArgs = genericMarker.GetGenericArguments().Select(x => x.Name).ToArray();
                }
            }

            var typeAttrs = TypeDescriptor.GetAttributes(type);
            var routeAttrs = typeAttrs.OfType<RouteAttribute>().ToList();
            if (routeAttrs.Count > 0)
            {
                metaType.Routes = routeAttrs.ConvertAll(x =>
                    new MetadataRoute
                    {
                        Path = x.Path,
                        Notes = x.Notes,
                        Summary = x.Summary,
                        Verbs = x.Verbs,
                    });
            }

            var descAttr = typeAttrs.OfType<DescriptionAttribute>().FirstOrDefault();
            if (descAttr != null)
            {
                metaType.Description = descAttr.Description;
            }

            var dcAttr = type.GetDataContract();
            if (dcAttr != null)
            {
                metaType.DataContract = new MetadataDataContract
                {
                    Name = dcAttr.Name,
                    Namespace = dcAttr.Namespace,
                };
            }

            return metaType;
        }

        /// <summary>An IEnumerable&lt;Attribute&gt; extension method that converts the attrs to the attributes.</summary>
        ///
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>attrs as a List&lt;MetadataAttribute&gt;</returns>
        public static List<MetadataAttribute> ToAttributes(this Type type)
        {
            return !type.IsUserType() || type.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>))
                ? null
                : type.GetCustomAttributes(false).ToAttributes();
        }

        /// <summary>A Type extension method that converts a type to the properties.</summary>
        ///
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>type as a List&lt;MetadataPropertyType&gt;</returns>
        public static List<MetadataPropertyType> ToProperties(this Type type)
        {
            var props = !type.IsUserType() || type.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>))
                ? null
                : type.GetInstancePublicProperties().ToList().ConvertAll(x => x.ToProperty());

            return props == null || props.Count == 0 ? null : props;
        }

        /// <summary>Exclude known attributes filter.</summary>
        ///
        /// <param name="x">The Attribute to process.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ExcludeKnownAttrsFilter(Attribute x)
        {
            return x.GetType() != typeof(RouteAttribute)
                && x.GetType() != typeof(DescriptionAttribute)
                && x.GetType().Name != "DataContractAttribute"  //Type equality issues with Mono .NET 3.5/4
                && x.GetType().Name != "DataMemberAttribute";
        }

        /// <summary>An IEnumerable&lt;Attribute&gt; extension method that converts the attrs to the attributes.</summary>
        ///
        /// <param name="attrs">The attrs to act on.</param>
        ///
        /// <returns>attrs as a List&lt;MetadataAttribute&gt;</returns>
        public static List<MetadataAttribute> ToAttributes(this object[] attrs)
        {
            var to = attrs.OfType<Attribute>()
                .Where(ExcludeKnownAttrsFilter)
                .ToList().ConvertAll(x => x.ToAttribute());

            return to.Count == 0 ? null : to;
        }

        /// <summary>An IEnumerable&lt;Attribute&gt; extension method that converts the attrs to the attributes.</summary>
        ///
        /// <param name="attrs">The attrs to act on.</param>
        ///
        /// <returns>attrs as a List&lt;MetadataAttribute&gt;</returns>
        public static List<MetadataAttribute> ToAttributes(this IEnumerable<Attribute> attrs)
        {
            var to = attrs
                .Where(ExcludeKnownAttrsFilter)
                .Select(attr => attr.ToAttribute())
                .ToList();
            return to.Count == 0 ? null : to;
        }

        /// <summary>An Attribute extension method that converts an attr to an attribute.</summary>
        ///
        /// <param name="attr">The attr to act on.</param>
        ///
        /// <returns>attr as a MetadataAttribute.</returns>
        public static MetadataAttribute ToAttribute(this Attribute attr)
        {
            var firstCtor = attr.GetType().GetConstructors().OrderBy(x => x.GetParameters().Length).FirstOrDefault();
            var metaAttr = new MetadataAttribute
            {
                Name = attr.GetType().Name,
                ConstructorArgs = firstCtor != null
                    ? firstCtor.GetParameters().ToList().ConvertAll(x => x.ToProperty())
                    : null,
                Args = attr.NonDefaultProperties(),
            };
            return metaAttr;
        }

        /// <summary>An Attribute extension method that non default properties.</summary>
        ///
        /// <param name="attr">The attr to act on.</param>
        ///
        /// <returns>A List&lt;MetadataPropertyType&gt;</returns>
        public static List<MetadataPropertyType> NonDefaultProperties(this Attribute attr)
        {
            return attr.GetType().GetPublicProperties()
                .Select(pi => pi.ToProperty(attr))
                .Where(property => property.Name != "TypeId"
                    && property.Value != null)
                .ToList();
        }

        /// <summary>A ParameterInfo extension method that converts a pi to a property.</summary>
        ///
        /// <param name="pi">      The pi to act on.</param>
        /// <param name="instance">The instance.</param>
        ///
        /// <returns>pi as a MetadataPropertyType.</returns>
        public static MetadataPropertyType ToProperty(this PropertyInfo pi, object instance = null)
        {
            var property = new MetadataPropertyType
            {
                Name = pi.Name,
                Attributes = pi.GetCustomAttributes(false).ToAttributes(),
                Type = pi.PropertyType.Name,
                DataMember = pi.GetDataMember().ToDataMember(),
                GenericArgs = pi.PropertyType.IsGenericType
                    ? pi.PropertyType.GetGenericArguments().Select(x => x.Name).ToArray()
                    : null,
            };
            if (instance != null)
            {
                var value = pi.GetValue(instance, null);
                if (value != pi.PropertyType.GetDefaultValue())
                {
                    property.Value = value.ToJson();
                }
            }
            return property;
        }

        /// <summary>A ParameterInfo extension method that converts a pi to a property.</summary>
        ///
        /// <param name="pi">The pi to act on.</param>
        ///
        /// <returns>pi as a MetadataPropertyType.</returns>
        public static MetadataPropertyType ToProperty(this ParameterInfo pi)
        {
            var propertyAttrs = pi.GetCustomAttributes(false);
            var property = new MetadataPropertyType
            {
                Name = pi.Name,
                Attributes = propertyAttrs.ToAttributes(),
                Type = pi.ParameterType.Name,
            };

            var descAttr = propertyAttrs.OfType<DescriptionAttribute>().FirstOrDefault();
            if (descAttr != null)
            {
                property.Description = descAttr.Description;
            }

            return property;
        }

        /// <summary>A DataMemberAttribute extension method that converts an attr to a data member.</summary>
        ///
        /// <param name="attr">The attr to act on.</param>
        ///
        /// <returns>attr as a MetadataDataMember.</returns>
        public static MetadataDataMember ToDataMember(this DataMemberAttribute attr)
        {
            if (attr == null) return null;

            var metaAttr = new MetadataDataMember
            {
                Name = attr.Name,
                EmitDefaultValue = attr.EmitDefaultValue != true ? attr.EmitDefaultValue : (bool?)null,
                Order = attr.Order >= 0 ? attr.Order : (int?)null,
                IsRequired = attr.IsRequired != false ? attr.IsRequired : (bool?)null,
            };

            return metaAttr;
        }

        /// <summary>A Type extension method that gets instance public properties.</summary>
        ///
        /// <param name="type">The type to act on.</param>
        ///
        /// <returns>An array of property information.</returns>
        public static PropertyInfo[] GetInstancePublicProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                .ToArray();
        }
    }
}