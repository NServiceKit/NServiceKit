using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.Html
{
    /// <summary>Dictionary of model states.</summary>
	[Serializable]
	public class ModelStateDictionary : IDictionary<string, ModelState>
	{
		private readonly Dictionary<string, ModelState> innerDictionary = new Dictionary<string, ModelState>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Initializes a new instance of the NServiceKit.Html.ModelStateDictionary class.</summary>
		public ModelStateDictionary() {}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ModelStateDictionary class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="dictionary">The dictionary.</param>
		public ModelStateDictionary(ModelStateDictionary dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			foreach (var entry in dictionary)
			{
				innerDictionary.Add(entry.Key, entry.Value);
			}
		}

        /// <summary>Gets the number of. </summary>
        ///
        /// <value>The count.</value>
		public int Count
		{
			get
			{
				return innerDictionary.Count;
			}
		}

        /// <summary>Gets a value indicating whether this object is read only.</summary>
        ///
        /// <value>true if this object is read only, false if not.</value>
		public bool IsReadOnly
		{
			get
			{
				return ((IDictionary<string, ModelState>)innerDictionary).IsReadOnly;
			}
		}

        /// <summary>Gets a value indicating whether this object is valid.</summary>
        ///
        /// <value>true if this object is valid, false if not.</value>
		public bool IsValid
		{
			get
			{
				return Values.All(modelState => modelState.Errors.Count == 0);
			}
		}

        /// <summary>Gets the keys.</summary>
        ///
        /// <value>The keys.</value>
		public ICollection<string> Keys
		{
			get
			{
				return innerDictionary.Keys;
			}
		}

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
		public ModelState this[string key]
		{
			get
			{
				ModelState value;
				innerDictionary.TryGetValue(key, out value);
				return value;
			}
			set
			{
				innerDictionary[key] = value;
			}
		}

        /// <summary>Gets the values.</summary>
        ///
        /// <value>The values.</value>
		public ICollection<ModelState> Values
		{
			get
			{
				return innerDictionary.Values;
			}
		}

        /// <summary>Adds key.</summary>
        ///
        /// <param name="item">The item to remove.</param>
		public void Add(KeyValuePair<string, ModelState> item)
		{
			((IDictionary<string, ModelState>)innerDictionary).Add(item);
		}

        /// <summary>Adds key.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		public void Add(string key, ModelState value)
		{
			innerDictionary.Add(key, value);
		}

        /// <summary>Adds a model error to 'errorMessage'.</summary>
        ///
        /// <param name="key">      The key.</param>
        /// <param name="exception">The exception.</param>
		public void AddModelError(string key, Exception exception)
		{
			GetModelStateForKey(key).Errors.Add(exception);
		}

        /// <summary>Adds a model error to 'errorMessage'.</summary>
        ///
        /// <param name="key">         The key.</param>
        /// <param name="errorMessage">Message describing the error.</param>
		public void AddModelError(string key, string errorMessage)
		{
			GetModelStateForKey(key).Errors.Add(errorMessage);
		}

        /// <summary>Clears this object to its blank/initial state.</summary>
		public void Clear()
		{
			innerDictionary.Clear();
		}

        /// <summary>Query if this object contains the given item.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        ///
        /// <returns>true if the object is in this collection, false if not.</returns>
		public bool Contains(KeyValuePair<string, ModelState> item)
		{
			return ((IDictionary<string, ModelState>)innerDictionary).Contains(item);
		}

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool ContainsKey(string key)
		{
			return innerDictionary.ContainsKey(key);
		}

        /// <summary>Copies to.</summary>
        ///
        /// <param name="array">     The array.</param>
        /// <param name="arrayIndex">Zero-based index of the array.</param>
		public void CopyTo(KeyValuePair<string, ModelState>[] array, int arrayIndex)
		{
			((IDictionary<string, ModelState>)innerDictionary).CopyTo(array, arrayIndex);
		}

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
		public IEnumerator<KeyValuePair<string, ModelState>> GetEnumerator()
		{
			return innerDictionary.GetEnumerator();
		}

		private ModelState GetModelStateForKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			ModelState modelState;
			if (!TryGetValue(key, out modelState))
			{
				modelState = new ModelState();
				this[key] = modelState;
			}

			return modelState;
		}

        /// <summary>Query if 'key' is valid field.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="key">The key to remove.</param>
        ///
        /// <returns>true if valid field, false if not.</returns>
		public bool IsValidField(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			// if the key is not found in the dictionary, we just say that it's valid (since there are no errors)
			return DictionaryHelpers.FindKeysWithPrefix(this, key).All(entry => entry.Value.Errors.Count == 0);
		}

        /// <summary>Merges the given dictionary.</summary>
        ///
        /// <param name="dictionary">The dictionary.</param>
		public void Merge(ModelStateDictionary dictionary)
		{
			if (dictionary == null)
			{
				return;
			}

			foreach (var entry in dictionary)
			{
				this[entry.Key] = entry.Value;
			}
		}

        /// <summary>Removes the given key.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Remove(KeyValuePair<string, ModelState> item)
		{
			return ((IDictionary<string, ModelState>)innerDictionary).Remove(item);
		}

        /// <summary>Removes the given key.</summary>
        ///
        /// <param name="key">The key to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Remove(string key)
		{
			return innerDictionary.Remove(key);
		}

        /// <summary>Sets model value.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		public void SetModelValue(string key, ValueProviderResult value)
		{
			GetModelStateForKey(key).Value = value;
		}

        /// <summary>Attempts to get value from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool TryGetValue(string key, out ModelState value)
		{
			return innerDictionary.TryGetValue(key, out value);
		}

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)innerDictionary).GetEnumerator();
		}
		#endregion
	}

	internal static class DictionaryHelpers
	{
        /// <summary>Finds the keys with prefixes in this collection.</summary>
        ///
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="prefix">    The prefix.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the keys with prefixes in this collection.</returns>
		public static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(IDictionary<string, TValue> dictionary, string prefix)
		{
			TValue exactMatchValue;
			if (dictionary.TryGetValue(prefix, out exactMatchValue))
			{
				yield return new KeyValuePair<string, TValue>(prefix, exactMatchValue);
			}

			foreach (var entry in dictionary)
			{
				string key = entry.Key;

				if (key.Length <= prefix.Length)
				{
					continue;
				}

				if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				char charAfterPrefix = key[prefix.Length];
				switch (charAfterPrefix)
				{
					case '[':
					case '.':
						yield return entry;
						break;
				}
			}
		}

        /// <summary>Query if 'dictionary' does any key have prefix.</summary>
        ///
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="prefix">    The prefix.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool DoesAnyKeyHavePrefix<TValue>(IDictionary<string, TValue> dictionary, string prefix)
		{
			return FindKeysWithPrefix(dictionary, prefix).Any();
		}

        /// <summary>An IDictionary&lt;TKey,TValue&gt; extension method that gets or default.</summary>
        ///
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="dict">   The dict to act on.</param>
        /// <param name="key">    The key.</param>
        /// <param name="default">The default.</param>
        ///
        /// <returns>The or default.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            TValue value;
            if (dict.TryGetValue(key, out value)) {
                return value;
            }
            return @default;
        }
	}
}
