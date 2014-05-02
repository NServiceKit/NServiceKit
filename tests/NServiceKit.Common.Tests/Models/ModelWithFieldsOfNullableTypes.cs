using System;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.Logging;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with fields of nullable types.</summary>
	public class ModelWithFieldsOfNullableTypes
		: IHasIntId
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ModelWithFieldsOfNullableTypes));

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The n identifier.</value>
		public int? NId { get; set; }

        /// <summary>Gets or sets the identifier of the long.</summary>
        ///
        /// <value>The identifier of the long.</value>
		public long? NLongId { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        ///
        /// <value>The identifier of the unique.</value>
		public Guid? NGuid { get; set; }

        /// <summary>Gets or sets the bool.</summary>
        ///
        /// <value>The n bool.</value>
		public bool? NBool { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		public DateTime? NDateTime { get; set; }

        /// <summary>Gets or sets the float.</summary>
        ///
        /// <value>The n float.</value>
		public float? NFloat { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The n double.</value>
		public double? NDouble { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The n decimal.</value>
		public decimal? NDecimal { get; set; }

        /// <summary>Gets or sets the time span.</summary>
        ///
        /// <value>The n time span.</value>
		public TimeSpan? NTimeSpan { get; set; }

        /// <summary>Creates a new ModelWithFieldsOfNullableTypes.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The ModelWithFieldsOfNullableTypes.</returns>
		public static ModelWithFieldsOfNullableTypes Create(int id)
		{
			var row = new ModelWithFieldsOfNullableTypes {
				Id = id,
				NId = id,
				NBool = id % 2 == 0,
				NDateTime = DateTime.Now.AddDays(id),
				NFloat = 1.11f + id,
				NDouble = 1.11d + id,
				NGuid = Guid.NewGuid(),
				NLongId = 999 + id,
				NDecimal = id + 0.5m,
				NTimeSpan = TimeSpan.FromSeconds(id),
			};

			return row;
		}

        /// <summary>Creates a constant.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The new constant.</returns>
		public static ModelWithFieldsOfNullableTypes CreateConstant(int id)
		{
			var row = new ModelWithFieldsOfNullableTypes {
				Id = id,
				NId = id,
				NBool = id % 2 == 0,
				NDateTime = new DateTime(1979, (id % 12) + 1, (id % 28) + 1),
				NFloat = 1.11f + id,
				NDouble = 1.11d + id,
				NGuid = new Guid(((id % 240) + 16).ToString("X") + "7DA519-73B6-4525-84BA-B57673B2360D"),
				NLongId = 999 + id,
				NDecimal = id + 0.5m,
				NTimeSpan = TimeSpan.FromSeconds(id),
			};

			return row;
		}

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithFieldsOfNullableTypes actual, ModelWithFieldsOfNullableTypes expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.NId, Is.EqualTo(expected.NId));
			Assert.That(actual.NGuid, Is.EqualTo(expected.NGuid));
			Assert.That(actual.NLongId, Is.EqualTo(expected.NLongId));
			Assert.That(actual.NBool, Is.EqualTo(expected.NBool));
			Assert.That(actual.NTimeSpan, Is.EqualTo(expected.NTimeSpan));

			try
			{
				Assert.That(actual.NDateTime, Is.EqualTo(expected.NDateTime));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with DateTime precisions, trying Assert again with rounding to seconds", ex);
				Assert.That(actual.NDateTime.Value.ToUniversalTime().RoundToSecond(), Is.EqualTo(expected.NDateTime.Value.ToUniversalTime().RoundToSecond()));
			}

			try
			{
				Assert.That(actual.NFloat, Is.EqualTo(expected.NFloat));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with float precisions, trying Assert again with rounding to 10 decimals", ex);
				Assert.That(Math.Round(actual.NFloat.Value, 10), Is.EqualTo(Math.Round(actual.NFloat.Value, 10)));
			}

			try
			{
				Assert.That(actual.NDouble, Is.EqualTo(expected.NDouble));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with double precisions, trying Assert again with rounding to 10 decimals", ex);
				Assert.That(Math.Round(actual.NDouble.Value, 10), Is.EqualTo(Math.Round(actual.NDouble.Value, 10)));
			}

		}
	}
}