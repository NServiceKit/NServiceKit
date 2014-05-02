using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A built insert factory.</summary>
	public class BuiltInsFactory
		: ModelFactoryBase<string>
	{
		readonly string[] StringValues = new[] {
			"one", "two", "three", "four", 
			"five", "six", "seven"
		};

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public override void AssertIsEqual(string actual, string expected)
		{
			Assert.That(actual, Is.EqualTo(expected));
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override string CreateInstance(int i)
		{
			return i < StringValues.Length
				? StringValues[i]
				: i.ToString();
		}
	}

    /// <summary>An int factory.</summary>
	public class IntFactory
		: ModelFactoryBase<int>
	{
        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public override void AssertIsEqual(int actual, int expected)
		{
			Assert.That(actual, Is.EqualTo(expected));
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override int CreateInstance(int i)
		{
			return i;
		}
	}

    /// <summary>A date time factory.</summary>
	public class DateTimeFactory
		: ModelFactoryBase<DateTime>
	{
        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual Date/Time.</param>
        /// <param name="expected">The expected Date/Time.</param>
		public override void AssertIsEqual(DateTime actual, DateTime expected)
		{
			Assert.That(actual, Is.EqualTo(expected));
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override DateTime CreateInstance(int i)
		{
			return new DateTime(i, DateTimeKind.Utc);
		}
	}
}