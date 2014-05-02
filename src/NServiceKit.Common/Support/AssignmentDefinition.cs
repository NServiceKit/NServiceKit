using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NServiceKit.Common.Utils;
using NServiceKit.Logging;
using NServiceKit.Text;

namespace NServiceKit.Common.Support
{
    /// <summary>An assignment entry.</summary>
    public class AssignmentEntry
    {
        /// <summary>The name.</summary>
        public string Name;
        /// <summary>Source for the.</summary>
        public AssignmentMember From;
        /// <summary>to.</summary>
        public AssignmentMember To;
        /// <summary>The get value function.</summary>
        public PropertyGetterDelegate GetValueFn;
        /// <summary>The set value function.</summary>
        public PropertySetterDelegate SetValueFn;

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.AssignmentEntry class.</summary>
        ///
        /// <param name="name">The name.</param>
        /// <param name="from">Source for the.</param>
        /// <param name="to">  to.</param>
        public AssignmentEntry(string name, AssignmentMember @from, AssignmentMember to)
        {
            Name = name;
            From = @from;
            To = to;

            GetValueFn = From.GetGetValueFn();
            SetValueFn = To.GetSetValueFn();
        }
    }

    /// <summary>An assignment member.</summary>
    public class AssignmentMember
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.AssignmentMember class.</summary>
        ///
        /// <param name="type">        The type.</param>
        /// <param name="propertyInfo">Information describing the property.</param>
        public AssignmentMember(Type type, PropertyInfo propertyInfo)
        {
            Type = type;
            PropertyInfo = propertyInfo;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.AssignmentMember class.</summary>
        ///
        /// <param name="type">     The type.</param>
        /// <param name="fieldInfo">Information describing the field.</param>
        public AssignmentMember(Type type, FieldInfo fieldInfo)
        {
            Type = type;
            FieldInfo = fieldInfo;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.AssignmentMember class.</summary>
        ///
        /// <param name="type">      The type.</param>
        /// <param name="methodInfo">Information describing the method.</param>
        public AssignmentMember(Type type, MethodInfo methodInfo)
        {
            Type = type;
            MethodInfo = methodInfo;
        }

        /// <summary>The type.</summary>
        public Type Type;
        /// <summary>Information describing the property.</summary>
        public PropertyInfo PropertyInfo;
        /// <summary>Information describing the field.</summary>
        public FieldInfo FieldInfo;
        /// <summary>Information describing the method.</summary>
        public MethodInfo MethodInfo;

        /// <summary>Gets value function.</summary>
        ///
        /// <returns>The value function.</returns>
        public PropertyGetterDelegate GetGetValueFn()
        {
            if (PropertyInfo != null)
                return PropertyInfo.GetPropertyGetterFn();
            if (FieldInfo != null)
                return o => FieldInfo.GetValue(o);
            if (MethodInfo != null)
#if NETFX_CORE
                return (PropertyGetterDelegate)
                    MethodInfo.CreateDelegate(typeof(PropertyGetterDelegate));
#else
                return (PropertyGetterDelegate)
                    Delegate.CreateDelegate(typeof(PropertyGetterDelegate), MethodInfo);
#endif
            return null;
        }

        /// <summary>Gets set value function.</summary>
        ///
        /// <returns>The set value function.</returns>
        public PropertySetterDelegate GetSetValueFn()
        {
            if (PropertyInfo != null)
                return PropertyInfo.GetPropertySetterFn();
            if (FieldInfo != null)
                return (o, v) => FieldInfo.SetValue(o, v);
            if (MethodInfo != null)
                return (PropertySetterDelegate)MethodInfo.MakeDelegate(typeof(PropertySetterDelegate));

            return null;
        }
    }

    /// <summary>An assignment definition.</summary>
    public class AssignmentDefinition
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssignmentDefinition));

        /// <summary>Initializes a new instance of the NServiceKit.Common.Support.AssignmentDefinition class.</summary>
        public AssignmentDefinition()
        {
            this.AssignmentMemberMap = new Dictionary<string, AssignmentEntry>();
        }

        /// <summary>Gets or sets the type of from.</summary>
        ///
        /// <value>The type of from.</value>
        public Type FromType { get; set; }

        /// <summary>Gets or sets the type of to.</summary>
        ///
        /// <value>The type of to.</value>
        public Type ToType { get; set; }

        /// <summary>Gets or sets the assignment member map.</summary>
        ///
        /// <value>The assignment member map.</value>
        public Dictionary<string, AssignmentEntry> AssignmentMemberMap { get; set; }

        /// <summary>Adds a match.</summary>
        ///
        /// <param name="name">       The name.</param>
        /// <param name="readMember"> The read member.</param>
        /// <param name="writeMember">The write member.</param>
        public void AddMatch(string name, AssignmentMember readMember, AssignmentMember writeMember)
        {
            this.AssignmentMemberMap[name] = new AssignmentEntry(name, readMember, writeMember);
        }

        /// <summary>Populate from properties with attribute.</summary>
        ///
        /// <param name="to">           to.</param>
        /// <param name="from">         Source for the.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        public void PopulateFromPropertiesWithAttribute(object to, object from, Type attributeType)
        {
            var hasAttributePredicate = (Func<PropertyInfo, bool>)
                (x => x.CustomAttributes(attributeType).Length > 0);
            Populate(to, from, hasAttributePredicate, null);
        }

        /// <summary>Populate with non default values.</summary>
        ///
        /// <param name="to">  to.</param>
        /// <param name="from">Source for the.</param>
        public void PopulateWithNonDefaultValues(object to, object from)
        {
            var nonDefaultPredicate = (Func<object, Type, bool>) ((x, t) => 
                    x != null && !Equals( x, ReflectionUtils.GetDefaultValue(t) )
                );
    
            Populate(to, from, null, nonDefaultPredicate);
        }

        /// <summary>Populates.</summary>
        ///
        /// <param name="to">  to.</param>
        /// <param name="from">Source for the.</param>
        public void Populate(object to, object from)
        {
            Populate(to, from, null, null);
        }

        /// <summary>Populates.</summary>
        ///
        /// <param name="to">                   to.</param>
        /// <param name="from">                 Source for the.</param>
        /// <param name="propertyInfoPredicate">The property information predicate.</param>
        /// <param name="valuePredicate">       The value predicate.</param>
        public void Populate(object to, object from,
            Func<PropertyInfo, bool> propertyInfoPredicate,
            Func<object, Type, bool> valuePredicate)
        {
            foreach (var assignmentEntry in AssignmentMemberMap)
            {
                var assignmentMember = assignmentEntry.Value;
                var fromMember = assignmentEntry.Value.From;
                var toMember = assignmentEntry.Value.To;

                if (fromMember.PropertyInfo != null && propertyInfoPredicate != null)
                {
                    if (!propertyInfoPredicate(fromMember.PropertyInfo)) continue;
                }

                try
                {
                    var fromValue = assignmentMember.GetValueFn(from);

                    if (valuePredicate != null)
                    {
                        if (!valuePredicate(fromValue, fromMember.PropertyInfo.PropertyType)) continue;
                    }

                    if (fromMember.Type != toMember.Type)
                    {
                        if (fromMember.Type == typeof(string))
                        {
                            fromValue = TypeSerializer.DeserializeFromString((string)fromValue, toMember.Type);
                        }
                        else if (toMember.Type == typeof(string))
                        {
                            fromValue = TypeSerializer.SerializeToString(fromValue);
                        }
                        else if (toMember.Type.IsGeneric()
                            && toMember.Type.GenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type genericArg = toMember.Type.GenericTypeArguments()[0];
                            if (genericArg.IsEnum())
                            {
								fromValue = Enum.ToObject(genericArg, fromValue);
							}
						}
                        else
                        {
                            var listResult = TranslateListWithElements.TryTranslateToGenericICollection(
                                fromMember.Type, toMember.Type, fromValue);

                            if (listResult != null)
                            {
                                fromValue = listResult;
                            }
                        }
                    }

                    var setterFn = assignmentMember.SetValueFn;
                    setterFn(to, fromValue);
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format("Error trying to set properties {0}.{1} > {2}.{3}",
                        FromType.FullName, fromMember.Type.Name,
                        ToType.FullName, toMember.Type.Name), ex);
                }
            }
        }
    }
}