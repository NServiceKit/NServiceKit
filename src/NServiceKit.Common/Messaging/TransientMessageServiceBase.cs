#if !SILVERLIGHT 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.Messaging
{
    /// <summary>A transient message service base.</summary>
    public abstract class TransientMessageServiceBase
        : IMessageService, IMessageHandlerDisposer
    {
        private bool isRunning;
        /// <summary>Will be a total of 3 attempts.</summary>
        public const int DefaultRetryCount = 2;

        /// <summary>Gets the number of retries.</summary>
        ///
        /// <value>The number of retries.</value>
        public int RetryCount { get; protected set; }

        /// <summary>Gets the request time out.</summary>
        ///
        /// <value>The request time out.</value>
        public TimeSpan? RequestTimeOut { get; protected set; }

        /// <summary>Gets the size of the pool.</summary>
        ///
        /// <value>The size of the pool.</value>
        public int PoolSize { get; protected set; } //use later

        /// <summary>Factory to create consumers and producers that work with this service.</summary>
        ///
        /// <value>The message factory.</value>
        public abstract IMessageFactory MessageFactory { get; }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.TransientMessageServiceBase class.</summary>
        protected TransientMessageServiceBase()
            : this(DefaultRetryCount, null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.TransientMessageServiceBase class.</summary>
        ///
        /// <param name="retryAttempts"> The retry attempts.</param>
        /// <param name="requestTimeOut">The request time out.</param>
        protected TransientMessageServiceBase(int retryAttempts, TimeSpan? requestTimeOut)
        {
            this.RetryCount = retryAttempts;
            this.RequestTimeOut = requestTimeOut;
        }

        private readonly Dictionary<Type, IMessageHandlerFactory> handlerMap
            = new Dictionary<Type, IMessageHandlerFactory>();

        private IMessageHandler[] messageHandlers;

        /// <summary>Registers the handler.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="processMessageFn">The process message function.</param>
        public void RegisterHandler<T>(Func<IMessage<T>, object> processMessageFn)
        {
            RegisterHandler(processMessageFn, null);
        }

        /// <summary>Registers the handler.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="processMessageFn">  The process message function.</param>
        /// <param name="processExceptionEx">The process exception ex.</param>
        public void RegisterHandler<T>(Func<IMessage<T>, object> processMessageFn,
            Action<IMessage<T>, Exception> processExceptionEx)
        {
            if (handlerMap.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Message handler has already been registered for type: " + typeof(T).Name);
            }

            handlerMap[typeof(T)] = CreateMessageHandlerFactory(processMessageFn, processExceptionEx);
        }

        /// <summary>Get Total Current Stats for all Message Handlers.</summary>
        ///
        /// <returns>The statistics.</returns>
        public IMessageHandlerStats GetStats()
        {
            var total = new MessageHandlerStats("All Handlers");
            messageHandlers.ToList().ForEach(x => total.Add(x.GetStats()));
            return total;
        }

        /// <summary>Get a list of all message types registered on this MQ Host.</summary>
        ///
        /// <value>A list of types of the registered.</value>
        public List<Type> RegisteredTypes
        {
            get { return handlerMap.Keys.ToList(); }
        }

        /// <summary>Get the status of the service. Potential Statuses: Disposed, Stopped, Stopping, Starting, Started.</summary>
        ///
        /// <returns>The status.</returns>
        public string GetStatus()
        {
            return isRunning ? "Started" : "Stopped";
        }

        /// <summary>Get a Stats dump.</summary>
        ///
        /// <returns>The statistics description.</returns>
        public string GetStatsDescription()
        {
            var sb = new StringBuilder("#MQ HOST STATS:\n");
            sb.AppendLine("===============");
            foreach (var messageHandler in messageHandlers)
            {
                sb.AppendLine(messageHandler.GetStats().ToString());
                sb.AppendLine("---------------");
            }
            return sb.ToString();
        }

        /// <summary>Creates message handler factory.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="processMessageFn">  The process message function.</param>
        /// <param name="processExceptionEx">The process exception ex.</param>
        ///
        /// <returns>The new message handler factory.</returns>
        protected IMessageHandlerFactory CreateMessageHandlerFactory<T>(
            Func<IMessage<T>, object> processMessageFn, 
            Action<IMessage<T>, Exception> processExceptionEx)
        {
            return new MessageHandlerFactory<T>(this, processMessageFn, processExceptionEx) {
                RetryCount = RetryCount,
            };
        }

        /// <summary>Start the MQ Host if not already started.</summary>
        public virtual void Start()
        {
            if (isRunning) return;
            isRunning = true;

            this.messageHandlers = this.handlerMap.Values.ToList().ConvertAll(
                x => x.CreateMessageHandler()).ToArray();

            using (var mqClient = MessageFactory.CreateMessageQueueClient())
            {
                foreach (var handler in messageHandlers)
                {
                    handler.Process(mqClient);
                }
            }

            this.Stop();
        }

        /// <summary>Stop the MQ Host if not already stopped.</summary>
        public virtual void Stop()
        {
            isRunning = false;
            messageHandlers = null;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            Stop();
        }

        /// <summary>Handler, called when the dispose message.</summary>
        ///
        /// <param name="messageHandler">The message handler.</param>
        public virtual void DisposeMessageHandler(IMessageHandler messageHandler)
        {
            lock (messageHandlers)
            {
                if (!isRunning) return;

                var allHandlersAreDisposed = true;
                for (var i = 0; i < messageHandlers.Length; i++)
                {
                    if (messageHandlers[i] == messageHandler)
                    {
                        messageHandlers[i] = null;
                    }
                    allHandlersAreDisposed = allHandlersAreDisposed
                        && messageHandlers[i] == null;
                }

                if (allHandlersAreDisposed)
                {
                    Stop();
                }
            }
        }
    }
}
#endif