//
// https://github.com/mythz/NServiceKit.Redis
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
using NServiceKit.DataAccess;
using NServiceKit.DesignPatterns.Model;
#if WINDOWS_PHONE
using NServiceKit.Text.WP;
#endif

namespace NServiceKit.Redis.Generic
{
    /// <summary>Interface for redis typed client.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IRedisTypedClient<T>
		: IBasicPersistenceProvider<T>
	{
        /// <summary>Gets or sets the lists.</summary>
        ///
        /// <value>The lists.</value>
		IHasNamed<IRedisList<T>> Lists { get; set; }

        /// <summary>Gets or sets the sets.</summary>
        ///
        /// <value>The sets.</value>
		IHasNamed<IRedisSet<T>> Sets { get; set; }

        /// <summary>Gets or sets the sets the sorted belongs to.</summary>
        ///
        /// <value>The sorted sets.</value>
		IHasNamed<IRedisSortedSet<T>> SortedSets { get; set; }

        /// <summary>Gets a hash.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>The hash.</returns>
		IRedisHash<TKey, T> GetHash<TKey>(string hashId);

        /// <summary>Creates the transaction.</summary>
        ///
        /// <returns>The new transaction.</returns>
		IRedisTypedTransaction<T> CreateTransaction();

        /// <summary>Creates the pipeline.</summary>
        ///
        /// <returns>The new pipeline.</returns>
        IRedisTypedPipeline<T> CreatePipeline();

        /// <summary>Gets the redis client.</summary>
        ///
        /// <value>The redis client.</value>
        IRedisClient RedisClient { get; }

        /// <summary>Acquires the lock described by timeOut.</summary>
        ///
        /// <returns>An IDisposable.</returns>
		IDisposable AcquireLock();

        /// <summary>Acquires the lock described by timeOut.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>An IDisposable.</returns>
		IDisposable AcquireLock(TimeSpan timeOut);

        /// <summary>Gets or sets the database.</summary>
        ///
        /// <value>The database.</value>
		long Db { get; set; }

        /// <summary>Gets all keys.</summary>
        ///
        /// <returns>all keys.</returns>
		List<string> GetAllKeys();

        /// <summary>Gets the set the type identifiers belongs to.</summary>
        ///
        /// <value>The type identifiers set.</value>
		IRedisSet TypeIdsSet { get; }

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
		T this[string key] { get; set; }

        /// <summary>Gets or sets the sequence key.</summary>
        ///
        /// <value>The sequence key.</value>
		string SequenceKey { get; set; }

        /// <summary>Sets a sequence.</summary>
        ///
        /// <param name="value">The value.</param>
		void SetSequence(int value);

        /// <summary>Gets the next sequence.</summary>
        ///
        /// <returns>The next sequence.</returns>
		long GetNextSequence();

        /// <summary>Gets the next sequence.</summary>
        ///
        /// <param name="incrBy">Amount to increment by.</param>
        ///
        /// <returns>The next sequence.</returns>
		long GetNextSequence(int incrBy);

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

        /// <summary>Sets an entry.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		void SetEntry(string key, T value);

        /// <summary>Sets an entry.</summary>
        ///
        /// <param name="key">     The key.</param>
        /// <param name="value">   The value.</param>
        /// <param name="expireIn">The expire in.</param>
		void SetEntry(string key, T value, TimeSpan expireIn);

        /// <summary>Sets entry if not exists.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetEntryIfNotExists(string key, T value);

        /// <summary>Gets a value.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The value.</returns>
		T GetValue(string key);

        /// <summary>Gets and set value.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The and set value.</returns>
		T GetAndSetValue(string key, T value);

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ContainsKey(string key);

        /// <summary>Removes the entry described by entities.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveEntry(string key);

        /// <summary>Removes the entry described by entities.</summary>
        ///
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveEntry(params string[] args);

        /// <summary>Removes the entry described by entities.</summary>
        ///
        /// <param name="entities">A variable-length parameters list containing entities.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveEntry(params IHasStringId[] entities);

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
        /// <returns>A long.</returns>
		long IncrementValueBy(string key, int count);

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

        /// <summary>Expire in.</summary>
        ///
        /// <param name="id">       The identifier.</param>
        /// <param name="expiresAt">The expires at.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireIn(object id, TimeSpan expiresAt);

        /// <summary>Expire at.</summary>
        ///
        /// <param name="id">      The identifier.</param>
        /// <param name="dateTime">The date time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireAt(object id, DateTime dateTime);

        /// <summary>Expire entry in.</summary>
        ///
        /// <param name="key">      The key.</param>
        /// <param name="expiresAt">The expires at.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireEntryIn(string key, TimeSpan expiresAt);

        /// <summary>Expire entry at.</summary>
        ///
        /// <param name="key">     The key.</param>
        /// <param name="dateTime">The date time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireEntryAt(string key, DateTime dateTime);

        /// <summary>Gets time to live.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The time to live.</returns>
		TimeSpan GetTimeToLive(string key);
        /// <summary>Saves this object.</summary>
		void Save();
        /// <summary>Saves the asynchronous.</summary>
		void SaveAsync();
        /// <summary>Flushes the database.</summary>
		void FlushDb();
        /// <summary>Flushes all.</summary>
		void FlushAll();

        /// <summary>Searches for the first keys.</summary>
        ///
        /// <param name="pattern">Specifies the pattern.</param>
        ///
        /// <returns>The found keys.</returns>
		T[] SearchKeys(string pattern);

        /// <summary>Gets the values.</summary>
        ///
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>The values.</returns>
		List<T> GetValues(List<string> keys);

        /// <summary>Gets sorted entry values.</summary>
        ///
        /// <param name="fromSet">     Set from belongs to.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The sorted entry values.</returns>
		List<T> GetSortedEntryValues(IRedisSet<T> fromSet, int startingFrom, int endingAt);

        /// <summary>Stores as hash.</summary>
        ///
        /// <param name="entity">The entity.</param>
	    void StoreAsHash(T entity);

        /// <summary>Gets from hash.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The data that was read from the hash.</returns>
	    T GetFromHash(object id);

        /// <summary>Gets all items from set.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        ///
        /// <returns>all items from set.</returns>
		HashSet<T> GetAllItemsFromSet(IRedisSet<T> fromSet);

        /// <summary>Adds an item to set to 'item'.</summary>
        ///
        /// <param name="toSet">Set to belongs to.</param>
        /// <param name="item"> The item.</param>
		void AddItemToSet(IRedisSet<T> toSet, T item);

        /// <summary>Removes the item from set.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        /// <param name="item">   The item.</param>
		void RemoveItemFromSet(IRedisSet<T> fromSet, T item);

        /// <summary>Pops the item from set described by fromSet.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        ///
        /// <returns>A T.</returns>
		T PopItemFromSet(IRedisSet<T> fromSet);

        /// <summary>Move between sets.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        /// <param name="toSet">  Set to belongs to.</param>
        /// <param name="item">   The item.</param>
		void MoveBetweenSets(IRedisSet<T> fromSet, IRedisSet<T> toSet, T item);

        /// <summary>Gets set count.</summary>
        ///
        /// <param name="set">The set.</param>
        ///
        /// <returns>The set count.</returns>
		long GetSetCount(IRedisSet<T> set);

        /// <summary>Sets contains item.</summary>
        ///
        /// <param name="set"> The set.</param>
        /// <param name="item">The item.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetContainsItem(IRedisSet<T> set, T item);

        /// <summary>Gets intersect from sets.</summary>
        ///
        /// <param name="sets">A variable-length parameters list containing sets.</param>
        ///
        /// <returns>The intersect from sets.</returns>
		HashSet<T> GetIntersectFromSets(params IRedisSet<T>[] sets);

        /// <summary>Stores intersect from sets.</summary>
        ///
        /// <param name="intoSet">Set the into belongs to.</param>
        /// <param name="sets">   A variable-length parameters list containing sets.</param>
		void StoreIntersectFromSets(IRedisSet<T> intoSet, params IRedisSet<T>[] sets);

        /// <summary>Gets union from sets.</summary>
        ///
        /// <param name="sets">A variable-length parameters list containing sets.</param>
        ///
        /// <returns>The union from sets.</returns>
		HashSet<T> GetUnionFromSets(params IRedisSet<T>[] sets);

        /// <summary>Stores union from sets.</summary>
        ///
        /// <param name="intoSet">Set the into belongs to.</param>
        /// <param name="sets">   A variable-length parameters list containing sets.</param>
		void StoreUnionFromSets(IRedisSet<T> intoSet, params IRedisSet<T>[] sets);

        /// <summary>Gets differences from set.</summary>
        ///
        /// <param name="fromSet"> Set from belongs to.</param>
        /// <param name="withSets">Sets the with belongs to.</param>
        ///
        /// <returns>The differences from set.</returns>
		HashSet<T> GetDifferencesFromSet(IRedisSet<T> fromSet, params IRedisSet<T>[] withSets);

        /// <summary>Stores differences from set.</summary>
        ///
        /// <param name="intoSet"> Set the into belongs to.</param>
        /// <param name="fromSet"> Set from belongs to.</param>
        /// <param name="withSets">Sets the with belongs to.</param>
		void StoreDifferencesFromSet(IRedisSet<T> intoSet, IRedisSet<T> fromSet, params IRedisSet<T>[] withSets);

        /// <summary>Gets random item from set.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        ///
        /// <returns>The random item from set.</returns>
		T GetRandomItemFromSet(IRedisSet<T> fromSet);

        /// <summary>Gets all items from list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        ///
        /// <returns>all items from list.</returns>
		List<T> GetAllItemsFromList(IRedisList<T> fromList);

        /// <summary>Gets range from list.</summary>
        ///
        /// <param name="fromList">    List of froms.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The range from list.</returns>
		List<T> GetRangeFromList(IRedisList<T> fromList, int startingFrom, int endingAt);

        /// <summary>Sort list.</summary>
        ///
        /// <param name="fromList">    List of froms.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The sorted list.</returns>
		List<T> SortList(IRedisList<T> fromList, int startingFrom, int endingAt);

        /// <summary>Adds an item to list to 'value'.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="value">   The value.</param>
		void AddItemToList(IRedisList<T> fromList, T value);

        /// <summary>Prepends an item to list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="value">   The value.</param>
		void PrependItemToList(IRedisList<T> fromList, T value);

        /// <summary>Removes the start from list described by fromList.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        ///
        /// <returns>A T.</returns>
		T RemoveStartFromList(IRedisList<T> fromList);

        /// <summary>Blocking remove start from list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="timeOut"> The time out.</param>
        ///
        /// <returns>A T.</returns>
		T BlockingRemoveStartFromList(IRedisList<T> fromList, TimeSpan? timeOut);

        /// <summary>Removes the end from list described by fromList.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        ///
        /// <returns>A T.</returns>
		T RemoveEndFromList(IRedisList<T> fromList);

        /// <summary>Removes all from list described by fromList.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
		void RemoveAllFromList(IRedisList<T> fromList);

        /// <summary>Trim list.</summary>
        ///
        /// <param name="fromList">        List of froms.</param>
        /// <param name="keepStartingFrom">The keep starting from.</param>
        /// <param name="keepEndingAt">    The keep ending at.</param>
		void TrimList(IRedisList<T> fromList, int keepStartingFrom, int keepEndingAt);

        /// <summary>Removes the item from list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="value">   The value.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveItemFromList(IRedisList<T> fromList, T value);

        /// <summary>Removes the item from list.</summary>
        ///
        /// <param name="fromList">   List of froms.</param>
        /// <param name="value">      The value.</param>
        /// <param name="noOfMatches">The no of matches.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveItemFromList(IRedisList<T> fromList, T value, int noOfMatches);

        /// <summary>Gets list count.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        ///
        /// <returns>The list count.</returns>
		long GetListCount(IRedisList<T> fromList);

        /// <summary>Gets item from list.</summary>
        ///
        /// <param name="fromList"> List of froms.</param>
        /// <param name="listIndex">Zero-based index of the list.</param>
        ///
        /// <returns>The item from list.</returns>
		T GetItemFromList(IRedisList<T> fromList, int listIndex);

        /// <summary>Sets item in list.</summary>
        ///
        /// <param name="toList">   List of toes.</param>
        /// <param name="listIndex">Zero-based index of the list.</param>
        /// <param name="value">    The value.</param>
		void SetItemInList(IRedisList<T> toList, int listIndex, T value);

        /// <summary>Inserts a before item in list.</summary>
        ///
        /// <param name="toList">List of toes.</param>
        /// <param name="pivot"> The pivot.</param>
        /// <param name="value"> The value.</param>
        void InsertBeforeItemInList(IRedisList<T> toList, T pivot, T value);

        /// <summary>Inserts an after item in list.</summary>
        ///
        /// <param name="toList">List of toes.</param>
        /// <param name="pivot"> The pivot.</param>
        /// <param name="value"> The value.</param>
        void InsertAfterItemInList(IRedisList<T> toList, T pivot, T value);

        /// <summary>Enqueue item on list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="item">    The item.</param>
		void EnqueueItemOnList(IRedisList<T> fromList, T item);

        /// <summary>Dequeue item from list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        ///
        /// <returns>A T.</returns>
		T DequeueItemFromList(IRedisList<T> fromList);

        /// <summary>Blocking dequeue item from list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="timeOut"> The time out.</param>
        ///
        /// <returns>A T.</returns>
		T BlockingDequeueItemFromList(IRedisList<T> fromList, TimeSpan? timeOut);

        /// <summary>Pushes an item to list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="item">    The item.</param>
		void PushItemToList(IRedisList<T> fromList, T item);

        /// <summary>Pops the item from list described by fromList.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        ///
        /// <returns>A T.</returns>
		T PopItemFromList(IRedisList<T> fromList);

        /// <summary>Blocking pop item from list.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="timeOut"> The time out.</param>
        ///
        /// <returns>A T.</returns>
		T BlockingPopItemFromList(IRedisList<T> fromList, TimeSpan? timeOut);

        /// <summary>Pops the and push item between lists.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="toList">  List of toes.</param>
        ///
        /// <returns>A T.</returns>
		T PopAndPushItemBetweenLists(IRedisList<T> fromList, IRedisList<T> toList);

        /// <summary>Blocking pop and push item between lists.</summary>
        ///
        /// <param name="fromList">List of froms.</param>
        /// <param name="toList">  List of toes.</param>
        /// <param name="timeOut"> The time out.</param>
        ///
        /// <returns>A T.</returns>
        T BlockingPopAndPushItemBetweenLists(IRedisList<T> fromList, IRedisList<T> toList, TimeSpan? timeOut);

        /// <summary>Adds an item to sorted set.</summary>
        ///
        /// <param name="toSet">Set to belongs to.</param>
        /// <param name="value">The value.</param>
		void AddItemToSortedSet(IRedisSortedSet<T> toSet, T value);

        /// <summary>Adds an item to sorted set.</summary>
        ///
        /// <param name="toSet">Set to belongs to.</param>
        /// <param name="value">The value.</param>
        /// <param name="score">The score.</param>
		void AddItemToSortedSet(IRedisSortedSet<T> toSet, T value, double score);

        /// <summary>Removes the item from sorted set.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        /// <param name="value">  The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveItemFromSortedSet(IRedisSortedSet<T> fromSet, T value);

        /// <summary>Pops the item with lowest score from sorted set described by fromSet.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        ///
        /// <returns>A T.</returns>
		T PopItemWithLowestScoreFromSortedSet(IRedisSortedSet<T> fromSet);

        /// <summary>Pops the item with highest score from sorted set described by fromSet.</summary>
        ///
        /// <param name="fromSet">Set from belongs to.</param>
        ///
        /// <returns>A T.</returns>
		T PopItemWithHighestScoreFromSortedSet(IRedisSortedSet<T> fromSet);

        /// <summary>Sorted set contains item.</summary>
        ///
        /// <param name="set">  The set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SortedSetContainsItem(IRedisSortedSet<T> set, T value);

        /// <summary>Increment item in sorted set.</summary>
        ///
        /// <param name="set">        The set.</param>
        /// <param name="value">      The value.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		double IncrementItemInSortedSet(IRedisSortedSet<T> set, T value, double incrementBy);

        /// <summary>Gets item index in sorted set.</summary>
        ///
        /// <param name="set">  The set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item index in sorted set.</returns>
		long GetItemIndexInSortedSet(IRedisSortedSet<T> set, T value);

        /// <summary>Gets item index in sorted set description.</summary>
        ///
        /// <param name="set">  The set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item index in sorted set description.</returns>
		long GetItemIndexInSortedSetDesc(IRedisSortedSet<T> set, T value);

        /// <summary>Gets all items from sorted set.</summary>
        ///
        /// <param name="set">The set.</param>
        ///
        /// <returns>all items from sorted set.</returns>
		List<T> GetAllItemsFromSortedSet(IRedisSortedSet<T> set);

        /// <summary>Gets all items from sorted set description.</summary>
        ///
        /// <param name="set">The set.</param>
        ///
        /// <returns>all items from sorted set description.</returns>
		List<T> GetAllItemsFromSortedSetDesc(IRedisSortedSet<T> set);

        /// <summary>Gets range from sorted set.</summary>
        ///
        /// <param name="set">     The set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range from sorted set.</returns>
		List<T> GetRangeFromSortedSet(IRedisSortedSet<T> set, int fromRank, int toRank);

        /// <summary>Gets range from sorted set description.</summary>
        ///
        /// <param name="set">     The set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range from sorted set description.</returns>
		List<T> GetRangeFromSortedSetDesc(IRedisSortedSet<T> set, int fromRank, int toRank);

        /// <summary>Gets all with scores from sorted set.</summary>
        ///
        /// <param name="set">The set.</param>
        ///
        /// <returns>all with scores from sorted set.</returns>
		IDictionary<T, double> GetAllWithScoresFromSortedSet(IRedisSortedSet<T> set);

        /// <summary>Gets range with scores from sorted set.</summary>
        ///
        /// <param name="set">     The set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range with scores from sorted set.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSet(IRedisSortedSet<T> set, int fromRank, int toRank);

        /// <summary>Gets range with scores from sorted set description.</summary>
        ///
        /// <param name="set">     The set.</param>
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The range with scores from sorted set description.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetDesc(IRedisSortedSet<T> set, int fromRank, int toRank);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore);

