using NUnit.Framework;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.ServiceModel.Tests.DataContracts.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NServiceKit.Text;

namespace NServiceKit.ServiceModel.Tests
{
    /// <summary>A key value data contract deserializer tests.</summary>
    [TestFixture]
    public class KeyValueDataContractDeserializerTests
    {
        /// <summary>Creates dto request from identifiers.</summary>
        [Test]
        public void create_dto_request_from_ids()
        {
            var dtoType = typeof(GetCustomers);
            var textValue = "1,2,3";
            var convertedValue = textValue.Split(',').ToList().ConvertAll(x => Convert.ToInt32(x));
            var valueMap = new Dictionary<string, string> { { "CustomerIds", textValue } };
            var result = (GetCustomers)KeyValueDataContractDeserializer.Instance.Parse(valueMap, dtoType);
            Assert.That(result.CustomerIds, Is.EquivalentTo(convertedValue));
        }

        /// <summary>A customer.</summary>
        [DataContract]
        public class Customer
        {
            /// <summary>Gets or sets the person's first name.</summary>
            ///
            /// <value>The name of the first.</value>
            [DataMember(Name = "first_name")]
            public string FirstName { get; set; }

            /// <summary>Gets or sets the person's last name.</summary>
            ///
            /// <value>The name of the last.</value>
            [DataMember(Name = "last_name")]
            public string LastName { get; set; }

            /// <summary>Gets or sets the age.</summary>
            ///
            /// <value>The age.</value>
            [DataMember(Name = "age")]
            public int? Age { get; set; }

            /// <summary>Gets or sets the Date/Time of the dob.</summary>
            ///
            /// <value>The dob.</value>
            [DataMember(Name = "dob")]
            public DateTime? DOB { get; set; }
        }

        /// <summary>Kvp serializer does use data member name attribute.</summary>
        [Test]
        public void KVP_Serializer_does_use_DataMember_Name_attribute()
        {
            var valueMap = new Dictionary<string, string> { { "first_name", "james" }, { "last_name", "bond" } };

            var result = (Customer)KeyValueDataContractDeserializer.Instance.Parse(valueMap, typeof(Customer));

            Assert.That(result.FirstName, Is.EqualTo("james"));
            Assert.That(result.LastName, Is.EqualTo("bond"));
        }

        /// <summary>Kvp serializer does not set nullable properties when values are empty.</summary>
        [Test]
        public void KVP_Serializer_does_not_set_nullable_properties_when_values_are_empty()
        {
            var valueMap = new Dictionary<string, string> { { "first_name", "james" }, { "last_name", "bond" }, { "age", "" }, { "dob", "" } };

            var result = (Customer)KeyValueDataContractDeserializer.Instance.Parse(valueMap, typeof(Customer));

            Assert.That(result.Age, Is.Null);
            Assert.That(result.DOB, Is.Null);
        }

        /// <summary>A customer 2.</summary>
        public class Customer2
        {
            /// <summary>Gets or sets the person's first name.</summary>
            ///
            /// <value>The name of the first.</value>
            public string FirstName { get; set; }

            /// <summary>Gets or sets the person's last name.</summary>
            ///
            /// <value>The name of the last.</value>
            public string LastName { get; set; }

            /// <summary>Gets the name of the full.</summary>
            ///
            /// <value>The name of the full.</value>
            public string FullName
            {
                get { return FirstName + " " + LastName; }
            }
        }

        /// <summary>Kvp serializer ignores readonly properties.</summary>
        [Test]
        public void KVP_Serializer_ignores_readonly_properties()
        {
            var valueMap = new Dictionary<string, string> { { "FirstName", "james" }, { "LastName", "bond" }, { "FullName", "james bond" } };
            var result = (Customer2)KeyValueDataContractDeserializer.Instance.Parse(valueMap, typeof(Customer2));
            Assert.That(result.FirstName, Is.EqualTo("james"));
            Assert.That(result.LastName, Is.EqualTo("bond"));
        }

        /// <summary>A customer with fields.</summary>
        public class CustomerWithFields
        {
            /// <summary>The person's first name.</summary>
            public readonly string FirstName;

            /// <summary>The person's last name.</summary>
            public readonly string LastName;

            /// <summary>Gets the name of the full.</summary>
            ///
            /// <value>The name of the full.</value>
            public string FullName
            {
                get { return FirstName + " " + LastName; }
            }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Tests.KeyValueDataContractDeserializerTests.CustomerWithFields class.</summary>
            ///
            /// <param name="firstName">The person's first name.</param>
            /// <param name="lastName"> The person's last name.</param>
            public CustomerWithFields(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }
        }

        /// <summary>Kvp serializer fills public fields.</summary>
        [Test]
        public void KVP_Serializer_fills_public_fields()
        {
            using (JsConfig.With(includePublicFields:true))
            {
                var valueMap = new Dictionary<string, string> { { "FirstName", "james" }, { "LastName", "bond" }, { "FullName", "james bond" } };
                var result = (CustomerWithFields)KeyValueDataContractDeserializer.Instance.Parse(valueMap, typeof(CustomerWithFields));
                Assert.That(result.FirstName, Is.EqualTo("james"));
                Assert.That(result.LastName, Is.EqualTo("bond"));
            }
        }

        /// <summary>Dont try to decode as jsv strings for string properties.</summary>
        [Test]
        public void Dont_try_to_decode_as_JSV_strings_for_string_properties()
        {
            var valueMap = new Dictionary<string, string> { { "FirstName", "\"this is a \" test" } };
            var result = (Customer)KeyValueDataContractDeserializer.Instance.Parse(valueMap, typeof(Customer));
            Assert.That(result.FirstName, Is.EqualTo("\"this is a \" test"));
        }
    }
}
