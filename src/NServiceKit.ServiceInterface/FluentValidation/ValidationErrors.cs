using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.FluentValidation
{
    /// <summary>A validation errors.</summary>
    public static class ValidationErrors
    {
        /// <summary>The credit card.</summary>
        public const string CreditCard = "CreditCard";
        /// <summary>The email.</summary>
        public const string Email = "Email";
        /// <summary>The equal.</summary>
        public const string Equal = "Equal";
        /// <summary>The exclusive between.</summary>
        public const string ExclusiveBetween = "ExclusiveBetween";
        /// <summary>The greater than or equal.</summary>
        public const string GreaterThanOrEqual = "GreaterThanOrEqual";
        /// <summary>The greater than.</summary>
        public const string GreaterThan = "GreaterThan";
        /// <summary>The inclusive between.</summary>
        public const string InclusiveBetween = "InclusiveBetween";
        /// <summary>The length.</summary>
        public const string Length = "Length";
        /// <summary>The less than or equal.</summary>
        public const string LessThanOrEqual = "LessThanOrEqual";
        /// <summary>The less than.</summary>
        public const string LessThan = "LessThan";
        /// <summary>The not empty.</summary>
        public const string NotEmpty = "NotEmpty";
        /// <summary>The not equal.</summary>
        public const string NotEqual = "NotEqual";
        /// <summary>The not null.</summary>
        public const string NotNull = "NotNull";
        /// <summary>The predicate.</summary>
        public const string Predicate = "Predicate";
        /// <summary>The regular expression.</summary>
        public const string RegularExpression = "RegularExpression";
    }
}