        /// <summary>Gets range from sorted set by lowest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range from sorted set by lowest score.</returns>
		List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore);

        /// <summary>Gets range with scores from sorted set by lowest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range with scores from sorted set by lowest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore, double toScore);

        /// <summary>Gets range from sorted set by highest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range from sorted set by highest score.</returns>
		List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="set">            The set.</param>
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore, double toScore);

        /// <summary>Gets range with scores from sorted set by highest score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range with scores from sorted set by highest score.</returns>
		IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore, double toScore, int? skip, int? take);

        /// <summary>Removes the range from sorted set.</summary>
        ///
        /// <param name="set">    The set.</param>
        /// <param name="minRank">The minimum rank.</param>
        /// <param name="maxRank">The maximum rank.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRangeFromSortedSet(IRedisSortedSet<T> set, int minRank, int maxRank);

        /// <summary>Removes the range from sorted set by score.</summary>
        ///
        /// <param name="set">      The set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRangeFromSortedSetByScore(IRedisSortedSet<T> set, double fromScore, double toScore);

        /// <summary>Gets sorted set count.</summary>
        ///
        /// <param name="set">The set.</param>
        ///
        /// <returns>The sorted set count.</returns>
		long GetSortedSetCount(IRedisSortedSet<T> set);

        /// <summary>Gets item score in sorted set.</summary>
        ///
        /// <param name="set">  The set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item score in sorted set.</returns>
		double GetItemScoreInSortedSet(IRedisSortedSet<T> set, T value);

        /// <summary>Stores intersect from sorted sets.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long StoreIntersectFromSortedSets(IRedisSortedSet<T> intoSetId, params IRedisSortedSet<T>[] setIds);

        /// <summary>Stores union from sorted sets.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long StoreUnionFromSortedSets(IRedisSortedSet<T> intoSetId, params IRedisSortedSet<T>[] setIds);

        /// <summary>Query if 'hash' hash contains entry.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        /// <param name="key"> The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool HashContainsEntry<TKey>(IRedisHash<TKey, T> hash, TKey key);

        /// <summary>Sets entry in hash.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash"> The hash.</param>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetEntryInHash<TKey>(IRedisHash<TKey, T> hash, TKey key, T value);

        /// <summary>Sets entry in hash if not exists.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash"> The hash.</param>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool SetEntryInHashIfNotExists<TKey>(IRedisHash<TKey, T> hash, TKey key, T value);

        /// <summary>Sets range in hash.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">         The hash.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
		void SetRangeInHash<TKey>(IRedisHash<TKey, T> hash, IEnumerable<KeyValuePair<TKey, T>> keyValuePairs);

        /// <summary>Gets value from hash.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        /// <param name="key"> The key.</param>
        ///
        /// <returns>The value from hash.</returns>
		T GetValueFromHash<TKey>(IRedisHash<TKey, T> hash, TKey key);

        /// <summary>Removes the entry from hash.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        /// <param name="key"> The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RemoveEntryFromHash<TKey>(IRedisHash<TKey, T> hash, TKey key);

        /// <summary>Gets hash count.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        ///
        /// <returns>The hash count.</returns>
		long GetHashCount<TKey>(IRedisHash<TKey, T> hash);

        /// <summary>Gets hash keys.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        ///
        /// <returns>The hash keys.</returns>
		List<TKey> GetHashKeys<TKey>(IRedisHash<TKey, T> hash);

        /// <summary>Gets hash values.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        ///
        /// <returns>The hash values.</returns>
		List<T> GetHashValues<TKey>(IRedisHash<TKey, T> hash);

        /// <summary>Gets all entries from hash.</summary>
        ///
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="hash">The hash.</param>
        ///
        /// <returns>all entries from hash.</returns>
		Dictionary<TKey, T> GetAllEntriesFromHash<TKey>(IRedisHash<TKey, T> hash);

        /// <summary>Stores related entities.</summary>
        ///
        /// <typeparam name="TChild">Type of the child.</typeparam>
        /// <param name="parentId">Identifier for the parent.</param>
        /// <param name="children">A variable-length parameters list containing children.</param>
		void StoreRelatedEntities<TChild>(object parentId, List<TChild> children);

        /// <summary>Stores related entities.</summary>
        ///
        /// <typeparam name="TChild">Type of the child.</typeparam>
        /// <param name="parentId">Identifier for the parent.</param>
        /// <param name="children">A variable-length parameters list containing children.</param>
		void StoreRelatedEntities<TChild>(object parentId, params TChild[] children);

        /// <summary>Deletes the related entities described by parentId.</summary>
        ///
        /// <typeparam name="TChild">Type of the child.</typeparam>
        /// <param name="parentId">Identifier for the parent.</param>
		void DeleteRelatedEntities<TChild>(object parentId);

        /// <summary>Deletes the related entity.</summary>
        ///
        /// <typeparam name="TChild">Type of the child.</typeparam>
        /// <param name="parentId">Identifier for the parent.</param>
        /// <param name="childId"> Identifier for the child.</param>
		void DeleteRelatedEntity<TChild>(object parentId, object childId);

        /// <summary>Gets related entities.</summary>
        ///
        /// <typeparam name="TChild">Type of the child.</typeparam>
        /// <param name="parentId">Identifier for the parent.</param>
        ///
        /// <returns>The related entities.</returns>
		List<TChild> GetRelatedEntities<TChild>(object parentId);

        /// <summary>Gets related entities count.</summary>
        ///
        /// <typeparam name="TChild">Type of the child.</typeparam>
        /// <param name="parentId">Identifier for the parent.</param>
        ///
        /// <returns>The related entities count.</returns>
		long GetRelatedEntitiesCount<TChild>(object parentId);

        /// <summary>Adds to the recents list.</summary>
        ///
        /// <param name="value">The value.</param>
		void AddToRecentsList(T value);

        /// <summary>Gets latest from recents list.</summary>
        ///
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        ///
        /// <returns>The latest from recents list.</returns>
		List<T> GetLatestFromRecentsList(int skip, int take);

        /// <summary>Gets earliest from recents list.</summary>
        ///
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        ///
        /// <returns>The earliest from recents list.</returns>
		List<T> GetEarliestFromRecentsList(int skip, int take);
	}

}