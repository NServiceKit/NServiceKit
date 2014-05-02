using System;
using System.Text;

namespace NServiceKit.Common
{
    /// <summary>
    /// Creates a Unified Resource Name (URN) with the following formats:
    /// 
    ///		- urn:{TypeName}:{IdFieldValue}						e.g. urn:UserSession:1
    ///		- urn:{TypeName}:{IdFieldName}:{IdFieldValue}		e.g. urn:UserSession:UserId:1			
    /// 
    /// </summary>
    public class UrnId
    {
        private const char FieldSeperator = ':';
        private const char FieldPartsSeperator = '/';

        /// <summary>Gets the name of the type.</summary>
        ///
        /// <value>The name of the type.</value>
        public string TypeName { get; private set; }

        /// <summary>Gets the identifier field value.</summary>
        ///
        /// <value>The identifier field value.</value>
        public string IdFieldValue { get; private set; }

        /// <summary>Gets the name of the identifier field.</summary>
        ///
        /// <value>The name of the identifier field.</value>
        public string IdFieldName { get; private set; }

        const int HasNoIdFieldName = 3;
        const int HasIdFieldName = 4;

        private UrnId() { }

        /// <summary>Parses.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="urnId">Identifier for the URN.</param>
        ///
        /// <returns>An UrnId.</returns>
        public static UrnId Parse(string urnId)
        {
            var urnParts = urnId.Split(FieldSeperator);
            if (urnParts.Length == HasNoIdFieldName)
            {
                return new UrnId { TypeName = urnParts[1], IdFieldValue = urnParts[2] };
            }
            if (urnParts.Length == HasIdFieldName)
            {
                return new UrnId { TypeName = urnParts[1], IdFieldName = urnParts[2], IdFieldValue = urnParts[3] };
            }
            throw new ArgumentException("Cannot parse invalid urn: '{0}'", urnId);
        }

        /// <summary>Creates a new string.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="objectTypeName">Name of the object type.</param>
        /// <param name="idFieldValue">  The identifier field value.</param>
        ///
        /// <returns>A string.</returns>
        public static string Create(string objectTypeName, string idFieldValue)
        {
            if (objectTypeName.Contains(FieldSeperator.ToString()))
            {
                throw new ArgumentException("objectTypeName cannot have the illegal characters: ':'", "objectTypeName");
            }
            if (idFieldValue.Contains(FieldSeperator.ToString()))
            {
                throw new ArgumentException("idFieldValue cannot have the illegal characters: ':'", "idFieldValue");
            }
            return string.Format("urn:{0}:{1}", objectTypeName, idFieldValue);
        }

        /// <summary>Creates with parts.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="objectTypeName">Name of the object type.</param>
        /// <param name="keyParts">      A variable-length parameters list containing key parts.</param>
        ///
        /// <returns>The new with parts.</returns>
        public static string CreateWithParts(string objectTypeName, params string[] keyParts)
        {
            if (objectTypeName.Contains(FieldSeperator.ToString()))
            {
                throw new ArgumentException("objectTypeName cannot have the illegal characters: ':'", "objectTypeName");
            }

            var sb = new StringBuilder();
            foreach (var keyPart in keyParts)
            {
                if (sb.Length > 0)
                    sb.Append(FieldPartsSeperator);
                sb.Append(keyPart);
            }

            return string.Format("urn:{0}:{1}", objectTypeName, sb);
        }

        /// <summary>Creates with parts.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="keyParts">A variable-length parameters list containing key parts.</param>
        ///
        /// <returns>The new with parts.</returns>
        public static string CreateWithParts<T>(params string[] keyParts)
        {
            return CreateWithParts(typeof(T).Name, keyParts);
        }

        /// <summary>Creates a new string.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="idFieldValue">The identifier field value.</param>
        ///
        /// <returns>A string.</returns>
        public static string Create<T>(string idFieldValue)
        {
            return Create(typeof(T), idFieldValue);
        }

        /// <summary>Creates a new string.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="idFieldValue">The identifier field value.</param>
        ///
        /// <returns>A string.</returns>
        public static string Create<T>(object idFieldValue)
        {
            return Create(typeof(T), idFieldValue.ToString());
        }

        /// <summary>Creates a new string.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="objectType">  Type of the object.</param>
        /// <param name="idFieldValue">The identifier field value.</param>
        ///
        /// <returns>A string.</returns>
        public static string Create(Type objectType, string idFieldValue)
        {
            if (idFieldValue.Contains(FieldSeperator.ToString()))
            {
                throw new ArgumentException("idFieldValue cannot have the illegal characters: ':'", "idFieldValue");
            }
            return string.Format("urn:{0}:{1}", objectType.Name, idFieldValue);
        }

        /// <summary>Creates a new string.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="idFieldName"> Name of the identifier field.</param>
        /// <param name="idFieldValue">The identifier field value.</param>
        ///
        /// <returns>A string.</returns>
        public static string Create<T>(string idFieldName, string idFieldValue)
        {
            return Create(typeof (T), idFieldName, idFieldValue);
        }

        /// <summary>Creates a new string.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="objectType">  Type of the object.</param>
        /// <param name="idFieldName"> Name of the identifier field.</param>
        /// <param name="idFieldValue">The identifier field value.</param>
        ///
        /// <returns>A string.</returns>
        public static string Create(Type objectType, string idFieldName, string idFieldValue)
        {
            if (idFieldValue.Contains(FieldSeperator.ToString()))
            {
                throw new ArgumentException("idFieldValue cannot have the illegal characters: ':'", "idFieldValue");
            }
            if (idFieldName.Contains(FieldSeperator.ToString()))
            {
                throw new ArgumentException("idFieldName cannot have the illegal characters: ':'", "idFieldName");
            }
            return string.Format("urn:{0}:{1}:{2}", objectType.Name, idFieldName, idFieldValue);
        }

        /// <summary>Gets string identifier.</summary>
        ///
        /// <param name="urn">The URN.</param>
        ///
        /// <returns>The string identifier.</returns>
        public static string GetStringId(string urn)
        {
            return Parse(urn).IdFieldValue;
        }

        /// <summary>Gets unique identifier.</summary>
        ///
        /// <param name="urn">The URN.</param>
        ///
        /// <returns>The unique identifier.</returns>
        public static Guid GetGuidId(string urn)
        {
            return new Guid(Parse(urn).IdFieldValue);
        }

        /// <summary>Gets long identifier.</summary>
        ///
        /// <param name="urn">The URN.</param>
        ///
        /// <returns>The long identifier.</returns>
        public static long GetLongId(string urn)
        {
            return long.Parse(Parse(urn).IdFieldValue);
        }
    }
}