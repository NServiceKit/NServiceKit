//
// RouteValueDictionary.cs
//
// Author:
//	Atsushi Enomoto <atsushi@ximian.com>
//
// Copyright (C) 2008 Novell Inc. http://novell.com
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Web;

namespace NServiceKit.Html
{
#if NET_4_0
	[TypeForwardedFrom ("System.Web.Routing, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35")]
#endif
    /// <summary>Dictionary of route values.</summary>
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class RouteValueDictionary : IDictionary<string, object>
	{
		internal class CaseInsensitiveStringComparer : IEqualityComparer<string>
		{
            /// <summary>The instance.</summary>
			public static readonly CaseInsensitiveStringComparer Instance = new CaseInsensitiveStringComparer();

            /// <summary>Returns a hash code for this object.</summary>
            ///
            /// <param name="obj">The object.</param>
            ///
            /// <returns>A hash code for this object.</returns>
			public int GetHashCode(string obj)
			{
				return obj.ToLower(CultureInfo.InvariantCulture).GetHashCode();
			}

            /// <summary>Tests if two string objects are considered equal.</summary>
            ///
            /// <param name="obj1">String to be compared.</param>
            /// <param name="obj2">String to be compared.</param>
            ///
            /// <returns>true if the objects are considered equal, false if they are not.</returns>
			public bool Equals(string obj1, string obj2)
			{
				return String.Equals(obj1, obj2, StringComparison.OrdinalIgnoreCase);
			}
		}

		Dictionary<string,object> d = new Dictionary<string, object>(CaseInsensitiveStringComparer.Instance);

        /// <summary>Initializes a new instance of the NServiceKit.Html.RouteValueDictionary class.</summary>
		public RouteValueDictionary() {}

        /// <summary>Initializes a new instance of the NServiceKit.Html.RouteValueDictionary class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="dictionary">The dictionary.</param>
		public RouteValueDictionary(IDictionary<string, object> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");
			foreach (var p in dictionary)
				Add(p.Key, p.Value);
		}

        /// <summary>Initializes a new instance of the NServiceKit.Html.RouteValueDictionary class.</summary>
        ///
        /// <param name="values">The values.</param>
		public RouteValueDictionary(object values) // anonymous type instance
		{
			if (values == null)
				return;

			foreach (var pi in values.GetType().GetProperties())
			{
				try
				{
					Add(pi.Name, pi.GetValue(values, null));
				}
				catch
				{
					// ignore
				}
			}
		}

        /// <summary>Gets the number of. </summary>
        ///
        /// <value>The count.</value>
		public int Count
		{
			get { return d.Count; }
		}

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<string, object>>)d).IsReadOnly; }
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get { return d.Keys; }
		}

		ICollection<Object> IDictionary<string, object>.Values
		{
			get { return d.Values; }
		}

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
		public object this[string key]
		{
			get { object v; return d.TryGetValue(key, out v) ? v : null; }
			set { d[key] = value; }
		}

        /// <summary>Gets the keys.</summary>
        ///
        /// <value>The keys.</value>
		public Dictionary<string, object>.KeyCollection Keys
		{
			get { return d.Keys; }
		}

        /// <summary>Gets the values.</summary>
        ///
        /// <value>The values.</value>
		public Dictionary<string, object>.ValueCollection Values
		{
			get { return d.Values; }
		}

        /// <summary>Adds key.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		public void Add(string key, object value)
		{
			d.Add(key, value);
		}

        /// <summary>Clears this object to its blank/initial state.</summary>
		public void Clear()
		{
			d.Clear();
		}

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool ContainsKey(string key)
		{
			return d.ContainsKey(key);
		}

        /// <summary>Query if 'value' contains value.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool ContainsValue(object value)
		{
			return d.ContainsValue(value);
		}

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
		public Dictionary<string, object>.Enumerator GetEnumerator()
		{
			return d.GetEnumerator();
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			((ICollection<KeyValuePair<string, object>>)d).Add(item);
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>)d).Contains(item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)d).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>)d).Remove(item);
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return d.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return d.GetEnumerator();
		}

        /// <summary>Removes the given key.</summary>
        ///
        /// <param name="key">The key to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Remove(string key)
		{
			return d.Remove(key);
		}

        /// <summary>Attempts to get value from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool TryGetValue(string key, out object value)
		{
			return d.TryGetValue(key, out value);
		}
	}
}