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

namespace NServiceKit.Redis
{
    /// <summary>Interface for redis native client.</summary>
	public interface IRedisNativeClient
		: IDisposable
	{
		//Redis utility operations
		
		/// <summary>
		/// Info
		/// </summary>
		Dictionary<string, string> Info { get; }

        /// <summary>Gets or sets the database.</summary>
        ///
        /// <value>The database.</value>
		long Db { get; set; }

        /// <summary>Gets the size of the database.</summary>
        ///
        /// <value>The size of the database.</value>
		long DbSize { get; }

        /// <summary>Gets the Date/Time of the last save.</summary>
        ///
        /// <value>The last save.</value>
		DateTime LastSave { get; }
        /// <summary>Saves this object.</summary>
		void Save();
        /// <summary>Background save.</summary>
		void BgSave();
        /// <summary>Shuts down this object and frees any resources it is using.</summary>
		void Shutdown();
        /// <summary>Background rewrite aof.</summary>
		void BgRewriteAof();
        /// <summary>Quits this object.</summary>
		void Quit();
        /// <summary>Flushes the database.</summary>
		void FlushDb();
        /// <summary>Flushes all.</summary>
		void FlushAll();

        /// <summary>Pings this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool Ping();

        /// <summary>Echoes.</summary>
        ///
        /// <param name="text">The text.</param>
        ///
        /// <returns>A string.</returns>
		string Echo(string text);

        /// <summary>Slave of.</summary>
        ///
        /// <param name="hostname">The hostname.</param>
        /// <param name="port">    The port.</param>
		void SlaveOf(string hostname, int port);
        /// <summary>Slave of no one.</summary>
		void SlaveOfNoOne();

        /// <summary>Configuration get.</summary>
        ///
        /// <param name="pattern">Specifies the pattern.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ConfigGet(string pattern);

        /// <summary>Configuration set.</summary>
        ///
        /// <param name="item"> The item.</param>
        /// <param name="value">The value.</param>
		void ConfigSet(string item, byte[] value);
        /// <summary>Configuration reset stat.</summary>
		void ConfigResetStat();

        /// <summary>Gets the time.</summary>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] Time();
        /// <summary>Debug segfault.</summary>
		void DebugSegfault();

        /// <summary>Dumps.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] Dump(string key);

        /// <summary>Restores.</summary>
        ///
        /// <param name="key">      The key.</param>
        /// <param name="expireMs"> The expire in milliseconds.</param>
        /// <param name="dumpValue">The dump value.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] Restore(string key, long expireMs, byte[] dumpValue);

        /// <summary>Migrates.</summary>
        ///
        /// <param name="host">         The host.</param>
        /// <param name="port">         The port.</param>
        /// <param name="destinationDb">Destination database.</param>
        /// <param name="timeoutMs">    The timeout in milliseconds.</param>
		void Migrate(string host, int port, int destinationDb, long timeoutMs);

        /// <summary>Moves.</summary>
        ///
        /// <param name="key">The key.</param>
        /// <param name="db"> The database.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool Move(string key, int db);

        /// <summary>Object idle time.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long ObjectIdleTime(string key);

        /// <summary>Keys.</summary>
        ///
        /// <param name="pattern">Specifies the pattern.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] Keys(string pattern);

        /// <summary>Exists.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long Exists(string key);

        /// <summary>String lens.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long StrLen(string key);

        /// <summary>Sets.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		void Set(string key, byte[] value);

        /// <summary>Sets an ex.</summary>
        ///
        /// <param name="key">            The key.</param>
        /// <param name="expireInSeconds">The expire in seconds.</param>
        /// <param name="value">          The value.</param>
		void SetEx(string key, int expireInSeconds, byte[] value);

        /// <summary>Persists.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool Persist(string key);

        /// <summary>Sets an ex.</summary>
        ///
        /// <param name="key">       The key.</param>
        /// <param name="expireInMs">The expire in milliseconds.</param>
        /// <param name="value">     The value.</param>
		void PSetEx(string key, long expireInMs, byte[] value);

        /// <summary>Sets a nx.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long SetNX(string key, byte[] value);

        /// <summary>Sets.</summary>
        ///
        /// <param name="keys">  The keys.</param>
        /// <param name="values">The values.</param>
		void MSet(byte[][] keys, byte[][] values);

        /// <summary>Sets.</summary>
        ///
        /// <param name="keys">  The keys.</param>
        /// <param name="values">The values.</param>
		void MSet(string[] keys, byte[][] values);

        /// <summary>Sets a nx.</summary>
        ///
        /// <param name="keys">  The keys.</param>
        /// <param name="values">The values.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool MSetNx(byte[][] keys, byte[][] values);

        /// <summary>Sets a nx.</summary>
        ///
        /// <param name="keys">  The keys.</param>
        /// <param name="values">The values.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool MSetNx(string[] keys, byte[][] values);

        /// <summary>Gets.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] Get(string key);

        /// <summary>Gets a set.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>An array of byte.</returns>
		byte[] GetSet(string key, byte[] value);

        /// <summary>Gets the given keys.</summary>
        ///
        /// <param name="keysAndArgs">A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] MGet(params byte[][] keysAndArgs);

        /// <summary>Gets the given keys.</summary>
        ///
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] MGet(params string[] keys);

