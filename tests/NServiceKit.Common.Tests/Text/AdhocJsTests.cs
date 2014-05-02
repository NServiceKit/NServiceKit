using System;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Text
{
    /// <summary>Values that represent EnumValues.</summary>
	public enum EnumValues
	{
        /// <summary>An enum constant representing the enum 1 option.</summary>
		Enum1,

        /// <summary>An enum constant representing the enum 2 option.</summary>
		Enum2,

        /// <summary>An enum constant representing the enum 3 option.</summary>
		Enum3,
	}

    /// <summary>An adhoc js tests.</summary>
	[TestFixture]
	public class AdhocJsTests
	{
        /// <summary>Can deserialize.</summary>
		[Test]
		public void Can_Deserialize()
		{
			var items = TypeSerializer.DeserializeFromString<List<string>>(
				"/CustomPath35/api,/CustomPath40/api,/RootPath35,/RootPath40,:82,:83,:5001/api,:5002/api,:5003,:5004");

			Console.WriteLine(items.Dump());
		}

        /// <summary>Can serialize array of enums.</summary>
		[Test]
		public void Can_Serialize_Array_of_enums()
		{
			var enumArr = new[] { EnumValues.Enum1, EnumValues.Enum2, EnumValues.Enum3, };
			var json = JsonSerializer.SerializeToString(enumArr);
			Assert.That(json, Is.EqualTo("[\"Enum1\",\"Enum2\",\"Enum3\"]"));
		}

        /// <summary>Can serialize array of characters.</summary>
		[Test]
		public void Can_Serialize_Array_of_chars()
		{
			var enumArr = new[] { 'A', 'B', 'C', };
			var json = JsonSerializer.SerializeToString(enumArr);
			Assert.That(json, Is.EqualTo("[\"A\",\"B\",\"C\"]"));
		}

        /// <summary>Can serialize array with nulls.</summary>
		[Test]
		public void Can_Serialize_Array_with_nulls()
		{
            using (JsConfig.With(includeNullValues:true))
            {
                var t = new {
                    Name = "MyName",
                    Number = (int?)null,
                    Data = new object[] { 5, null, "text" }
                };

                var json = JsonSerializer.SerializeToString(t);
                Assert.That(json, Is.EqualTo("{\"Name\":\"MyName\",\"Number\":null,\"Data\":[5,null,\"text\"]}"));
            }
		}

		class A
		{
            /// <summary>Gets or sets the value.</summary>
            ///
            /// <value>The value.</value>
			public string Value { get; set; }
		}

        /// <summary>Dumps the fail.</summary>
		[Test]
		public void DumpFail()
		{
			var arrayOfA = new[] { new A { Value = "a" }, null, new A { Value = "b" } };
			Console.WriteLine(arrayOfA.Dump());
		}

        /// <summary>Deserialize array with null elements.</summary>
		[Test]
		public void Deserialize_array_with_null_elements()
		{
			var json = "[{\"Value\": \"a\"},null,{\"Value\": \"b\"}]";
			var o = JsonSerializer.DeserializeFromString<A[]>(json);
			o.PrintDump();
		}
	}
}