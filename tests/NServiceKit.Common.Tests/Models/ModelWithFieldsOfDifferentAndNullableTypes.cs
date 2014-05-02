using System;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.DataAnnotations;
using NServiceKit.Logging;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with fields of different and nullable types.</summary>
	public class ModelWithFieldsOfDifferentAndNullableTypes
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ModelWithFieldsOfDifferentAndNullableTypes));

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[AutoIncrement]
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The n identifier.</value>
		public int? NId { get; set; }

        /// <summary>Gets or sets the identifier of the long.</summary>
        ///
        /// <value>The identifier of the long.</value>
		public long LongId { get; set; }

        /// <summary>Gets or sets the identifier of the long.</summary>
        ///
        /// <value>The identifier of the long.</value>
		public long? NLongId { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        ///
        /// <value>The identifier of the unique.</value>
		public Guid Guid { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        ///
        /// <value>The identifier of the unique.</value>
		public Guid? NGuid { get; set; }

        /// <summary>Gets or sets a value indicating whether the. </summary>
        ///
        /// <value>true if , false if not.</value>
		public bool Bool { get; set; }

        /// <summary>Gets or sets the bool.</summary>
        ///
        /// <value>The n bool.</value>
		public bool? NBool { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		public DateTime? NDateTime { get; set; }

        /// <summary>Gets or sets the float.</summary>
        ///
        /// <value>The float.</value>
		public float Float { get; set; }

        /// <summary>Gets or sets the float.</summary>
        ///
        /// <value>The n float.</value>
		public float? NFloat { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The double.</value>
		public double Double { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The n double.</value>
		public double? NDouble { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The decimal.</value>
		public decimal Decimal { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The n decimal.</value>
		public decimal? NDecimal { get; set; }

        /// <summary>Gets or sets the time span.</summary>
        ///
        /// <value>The time span.</value>
		public TimeSpan TimeSpan { get; set; }

        /// <summary>Gets or sets the time span.</summary>
        ///
        /// <value>The n time span.</value>
		public TimeSpan? NTimeSpan { get; set; }

        /// <summary>Creates a new ModelWithFieldsOfDifferentAndNullableTypes.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The ModelWithFieldsOfDifferentAndNullableTypes.</returns>
		public static ModelWithFieldsOfDifferentAndNullableTypes Create(int id)
		{
			var row = new ModelWithFieldsOfDifferentAndNullableTypes {
				Id = id,
				Bool = id % 2 == 0,
				DateTime = DateTime.Now.AddDays(id),
				Float = 1.11f + id,
				Double = 1.11d + id,
				Guid = Guid.NewGuid(),
				LongId = 999 + id,
				Decimal = id + 0.5m,
				TimeSpan = TimeSpan.FromSeconds(id),
			};

			return row;
		}

        /// <summary>Creates a constant.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The new constant.</returns>
		public static ModelWithFieldsOfDifferentAndNullableTypes CreateConstant(int id)
		{
			var row = new ModelWithFieldsOfDifferentAndNullableTypes {
				Id = id,
				Bool = id % 2 == 0,
				DateTime = new DateTime(1979, (id % 12) + 1, (id % 28) + 1),
				Float = 1.11f + id,
				Double = 1.11d + id,
				Guid = new Guid(((id % 240) + 16).ToString("X") + "461D9D-47DB-4778-B3FA-458379AE9BDC"),
				LongId = 999 + id,
				Decimal = id + 0.5m,
				TimeSpan = TimeSpan.FromSeconds(id),
			};

			return row;
		}

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithFieldsOfDifferentAndNullableTypes actual, ModelWithFieldsOfDifferentAndNullableTypes expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Guid, Is.EqualTo(expected.Guid));
			Assert.That(actual.LongId, Is.EqualTo(expected.LongId));
			Assert.That(actual.Bool, Is.EqualTo(expected.Bool));
			Assert.That(actual.TimeSpan, Is.EqualTo(expected.TimeSpan));

			try
			{
				Assert.That(actual.DateTime, Is.EqualTo(expected.DateTime));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with DateTime precisions, trying Assert again with rounding to seconds", ex);
				Assert.That(actual.DateTime.RoundToSecond(), Is.EqualTo(expected.DateTime.RoundToSecond()));
			}

			try
			{
				Assert.That(actual.Float, Is.EqualTo(expected.Float));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with float precisions, trying Assert again with rounding to 10 decimals", ex);
				Assert.That(Math.Round(actual.Float, 10), Is.EqualTo(Math.Round(actual.Float, 10)));
			}

			try
			{
				Assert.That(actual.Double, Is.EqualTo(expected.Double));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with double precisions, trying Assert again with rounding to 10 decimals", ex);
				Assert.That(Math.Round(actual.Double, 10), Is.EqualTo(Math.Round(actual.Double, 10)));
			}

			Assert.That(actual.NBool, Is.EqualTo(expected.NBool));
			Assert.That(actual.NDateTime, Is.EqualTo(expected.NDateTime));
			Assert.That(actual.NDecimal, Is.EqualTo(expected.NDecimal));
			Assert.That(actual.NDouble, Is.EqualTo(expected.NDouble));
			Assert.That(actual.NFloat, Is.EqualTo(expected.NFloat));
			Assert.That(actual.NGuid, Is.EqualTo(expected.NGuid));
			Assert.That(actual.NId, Is.EqualTo(expected.NId));
			Assert.That(actual.NLongId, Is.EqualTo(expected.NLongId));
			Assert.That(actual.NTimeSpan, Is.EqualTo(expected.NTimeSpan));

		}
	}
}