        /// <summary>Deletes the given keys.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long Del(string key);

        /// <summary>Deletes the given keys.</summary>
        ///
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>A long.</returns>
		long Del(params string[] keys);

        /// <summary>Incrs.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long Incr(string key);

        /// <summary>Increment by.</summary>
        ///
        /// <param name="key">   The key.</param>
        /// <param name="incrBy">Amount to increment by.</param>
        ///
        /// <returns>A long.</returns>
		long IncrBy(string key, int incrBy);

        /// <summary>Increment by float.</summary>
        ///
        /// <param name="key">   The key.</param>
        /// <param name="incrBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		double IncrByFloat(string key, double incrBy);

        /// <summary>Decrs.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long Decr(string key);

        /// <summary>Decrement by.</summary>
        ///
        /// <param name="key">   The key.</param>
        /// <param name="decrBy">Amount to decrement by.</param>
        ///
        /// <returns>A long.</returns>
		long DecrBy(string key, int decrBy);

        /// <summary>Appends a key.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long Append(string key, byte[] value);

        /// <summary>(This method is obsolete) substrs.</summary>
        ///
        /// <param name="key">      The key.</param>
        /// <param name="fromIndex">Zero-based index of from.</param>
        /// <param name="toIndex">  Zero-based index of to.</param>
        ///
        /// <returns>A byte[].</returns>
		[Obsolete("Was renamed to GetRange in 2.4")]
		byte[] Substr(string key, int fromIndex, int toIndex);

        /// <summary>Finds the range of the given arguments.</summary>
        ///
        /// <param name="key">      The key.</param>
        /// <param name="fromIndex">Zero-based index of from.</param>
        /// <param name="toIndex">  Zero-based index of to.</param>
        ///
        /// <returns>The calculated range.</returns>
		byte[] GetRange(string key, int fromIndex, int toIndex);

        /// <summary>Sets a range.</summary>
        ///
        /// <param name="key">   The key.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long SetRange(string key, int offset, byte[] value);

        /// <summary>Gets a bit.</summary>
        ///
        /// <param name="key">   The key.</param>
        /// <param name="offset">The offset.</param>
        ///
        /// <returns>The bit.</returns>
		long GetBit(string key, int offset);

        /// <summary>Sets a bit.</summary>
        ///
        /// <param name="key">   The key.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long SetBit(string key, int offset, int value);

        /// <summary>Random key.</summary>
        ///
        /// <returns>A string.</returns>
		string RandomKey();

        /// <summary>Renames.</summary>
        ///
        /// <param name="oldKeyname">The old keyname.</param>
        /// <param name="newKeyname">The new keyname.</param>
		void Rename(string oldKeyname, string newKeyname);

        /// <summary>Rename nx.</summary>
        ///
        /// <param name="oldKeyname">The old keyname.</param>
        /// <param name="newKeyname">The new keyname.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool RenameNx(string oldKeyname, string newKeyname);

        /// <summary>Expires.</summary>
        ///
        /// <param name="key">    The key.</param>
        /// <param name="seconds">The seconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool Expire(string key, int seconds);

        /// <summary>Expires.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="ttlMs">The TTL in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool PExpire(string key, long ttlMs);

        /// <summary>Expire at.</summary>
        ///
        /// <param name="key">     The key.</param>
        /// <param name="unixTime">The unix time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool ExpireAt(string key, long unixTime);

