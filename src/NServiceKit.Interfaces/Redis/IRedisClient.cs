//
// https://github.com/NServiceKit/NServiceKit.Redis/
// NServiceKit.Redis: ECMA CLI Binding to the Redis key-value storage system
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2013 ServiceStack.
//
// Licensed under the same terms of Redis and ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;
using NServiceKit.CacheAccess;
using NServiceKit.DataAccess;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.Redis.Generic;
using NServiceKit.Redis.Pipeline;
#if WINDOWS_PHONE
using NServiceKit.Text.WP;
#endif

namespace NServiceKit.Redis
{
    /// <summary>Interface for redis client.</summary>
	public interface IRedisClient
		: IBasicPersistenceProvider, ICacheClient
	{
		//Basic Redis Connection operations

        /// <summary>Gets or sets the database.</summary>
        ///
        /// <value>The database.</value>
		long Db { get; set; }

        /// <summary>Gets the size of the database.</summary>
        ///
        /// <value>The size of the database.</value>
		long DbSize { get; }

        /// <summary>Gets the information.</summary>
        ///
        /// <value>The information.</value>
		Dictionary<string, string> Info { get; }

        /// <summary>Gets the Date/Time of the last save.</summary>
        ///
        /// <value>The last save.</value>
		DateTime LastSave { get; }

        /// <summary>Gets the host.</summary>
        ///
        /// <value>The host.</value>
		string Host { get; }

        /// <summary>Gets the port.</summary>
        ///
        /// <value>The port.</value>
		int Port { get; }

        /// <summary>Gets or sets the connect timeout.</summary>
        ///
        /// <value>The connect timeout.</value>
        int ConnectTimeout { get; set; }

        /// <summary>Gets or sets the retry timeout.</summary>
        ///
        /// <value>The retry timeout.</value>
		int RetryTimeout { get; set; }

        /// <summary>Gets or sets the number of retries.</summary>
        ///
        /// <value>The number of retries.</value>
		int RetryCount { get; set; }

        /// <summary>Gets or sets the send timeout.</summary>
        ///
        /// <value>The send timeout.</value>
		int SendTimeout { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
		string Password { get; set; }

        /// <summary>Gets a value indicating whether the had exceptions.</summary>
        ///
        /// <value>true if had exceptions, false if not.</value>
		bool HadExceptions { get; }

        /// <summary>Saves this object.</summary>
		void Save();
        /// <summary>Saves the asynchronous.</summary>
		void SaveAsync();
        /// <summary>Shuts down this object and frees any resources it is using.</summary>
		void Shutdown();
        /// <summary>Rewrite append only file asynchronous.</summary>
		void RewriteAppendOnlyFileAsync();
        /// <summary>Flushes the database.</summary>
		void FlushDb();

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
		string this[string key] { get; set; }
		
	/// <summary>scan keys.</summary>
        ///
        /// <param name="cursor">start cursor.</param>
        /// <param name="pattern">key pattern.</param>
        /// <param name="count">scan count.</param>
        /// <returns>keys</returns>
	string[] ScanKeys(ref long cursor, string pattern = null, long? count = null);
	
        /// <summary>Gets all keys.</summary>
        ///
        /// <returns>all keys.</returns>
		List<string> GetAllKeys();

        /// <summary>Sets an entry.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		void SetEntry(string key, string value);

        /// <summary>Sets an entry.</summary>
        ///
        /// <param name="key">     The key.</param>
        /// <param name="value">   The value.</param>
        /// <param name="expireIn">The expire in.</param>
		void SetEntry(string key, string value, TimeSpan expireIn);

        /// <summary>Sets entry if not exists.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetEntryIfNotExists(string key, string value);

        /// <summary>Sets all.</summary>
        ///
        /// <param name="keys">  The keys.</param>
        /// <param name="values">The values.</param>
	    void SetAll(IEnumerable<string> keys, IEnumerable<string> values);

        /// <summary>Sets all.</summary>
        ///
        /// <param name="map">The map.</param>
	    void SetAll(Dictionary<string, string> map);

        /// <summary>Gets a value.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The value.</returns>
		string GetValue(string key);

        /// <summary>Gets and set entry.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The and set entry.</returns>
		string GetAndSetEntry(string key, string value);

        /// <summary>Gets the values.</summary>
        ///
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>The values.</returns>
		List<string> GetValues(List<string> keys);

        /// <summary>Gets the values.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>The values.</returns>
		List<T> GetValues<T>(List<string> keys);

        /// <summary>Gets values map.</summary>
        ///
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>The values map.</returns>
		Dictionary<string, string> GetValuesMap(List<string> keys);

        /// <summary>Gets values map.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>The values map.</returns>
		Dictionary<string, T> GetValuesMap<T>(List<string> keys);

        /// <summary>Appends to value.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long AppendToValue(string key, string value);

        /// <summary>Rename key.</summary>
        ///
        /// <param name="fromName">Name of from.</param>
        /// <param name="toName">  Name of to.</param>
		void RenameKey(string fromName, string toName);

        /// <summary>Gets a substring.</summary>
        ///
        /// <param name="key">      The key.</param>
        /// <param name="fromIndex">Zero-based index of from.</param>
        /// <param name="toIndex">  Zero-based index of to.</param>
        ///
        /// <returns>The substring.</returns>
		string GetSubstring(string key, int fromIndex, int toIndex);

        /// <summary>Gets from hash.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The data that was read from the hash.</returns>
	    T GetFromHash<T>(object id);

        /// <summary>Stores as hash.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity.</param>
	    void StoreAsHash<T>(T entity);

        /// <summary>Stores an object.</summary>
        ///
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>An object.</returns>
	    object StoreObject(object entity);

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ContainsKey(string key);

        /// <summary>Removes the entry described by args.</summary>
        ///
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveEntry(params string[] args);

        /// <summary>Increment value.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long IncrementValue(string key);

        /// <summary>Increment value by.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="count">Number of.</param>
        ///
        /// <returns>A double.</returns>
		long IncrementValueBy(string key, int count);

        /// <summary>Increment value by.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="count">Number of.</param>
        ///
        /// <returns>A double.</returns>
        long IncrementValueBy(string key, long count);

        /// <summary>Increment value by.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="count">Number of.</param>
        ///
        /// <returns>A double.</returns>
        double IncrementValueBy(string key, double count);

        /// <summary>Decrement value.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long DecrementValue(string key);

        /// <summary>Decrement value by.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="count">Number of.</param>
        ///
        /// <returns>A long.</returns>
		long DecrementValueBy(string key, int count);

        /// <summary>Searches for the first keys.</summary>
        ///
        /// <param name="pattern">Specifies the pattern.</param>
        ///
        /// <returns>The found keys.</returns>
		List<string> SearchKeys(string pattern);

        /// <summary>Gets entry type.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The entry type.</returns>
		RedisKeyType GetEntryType(string key);

        /// <summary>Gets random key.</summary>
        ///
        /// <returns>The random key.</returns>
		string GetRandomKey();

        /// <summary>Expire entry in.</summary>
        ///
        /// <param name="key">     The key.</param>
        /// <param name="expireIn">The expire in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireEntryIn(string key, TimeSpan expireIn);

        /// <summary>Expire entry at.</summary>
        ///
        /// <param name="key">     The key.</param>
        /// <param name="expireAt">The expire at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireEntryAt(string key, DateTime expireAt);

        /// <summary>Gets time to live.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The time to live.</returns>
		TimeSpan GetTimeToLive(string key);

        /// <summary>Gets sorted entry values.</summary>
        ///
        /// <param name="key">         The key.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The sorted entry values.</returns>
		List<string> GetSortedEntryValues(string key, int startingFrom, int endingAt);

        /// <summary>Writes all.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">The entities.</param>
		void WriteAll<TEntity>(IEnumerable<TEntity> entities);

		/// <summary>
		/// Returns a high-level typed client API
		/// Shorter Alias is As&lt;T&gt;();
		/// </summary>
		/// <typeparam name="T"></typeparam>
		IRedisTypedClient<T> GetTypedClient<T>();

		/// <summary>
		/// Returns a high-level typed client API
		/// </summary>
		/// <typeparam name="T"></typeparam>
		IRedisTypedClient<T> As<T>(); 

        /// <summary>Gets or sets the lists.</summary>
        ///
        /// <value>The lists.</value>
		IHasNamed<IRedisList> Lists { get; set; }

        /// <summary>Gets or sets the sets.</summary>
        ///
        /// <value>The sets.</value>
		IHasNamed<IRedisSet> Sets { get; set; }

        /// <summary>Gets or sets the sets the sorted belongs to.</summary>
        ///
        /// <value>The sorted sets.</value>
		IHasNamed<IRedisSortedSet> SortedSets { get; set; }

        /// <summary>Gets or sets the hashes.</summary>
        ///
        /// <value>The hashes.</value>
		IHasNamed<IRedisHash> Hashes { get; set; }

        /// <summary>Creates the transaction.</summary>
        ///
        /// <returns>The new transaction.</returns>
		IRedisTransaction CreateTransaction();

        /// <summary>Creates the pipeline.</summary>
        ///
        /// <returns>The new pipeline.</returns>
	    IRedisPipeline CreatePipeline();

        /// <summary>Acquires the lock.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>An IDisposable.</returns>
		IDisposable AcquireLock(string key);

        /// <summary>Acquires the lock.</summary>
        ///
        /// <param name="key">    The key.</param>
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>An IDisposable.</returns>
		IDisposable AcquireLock(string key, TimeSpan timeOut);

		#region Redis pubsub

        /// <summary>Watches the given keys.</summary>
        ///
        /// <param name="keys">The keys.</param>
		void Watch(params string[] keys);
        /// <summary>Un watch.</summary>
		void UnWatch();

        /// <summary>Creates the subscription.</summary>
        ///
        /// <returns>The new subscription.</returns>
		IRedisSubscription CreateSubscription();

        /// <summary>Publish message.</summary>
        ///
        /// <param name="toChannel">to channel.</param>
        /// <param name="message">  The message.</param>
        ///
        /// <returns>A long.</returns>
		long PublishMessage(string toChannel, string message);

		#endregion


		#region Set operations

        /// <summary>Gets all items from set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>all items from set.</returns>
		HashSet<string> GetAllItemsFromSet(string setId);

        /// <summary>Adds an item to set to 'item'.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="item"> The item.</param>
		void AddItemToSet(string setId, string item);

        /// <summary>Adds a range to set to 'items'.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="items">The items.</param>
		void AddRangeToSet(string setId, List<string> items);

        /// <summary>Removes the item from set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="item"> The item.</param>
		void RemoveItemFromSet(string setId, string item);

        /// <summary>Pops the item from set described by setId.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A string.</returns>
		string PopItemFromSet(string setId);

        /// <summary>Move between sets.</summary>
        ///
        /// <param name="fromSetId">Identifier for from set.</param>
        /// <param name="toSetId">  Identifier for to set.</param>
        /// <param name="item">     The item.</param>
		void MoveBetweenSets(string fromSetId, string toSetId, string item);

        /// <summary>Gets set count.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>The set count.</returns>
		long GetSetCount(string setId);

        /// <summary>Sets contains item.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="item"> The item.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetContainsItem(string setId, string item);

        /// <summary>Gets intersect from sets.</summary>
        ///
        /// <param name="setIds">List of identifiers for the sets.</param>
        ///
        /// <returns>The intersect from sets.</returns>
		HashSet<string> GetIntersectFromSets(params string[] setIds);

        /// <summary>Stores intersect from sets.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
		void StoreIntersectFromSets(string intoSetId, params string[] setIds);

        /// <summary>Gets union from sets.</summary>
        ///
        /// <param name="setIds">List of identifiers for the sets.</param>
        ///
        /// <returns>The union from sets.</returns>
		HashSet<string> GetUnionFromSets(params string[] setIds);

        /// <summary>Stores union from sets.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
		void StoreUnionFromSets(string intoSetId, params string[] setIds);

        /// <summary>Gets differences from set.</summary>
        ///
        /// <param name="fromSetId"> Identifier for from set.</param>
        /// <param name="withSetIds">List of identifiers for the with sets.</param>
        ///
        /// <returns>The differences from set.</returns>
		HashSet<string> GetDifferencesFromSet(string fromSetId, params string[] withSetIds);

        /// <summary>Stores differences from set.</summary>
        ///
        /// <param name="intoSetId"> Identifier for the into set.</param>
        /// <param name="fromSetId"> Identifier for from set.</param>
        /// <param name="withSetIds">List of identifiers for the with sets.</param>
		void StoreDifferencesFromSet(string intoSetId, string fromSetId, params string[] withSetIds);

        /// <summary>Gets random item from set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>The random item from set.</returns>
		string GetRandomItemFromSet(string setId);

		#endregion


		#region List operations

        /// <summary>Gets all items from list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>all items from list.</returns>
		List<string> GetAllItemsFromList(string listId);

        /// <summary>Gets range from list.</summary>
        ///
        /// <param name="listId">      Identifier for the list.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The range from list.</returns>
		List<string> GetRangeFromList(string listId, int startingFrom, int endingAt);

        /// <summary>Gets range from sorted list.</summary>
        ///
        /// <param name="listId">      Identifier for the list.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The range from sorted list.</returns>
		List<string> GetRangeFromSortedList(string listId, int startingFrom, int endingAt);

        /// <summary>Gets sorted items from list.</summary>
        ///
        /// <param name="listId">     Identifier for the list.</param>
        /// <param name="sortOptions">Options for controlling the sort.</param>
        ///
        /// <returns>The sorted items from list.</returns>
		List<string> GetSortedItemsFromList(string listId, SortOptions sortOptions);

        /// <summary>Adds an item to list to 'value'.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
		void AddItemToList(string listId, string value);

        /// <summary>Adds a range to list to 'values'.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="values">The values.</param>
		void AddRangeToList(string listId, List<string> values);

        /// <summary>Prepends an item to list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
		void PrependItemToList(string listId, string value);

        /// <summary>Prepends a range to list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="values">The values.</param>
		void PrependRangeToList(string listId, List<string> values);

        /// <summary>Removes all from list described by listId.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
		void RemoveAllFromList(string listId);

        /// <summary>Removes the start from list described by listId.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>A string.</returns>
		string RemoveStartFromList(string listId);

        /// <summary>Blocking remove start from list.</summary>
        ///
        /// <param name="listId"> Identifier for the list.</param>
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A string.</returns>
		string BlockingRemoveStartFromList(string listId, TimeSpan? timeOut);

        /// <summary>Blocking remove start from lists.</summary>
        ///
        /// <param name="listIds">List of identifiers for the ]lists.</param>
        /// <param name="timeOut">  The time out.</param>
        ///
        /// <returns>An ItemRef.</returns>
        ItemRef BlockingRemoveStartFromLists(string []listIds, TimeSpan? timeOut);

        /// <summary>Removes the end from list described by listId.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>A string.</returns>
		string RemoveEndFromList(string listId);

        /// <summary>Trim list.</summary>
        ///
        /// <param name="listId">          Identifier for the list.</param>
        /// <param name="keepStartingFrom">The keep starting from.</param>
        /// <param name="keepEndingAt">    The keep ending at.</param>
		void TrimList(string listId, int keepStartingFrom, int keepEndingAt);

        /// <summary>Removes the item from list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveItemFromList(string listId, string value);

        /// <summary>Removes the item from list.</summary>
        ///
        /// <param name="listId">     Identifier for the list.</param>
        /// <param name="value">      The value.</param>
        /// <param name="noOfMatches">The no of matches.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveItemFromList(string listId, string value, int noOfMatches);

        /// <summary>Gets list count.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>The list count.</returns>
		long GetListCount(string listId);

        /// <summary>Gets item from list.</summary>
        ///
        /// <param name="listId">   Identifier for the list.</param>
        /// <param name="listIndex">Zero-based index of the list.</param>
        ///
        /// <returns>The item from list.</returns>
		string GetItemFromList(string listId, int listIndex);

        /// <summary>Sets item in list.</summary>
        ///
        /// <param name="listId">   Identifier for the list.</param>
        /// <param name="listIndex">Zero-based index of the list.</param>
        /// <param name="value">    The value.</param>
		void SetItemInList(string listId, int listIndex, string value);

        /// <summary>Enqueue item on list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
		void EnqueueItemOnList(string listId, string value);

        /// <summary>Dequeue item from list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>A string.</returns>
		string DequeueItemFromList(string listId);

        /// <summary>Blocking dequeue item from list.</summary>
        ///
        /// <param name="listId"> Identifier for the list.</param>
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A string.</returns>
		string BlockingDequeueItemFromList(string listId, TimeSpan? timeOut);

        /// <summary>Blocking dequeue item from lists.</summary>
        ///
        /// <param name="listIds">List of identifiers for the ]lists.</param>
        /// <param name="timeOut">  The time out.</param>
        ///
        /// <returns>An ItemRef.</returns>
        ItemRef BlockingDequeueItemFromLists(string []listIds, TimeSpan? timeOut);

        /// <summary>Pushes an item to list.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
		void PushItemToList(string listId, string value);

        /// <summary>Pops the item from list described by listId.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>A string.</returns>
		string PopItemFromList(string listId);

        /// <summary>Blocking pop item from list.</summary>
        ///
        /// <param name="listId"> Identifier for the list.</param>
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A string.</returns>
		string BlockingPopItemFromList(string listId, TimeSpan? timeOut);

        /// <summary>Blocking pop item from lists.</summary>
        ///
        /// <param name="listIds">List of identifiers for the ]lists.</param>
        /// <param name="timeOut">  The time out.</param>
        ///
        /// <returns>An ItemRef.</returns>
        ItemRef BlockingPopItemFromLists(string []listIds, TimeSpan? timeOut);

        /// <summary>Pops the and push item between lists.</summary>
        ///
        /// <param name="fromListId">Identifier for from list.</param>
        /// <param name="toListId">  Identifier for to list.</param>
        ///
        /// <returns>A string.</returns>
		string PopAndPushItemBetweenLists(string fromListId, string toListId);

        /// <summary>Blocking pop and push item between lists.</summary>
        ///
        /// <param name="fromListId">Identifier for from list.</param>
        /// <param name="toListId">  Identifier for to list.</param>
        /// <param name="timeOut">   The time out.</param>
        ///
        /// <returns>A string.</returns>
        string BlockingPopAndPushItemBetweenLists(string fromListId, string toListId, TimeSpan? timeOut);

		#endregion


		#region Sorted Set operations

        /// <summary>Adds an item to sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool AddItemToSortedSet(string setId, string value);

        /// <summary>Adds an item to sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        /// <param name="score">The score.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool AddItemToSortedSet(string setId, string value, double score);

        /// <summary>Adds a range to sorted set.</summary>
        ///
        /// <param name="setId"> Identifier for the set.</param>
        /// <param name="values">The values.</param>
        /// <param name="score"> The score.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool AddRangeToSortedSet(string setId, List<string> values, double score);

        /// <summary>Adds a range to sorted set.</summary>
        ///
        /// <param name="setId"> Identifier for the set.</param>
        /// <param name="values">The values.</param>
        /// <param name="score"> The score.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool AddRangeToSortedSet(string setId, List<string> values, long score);

        /// <summary>Adds a range to sorted set.</summary>
        ///
        /// <param name="setId"> Identifier for the set.</param>
        /// <param name="valuesWithScore">The values with their respective scores.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        long AddRangeToSortedSetWithScores(string setId, List<KeyValuePair<string, double>> valuesWithScore);

        /// <summary>Removes the item from sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveItemFromSortedSet(string setId, string value);

        /// <summary>Pops the item with lowest score from sorted set described by setId.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A string.</returns>
		string PopItemWithLowestScoreFromSortedSet(string setId);

        /// <summary>Pops the item with highest score from sorted set described by setId.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A string.</returns>
		string PopItemWithHighestScoreFromSortedSet(string setId);

        /// <summary>Sorted set contains item.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SortedSetContainsItem(string setId, string value);

        /// <summary>Increment item in sorted set.</summary>
        ///
        /// <param name="setId">      Identifier for the set.</param>
        /// <param name="value">      The value.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		double IncrementItemInSortedSet(string setId, string value, double incrementBy);

        /// <summary>Increment item in sorted set.</summary>
        ///
        /// <param name="setId">      Identifier for the set.</param>
        /// <param name="value">      The value.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		double IncrementItemInSortedSet(string setId, string value, long incrementBy);

        /// <summary>Gets item index in sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item index in sorted set.</returns>
		long GetItemIndexInSortedSet(string setId, string value);

        /// <summary>Gets item index in sorted set description.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item index in sorted set description.</returns>
		long GetItemIndexInSortedSetDesc(string setId, string value);

        /// <summary>Gets all items from sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>all items from sorted set.</returns>
		List<string> GetAllItemsFromSortedSet(string setId);

        /// <summary>Gets all items from sorted set description.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>all items from sorted set description.</returns>
		List<string> GetAllItemsFromSortedSetDesc(string setId);

        /// <summary>Gets range from sorted set.</summary>
        ///
        /// <param name="setId">   Identifier for the set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range from sorted set.</returns>
		List<string> GetRangeFromSortedSet(string setId, int fromRank, int toRank);

        /// <summary>Gets range from sorted set description.</summary>
        ///
        /// <param name="setId">   Identifier for the set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range from sorted set description.</returns>
		List<string> GetRangeFromSortedSetDesc(string setId, int fromRank, int toRank);

        /// <summary>Gets all with scores from sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>all with scores from sorted set.</returns>
		IDictionary<string, double> GetAllWithScoresFromSortedSet(string setId);

        /// <summary>Gets range with scores from sorted set.</summary>
        ///
        /// <param name="setId">   Identifier for the set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range with scores from sorted set.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSet(string setId, int fromRank, int toRank);

        /// <summary>Gets range with scores from sorted set description.</summary>
        ///
        /// <param name="setId">   Identifier for the set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range with scores from sorted set description.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetDesc(string setId, int fromRank, int toRank);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take);

        /// <summary>Removes the range from sorted set.</summary>
        ///
        /// <param name="setId">  Identifier for the set.</param>
        /// <param name="minRank">The minimum rank.</param>
        /// <param name="maxRank">The maximum rank.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRangeFromSortedSet(string setId, int minRank, int maxRank);

        /// <summary>Removes the range from sorted set by score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRangeFromSortedSetByScore(string setId, double fromScore, double toScore);

        /// <summary>Removes the range from sorted set by score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRangeFromSortedSetByScore(string setId, long fromScore, long toScore);

        /// <summary>Gets sorted set count.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>The sorted set count.</returns>
		long GetSortedSetCount(string setId);

        /// <summary>Gets sorted set count.</summary>
        ///
        /// <param name="setId">          Identifier for the set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The sorted set count.</returns>
		long GetSortedSetCount(string setId, string fromStringScore, string toStringScore);

        /// <summary>Gets sorted set count.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The sorted set count.</returns>
		long GetSortedSetCount(string setId, long fromScore, long toScore);

        /// <summary>Gets sorted set count.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The sorted set count.</returns>
		long GetSortedSetCount(string setId, double fromScore, double toScore);

        /// <summary>Gets item score in sorted set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item score in sorted set.</returns>
		double GetItemScoreInSortedSet(string setId, string value);

        /// <summary>Stores intersect from sorted sets.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long StoreIntersectFromSortedSets(string intoSetId, params string[] setIds);

        /// <summary>Stores intersect from sorted sets while multiplying the scores of each set with a weight.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIdWithWeightPairs">   List of identifiers for the sets along with the weight to multiply the scores with.</param>
        ///
        /// <returns>A long.</returns>
		long StoreIntersectFromSortedSetsWithWeights(string intoSetId, params KeyValuePair<string, double>[] setIdWithWeightPairs);
        
        /// <summary>Stores union from sorted sets.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long StoreUnionFromSortedSets(string intoSetId, params string[] setIds);

        /// <summary>Stores union from sorted sets while multiplying the scores of each set with a weight.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIdWithWeightPairs">   List of identifiers for the sets along with the weight to multiply the scores with.</param>
        ///
        /// <returns>A long.</returns>
		long StoreUnionFromSortedSetsWithWeights(string intoSetId, params KeyValuePair<string, double>[] setIdWithWeightPairs);

        #endregion


		#region Hash operations

        /// <summary>Query if 'hashId' hash contains entry.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool HashContainsEntry(string hashId, string key);

        /// <summary>Sets entry in hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetEntryInHash(string hashId, string key, string value);

        /// <summary>Sets entry in hash if not exists.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetEntryInHashIfNotExists(string hashId, string key, string value);

        /// <summary>Sets range in hash.</summary>
        ///
        /// <param name="hashId">       Identifier for the hash.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
		void SetRangeInHash(string hashId, IEnumerable<KeyValuePair<string, string>> keyValuePairs);

        /// <summary>Increment value in hash.</summary>
        ///
        /// <param name="hashId">     Identifier for the hash.</param>
        /// <param name="key">        The key.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		long IncrementValueInHash(string hashId, string key, int incrementBy);

        /// <summary>Increment value in hash.</summary>
        ///
        /// <param name="hashId">     Identifier for the hash.</param>
        /// <param name="key">        The key.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
        double IncrementValueInHash(string hashId, string key, double incrementBy);

        /// <summary>Gets value from hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>The value from hash.</returns>
		string GetValueFromHash(string hashId, string key);

        /// <summary>Gets values from hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="keys">  The keys.</param>
        ///
        /// <returns>The values from hash.</returns>
		List<string> GetValuesFromHash(string hashId, params string[] keys);

        /// <summary>Removes the entry from hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveEntryFromHash(string hashId, string key);

        /// <summary>Gets hash count.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>The hash count.</returns>
		long GetHashCount(string hashId);

        /// <summary>Gets hash keys.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>The hash keys.</returns>
		List<string> GetHashKeys(string hashId);

        /// <summary>Gets hash values.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>The hash values.</returns>
		List<string> GetHashValues(string hashId);

        /// <summary>Gets all entries from hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>all entries from hash.</returns>
		Dictionary<string, string> GetAllEntriesFromHash(string hashId);

		#endregion


		#region Eval/Lua operations

        /// <summary>Executes the lua as string operation.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        /// <param name="args">   The arguments.</param>
        ///
        /// <returns>A string.</returns>
        string ExecLuaAsString(string luaBody, params string[] args);

        /// <summary>Executes the lua as string operation.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        /// <param name="keys">   The keys.</param>
        /// <param name="args">   The arguments.</param>
        ///
        /// <returns>A string.</returns>
        string ExecLuaAsString(string luaBody, string[] keys, string[] args);

        /// <summary>Executes the lua sha as string operation.</summary>
        ///
        /// <param name="sha1">The first sha.</param>
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>A string.</returns>
        string ExecLuaShaAsString(string sha1, params string[] args);

        /// <summary>Executes the lua sha as string operation.</summary>
        ///
        /// <param name="sha1">The first sha.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>A string.</returns>
        string ExecLuaShaAsString(string sha1, string[] keys, string[] args);

        /// <summary>Executes the lua as int operation.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        /// <param name="args">   The arguments.</param>
        ///
        /// <returns>A long.</returns>
		long ExecLuaAsInt(string luaBody, params string[] args);

        /// <summary>Executes the lua as int operation.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        /// <param name="keys">   The keys.</param>
        /// <param name="args">   The arguments.</param>
        ///
        /// <returns>A long.</returns>
		long ExecLuaAsInt(string luaBody, string[] keys, string[] args);

        /// <summary>Executes the lua sha as int operation.</summary>
        ///
        /// <param name="sha1">The first sha.</param>
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>A long.</returns>
		long ExecLuaShaAsInt(string sha1, params string[] args);

        /// <summary>Executes the lua sha as int operation.</summary>
        ///
        /// <param name="sha1">The first sha.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>A long.</returns>
		long ExecLuaShaAsInt(string sha1, string[] keys, string[] args);

        /// <summary>Executes the lua as list operation.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        /// <param name="args">   The arguments.</param>
        ///
        /// <returns>A List&lt;string&gt;</returns>
        List<string> ExecLuaAsList(string luaBody, params string[] args);

        /// <summary>Executes the lua as list operation.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        /// <param name="keys">   The keys.</param>
        /// <param name="args">   The arguments.</param>
        ///
        /// <returns>A List&lt;string&gt;</returns>
        List<string> ExecLuaAsList(string luaBody, string[] keys, string[] args);

        /// <summary>Executes the lua sha as list operation.</summary>
        ///
        /// <param name="sha1">The first sha.</param>
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>A List&lt;string&gt;</returns>
        List<string> ExecLuaShaAsList(string sha1, params string[] args);

        /// <summary>Executes the lua sha as list operation.</summary>
        ///
        /// <param name="sha1">The first sha.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="args">The arguments.</param>
        ///
        /// <returns>A List&lt;string&gt;</returns>
        List<string> ExecLuaShaAsList(string sha1, string[] keys, string[] args);

        /// <summary>Calculates the sha 1.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        ///
        /// <returns>The calculated sha 1.</returns>
        string CalculateSha1(string luaBody);

        /// <summary>Query if 'sha1Ref' has lua script.</summary>
        ///
        /// <param name="sha1Ref">The sha 1 reference.</param>
        ///
        /// <returns>true if lua script, false if not.</returns>
        bool HasLuaScript(string sha1Ref);

        /// <summary>Queries if a given which lua scripts exists.</summary>
        ///
        /// <param name="sha1Refs">A variable-length parameters list containing sha 1 references.</param>
        ///
        /// <returns>A Dictionary&lt;string,bool&gt;</returns>
        Dictionary<string, bool> WhichLuaScriptsExists(params string[] sha1Refs);
        /// <summary>Removes all lua scripts.</summary>
        void RemoveAllLuaScripts();
        /// <summary>Kill running lua script.</summary>
        void KillRunningLuaScript();

        /// <summary>Loads lua script.</summary>
        ///
        /// <param name="body">The body.</param>
        ///
        /// <returns>The lua script.</returns>
        string LoadLuaScript(string body);
        
        #endregion

	}
}
