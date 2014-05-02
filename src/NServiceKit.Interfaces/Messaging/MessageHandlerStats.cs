using System;
using System.Text;

namespace NServiceKit.Messaging
{
    /// <summary>Interface for message handler statistics.</summary>
    public interface IMessageHandlerStats
    {
        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>Gets the total number of messages processed.</summary>
        ///
        /// <value>The total number of messages processed.</value>
        int TotalMessagesProcessed { get; }

        /// <summary>Gets the total number of messages failed.</summary>
        ///
        /// <value>The total number of messages failed.</value>
        int TotalMessagesFailed { get; }

        /// <summary>Gets the total number of retries.</summary>
        ///
        /// <value>The total number of retries.</value>
        int TotalRetries { get; }

        /// <summary>Gets the total number of normal messages received.</summary>
        ///
        /// <value>The total number of normal messages received.</value>
        int TotalNormalMessagesReceived { get; }

        /// <summary>Gets the total number of priority messages received.</summary>
        ///
        /// <value>The total number of priority messages received.</value>
        int TotalPriorityMessagesReceived { get; }

        /// <summary>Gets the Date/Time of the last message processed.</summary>
        ///
        /// <value>The last message processed.</value>
        DateTime? LastMessageProcessed { get; }

        /// <summary>Adds stats.</summary>
        ///
        /// <param name="stats">The statistics to add.</param>
        void Add(IMessageHandlerStats stats);
    }

    /// <summary>A message handler statistics.</summary>
    public class MessageHandlerStats : IMessageHandlerStats
    {
        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessageHandlerStats class.</summary>
        ///
        /// <param name="name">The name.</param>
        public MessageHandlerStats(string name)
        {
            Name = name;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessageHandlerStats class.</summary>
        ///
        /// <param name="name">                         The name.</param>
        /// <param name="totalMessagesProcessed">       The total number of messages processed.</param>
        /// <param name="totalMessagesFailed">          The total number of messages failed.</param>
        /// <param name="totalRetries">                 The total number of retries.</param>
        /// <param name="totalNormalMessagesReceived">  The total number of normal messages received.</param>
        /// <param name="totalPriorityMessagesReceived">The total number of priority messages received.</param>
        /// <param name="lastMessageProcessed">         The last message processed.</param>
        public MessageHandlerStats(string name, int totalMessagesProcessed, int totalMessagesFailed, int totalRetries,
            int totalNormalMessagesReceived, int totalPriorityMessagesReceived, DateTime? lastMessageProcessed)
        {
            Name = name;
            TotalMessagesProcessed = totalMessagesProcessed;
            TotalMessagesFailed = totalMessagesFailed;
            TotalRetries = totalRetries;
            TotalNormalMessagesReceived = totalNormalMessagesReceived;
            TotalPriorityMessagesReceived = totalPriorityMessagesReceived;
            LastMessageProcessed = lastMessageProcessed;
        }

        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>Gets the Date/Time of the last message processed.</summary>
        ///
        /// <value>The last message processed.</value>
        public DateTime? LastMessageProcessed { get; private set; }

        /// <summary>Gets the total number of messages processed.</summary>
        ///
        /// <value>The total number of messages processed.</value>
        public int TotalMessagesProcessed { get; private set; }

        /// <summary>Gets the total number of messages failed.</summary>
        ///
        /// <value>The total number of messages failed.</value>
        public int TotalMessagesFailed { get; private set; }

        /// <summary>Gets the total number of retries.</summary>
        ///
        /// <value>The total number of retries.</value>
        public int TotalRetries { get; private set; }

        /// <summary>Gets the total number of normal messages received.</summary>
        ///
        /// <value>The total number of normal messages received.</value>
        public int TotalNormalMessagesReceived { get; private set; }

        /// <summary>Gets the total number of priority messages received.</summary>
        ///
        /// <value>The total number of priority messages received.</value>
        public int TotalPriorityMessagesReceived { get; private set; }

        /// <summary>Adds stats.</summary>
        ///
        /// <param name="stats">The statistics to add.</param>
        public virtual void Add(IMessageHandlerStats stats)
        {
            TotalMessagesProcessed += stats.TotalMessagesProcessed;
            TotalMessagesFailed += stats.TotalMessagesFailed;
            TotalRetries += stats.TotalRetries;
            TotalNormalMessagesReceived += stats.TotalNormalMessagesReceived;
            TotalPriorityMessagesReceived += stats.TotalPriorityMessagesReceived;
            if (LastMessageProcessed == null || stats.LastMessageProcessed > LastMessageProcessed)
                LastMessageProcessed = stats.LastMessageProcessed;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder("Stats for " + Name);
            sb.AppendLine("\n---------------");
            sb.AppendFormat("\nTotalNormalMessagesReceived: {0}", TotalNormalMessagesReceived);
            sb.AppendFormat("\nTotalPriorityMessagesReceived: {0}", TotalPriorityMessagesReceived);
            sb.AppendFormat("\nTotalProcessed: {0}", TotalMessagesProcessed);
            sb.AppendFormat("\nTotalRetries: {0}", TotalRetries);
            sb.AppendFormat("\nTotalFailed: {0}", TotalMessagesFailed);
            sb.AppendFormat("\nLastMessageProcessed: {0}", LastMessageProcessed.HasValue ? LastMessageProcessed.Value.ToString() : "");
            return sb.ToString();
        }
    }
}