        /// <summary>Expire at.</summary>
        ///
        /// <param name="key">       The key.</param>
        /// <param name="unixTimeMs">The unix time in milliseconds.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool PExpireAt(string key, long unixTimeMs);

        /// <summary>Ttls.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long Ttl(string key);

        /// <summary>Ttls.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>A long.</returns>
		long PTtl(string key);

        /// <summary>Sorts.</summary>
        ///
        /// <param name="listOrSetId">Identifier for the list or set.</param>
        /// <param name="sortOptions">Options for controlling the sort.</param>
        ///
        /// <returns>The sorted values.</returns>
		byte[][] Sort(string listOrSetId, SortOptions sortOptions);

        /// <summary>Ranges.</summary>
        ///
        /// <param name="listId">      Identifier for the list.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] LRange(string listId, int startingFrom, int endingAt);

        /// <summary>Pushes an object onto this stack.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long RPush(string listId, byte[] value);

        /// <summary>Pushes an x coordinate.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long RPushX(string listId, byte[] value);

        /// <summary>Pushes an object onto this stack.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long LPush(string listId, byte[] value);

        /// <summary>Pushes an x coordinate.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long LPushX(string listId, byte[] value);

        /// <summary>Trims.</summary>
        ///
        /// <param name="listId">          Identifier for the list.</param>
        /// <param name="keepStartingFrom">The keep starting from.</param>
        /// <param name="keepEndingAt">    The keep ending at.</param>
		void LTrim(string listId, int keepStartingFrom, int keepEndingAt);

        /// <summary>Rems.</summary>
        ///
        /// <param name="listId">           Identifier for the list.</param>
        /// <param name="removeNoOfMatches">The remove no of matches.</param>
        /// <param name="value">            The value.</param>
        ///
        /// <returns>A long.</returns>
		long LRem(string listId, int removeNoOfMatches, byte[] value);

        /// <summary>Lens.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>A long.</returns>
		long LLen(string listId);

        /// <summary>Indexes.</summary>
        ///
        /// <param name="listId">   Identifier for the list.</param>
        /// <param name="listIndex">Zero-based index of the list.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] LIndex(string listId, int listIndex);

        /// <summary>Inserts.</summary>
        ///
        /// <param name="listId">      Identifier for the list.</param>
        /// <param name="insertBefore">true to insert before.</param>
        /// <param name="pivot">       The pivot.</param>
        /// <param name="value">       The value.</param>
        void LInsert(string listId, bool insertBefore, byte[] pivot, byte[] value);

        /// <summary>Sets.</summary>
        ///
        /// <param name="listId">   Identifier for the list.</param>
        /// <param name="listIndex">Zero-based index of the list.</param>
        /// <param name="value">    The value.</param>
		void LSet(string listId, int listIndex, byte[] value);

        /// <summary>Removes and returns the top-of-stack object.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>The previous top-of-stack object.</returns>
		byte[] LPop(string listId);

        /// <summary>Removes and returns the top-of-stack object.</summary>
        ///
        /// <param name="listId">Identifier for the list.</param>
        ///
        /// <returns>The previous top-of-stack object.</returns>
		byte[] RPop(string listId);

        /// <summary>Bl pop.</summary>
        ///
        /// <param name="listId">     Identifier for the list.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] BLPop(string listId, int timeOutSecs);

        /// <summary>Bl pop.</summary>
        ///
        /// <param name="listIds">    List of identifiers for the lists.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
	    byte[][] BLPop(string[] listIds, int timeOutSecs);

        /// <summary>Bl pop value.</summary>
        ///
        /// <param name="listId">     Identifier for the list.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[] BLPopValue(string listId, int timeOutSecs);

        /// <summary>Bl pop value.</summary>
        ///
        /// <param name="listIds">    List of identifiers for the lists.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
	    byte[][] BLPopValue(string[] listIds, int timeOutSecs);

        /// <summary>Line break pop.</summary>
        ///
        /// <param name="listId">     Identifier for the list.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] BRPop(string listId, int timeOutSecs);

        /// <summary>Line break pop.</summary>
        ///
        /// <param name="listIds">    List of identifiers for the lists.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
	    byte[][] BRPop(string[] listIds, int timeOutSecs);

        /// <summary>Pops the l push.</summary>
        ///
        /// <param name="fromListId">Identifier for from list.</param>
        /// <param name="toListId">  Identifier for to list.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] RPopLPush(string fromListId, string toListId);

