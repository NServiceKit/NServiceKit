using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NServiceKit.Text;

namespace NServiceKit.Razor.Json
{
    /// <summary>
    /// Also in .NET 4.0 of NServiceKit.Text
    /// </summary>
    public class DynamicJson : DynamicObject
    {
        private readonly IDictionary<string, object> _hash = new Dictionary<string, object>();

        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="instance">The instance.</param>
        ///
        /// <returns>A string.</returns>
        public static string Serialize(dynamic instance)
        {
            var json = JsonSerializer.SerializeToString(instance);
            return json;
        }

        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="json">The JSON.</param>
        ///
        /// <returns>A dynamic.</returns>
        public static dynamic Deserialize(string json)
        {
            // Support arbitrary nesting by using JsonObject
            var deserialized = JsonSerializer.DeserializeFromString<JsonObject>(json);
            var hash = deserialized.ToDictionary<KeyValuePair<string, string>, string, object>(entry => entry.Key, entry => entry.Value);
            return new DynamicJson(hash);
        }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Json.DynamicJson class.</summary>
        ///
        /// <param name="hash">The hash.</param>
        public DynamicJson(IEnumerable<KeyValuePair<string, object>> hash)
        {
            _hash.Clear();
            foreach (var entry in hash)
            {
                _hash.Add(Underscored(entry.Key), entry.Value);
            }
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
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = Underscored(binder.Name);
            _hash[name] = value;
            return _hash[name] == value;
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
            var name = Underscored(binder.Name);
            return YieldMember(name, out result);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonSerializer.SerializeToString(_hash);
        }

        private bool YieldMember(string name, out object result)
        {
            if (_hash.ContainsKey(name))
            {
                var json = _hash[name].ToString();
                if (json.TrimStart(' ').StartsWith("{"))
                {
                    result = Deserialize(json);
                    return true;
                }
                result = json;
                return _hash[name] == result;
            }
            result = null;
            return false;
        }

        internal static string Underscored(IEnumerable<char> pascalCase)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var c in pascalCase)
            {
                if (char.IsUpper(c) && i > 0)
                {
                    sb.Append("_");
                }
                sb.Append(c);
                i++;
            }
            return sb.ToString().ToLowerInvariant();
        }
    }
}