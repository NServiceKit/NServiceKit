using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceKit.Messaging
{
	public interface IMessageProducer
		: IDisposable
	{
		void Publish<T>(T messageBody);
		void Publish<T>(IMessage<T> message);
	}

}