        /// <summary>Line break pop value.</summary>
        ///
        /// <param name="listId">     Identifier for the list.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[] BRPopValue(string listId, int timeOutSecs);

        /// <summary>Line break pop value.</summary>
        ///
        /// <param name="listIds">    List of identifiers for the lists.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[][].</returns>
        byte[][] BRPopValue(string[] listIds, int timeOutSecs);

        /// <summary>Line break pop l push.</summary>
        ///
        /// <param name="fromListId"> Identifier for from list.</param>
        /// <param name="toListId">   Identifier for to list.</param>
        /// <param name="timeOutSecs">The time out in seconds.</param>
        ///
        /// <returns>A byte[].</returns>
        byte[] BRPopLPush(string fromListId, string toListId, int timeOutSecs);

        /// <summary>Members.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] SMembers(string setId);

        /// <summary>Adds setId.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
        long SAdd(string setId, byte[] value);

        /// <summary>Adds setId.</summary>
        ///
        /// <param name="setId"> Identifier for the set.</param>
        /// <param name="values">The values.</param>
        ///
        /// <returns>A long.</returns>
        long SAdd(string setId, byte[][] values);

        /// <summary>Rems.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long SRem(string setId, byte[] value);

        /// <summary>Removes and returns the top-of-stack object.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>The previous top-of-stack object.</returns>
		byte[] SPop(string setId);

        /// <summary>Moves.</summary>
        ///
        /// <param name="fromSetId">Identifier for from set.</param>
        /// <param name="toSetId">  Identifier for to set.</param>
        /// <param name="value">    The value.</param>
		void SMove(string fromSetId, string toSetId, byte[] value);

        /// <summary>Cards.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A long.</returns>
		long SCard(string setId);

        /// <summary>Is member.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long SIsMember(string setId, byte[] value);

        /// <summary>Inters the given set identifiers.</summary>
        ///
        /// <param name="setIds">List of identifiers for the sets.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] SInter(params string[] setIds);

        /// <summary>Inter store.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
		void SInterStore(string intoSetId, params string[] setIds);

        /// <summary>Unions the given set identifiers.</summary>
        ///
        /// <param name="setIds">List of identifiers for the sets.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] SUnion(params string[] setIds);

        /// <summary>Union store.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
		void SUnionStore(string intoSetId, params string[] setIds);

        /// <summary>Compares two string objects to determine their relative ordering.</summary>
        ///
        /// <param name="fromSetId"> Identifier for from set.</param>
        /// <param name="withSetIds">List of identifiers for the with sets.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] SDiff(string fromSetId, params string[] withSetIds);

        /// <summary>Difference store.</summary>
        ///
        /// <param name="intoSetId"> Identifier for the into set.</param>
        /// <param name="fromSetId"> Identifier for from set.</param>
        /// <param name="withSetIds">List of identifiers for the with sets.</param>
		void SDiffStore(string intoSetId, string fromSetId, params string[] withSetIds);

        /// <summary>Random member.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] SRandMember(string setId);

        /// <summary>Adds setId.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="score">The score.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long ZAdd(string setId, double score, byte[] value);

        /// <summary>Adds setId.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="score">The score.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long ZAdd(string setId, long score, byte[] value);

        /// <summary>Z coordinate rems.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long ZRem(string setId, byte[] value);

        /// <summary>Increment by.</summary>
        ///
        /// <param name="setId"> Identifier for the set.</param>
        /// <param name="incrBy">Amount to increment by.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A double.</returns>
		double ZIncrBy(string setId, double incrBy, byte[] value);

        /// <summary>Increment by.</summary>
        ///
        /// <param name="setId"> Identifier for the set.</param>
        /// <param name="incrBy">Amount to increment by.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A double.</returns>
		double ZIncrBy(string setId, long incrBy, byte[] value);

        /// <summary>Z coordinate ranks.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long ZRank(string setId, byte[] value);

        /// <summary>Reverse rank.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A long.</returns>
		long ZRevRank(string setId, byte[] value);

        /// <summary>Z coordinate ranges.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRange(string setId, int min, int max);

        /// <summary>Range with scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRangeWithScores(string setId, int min, int max);

        /// <summary>Reverse range.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRevRange(string setId, int min, int max);

        /// <summary>Reverse range with scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRevRangeWithScores(string setId, int min, int max);

        /// <summary>Range by score.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRangeByScore(string setId, double min, double max, int? skip, int? take);

