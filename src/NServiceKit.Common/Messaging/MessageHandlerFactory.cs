using System;

namespace NServiceKit.Messaging
{
    /// <summary>A message handler factory.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class MessageHandlerFactory<T>
        : IMessageHandlerFactory
    {
        /// <summary>Will be a total of 3 attempts.</summary>
        public const int DefaultRetryCount = 2;
        private readonly IMessageService messageService;

        /// <summary>Gets or sets the request filter.</summary>
        ///
        /// <value>The request filter.</value>
        public Func<IMessage, IMessage> RequestFilter { get; set; }

        /// <summary>Gets or sets the response filter.</summary>
        ///
        /// <value>The response filter.</value>
        public Func<object, object> ResponseFilter { get; set; }

        /// <summary>Gets or sets the publish responses whitelist.</summary>
        ///
        /// <value>The publish responses whitelist.</value>
        public string[] PublishResponsesWhitelist { get; set; }

        private readonly Func<IMessage<T>, object> processMessageFn;
        private readonly Action<IMessage<T>, Exception> processExceptionFn;

        /// <summary>Gets or sets the number of retries.</summary>
        ///
        /// <value>The number of retries.</value>
        public int RetryCount { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessageHandlerFactory&lt;T&gt; class.</summary>
        ///
        /// <param name="messageService">  The message service.</param>
        /// <param name="processMessageFn">The process message function.</param>
        public MessageHandlerFactory(IMessageService messageService, Func<IMessage<T>, object> processMessageFn)
            : this(messageService, processMessageFn, null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Messaging.MessageHandlerFactory&lt;T&gt; class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="messageService">    The message service.</param>
        /// <param name="processMessageFn">  The process message function.</param>
        /// <param name="processExceptionEx">The process exception ex.</param>
        public MessageHandlerFactory(IMessageService messageService,
            Func<IMessage<T>, object> processMessageFn,
            Action<IMessage<T>, Exception> processExceptionEx)
        {
            if (messageService == null)
                throw new ArgumentNullException("messageService");

            if (processMessageFn == null)
                throw new ArgumentNullException("processMessageFn");

            this.messageService = messageService;
            this.processMessageFn = processMessageFn;
            this.processExceptionFn = processExceptionEx;
            this.RetryCount = DefaultRetryCount;
        }

        /// <summary>Handler, called when the create message.</summary>
        ///
        /// <returns>The new message handler.</returns>
        public IMessageHandler CreateMessageHandler()
        {
            if (this.RequestFilter == null && this.ResponseFilter == null)
            {
                return new MessageHandler<T>(messageService, processMessageFn, processExceptionFn, this.RetryCount)
                {
                    PublishResponsesWhitelist = PublishResponsesWhitelist,
                };
            }

            return new MessageHandler<T>(messageService, msg =>
                {
                    if (this.RequestFilter != null)
                        msg = (IMessage<T>)this.RequestFilter(msg);

                    var result = this.processMessageFn(msg);

                    if (this.ResponseFilter != null)
                        result = this.ResponseFilter(result);

                    return result;
                },
                processExceptionFn, this.RetryCount)
            {
                PublishResponsesWhitelist = PublishResponsesWhitelist,
            };
        }
    }
}