using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceKit.Messaging
{
    /// <summary>Interface for message producer.</summary>
	public interface IMessageProducer
		: IDisposable
	{
        /// <summary>Publishes the given message.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="messageBody">The message body.</param>
		void Publish<T>(T messageBody);

        /// <summary>Publishes the given message.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="message">The message.</param>
		void Publish<T>(IMessage<T> message);
	}

}