        /// <summary>Range by score.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRangeByScore(string setId, long min, long max, int? skip, int? take);

        /// <summary>Range by score with scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRangeByScoreWithScores(string setId, double min, double max, int? skip, int? take);

        /// <summary>Range by score with scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRangeByScoreWithScores(string setId, long min, long max, int? skip, int? take);

        /// <summary>Reverse range by score.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRevRangeByScore(string setId, double min, double max, int? skip, int? take);

        /// <summary>Reverse range by score.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRevRangeByScore(string setId, long min, long max, int? skip, int? take);

        /// <summary>Reverse range by score with scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRevRangeByScoreWithScores(string setId, double min, double max, int? skip, int? take);

        /// <summary>Reverse range by score with scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        /// <param name="skip"> The skip.</param>
        /// <param name="take"> The take.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ZRevRangeByScoreWithScores(string setId, long min, long max, int? skip, int? take);

        /// <summary>Rem range by rank.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="min">  The minimum.</param>
        /// <param name="max">  The maximum.</param>
        ///
        /// <returns>A long.</returns>
		long ZRemRangeByRank(string setId, int min, int max);

        /// <summary>Rem range by score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>A long.</returns>
		long ZRemRangeByScore(string setId, double fromScore, double toScore);

        /// <summary>Rem range by score.</summary>
        ///
        /// <param name="setId">    Identifier for the set.</param>
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>A long.</returns>
		long ZRemRangeByScore(string setId, long fromScore, long toScore);

        /// <summary>Z coordinate cards.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>A long.</returns>
		long ZCard(string setId);

        /// <summary>Z coordinate scores.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>A double.</returns>
		double ZScore(string setId, byte[] value);

        /// <summary>Union store.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long ZUnionStore(string intoSetId, params string[] setIds);

        /// <summary>Union store with weights.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIdWithWeightPairs">   List of identifiers for the sets along with the weight to multiply the scores with.</param>
        ///
        /// <returns>A long.</returns>
		long ZUnionStoreWithWeights(string intoSetId, params KeyValuePair<string, double>[] setIdWithWeightPairs);
        
        /// <summary>Inter store.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIds">   List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long ZInterStore(string intoSetId, params string[] setIds);

        /// <summary>Inter store with weights.</summary>
        ///
        /// <param name="intoSetId">Identifier for the into set.</param>
        /// <param name="setIdWithWeightPairs">   List of identifiers for the sets along with the weight to multiply the scores with.</param>
        ///
        /// <returns>A long.</returns>
		long ZInterStoreWithWeights(string intoSetId, params KeyValuePair<string, double>[] setIdWithWeightPairs);

        /// <summary>Sets.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long HSet(string hashId, byte[] key, byte[] value);

        /// <summary>Hm set.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="keys">  The keys.</param>
        /// <param name="values">The values.</param>
		void HMSet(string hashId, byte[][] keys, byte[][] values);

        /// <summary>Sets a nx.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        /// <param name="value"> The value.</param>
        ///
        /// <returns>A long.</returns>
		long HSetNX(string hashId, byte[] key, byte[] value);

        /// <summary>Incrbies.</summary>
        ///
        /// <param name="hashId">     Identifier for the hash.</param>
        /// <param name="key">        The key.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A long.</returns>
		long HIncrby(string hashId, byte[] key, int incrementBy);

        /// <summary>Incrby float.</summary>
        ///
        /// <param name="hashId">     Identifier for the hash.</param>
        /// <param name="key">        The key.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		double HIncrbyFloat(string hashId, byte[] key, double incrementBy);

        /// <summary>Gets.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] HGet(string hashId, byte[] key);

        /// <summary>Hm get.</summary>
        ///
        /// <param name="hashId">     Identifier for the hash.</param>
        /// <param name="keysAndArgs">A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] HMGet(string hashId, params byte[][] keysAndArgs);

        /// <summary>Deletes this object.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>A long.</returns>
		long HDel(string hashId, byte[] key);

        /// <summary>Exists.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>A long.</returns>
		long HExists(string hashId, byte[] key);

        /// <summary>Lens.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>A long.</returns>
		long HLen(string hashId);

        /// <summary>Keys.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] HKeys(string hashId);

        /// <summary>Vals.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] HVals(string hashId);

        /// <summary>Gets all.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        ///
        /// <returns>An array of byte[].</returns>
		byte[][] HGetAll(string hashId);

