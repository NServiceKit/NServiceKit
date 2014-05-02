using System;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Messaging
{
    /// <summary>Interface for message.</summary>
    public interface IMessage
        : IHasId<Guid>
    {
        /// <summary>Gets the created date.</summary>
        ///
        /// <value>The created date.</value>
        DateTime CreatedDate { get; }

        /// <summary>Gets or sets the priority.</summary>
        ///
        /// <value>The priority.</value>
        long Priority { get; set; }

        /// <summary>Gets or sets the retry attempts.</summary>
        ///
        /// <value>The retry attempts.</value>
        int RetryAttempts { get; set; }

        /// <summary>Gets or sets the identifier of the reply.</summary>
        ///
        /// <value>The identifier of the reply.</value>
        Guid? ReplyId { get; set; }

        /// <summary>Gets or sets the reply to.</summary>
        ///
        /// <value>The reply to.</value>
        string ReplyTo { get; set; }

        /// <summary>Gets or sets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        int Options { get; set; }

        /// <summary>Gets or sets the error.</summary>
        ///
        /// <value>The error.</value>
        MessageError Error { get; set; }

        /// <summary>Gets or sets the body.</summary>
        ///
        /// <value>The body.</value>
        object Body { get; set; }
    }

    /// <summary>Interface for message.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IMessage<T>
        : IMessage
    {
        /// <summary>Gets the body.</summary>
        ///
        /// <returns>The body.</returns>
        T GetBody();
    }
}