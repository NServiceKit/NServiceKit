using System;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Common.Tests.Perf
{
    /// <summary>An identifier utilities performance.</summary>
	[Ignore("Benchmarks for measuring Id access")]
	[TestFixture]
	public class IdUtilsPerf
		: PerfTestBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Perf.IdUtilsPerf class.</summary>
		public IdUtilsPerf()
		{
			this.MultipleIterations = new List<int> { 100000 };
		}

        /// <summary>Old get identifier.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>An object.</returns>
		public static object OldGetId<T>(T entity)
		{
			const string idField = "Id";

			var guidEntity = entity as IHasGuidId;
			if (guidEntity != null)
			{
				return guidEntity.Id;
			}

			var intEntity = entity as IHasIntId;
			if (intEntity != null)
			{
				return intEntity.Id;
			}

			var longEntity = entity as IHasLongId;
			if (longEntity != null)
			{
				return longEntity.Id;
			}

			var stringEntity = entity as IHasStringId;
			if (stringEntity != null)
			{
				return stringEntity.Id;
			}

			var propertyInfo = typeof(T).GetProperty(idField);
			if (propertyInfo != null)
			{
				return propertyInfo.GetGetMethod().Invoke(entity, new object[0]);
			}

			if (typeof(T).IsValueType || typeof(T) == typeof(string))
			{
				return entity.GetHashCode();
			}

			throw new NotSupportedException("Cannot retrieve value of Id field, use IHasId<>");
		}

		private void CompareForInstance<T>(T obj)
		{
			CompareMultipleRuns(
				"OldGetId", () => OldGetId(obj),
				"obj.GetId()", () => obj.GetId()
			);
		}

        /// <summary>Compare has int identifier.</summary>
		[Test]
		public void Compare_HasIntId()
		{
			CompareForInstance(new IdUtilsTests.HasIntId());
		}

        /// <summary>Compare has generic identifier int.</summary>
		[Test]
		public void Compare_HasGenericIdInt()
		{
			CompareForInstance(new IdUtilsTests.HasGenericIdInt());
		}

        /// <summary>Compare has generic identifier string.</summary>
		[Test]
		public void Compare_HasGenericIdString()
		{
			CompareForInstance(new IdUtilsTests.HasGenericIdString());
		}

        /// <summary>Compare has identifier string property.</summary>
		[Test]
		public void Compare_HasIdStringProperty()
		{
			CompareForInstance(new IdUtilsTests.HasIdStringProperty());
		}

        /// <summary>Compare has identifier property.</summary>
		[Test]
		public void Compare_HasIdProperty()
		{
			CompareForInstance(new IdUtilsTests.HasIdProperty());
		}
	}
}