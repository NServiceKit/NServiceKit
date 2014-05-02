using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NServiceKit.Messaging
{
    internal delegate IMessage MessageFactoryDelegate(object body);

    /// <summary>A message factory.</summary>
    public static class MessageFactory
    {
        static readonly Dictionary<Type, MessageFactoryDelegate> CacheFn
            = new Dictionary<Type, MessageFactoryDelegate>();

        /// <summary>Creates a new IMessage.</summary>
        ///
        /// <param name="response">The response.</param>
        ///
        /// <returns>An IMessage.</returns>
        public static IMessage Create(object response)
        {
            if (response == null) return null;
            var type = response.GetType();

            MessageFactoryDelegate factoryFn;
            lock (CacheFn) CacheFn.TryGetValue(type, out factoryFn);

            if (factoryFn != null)
                return factoryFn(response);

            var genericMessageType = typeof(Message<>).MakeGenericType(type);
#if NETFX_CORE
            var mi = genericMessageType.GetRuntimeMethods().First(p => p.Name.Equals("Create"));
            factoryFn = (MessageFactoryDelegate)mi.CreateDelegate(
                typeof(MessageFactoryDelegate));
#else
            var mi = genericMessageType.GetMethod("Create",
                BindingFlags.Public | BindingFlags.Static);
            factoryFn = (MessageFactoryDelegate)Delegate.CreateDelegate(
                typeof(MessageFactoryDelegate), mi);
#endif

            lock (CacheFn) CacheFn[type] = factoryFn;

            return factoryFn(response);
        }
    }

    /// <summary>A message.</summary>
    public class Message : IMessage
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the priority.</summary>
        ///
        /// <value>The priority.</value>
        public long Priority { get; set; }

        /// <summary>Gets or sets the retry attempts.</summary>
        ///
        /// <value>The retry attempts.</value>
        public int RetryAttempts { get; set; }

        /// <summary>Gets or sets the identifier of the reply.</summary>
        ///
        /// <value>The identifier of the reply.</value>
        public Guid? ReplyId { get; set; }

        /// <summary>Gets or sets the reply to.</summary>
        ///
        /// <value>The reply to.</value>
        public string ReplyTo { get; set; }

        /// <summary>Gets or sets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        public int Options { get; set; }

        /// <summary>Gets or sets the error.</summary>
        ///
        /// <value>The error.</value>
        public MessageError Error { get; set; }

        /// <summary>Gets or sets the body.</summary>
        ///
        /// <value>The body.</value>
        public object Body { get; set; }
    }

    /// <summary>
    /// Basic implementation of IMessage[T]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Message<T>
        : Message, IMessage<T>
    {
        /// <summary>Initializes a new instance of the NServiceKit.Messaging.Message&lt;T&gt; class.</summary>
        public Message()
        {
            this.Id = Guid.NewGuid();
            this.CreatedDate = DateTime.UtcNow;
            this.Options = (int)MessageOption.NotifyOneWay;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.Message&lt;T&gt; class.</summary>
        ///
        /// <param name="body">The body.</param>
        public Message(T body)
            : this()
        {
            Body = body;
        }

        /// <summary>Creates a new IMessage.</summary>
        ///
        /// <param name="oBody">The body.</param>
        ///
        /// <returns>An IMessage.</returns>
        public static IMessage Create(object oBody)
        {
            return new Message<T>((T)oBody);
        }

        /// <summary>Gets the body.</summary>
        ///
        /// <returns>The body.</returns>
        public T GetBody()
        {
            return (T)Body;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("CreatedDate={0}, Id={1}, Type={2}, Retry={3}",
                this.CreatedDate,
                this.Id.ToString("N"),
                typeof(T).Name,
                this.RetryAttempts);
        }

    }
}