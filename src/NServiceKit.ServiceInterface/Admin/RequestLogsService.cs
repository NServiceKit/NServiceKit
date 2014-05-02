using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.ServiceInterface.Admin
{
    /// <summary>A request logs.</summary>
    [DataContract]
    public class RequestLogs
    {
        /// <summary>Gets or sets the before seconds.</summary>
        ///
        /// <value>The before seconds.</value>
        [DataMember(Order=1)] public int? BeforeSecs { get; set; }

        /// <summary>Gets or sets the after seconds.</summary>
        ///
        /// <value>The after seconds.</value>
        [DataMember(Order=2)] public int? AfterSecs { get; set; }

        /// <summary>Gets or sets the IP address.</summary>
        ///
        /// <value>The IP address.</value>
        [DataMember(Order=3)] public string IpAddress { get; set; }

        /// <summary>Gets or sets the forwarded for.</summary>
        ///
        /// <value>The forwarded for.</value>
        [DataMember(Order=4)] public string ForwardedFor { get; set; }

        /// <summary>Gets or sets the identifier of the user authentication.</summary>
        ///
        /// <value>The identifier of the user authentication.</value>
        [DataMember(Order=5)] public string UserAuthId { get; set; }

        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
        [DataMember(Order=6)] public string SessionId { get; set; }

        /// <summary>Gets or sets the referer.</summary>
        ///
        /// <value>The referer.</value>
        [DataMember(Order=7)] public string Referer { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        [DataMember(Order=8)] public string PathInfo { get; set; }

        /// <summary>Gets or sets the identifiers.</summary>
        ///
        /// <value>The identifiers.</value>
        [DataMember(Order=9)] public long[] Ids { get; set; }

        /// <summary>Gets or sets the identifier of the before.</summary>
        ///
        /// <value>The identifier of the before.</value>
        [DataMember(Order=10)] public int? BeforeId { get; set; }

        /// <summary>Gets or sets the identifier of the after.</summary>
        ///
        /// <value>The identifier of the after.</value>
        [DataMember(Order=11)] public int? AfterId { get; set; }

        /// <summary>Gets or sets the has response.</summary>
        ///
        /// <value>The has response.</value>
        [DataMember(Order=12)] public bool? HasResponse { get; set; }

        /// <summary>Gets or sets the with errors.</summary>
        ///
        /// <value>The with errors.</value>
        [DataMember(Order=13)] public bool? WithErrors { get; set; }

        /// <summary>Gets or sets the skip.</summary>
        ///
        /// <value>The skip.</value>
        [DataMember(Order=14)] public int Skip { get; set; }

        /// <summary>Gets or sets the take.</summary>
        ///
        /// <value>The take.</value>
        [DataMember(Order=15)] public int? Take { get; set; }

        /// <summary>Gets or sets the enable session tracking.</summary>
        ///
        /// <value>The enable session tracking.</value>
        [DataMember(Order=16)] public bool? EnableSessionTracking { get; set; }

        /// <summary>Gets or sets the enable response tracking.</summary>
        ///
        /// <value>The enable response tracking.</value>
        [DataMember(Order=17)] public bool? EnableResponseTracking { get; set; }

        /// <summary>Gets or sets the enable error tracking.</summary>
        ///
        /// <value>The enable error tracking.</value>
        [DataMember(Order=18)] public bool? EnableErrorTracking { get; set; }

        /// <summary>Gets or sets the duration longer than.</summary>
        ///
        /// <value>The duration longer than.</value>
        [DataMember(Order=19)] public TimeSpan? DurationLongerThan { get; set; }

        /// <summary>Gets or sets the duration less than.</summary>
        ///
        /// <value>The duration less than.</value>
        [DataMember(Order=20)] public TimeSpan? DurationLessThan { get; set; }
    }

    /// <summary>A request logs response.</summary>
    [DataContract]
    public class RequestLogsResponse
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Admin.RequestLogsResponse class.</summary>
        public RequestLogsResponse()
        {
            this.Results = new List<RequestLogEntry>();
        }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        [DataMember(Order=1)] public List<RequestLogEntry> Results { get; set; }

        /// <summary>Gets or sets the usage.</summary>
        ///
        /// <value>The usage.</value>
        [DataMember(Order=2)] public Dictionary<string, string> Usage { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order=3)] public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A request logs service.</summary>
    [DefaultRequest(typeof(RequestLogs))]
    public class RequestLogsService : Service
    {
        private static readonly Dictionary<string, string> Usage = new Dictionary<string, string> {
            {"int BeforeSecs",      "Requests before elapsed time"},
            {"int AfterSecs",       "Requests after elapsed time"},
            {"string IpAddress",    "Requests matching Ip Address"},
            {"string ForwardedFor", "Requests matching Forwarded Ip Address"},
            {"string UserAuthId",   "Requests matching UserAuthId"},
            {"string SessionId",    "Requests matching SessionId"},
            {"string Referer",      "Requests matching Http Referer"},
            {"string PathInfo",     "Requests matching PathInfo"},
            {"int BeforeId",        "Requests before RequestLog Id"},
            {"int AfterId",         "Requests after RequestLog Id"},
            {"bool WithErrors",     "Requests with errors"},
            {"int Skip",            "Skip past N results"},
            {"int Take",            "Only look at last N results"},
            {"bool EnableSessionTracking",  "Turn On/Off Session Tracking"},
            {"bool EnableResponseTracking", "Turn On/Off Tracking of Responses"},
            {"bool EnableErrorTracking",    "Turn On/Off Tracking of Errors"},
            {"TimeSpan DurationLongerThan", "Requests with a duration longer than"},
            {"TimeSpan DurationLessThan", "Requests with a duration less than"},
        };

        /// <summary>Gets or sets the request logger.</summary>
        ///
        /// <value>The request logger.</value>
        public IRequestLogger RequestLogger { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(RequestLogs request)
        {
            if (RequestLogger == null)
                throw new Exception("No IRequestLogger is registered");

            RequiredRoleAttribute.AssertRequiredRoles(RequestContext, RequestLogger.RequiredRoles);

            if (request.EnableSessionTracking.HasValue)
                RequestLogger.EnableSessionTracking = request.EnableSessionTracking.Value;

            var now = DateTime.UtcNow;
            var logs = RequestLogger.GetLatestLogs(request.Take).AsQueryable();

            if (request.BeforeSecs.HasValue)
                logs = logs.Where(x => (now - x.DateTime) <= TimeSpan.FromSeconds(request.BeforeSecs.Value));
            if (request.AfterSecs.HasValue)
                logs = logs.Where(x => (now - x.DateTime) > TimeSpan.FromSeconds(request.AfterSecs.Value));
            if (!request.IpAddress.IsNullOrEmpty())
                logs = logs.Where(x => x.IpAddress == request.IpAddress);
            if (!request.UserAuthId.IsNullOrEmpty())
                logs = logs.Where(x => x.UserAuthId == request.UserAuthId);
            if (!request.SessionId.IsNullOrEmpty())
                logs = logs.Where(x => x.SessionId == request.SessionId);
            if (!request.Referer.IsNullOrEmpty())
                logs = logs.Where(x => x.Referer == request.Referer);
            if (!request.PathInfo.IsNullOrEmpty())
                logs = logs.Where(x => x.PathInfo == request.PathInfo);
            if (!request.Ids.IsEmpty())
                logs = logs.Where(x => request.Ids.Contains(x.Id));
            if (request.BeforeId.HasValue)
                logs = logs.Where(x => x.Id <= request.BeforeId);
            if (request.AfterId.HasValue)
                logs = logs.Where(x => x.Id > request.AfterId);
            if (request.WithErrors.HasValue)
                logs = request.WithErrors.Value
                    ? logs.Where(x => x.ErrorResponse != null)
                    : logs.Where(x => x.ErrorResponse == null);
            if (request.DurationLongerThan.HasValue)
                logs = logs.Where(x => x.RequestDuration > request.DurationLongerThan.Value);
            if (request.DurationLessThan.HasValue)
                logs = logs.Where(x => x.RequestDuration < request.DurationLessThan.Value);

            var results = logs.Skip(request.Skip).OrderByDescending(x => x.Id).ToList();

            return new RequestLogsResponse {
                Results = results,
                Usage = Usage,
            };
        }
    }
}