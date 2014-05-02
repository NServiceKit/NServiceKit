#region (c)2009 Lokad - New BSD license

// Copyright (c) Lokad 2009 
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD licence

#endregion

#if !NET_4_0 && !SILVERLIGHT && !MONOTOUCH && !XBOX

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NServiceKit.Net30
{
    /// <summary>A system utility.</summary>
    public static class SystemUtil
    {
        internal static int GetHashCode(params object[] args)
        {
            unchecked
            {
                int result = 0;
                foreach (var o in args)
                {
                    result = (result * 397) ^ (o != null ? o.GetHashCode() : 0);
                }
                return result;
            }
        }		
    }

    /// <summary>Attribute for immutable.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ImmutableAttribute : Attribute
    {
    }

    /// <summary>
    /// Helper extensions for tuples
    /// </summary>
    public static class ExtendTuple
    {
        /// <summary>A Tuple&lt;T1,T2&gt; extension method that appends a tuple.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <param name="tuple">The tuple to act on.</param>
        /// <param name="item"> The item.</param>
        ///
        /// <returns>A Triple&lt;T1,T2,T3&gt;</returns>
        public static Triple<T1, T2, T3> Append<T1, T2, T3>(this Tuple<T1, T2> tuple, T3 item)
        {
            return Tuple.From(tuple.Item1, tuple.Item2, item);
        }

        /// <summary>A Tuple&lt;T1,T2,T3&gt; extension method that appends a tuple.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <typeparam name="T4">Generic type parameter.</typeparam>
        /// <param name="tuple">The tuple to act on.</param>
        /// <param name="item"> The item.</param>
        ///
        /// <returns>A Quad&lt;T1,T2,T3,T4&gt;</returns>
        public static Quad<T1, T2, T3, T4> Append<T1, T2, T3, T4>(this Tuple<T1, T2, T3> tuple, T4 item)
        {
            return Tuple.From(tuple.Item1, tuple.Item2, tuple.Item3, item);
        }

        /// <summary>An ICollection&lt;Pair&lt;T1,T2&gt;&gt; extension method that adds a tuple.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="first">     The first.</param>
        /// <param name="second">    The second.</param>
        public static void AddTuple<T1, T2>(this ICollection<Tuple<T1, T2>> collection, T1 first, T2 second)
        {
            collection.Add(Tuple.From(first, second));
        }

        /// <summary>An ICollection&lt;Pair&lt;T1,T2&gt;&gt; extension method that adds a tuple.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="first">     The first.</param>
        /// <param name="second">    The second.</param>
        public static void AddTuple<T1, T2>(this ICollection<Pair<T1, T2>> collection, T1 first, T2 second)
        {
            collection.Add(Tuple.From(first, second));
        }

        /// <summary>An ICollection&lt;Tuple&lt;T1,T2,T3&gt;&gt; extension method that adds a tuple.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="first">     The first.</param>
        /// <param name="second">    The second.</param>
        /// <param name="third">     The third.</param>
        public static void AddTuple<T1, T2, T3>(this ICollection<Tuple<T1, T2, T3>> collection, T1 first, T2 second, T3 third)
        {
            collection.Add(Tuple.From(first, second, third));
        }

        /// <summary>An ICollection&lt;Tuple&lt;T1,T2,T3,T4&gt;&gt; extension method that adds a tuple.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <typeparam name="T4">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="first">     The first.</param>
        /// <param name="second">    The second.</param>
        /// <param name="third">     The third.</param>
        /// <param name="fourth">    The fourth.</param>
        public static void AddTuple<T1, T2, T3, T4>(this ICollection<Tuple<T1, T2, T3, T4>> collection, T1 first, T2 second,
                                                    T3 third, T4 fourth)
        {
            collection.Add(Tuple.From(first, second, third, fourth));
        }

    }

    /// <summary>A pair.</summary>
    /// <typeparam name="TKey">  Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    [Serializable]
    [Immutable]
    public sealed class Pair<TKey, TValue> : Tuple<TKey, TValue>
    {
        /// <summary>Initializes a new instance of the NServiceKit.Net30.Pair&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        public Pair(TKey first, TValue second) : base(first, second) {}

        /// <summary>Gets the key.</summary>
        ///
        /// <value>The key.</value>
        public TKey Key
        {
            get { return Item1; }
        }

        /// <summary>Gets the value.</summary>
        ///
        /// <value>The value.</value>
        public TValue Value
        {
            get { return Item2; }
        }
    }

    /// <summary>A quad.</summary>
    /// <typeparam name="T1">Generic type parameter.</typeparam>
    /// <typeparam name="T2">Generic type parameter.</typeparam>
    /// <typeparam name="T3">Generic type parameter.</typeparam>
    /// <typeparam name="T4">Generic type parameter.</typeparam>
    [Serializable]
    [Immutable]
    public sealed class Quad<T1, T2, T3, T4> : Tuple<T1, T2, T3, T4>
    {
        /// <summary>Initializes a new instance of the NServiceKit.Net30.Quad&lt;T1, T2, T3, T4&gt; class.</summary>
        ///
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        /// <param name="fourth">The fourth.</param>
        public Quad(T1 first, T2 second, T3 third, T4 fourth) : base(first, second, third, fourth)
        {
        }
    }

    /// <summary>A triple.</summary>
    /// <typeparam name="T1">Generic type parameter.</typeparam>
    /// <typeparam name="T2">Generic type parameter.</typeparam>
    /// <typeparam name="T3">Generic type parameter.</typeparam>
    [Serializable]
    [Immutable]
    public sealed class Triple<T1, T2, T3> : Tuple<T1, T2, T3>
    {
        /// <summary>Initializes a new instance of the NServiceKit.Net30.Triple&lt;T1, T2, T3&gt; class.</summary>
        ///
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        public Triple(T1 first, T2 second, T3 third)
            : base(first, second, third)
        {
        }
    }

    /// <summary>A tuple.</summary>
    /// <typeparam name="T1">Generic type parameter.</typeparam>
    /// <typeparam name="T2">Generic type parameter.</typeparam>
    [Serializable]
    [Immutable]
    [DebuggerDisplay("({Item1},{Item2})")]
    public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
    {
        readonly T1 _item1;

        /// <summary>Gets the item 1.</summary>
        ///
        /// <value>The item 1.</value>
        public T1 Item1
        {
            get { return _item1; }
        }

        readonly T2 _item2;

        /// <summary>Gets the item 2.</summary>
        ///
        /// <value>The item 2.</value>
        public T2 Item2
        {
            get { return _item2; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Tuple&lt;T1, T2&gt; class.</summary>
        ///
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        public Tuple(T1 first, T2 second)
        {
            _item1 = first;
            _item2 = second;
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                throw new NullReferenceException("obj is null");
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Tuple<T1, T2>)) return false;
            return Equals((Tuple<T1, T2>)obj);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("({0},{1})", Item1, Item2);
        }

        /// <summary>Tests if this Tuple&lt;T1,T2&gt; is considered equal to another.</summary>
        ///
        /// <param name="obj">The tuple&lt; t 1, t 2&gt; to compare to this object.</param>
        ///
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public bool Equals(Tuple<T1, T2> obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Item1, Item1) && Equals(obj.Item2, Item2);
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return SystemUtil.GetHashCode(Item1, Item2);
        }

        /// <summary>Equality operator.</summary>
        ///
        /// <param name="left"> The left.</param>
        /// <param name="right">The right.</param>
        ///
        /// <returns>The result of the operation.</returns>
        public static bool operator ==(Tuple<T1, T2> left, Tuple<T1, T2> right)
        {
            return Equals(left, right);
        }

        /// <summary>Inequality operator.</summary>
        ///
        /// <param name="left"> The left.</param>
        /// <param name="right">The right.</param>
        ///
        /// <returns>The result of the operation.</returns>
        public static bool operator !=(Tuple<T1, T2> left, Tuple<T1, T2> right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>A tuple.</summary>
    /// <typeparam name="T1">Generic type parameter.</typeparam>
    /// <typeparam name="T2">Generic type parameter.</typeparam>
    /// <typeparam name="T3">Generic type parameter.</typeparam>
    [Serializable]
    [DebuggerDisplay("({Item1},{Item2},{Item3})")]
    public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
    {
        readonly T1 _item1;

        /// <summary>Gets the item 1.</summary>
        ///
        /// <value>The item 1.</value>
        public T1 Item1
        {
            get { return _item1; }
        }

        readonly T2 _item2;

        /// <summary>Gets the item 2.</summary>
        ///
        /// <value>The item 2.</value>
        public T2 Item2
        {
            get { return _item2; }
        }

        readonly T3 _item3;

        /// <summary>Gets the item 3.</summary>
        ///
        /// <value>The item 3.</value>
        public T3 Item3
        {
            get { return _item3; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Tuple&lt;T1, T2, T3&gt; class.</summary>
        ///
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        public Tuple(T1 first, T2 second, T3 third)
        {
            _item1 = first;
            _item2 = second;
            _item3 = third;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("({0},{1},{2})", Item1, Item2, Item3);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                throw new NullReferenceException("obj is null");
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Tuple<T1, T2, T3>)) return false;
            return Equals((Tuple<T1, T2, T3>)obj);
        }

        /// <summary>Tests if this Tuple&lt;T1,T2,T3&gt; is considered equal to another.</summary>
        ///
        /// <param name="obj">The tuple&lt; t 1, t 2, t 3&gt; to compare to this object.</param>
        ///
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public bool Equals(Tuple<T1, T2, T3> obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Item1, Item1) && Equals(obj.Item2, Item2) && Equals(obj.Item3, Item3);
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return SystemUtil.GetHashCode(Item1, Item2, Item3);
        }

        /// <summary>Equality operator.</summary>
        ///
        /// <param name="left"> The left.</param>
        /// <param name="right">The right.</param>
        ///
        /// <returns>The result of the operation.</returns>
        public static bool operator ==(Tuple<T1, T2, T3> left, Tuple<T1, T2, T3> right)
        {
            return Equals(left, right);
        }

        /// <summary>Inequality operator.</summary>
        ///
        /// <param name="left"> The left.</param>
        /// <param name="right">The right.</param>
        ///
        /// <returns>The result of the operation.</returns>
        public static bool operator !=(Tuple<T1, T2, T3> left, Tuple<T1, T2, T3> right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>A tuple.</summary>
    /// <typeparam name="T1">Generic type parameter.</typeparam>
    /// <typeparam name="T2">Generic type parameter.</typeparam>
    /// <typeparam name="T3">Generic type parameter.</typeparam>
    /// <typeparam name="T4">Generic type parameter.</typeparam>
    [Serializable]
    [DebuggerDisplay("({Item1},{Item2},{Item3},{Item4})")]
    [Immutable]
    public class Tuple<T1, T2, T3, T4> : IEquatable<Tuple<T1, T2, T3, T4>>
    {
        readonly T1 _item1;

        /// <summary>Gets the item 1.</summary>
        ///
        /// <value>The item 1.</value>
        public T1 Item1
        {
            get { return _item1; }
        }

        readonly T2 _item2;

        /// <summary>Gets the item 2.</summary>
        ///
        /// <value>The item 2.</value>
        public T2 Item2
        {
            get { return _item2; }
        }

        readonly T3 _item3;

        /// <summary>Gets the item 3.</summary>
        ///
        /// <value>The item 3.</value>
        public T3 Item3
        {
            get { return _item3; }
        }

        readonly T4 _item4;

        /// <summary>Gets the item 4.</summary>
        ///
        /// <value>The item 4.</value>
        public T4 Item4
        {
            get { return _item4; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Tuple&lt;T1, T2, T3, T4&gt; class.</summary>
        ///
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        /// <param name="fourth">The fourth.</param>
        public Tuple(T1 first, T2 second, T3 third, T4 fourth)
        {
            _item1 = first;
            _item2 = second;
            _item3 = third;
            _item4 = fourth;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", Item1, Item2, Item3, Item4);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                throw new NullReferenceException("obj is null");
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Tuple<T1, T2, T3, T4>)) return false;
            return Equals((Tuple<T1, T2, T3, T4>)obj);
        }

        /// <summary>Tests if this Tuple&lt;T1,T2,T3,T4&gt; is considered equal to another.</summary>
        ///
        /// <param name="obj">The tuple&lt; t 1, t 2, t 3, t 4&gt; to compare to this object.</param>
        ///
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public bool Equals(Tuple<T1, T2, T3, T4> obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Item1, Item1)
                && Equals(obj.Item2, Item2)
                    && Equals(obj.Item3, Item3)
                        && Equals(obj.Item4, Item4);
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return SystemUtil.GetHashCode(Item1, Item2, Item3, Item4);
        }

        /// <summary>Equality operator.</summary>
        ///
        /// <param name="left"> The left.</param>
        /// <param name="right">The right.</param>
        ///
        /// <returns>The result of the operation.</returns>
        public static bool operator ==(Tuple<T1, T2, T3, T4> left, Tuple<T1, T2, T3, T4> right)
        {
            return Equals(left, right);
        }

        /// <summary>Inequality operator.</summary>
        ///
        /// <param name="left"> The left.</param>
        /// <param name="right">The right.</param>
        ///
        /// <returns>The result of the operation.</returns>
        public static bool operator !=(Tuple<T1, T2, T3, T4> left, Tuple<T1, T2, T3, T4> right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>A tuple.</summary>
    public static class Tuple
    {
        /// <summary>Froms.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        ///
        /// <returns>A Pair&lt;T1,T2&gt;</returns>
        public static Pair<T1, T2> From<T1, T2>(T1 first, T2 second)
        {
            return new Pair<T1, T2>(first, second);
        }

        /// <summary>Creates a new Tuple&lt;T1,T2&gt;</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        ///
        /// <returns>A Tuple&lt;T1,T2&gt;</returns>
        public static Tuple<T1, T2> Create<T1, T2>(T1 first, T2 second)
        {
            return new Pair<T1, T2>(first, second);
        }

        /// <summary>Froms.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        ///
        /// <returns>A Triple&lt;T1,T2,T3&gt;</returns>
        public static Triple<T1, T2, T3> From<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            return new Triple<T1, T2, T3>(first, second, third);
        }

        /// <summary>Creates a new Tuple&lt;T1,T2,T3&gt;</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        ///
        /// <returns>A Tuple&lt;T1,T2,T3&gt;</returns>
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            return new Triple<T1, T2, T3>(first, second, third);
        }

        /// <summary>Froms.</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <typeparam name="T4">Generic type parameter.</typeparam>
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        /// <param name="fourth">The fourth.</param>
        ///
        /// <returns>A Quad&lt;T1,T2,T3,T4&gt;</returns>
        public static Quad<T1, T2, T3, T4> From<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            return new Quad<T1, T2, T3, T4>(first, second, third, fourth);
        }

        /// <summary>Creates a new Tuple&lt;T1,T2,T3,T4&gt;</summary>
        ///
        /// <typeparam name="T1">Generic type parameter.</typeparam>
        /// <typeparam name="T2">Generic type parameter.</typeparam>
        /// <typeparam name="T3">Generic type parameter.</typeparam>
        /// <typeparam name="T4">Generic type parameter.</typeparam>
        /// <param name="first"> The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third"> The third.</param>
        /// <param name="fourth">The fourth.</param>
        ///
        /// <returns>A Tuple&lt;T1,T2,T3,T4&gt;</returns>
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            return new Quad<T1, T2, T3, T4>(first, second, third, fourth);
        }
    }

}


#endif

