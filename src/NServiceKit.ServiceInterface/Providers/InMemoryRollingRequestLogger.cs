using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NServiceKit.Common.Net30;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.ServiceModel;

namespace NServiceKit.ServiceInterface.Providers
{
    /// <summary>An in memory rolling request logger.</summary>
    public class InMemoryRollingRequestLogger : IRequestLogger
    {
        private static int requestId = 0;

        /// <summary>The default capacity.</summary>
        public const int DefaultCapacity = 1000;
        private readonly ConcurrentQueue<RequestLogEntry> logEntries = new ConcurrentQueue<RequestLogEntry>();
        readonly int capacity;

        /// <summary>Turn On/Off Session Tracking.</summary>
        ///
        /// <value>true if enable session tracking, false if not.</value>
        public bool EnableSessionTracking { get; set; }

        /// <summary>Turn On/Off Raw Request Body Tracking.</summary>
        ///
        /// <value>true if enable request body tracking, false if not.</value>
        public bool EnableRequestBodyTracking { get; set; }

        /// <summary>Turn On/Off Tracking of Responses.</summary>
        ///
        /// <value>true if enable response tracking, false if not.</value>
        public bool EnableResponseTracking { get; set; }

        /// <summary>Turn On/Off Tracking of Exceptions.</summary>
        ///
        /// <value>true if enable error tracking, false if not.</value>
        public bool EnableErrorTracking { get; set; }

        /// <summary>Limit access to /requestlogs service to role.</summary>
        ///
        /// <value>The required roles.</value>
        public string[] RequiredRoles { get; set; }

        /// <summary>Don't log requests of these types.</summary>
        ///
        /// <value>A list of types of the exclude request dtoes.</value>
        public Type[] ExcludeRequestDtoTypes { get; set; }

        /// <summary>Don't log request bodys for services with sensitive information. By default Auth and Registration requests are hidden.</summary>
        ///
        /// <value>A list of types of the hide request body for request dtoes.</value>
        public Type[] HideRequestBodyForRequestDtoTypes { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Providers.InMemoryRollingRequestLogger class.</summary>
        ///
        /// <param name="capacity">The capacity.</param>
        public InMemoryRollingRequestLogger(int? capacity = DefaultCapacity)
        {
            this.capacity = capacity.GetValueOrDefault(DefaultCapacity);
        }

        /// <summary>Log a request.</summary>
        ///
        /// <param name="requestContext"> The RequestContext.</param>
        /// <param name="requestDto">     Request DTO.</param>
        /// <param name="response">       Response DTO or Exception.</param>
        /// <param name="requestDuration">How long did the Request take.</param>
        public void Log(IRequestContext requestContext, object requestDto, object response, TimeSpan requestDuration)
        {
            var requestType = requestDto != null ? requestDto.GetType() : null;

            if (ExcludeRequestDtoTypes != null
                && requestType != null
                && ExcludeRequestDtoTypes.Contains(requestType))
                return;
                
            var entry = new RequestLogEntry {
                Id = Interlocked.Increment(ref requestId),
                DateTime = DateTime.UtcNow,
                RequestDuration = requestDuration,
            };

            var httpReq = requestContext != null ? requestContext.Get<IHttpRequest>() : null;
            if (httpReq != null)
            {
                entry.HttpMethod = httpReq.HttpMethod;
                entry.AbsoluteUri = httpReq.AbsoluteUri;
                entry.PathInfo = httpReq.PathInfo;
                entry.IpAddress = requestContext.IpAddress;
                entry.ForwardedFor = httpReq.Headers[HttpHeaders.XForwardedFor];
                entry.Referer = httpReq.Headers[HttpHeaders.Referer];
                entry.Headers = httpReq.Headers.ToDictionary();
                entry.UserAuthId = httpReq.GetItemOrCookie(HttpHeaders.XUserAuthId);
                entry.SessionId = httpReq.GetSessionId();
                entry.Items = httpReq.Items;
                entry.Session = EnableSessionTracking ? httpReq.GetSession() : null;
            }

            if (HideRequestBodyForRequestDtoTypes != null
                && requestType != null
                && !HideRequestBodyForRequestDtoTypes.Contains(requestType)) 
            {
                entry.RequestDto = requestDto;
                if (httpReq != null)
                {
                    entry.FormData = httpReq.FormData.ToDictionary();

                    if (EnableRequestBodyTracking)
                    {
                        entry.RequestBody = httpReq.GetRawBody();
                    }
                }
            }
            if (!response.IsErrorResponse()) {
                if (EnableResponseTracking)
                    entry.ResponseDto = response;
            }
            else {
                if (EnableErrorTracking)
                    entry.ErrorResponse = ToSerializableErrorResponse(response);
            }

            logEntries.Enqueue(entry);

            RequestLogEntry dummy;
            if (logEntries.Count > capacity)
                logEntries.TryDequeue(out dummy);
        }

        /// <summary>View the most recent logs.</summary>
        ///
        /// <param name="take">.</param>
        ///
        /// <returns>The latest logs.</returns>
        public List<RequestLogEntry> GetLatestLogs(int? take)
        {
            var requestLogEntries = logEntries.ToArray();			
            return take.HasValue 
                ? new List<RequestLogEntry>(requestLogEntries.Take(take.Value))
                : new List<RequestLogEntry>(requestLogEntries);
        }

        /// <summary>Converts a response to a serializable error response.</summary>
        ///
        /// <param name="response">The response.</param>
        ///
        /// <returns>response as an object.</returns>
        public static object ToSerializableErrorResponse(object response)
        {
            var errorResult = response as IHttpResult;
            if (errorResult != null)
                return errorResult.Response;

            var ex = response as Exception;
            return ex != null ? ex.ToResponseStatus() : null;
        }
    }
}