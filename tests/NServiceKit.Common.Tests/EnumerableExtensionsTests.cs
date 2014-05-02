using NUnit.Framework;
using NServiceKit.Common.Extensions;
using System.Linq;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests
{
    /// <summary>An enumerable extensions tests.</summary>
	[TestFixture]
	public class EnumerableExtensionsTests
	{
		readonly int[] IntValues = new[] { 1, 2, 3 };
		readonly int[] NoValues = new int[]{};
		readonly int[] DifferentValues = new[] { 5, 6, 7};
		readonly int[] MoreIntValues = new[] { 1, 2, 3, 4 };
		readonly int[] LessIntValues = new[] { 1, 2 };
		readonly int[] UnorderedIntValues = new[] { 3, 2, 1 };

		readonly string[] StringValues = new[] { "A", "B", "C" };
		readonly string[] NoStringValues = new string[] { };

        /// <summary>Can join.</summary>
		[Test]
		public void Can_Join()
		{
			Assert.That(IntValues.Join(), Is.EqualTo("1,2,3"));
		}

        /// <summary>Equivalent to self.</summary>
		[Test]
		public void EquivalentTo_self()
		{
			Assert.That(IntValues.EquivalentTo(IntValues), Is.True);
		}

        /// <summary>Equivalent to list.</summary>
		[Test]
		public void EquivalentTo_List()
		{
			Assert.That(IntValues.EquivalentTo(IntValues.ToList()), Is.True);
		}

        /// <summary>Not equivalent to no values.</summary>
		[Test]
		public void Not_EquivalentTo_NoValues()
		{
			Assert.That(IntValues.EquivalentTo(NoValues), Is.False);
		}

        /// <summary>Not equivalent to different values.</summary>
		[Test]
		public void Not_EquivalentTo_DifferentValues()
		{
			Assert.That(IntValues.EquivalentTo(DifferentValues), Is.False);
		}

        /// <summary>Not equivalent to less int values.</summary>
		[Test]
		public void Not_EquivalentTo_LessIntValues()
		{
			Assert.That(IntValues.EquivalentTo(LessIntValues), Is.False);
		}

        /// <summary>Not equivalent to more int values.</summary>
		[Test]
		public void Not_EquivalentTo_MoreIntValues()
		{
			Assert.That(IntValues.EquivalentTo(MoreIntValues), Is.False);
		}

        /// <summary>Not equivalent to unordered int values.</summary>
		[Test]
		public void Not_EquivalentTo_UnorderedIntValues()
		{
			Assert.That(IntValues.EquivalentTo(UnorderedIntValues), Is.False);
		}

        /// <summary>Not equivalent to null.</summary>
		[Test]
		public void Not_EquivalentTo_null()
		{
			Assert.That(IntValues.EquivalentTo(null), Is.False);
		}

        /// <summary>Equivalent to string values.</summary>
		[Test]
		public void EquivalentTo_StringValues()
		{
			Assert.That(StringValues.EquivalentTo(NoStringValues), Is.False);
			Assert.That(NoStringValues.EquivalentTo(StringValues), Is.False);
			Assert.That(NoStringValues.EquivalentTo(NoStringValues), Is.True);
			Assert.That(StringValues.EquivalentTo(StringValues), Is.True);

			Assert.That(StringValues.EquivalentTo(new string[] { null }), Is.False);
			Assert.That(new string[] { null }.EquivalentTo(StringValues), Is.False);
		}

	}
}