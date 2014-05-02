using System;

namespace NServiceKit.ServiceHost.Tests.AppData
{
    /// <summary>A format helpers.</summary>
	public class FormatHelpers
	{
        /// <summary>The instance.</summary>
		public static FormatHelpers Instance = new FormatHelpers();

        /// <summary>Moneys the given value.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A string.</returns>
		public string Money(decimal value)
		{
			return value.ToString("C");
		}

        /// <summary>Short date.</summary>
        ///
        /// <param name="dateTime">The date time.</param>
        ///
        /// <returns>A string.</returns>
		public string ShortDate(DateTime? dateTime)
		{
			if (dateTime == null) return "";
			return String.Format("{0:dd/MM/yyyy}", dateTime);
		}
	}
}