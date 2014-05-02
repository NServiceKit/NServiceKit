using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NServiceKit.Text;

namespace NServiceKit.Common
{
    /// <summary>An enum extensions.</summary>
    public static class EnumExtensions
    {
#if !NETFX_CORE
        /// <summary>
        /// Gets the textual description of the enum if it has one. e.g.
        /// 
        /// <code>
        /// enum UserColors
        /// {
        ///     [Description("Bright Red")]
        ///     BrightRed
        /// }
        /// UserColors.BrightRed.ToDescription();
        /// </code>
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum @enum) 
        {
            var type = @enum.GetType();

            var memInfo = type.GetMember(@enum.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return @enum.ToString();
        }
#endif

        /// <summary>An Enum extension method that converts a @enum to a list.</summary>
        ///
        /// <param name="enum">The @enum to act on.</param>
        ///
        /// <returns>@enum as a List&lt;string&gt;</returns>
        public static List<string> ToList(this Enum @enum)
        {
#if !(SILVERLIGHT4 || WINDOWS_PHONE)
            return new List<string>(Enum.GetNames(@enum.GetType()));
#else
            return @enum.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
#endif
        }

        /// <summary>An Enum extension method that has.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="enum"> The @enum to act on.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool Has<T>(this Enum @enum, T value)
        {
            var enumType = Enum.GetUnderlyingType(@enum.GetType());
            if (enumType == typeof(int))
                return (((int)(object)@enum & (int)(object)value) == (int)(object)value);
            if (enumType == typeof(long))
                return (((long)(object)@enum & (long)(object)value) == (long)(object)value);
            if (enumType == typeof(byte))
                return (((byte)(object)@enum & (byte)(object)value) == (byte)(object)value);

            throw new NotSupportedException("Enums of type {0}".Fmt(enumType.Name));
        }

        /// <summary>An Enum extension method that is.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="enum"> The @enum to act on.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool Is<T>(this Enum @enum, T value)
        {
            var enumType = Enum.GetUnderlyingType(@enum.GetType());
            if (enumType == typeof(int))
                return (int)(object)@enum == (int)(object)value;
            if (enumType == typeof(long))
                return (long)(object)@enum == (long)(object)value;
            if (enumType == typeof(byte))
                return (byte)(object)@enum == (byte)(object)value;

            throw new NotSupportedException("Enums of type {0}".Fmt(enumType.Name));
        }

        /// <summary>An Enum extension method that adds @enum.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="enum"> The @enum to act on.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A T.</returns>
        public static T Add<T>(this Enum @enum, T value)
        {
            var enumType = Enum.GetUnderlyingType(@enum.GetType());
            if (enumType == typeof(int))
                return (T)(object)(((int)(object)@enum | (int)(object)value));
            if (enumType == typeof(long))
                return (T)(object)(((long)(object)@enum | (long)(object)value));
            if (enumType == typeof(byte))
                return (T)(object)(((byte)(object)@enum | (byte)(object)value));

            throw new NotSupportedException("Enums of type {0}".Fmt(enumType.Name));
        }

        /// <summary>An Enum extension method that removes this object.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="enum"> The @enum to act on.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A T.</returns>
        public static T Remove<T>(this Enum @enum, T value)
        {
            var enumType = Enum.GetUnderlyingType(@enum.GetType());
            if (enumType == typeof(int))
                return (T)(object)(((int)(object)@enum & ~(int)(object)value));
            if (enumType == typeof(long))
                return (T)(object)(((long)(object)@enum & ~(long)(object)value));
            if (enumType == typeof(byte))
                return (T)(object)(((byte)(object)@enum & ~(byte)(object)value));

            throw new NotSupportedException("Enums of type {0}".Fmt(enumType.Name));
        }

    }

}