using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Linq;
using NServiceKit.Text;

namespace NServiceKit.Messaging
{
    /// <summary>A message extensions.</summary>
    public static class MessageExtensions
    {
        /// <summary>Convert this object into a string representation.</summary>
        ///
        /// <param name="bytes">The bytes to act on.</param>
        ///
        /// <returns>A string that represents this object.</returns>
        public static string ToString(byte[] bytes)
        {
#if !SILVERLIGHT 
            return System.Text.Encoding.UTF8.GetString(bytes);
#else
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
#endif
        }

        private static Dictionary<Type, ToMessageDelegate> ToMessageFnCache = new Dictionary<Type, ToMessageDelegate>();
        internal static ToMessageDelegate GetToMessageFn(Type type)
        {
            ToMessageDelegate toMessageFn;
            ToMessageFnCache.TryGetValue(type, out toMessageFn);

            if (toMessageFn != null) return toMessageFn;

            var genericType = typeof(MessageExtensions<>).MakeGenericType(type);
            var mi = genericType.GetPublicStaticMethod("ConvertToMessage");
            toMessageFn = (ToMessageDelegate)mi.MakeDelegate(typeof(ToMessageDelegate));

            Dictionary<Type, ToMessageDelegate> snapshot, newCache;
            do
            {
                snapshot = ToMessageFnCache;
                newCache = new Dictionary<Type, ToMessageDelegate>(ToMessageFnCache);
                newCache[type] = toMessageFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ToMessageFnCache, newCache, snapshot), snapshot));

            return toMessageFn;
        }

        /// <summary>A byte[] extension method that converts this object to a message.</summary>
        ///
        /// <param name="bytes"> The bytes to act on.</param>
        /// <param name="ofType">Type of the of.</param>
        ///
        /// <returns>The given data converted to an IMessage.</returns>
        public static IMessage ToMessage(this byte[] bytes, Type ofType)
        {
            var msgFn = GetToMessageFn(ofType);
            var msg = msgFn(bytes);
            return msg;
        }

        /// <summary>A byte[] extension method that converts the bytes to a message.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="bytes">The bytes to act on.</param>
        ///
        /// <returns>bytes as a Message&lt;T&gt;</returns>
        public static Message<T> ToMessage<T>(this byte[] bytes)
        {
            var messageText = ToString(bytes);
            return JsonSerializer.DeserializeFromString<Message<T>>(messageText);
        }

        /// <summary>An IMessage extension method that converts a message to the bytes.</summary>
        ///
        /// <param name="message">The message to act on.</param>
        ///
        /// <returns>message as a byte[].</returns>
        public static byte[] ToBytes(this IMessage message)
        {
            var serializedMessage = JsonSerializer.SerializeToString((object)message);
            return System.Text.Encoding.UTF8.GetBytes(serializedMessage);
        }

        /// <summary>An IMessage&lt;T&gt; extension method that converts a message to the bytes.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="message">The message to act on.</param>
        ///
        /// <returns>message as a byte[].</returns>
        public static byte[] ToBytes<T>(this IMessage<T> message)
        {
            var serializedMessage = JsonSerializer.SerializeToString(message);
            return System.Text.Encoding.UTF8.GetBytes(serializedMessage);
        }

        /// <summary>An IMessage extension method that converts a message to an in queue name.</summary>
        ///
        /// <param name="message">The message to act on.</param>
        ///
        /// <returns>message as a string.</returns>
        public static string ToInQueueName(this IMessage message)
        {
            var queueName = message.Priority > 0
                ? new QueueNames(message.Body.GetType()).Priority
                : new QueueNames(message.Body.GetType()).In;
            
            return queueName;
        }

        /// <summary>An IMessage&lt;T&gt; extension method that converts a message to an in queue name.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="message">The message to act on.</param>
        ///
        /// <returns>message as a string.</returns>
        public static string ToInQueueName<T>(this IMessage<T> message)
        {
            return message.Priority > 0
                ? QueueNames<T>.Priority
                : QueueNames<T>.In;
        }
    }

    internal delegate IMessage ToMessageDelegate(object param);

    internal static class MessageExtensions<T>
    {
        /// <summary>Converts the oBytes to a message.</summary>
        ///
        /// <param name="oBytes">The bytes.</param>
        ///
        /// <returns>The given data converted to a message.</returns>
        public static IMessage ConvertToMessage(object oBytes)
        {
            var bytes = (byte[]) oBytes;
            return bytes.ToMessage<T>();
        }
    }
}