        /// <summary>Watches the given keys.</summary>
        ///
        /// <param name="keys">The keys.</param>
		void Watch(params string[] keys);

        /// <summary>Un watch.</summary>
		void UnWatch();

        /// <summary>Publishes.</summary>
        ///
        /// <param name="toChannel">to channel.</param>
        /// <param name="message">  The message.</param>
        ///
        /// <returns>A long.</returns>
		long Publish(string toChannel, byte[] message);

        /// <summary>Subscribes the given to channels.</summary>
        ///
        /// <param name="toChannels">A variable-length parameters list containing to channels.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] Subscribe(params string[] toChannels);

        /// <summary>Un subscribe.</summary>
        ///
        /// <param name="toChannels">A variable-length parameters list containing to channels.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] UnSubscribe(params string[] toChannels);

        /// <summary>Subscribes the given to channels matching patterns.</summary>
        ///
        /// <param name="toChannelsMatchingPatterns">A variable-length parameters list containing to channels matching patterns.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] PSubscribe(params string[] toChannelsMatchingPatterns);

        /// <summary>Un subscribe.</summary>
        ///
        /// <param name="toChannelsMatchingPatterns">A variable-length parameters list containing to channels matching patterns.</param>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] PUnSubscribe(params string[] toChannelsMatchingPatterns);

        /// <summary>Receive messages.</summary>
        ///
        /// <returns>A byte[][].</returns>
		byte[][] ReceiveMessages();

        /// <summary>Redis LUA support.</summary>
        ///
        /// <param name="luaBody">     The lua body.</param>
        /// <param name="numberOfKeys">Number of keys.</param>
        /// <param name="keysAndArgs"> A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A long.</returns>
		long EvalInt(string luaBody, int numberOfKeys, params byte[][] keysAndArgs);

        /// <summary>Eval sha int.</summary>
        ///
        /// <param name="sha1">        The first sha.</param>
        /// <param name="numberOfKeys">Number of keys.</param>
        /// <param name="keysAndArgs"> A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A long.</returns>
		long EvalShaInt(string sha1, int numberOfKeys, params byte[][] keysAndArgs);

        /// <summary>Eval string.</summary>
        ///
        /// <param name="luaBody">     The lua body.</param>
        /// <param name="numberOfKeys">Number of keys.</param>
        /// <param name="keysAndArgs"> A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A string.</returns>
        string EvalStr(string luaBody, int numberOfKeys, params byte[][] keysAndArgs);

        /// <summary>Eval sha string.</summary>
        ///
        /// <param name="sha1">        The first sha.</param>
        /// <param name="numberOfKeys">Number of keys.</param>
        /// <param name="keysAndArgs"> A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A string.</returns>
        string EvalShaStr(string sha1, int numberOfKeys, params byte[][] keysAndArgs);

        /// <summary>Evals.</summary>
        ///
        /// <param name="luaBody">     The lua body.</param>
        /// <param name="numberOfKeys">Number of keys.</param>
        /// <param name="keysAndArgs"> A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A byte[][].</returns>
        byte[][] Eval(string luaBody, int numberOfKeys, params byte[][] keysAndArgs);

        /// <summary>Eval sha.</summary>
        ///
        /// <param name="sha1">        The first sha.</param>
        /// <param name="numberOfKeys">Number of keys.</param>
        /// <param name="keysAndArgs"> A variable-length parameters list containing keys and arguments.</param>
        ///
        /// <returns>A byte[][].</returns>
        byte[][] EvalSha(string sha1, int numberOfKeys, params byte[][] keysAndArgs);

        /// <summary>Calculates the sha 1.</summary>
        ///
        /// <param name="luaBody">The lua body.</param>
        ///
        /// <returns>The calculated sha 1.</returns>
        string CalculateSha1(string luaBody);

        /// <summary>Queries if a given script exists.</summary>
        ///
        /// <param name="sha1Refs">A variable-length parameters list containing sha 1 references.</param>
        ///
        /// <returns>A byte[][].</returns>
        byte[][] ScriptExists(params byte[][] sha1Refs);
        /// <summary>Script flush.</summary>
		void ScriptFlush();
        /// <summary>Script kill.</summary>
		void ScriptKill();

        /// <summary>Script load.</summary>
        ///
        /// <param name="body">The body.</param>
        ///
        /// <returns>A byte[].</returns>
		byte[] ScriptLoad(string body);
	}
}