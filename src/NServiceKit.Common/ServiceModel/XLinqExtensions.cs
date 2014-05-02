#if !SILVERLIGHT && !MONOTOUCH && !XBOX
//
// NServiceKit: Useful extensions to simplify parsing xml with XLinq
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2013 ServiceStack
//
// Licensed under the new BSD license.
//

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace NServiceKit.ServiceModel
{
    /// <summary>A linq extensions.</summary>
    public static class XLinqExtensions
    {
        /// <summary>An XElement extension method that gets a string.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The string.</returns>
        public static string GetString(this XElement el, string name)
        {
            return el == null ? null : GetElementValueOrDefault(el, name, x => x.Value);
        }

        /// <summary>An XElement extension method that gets string attribute or default.</summary>
        ///
        /// <param name="element">The element to act on.</param>
        /// <param name="name">   The name.</param>
        ///
        /// <returns>The string attribute or default.</returns>
        public static string GetStringAttributeOrDefault(this XElement element, string name)
        {
            var attr = AnyAttribute(element, name);
            return attr == null ? null : GetAttributeValueOrDefault(attr, name, x => x.Value);
        }

        /// <summary>An XElement extension method that gets a bool.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool GetBool(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (bool)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets bool or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool GetBoolOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (bool)x);
        }

        /// <summary>An XElement extension method that gets nullable bool.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable bool.</returns>
        public static bool? GetNullableBool(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (bool?)childEl;
        }

        /// <summary>An XElement extension method that gets an int.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The int.</returns>
        public static int GetInt(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (int)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets int or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The int or default.</returns>
        public static int GetIntOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (int)x);
        }

        /// <summary>An XElement extension method that gets nullable int.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable int.</returns>
        public static int? GetNullableInt(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (int?)childEl;
        }

        /// <summary>An XElement extension method that gets a long.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The long.</returns>
        public static long GetLong(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (long)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets long or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The long or default.</returns>
        public static long GetLongOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (long)x);
        }

        /// <summary>An XElement extension method that gets nullable long.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable long.</returns>
        public static long? GetNullableLong(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (long?)childEl;
        }

        /// <summary>An XElement extension method that gets a decimal.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The decimal.</returns>
        public static decimal GetDecimal(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (decimal)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets decimal or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The decimal or default.</returns>
        public static decimal GetDecimalOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (decimal)x);
        }

        /// <summary>An XElement extension method that gets nullable decimal.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable decimal.</returns>
        public static decimal? GetNullableDecimal(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (decimal?)childEl;
        }

        /// <summary>An XElement extension method that gets date time.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The date time.</returns>
        public static DateTime GetDateTime(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (DateTime)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets date time or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The date time or default.</returns>
        public static DateTime GetDateTimeOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (DateTime)x);
        }

        /// <summary>An XElement extension method that gets nullable date time.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable date time.</returns>
        public static DateTime? GetNullableDateTime(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (DateTime?)childEl;
        }

        /// <summary>An XElement extension method that gets time span.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The time span.</returns>
        public static TimeSpan GetTimeSpan(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (TimeSpan)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets time span or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The time span or default.</returns>
        public static TimeSpan GetTimeSpanOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (TimeSpan)x);
        }

        /// <summary>An XElement extension method that gets nullable time span.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable time span.</returns>
        public static TimeSpan? GetNullableTimeSpan(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (TimeSpan?)childEl;
        }

        /// <summary>An XElement extension method that gets a unique identifier.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The unique identifier.</returns>
        public static Guid GetGuid(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (Guid)GetElement(el, name);
        }

        /// <summary>An XElement extension method that gets unique identifier or default.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The unique identifier or default.</returns>
        public static Guid GetGuidOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (Guid)x);
        }

        /// <summary>An XElement extension method that gets nullable unique identifier.</summary>
        ///
        /// <param name="el">  The el to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>The nullable unique identifier.</returns>
        public static Guid? GetNullableGuid(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return childEl == null || string.IsNullOrEmpty(childEl.Value) ? null : (Guid?)childEl;
        }

        /// <summary>An XElement extension method that gets element value or default.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="element">  The element to act on.</param>
        /// <param name="name">     The name.</param>
        /// <param name="converter">The converter.</param>
        ///
        /// <returns>The element value or default.</returns>
        public static T GetElementValueOrDefault<T>(this XElement element, string name, Func<XElement, T> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var el = GetElement(element, name);
            return el == null || string.IsNullOrEmpty(el.Value) ? default(T) : converter(el);
        }

        /// <summary>An XElement extension method that gets an element.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="element">The element to act on.</param>
        /// <param name="name">   The name.</param>
        ///
        /// <returns>The element.</returns>
        public static XElement GetElement(this XElement element, string name)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return element.AnyElement(name);
        }

        /// <summary>An XAttribute extension method that gets attribute value or default.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="attr">     The attr to act on.</param>
        /// <param name="name">     The name.</param>
        /// <param name="converter">The converter.</param>
        ///
        /// <returns>The attribute value or default.</returns>
        public static T GetAttributeValueOrDefault<T>(this XAttribute attr, string name, Func<XAttribute, T> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            return attr == null || string.IsNullOrEmpty(attr.Value) ? default(T) : converter(attr);
        }

        /// <summary>An XElement extension method that assert exactly one result.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="queryListItems"> The queryListItems to act on.</param>
        /// <param name="referenceNumber">The reference number.</param>
        /// <param name="formType">       Type of the form.</param>
        public static void AssertExactlyOneResult(this XElement queryListItems, string referenceNumber, string formType)
        {
            int count = Convert.ToInt32(queryListItems.AnyAttribute("ItemCount").Value);
            if (count == 0)
                throw new InvalidOperationException(string.Format("There is no {0} for with a deal reference number {1}", formType, referenceNumber));
            if (count > 1)
                throw new InvalidOperationException(
                    string.Format("There are more than one {0}s with deal reference number {1}", formType, referenceNumber));
        }

        /// <summary>An XElement extension method that assert element has value.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="element">The element to act on.</param>
        /// <param name="name">   The name.</param>
        public static void AssertElementHasValue(this XElement element, string name)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var childEl = element.AnyElement(name);
            if (childEl == null || string.IsNullOrEmpty(childEl.Value))
            {
                throw new ArgumentNullException(name, string.Format("{0} is required", name));
            }
        }

        /// <summary>An IEnumerable&lt;XElement&gt; extension method that gets the values.</summary>
        ///
        /// <param name="els">The els to act on.</param>
        ///
        /// <returns>The values.</returns>
        public static List<string> GetValues(this IEnumerable<XElement> els)
        {
            var values = new List<string>();
            foreach (var el in els)
            {
                values.Add(el.Value);
            }
            return values;
        }

        /// <summary>An XElement extension method that any attribute.</summary>
        ///
        /// <param name="element">The element to act on.</param>
        /// <param name="name">   The name.</param>
        ///
        /// <returns>An XAttribute.</returns>
        public static XAttribute AnyAttribute(this XElement element, string name)
        {
            if (element == null) return null;
            foreach (var attribute in element.Attributes())
            {
                if (attribute.Name.LocalName == name)
                {
                    return attribute;
                }
            }
            return null;
        }

        /// <summary>Enumerates all elements in this collection.</summary>
        ///
        /// <param name="element">The element to act on.</param>
        /// <param name="name">   The name.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process all elements in this collection.</returns>
        public static IEnumerable<XElement> AllElements(this XElement element, string name)
        {
            var els = new List<XElement>();
            if (element == null) return els;
            foreach (var node in element.Nodes())
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                var childEl = (XElement)node;
                if (childEl.Name.LocalName == name)
                {
                    els.Add(childEl);
                }
            }
            return els;
        }

        /// <summary>An IEnumerable&lt;XElement&gt; extension method that any element.</summary>
        ///
        /// <param name="element">The element to act on.</param>
        /// <param name="name">   The name.</param>
        ///
        /// <returns>An XElement.</returns>
        public static XElement AnyElement(this XElement element, string name)
        {
            if (element == null) return null;
            foreach (var node in element.Nodes())
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                var childEl = (XElement)node;
                if (childEl.Name.LocalName == name)
                {
                    return childEl;
                }
            }
            return null;
        }

        /// <summary>An IEnumerable&lt;XElement&gt; extension method that any element.</summary>
        ///
        /// <param name="elements">The elements to act on.</param>
        /// <param name="name">    The name.</param>
        ///
        /// <returns>An XElement.</returns>
        public static XElement AnyElement(this IEnumerable<XElement> elements, string name)
        {
            foreach (var element in elements)
            {
                if (element.Name.LocalName == name)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>Enumerates all elements in this collection.</summary>
        ///
        /// <param name="elements">The elements to act on.</param>
        /// <param name="name">    The name.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process all elements in this collection.</returns>
        public static IEnumerable<XElement> AllElements(this IEnumerable<XElement> elements, string name)
        {
            var els = new List<XElement>();
            foreach (var element in elements)
            {
                els.AddRange(AllElements(element, name));
            }
            return els;
        }

        /// <summary>An XElement extension method that first element.</summary>
        ///
        /// <param name="element">The element to act on.</param>
        ///
        /// <returns>An XElement.</returns>
        public static XElement FirstElement(this XElement element)
        {
            if (element.FirstNode.NodeType == XmlNodeType.Element)
            {
                return (XElement)element.FirstNode;
            }
            return null;
        }
    }

}
#endif
