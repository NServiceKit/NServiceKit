using System;
using System.Text;

namespace NServiceKit.Messaging
{
	/// <summary>
	/// Util static generic class to create unique queue names for types
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class QueueNames<T>
	{
		static QueueNames()
		{
			var utf8 = new UTF8Encoding(false);

			Priority = "mq:" + typeof(T).Name + ".priorityq";
			PriorityBytes = utf8.GetBytes(Priority);
			In = "mq:" + typeof(T).Name + ".inq";
			InBytes = utf8.GetBytes(In);
			Out = "mq:" + typeof(T).Name + ".outq";
			OutBytes = utf8.GetBytes(Out);
			Dlq = "mq:" + typeof(T).Name + ".dlq";
			DlqBytes = utf8.GetBytes(Dlq);
		}

        /// <summary>Gets the priority.</summary>
        ///
        /// <value>The priority.</value>
		public static string Priority { get; private set; }

        /// <summary>Gets the priority bytes.</summary>
        ///
        /// <value>The priority bytes.</value>
		public static byte[] PriorityBytes { get; private set; }

        /// <summary>Gets the in.</summary>
        ///
        /// <value>The in.</value>
		public static string In { get; private set; }

        /// <summary>Gets the in bytes.</summary>
        ///
        /// <value>The in bytes.</value>
		public static byte[] InBytes { get; private set; }

        /// <summary>Gets the out.</summary>
        ///
        /// <value>The out.</value>
		public static string Out { get; private set; }

        /// <summary>Gets the out bytes.</summary>
        ///
        /// <value>The out bytes.</value>
		public static byte[] OutBytes { get; private set; }

        /// <summary>Gets the dlq.</summary>
        ///
        /// <value>The dlq.</value>
		public static string Dlq { get; private set; }

        /// <summary>Gets the dlq bytes.</summary>
        ///
        /// <value>The dlq bytes.</value>
		public static byte[] DlqBytes { get; private set; }
	}

	/// <summary>
	/// Util class to create unique queue names for runtime types
	/// </summary>
	public class QueueNames
	{
        /// <summary>The topic in.</summary>
		public static string TopicIn = "mq:topic:in";
        /// <summary>The topic out.</summary>
		public static string TopicOut = "mq:topic:out";
        /// <summary>The queue prefix.</summary>
		public static string QueuePrefix = "";

        /// <summary>Sets queue prefix.</summary>
        ///
        /// <param name="prefix">The prefix.</param>
		public static void SetQueuePrefix(string prefix)
		{
			TopicIn = prefix + "mq:topic:in";
			TopicOut = prefix + "mq:topic:out";
			QueuePrefix = prefix;
		}

		private readonly Type messageType;

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.QueueNames class.</summary>
        ///
        /// <param name="messageType">Type of the message.</param>
		public QueueNames(Type messageType)
		{
			this.messageType = messageType;
		}

        /// <summary>Gets the priority.</summary>
        ///
        /// <value>The priority.</value>
		public string Priority
		{
			get { return QueuePrefix + "mq:" + messageType.Name + ".priorityq"; }
		}

        /// <summary>Gets the in.</summary>
        ///
        /// <value>The in.</value>
		public string In
		{
			get { return QueuePrefix + "mq:" + messageType.Name + ".inq"; }
		}

        /// <summary>Gets the out.</summary>
        ///
        /// <value>The out.</value>
		public string Out
		{
			get { return QueuePrefix + "mq:" + messageType.Name + ".outq"; }
		}

        /// <summary>Gets the dlq.</summary>
        ///
        /// <value>The dlq.</value>
		public string Dlq
		{
			get { return QueuePrefix + "mq:" + messageType.Name + ".dlq"; }
		}
	}

}