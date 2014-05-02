using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.Common.Tests
{
    /// <summary>Values that represent UserFileType.</summary>
	public enum UserFileType
	{
        /// <summary>An enum constant representing the default profile option.</summary>
		DefaultProfile,

        /// <summary>An enum constant representing the original profile option.</summary>
		OriginalProfile,

        /// <summary>An enum constant representing the profile 75 x coordinate 75 option.</summary>
		Profile75X75,

        /// <summary>An enum constant representing the profile 66 x coordinate 66 option.</summary>
		Profile66X66,

        /// <summary>An enum constant representing the profile 63 x coordinate 63 option.</summary>
		Profile63X63,
	}

    /// <summary>A test class a.</summary>
	public class TestClassA
	{
        /// <summary>Gets or sets a list of to strings.</summary>
        ///
        /// <value>A List of to strings.</value>
		public IList<string> ToStringList { get; set; }

        /// <summary>Gets or sets a list of from strings.</summary>
        ///
        /// <value>A List of from strings.</value>
		public ArrayOfString FromStringList { get; set; }

        /// <summary>Gets or sets a list of types of from user files.</summary>
        ///
        /// <value>A list of types of from user files.</value>
		public IList<UserFileType> FromUserFileTypes { get; set; }
	}

    /// <summary>A test class b.</summary>
	public class TestClassB
	{
        /// <summary>Gets or sets a list of to strings.</summary>
        ///
        /// <value>A List of to strings.</value>
		public ArrayOfString ToStringList { get; set; }

        /// <summary>Gets or sets a list of from strings.</summary>
        ///
        /// <value>A List of from strings.</value>
		public IList<string> FromStringList { get; set; }

        /// <summary>Gets or sets a list of types of from user files.</summary>
        ///
        /// <value>A list of types of from user files.</value>
		public ArrayOfString FromUserFileTypes { get; set; }
	}

    /// <summary>A test class c.</summary>
    public class TestClassC
    {
        /// <summary>Gets a list of from strings.</summary>
        ///
        /// <value>A List of from strings.</value>
        public IList<string> FromStringList { get; protected set; }
    }

    /// <summary>A reflection extensions tests.</summary>
	[TestFixture]
	public class ReflectionExtensionsTests
	{
        /// <summary>Can translate generic lists.</summary>
		[Test]
		public void Can_translate_generic_lists()
		{
			var values = new[] { "A", "B", "C" };
			var testA = new TestClassA {
				FromStringList = new ArrayOfString(values),
				ToStringList = new List<string>(values),
				FromUserFileTypes = new List<UserFileType>
            	{
            		UserFileType.DefaultProfile, UserFileType.OriginalProfile
            	},
			};

			var fromTestA = testA.TranslateTo<TestClassB>();

			AssertAreEqual(testA, fromTestA);

			var userFileTypeValues = testA.FromUserFileTypes.ConvertAll(x => x.ToString());
			var testB = new TestClassB {
				FromStringList = new List<string>(values),
				ToStringList = new ArrayOfString(values),
				FromUserFileTypes = new ArrayOfString(userFileTypeValues),
			};

			var fromTestB = testB.TranslateTo<TestClassA>();
			AssertAreEqual(fromTestB, testB);
		}

        /// <summary>Can translate generic list does ignore protected setters.</summary>
        [Test]
        public void Can_translate_generic_list_does_ignore_protected_setters()
        {
            var values = new[] { "A", "B", "C" };
            var testA = new TestClassA
            {
                ToStringList = new List<string>(values),
            };

            var fromTestA = testA.TranslateTo<TestClassC>();
            Assert.NotNull(fromTestA);
            Assert.IsNull(fromTestA.FromStringList);
        }

		private static void AssertAreEqual(TestClassA testA, TestClassB testB)
		{
			Assert.That(testA, Is.Not.Null);
			Assert.That(testB, Is.Not.Null);

			Assert.That(testA.FromStringList, Is.Not.Null);
			Assert.That(testB.FromStringList, Is.Not.Null);
			Assert.That(testA.FromStringList,
				Is.EquivalentTo(new List<string>(testB.FromStringList)));

			Assert.That(testA.ToStringList, Is.Not.Null);
			Assert.That(testB.ToStringList, Is.Not.Null);
			Assert.That(testA.ToStringList, Is.EquivalentTo(testB.ToStringList));

			Assert.That(testA.FromUserFileTypes, Is.Not.Null);
			Assert.That(testB.FromUserFileTypes, Is.Not.Null);
			Assert.That(testA.FromUserFileTypes,
				Is.EquivalentTo(testB.FromUserFileTypes.ConvertAll(x => x.ToEnum<UserFileType>())));
		}
	}
}