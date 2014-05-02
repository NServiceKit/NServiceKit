using System;
using System.Collections.Generic;
using System.Net;
using NServiceKit.Common;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Redis;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.Validation;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>A dto utilities.</summary>
    public static class DtoUtils
    {
        private static ILog Log = LogManager.GetLogger(typeof (DtoUtils));

        /// <summary>
        /// Naming convention for the ResponseStatus property name on the response DTO
        /// </summary>
        public const string ResponseStatusPropertyName = "ResponseStatus";

        /// <summary>A ValidationErrorResult extension method that converts a validationResult to a response status.</summary>
        ///
        /// <param name="exception">The exception to act on.</param>
        ///
        /// <returns>validationResult as the ResponseStatus.</returns>
        public static ResponseStatus ToResponseStatus(this Exception exception)
        {
            var responseStatusConverter = exception as IResponseStatusConvertible;
            if (responseStatusConverter != null)
            {
                return responseStatusConverter.ToResponseStatus();
            }

            var httpError = exception as IHttpError;
            return httpError != null
                ? CreateResponseStatus(httpError.ErrorCode, httpError.Message)
                : CreateResponseStatus(exception.GetType().Name, exception.Message);
        }

        /// <summary>A ValidationErrorResult extension method that converts a validationResult to a response status.</summary>
        ///
        /// <param name="validationException">The validationException to act on.</param>
        ///
        /// <returns>validationResult as the ResponseStatus.</returns>
        public static ResponseStatus ToResponseStatus(this ValidationError validationException)
        {
            return ResponseStatusUtils.CreateResponseStatus(validationException.ErrorCode, validationException.Message, validationException.Violations);
        }

        /// <summary>A ValidationErrorResult extension method that converts a validationResult to a response status.</summary>
        ///
        /// <param name="validationResult">The validationResult to act on.</param>
        ///
        /// <returns>validationResult as the ResponseStatus.</returns>
        public static ResponseStatus ToResponseStatus(this ValidationErrorResult validationResult)
        {
            return validationResult.IsValid
                ? CreateSuccessResponse(validationResult.SuccessMessage)
                : ResponseStatusUtils.CreateResponseStatus(validationResult.ErrorCode, validationResult.ErrorMessage, validationResult.Errors);
        }

        /// <summary>Creates success response.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>The new success response.</returns>
        public static ResponseStatus CreateSuccessResponse(string message)
        {
            return new ResponseStatus { Message = message };
        }

        /// <summary>Creates response status.</summary>
        ///
        /// <param name="errorCode">The error code.</param>
        ///
        /// <returns>The new response status.</returns>
        public static ResponseStatus CreateResponseStatus(string errorCode)
        {
            var errorMessage = errorCode.SplitCamelCase();
            return ResponseStatusUtils.CreateResponseStatus(errorCode, errorMessage, null);
        }

        /// <summary>Creates response status.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        ///
        /// <returns>The new response status.</returns>
        public static ResponseStatus CreateResponseStatus(string errorCode, string errorMessage)
        {
            return ResponseStatusUtils.CreateResponseStatus(errorCode, errorMessage, null);
        }

        /// <summary>Creates error response.</summary>
        ///
        /// <param name="errorCode">       The error code.</param>
        /// <param name="errorMessage">    Message describing the error.</param>
        /// <param name="validationErrors">The validation errors.</param>
        ///
        /// <returns>The new error response.</returns>
        public static object CreateErrorResponse(string errorCode, string errorMessage, IEnumerable<ValidationErrorField> validationErrors)
        {
            var responseStatus = ResponseStatusUtils.CreateResponseStatus(errorCode, errorMessage, validationErrors);
            var responseDto = CreateResponseDto(null, responseStatus);
            return new HttpError(responseDto, HttpStatusCode.BadRequest, errorCode, errorMessage);
        }

        /// <summary>Creates error response.</summary>
        ///
        /// <param name="request">        .</param>
        /// <param name="validationError">The validation error.</param>
        ///
        /// <returns>The new error response.</returns>
        public static object CreateErrorResponse(object request, ValidationErrorResult validationError)
        {
            var responseStatus = validationError.ToResponseStatus();

            var errorResponse = CreateErrorResponse(
                request,
                new ValidationError(validationError),
                responseStatus);

            return errorResponse;
        }

        /// <summary>Creates error response.</summary>
        ///
        /// <param name="request">       .</param>
        /// <param name="ex">            .</param>
        /// <param name="responseStatus">.</param>
        ///
        /// <returns>The new error response.</returns>
        public static object CreateErrorResponse(object request, Exception ex, ResponseStatus responseStatus)
        {
            var responseDto = CreateResponseDto(request, responseStatus);

            var httpError = ex as IHttpError;
            if (httpError != null)
            {
                if (responseDto != null)
                    httpError.Response = responseDto;

                return httpError;
            }

            var errorCode = ex.GetType().Name;
            var errorMsg = ex.Message;
            if (responseStatus != null)
            {
                errorCode = responseStatus.ErrorCode ?? errorCode;
                errorMsg = responseStatus.Message ?? errorMsg;
            }

            return new HttpError(responseDto, ex.ToStatusCode(), errorCode, errorMsg);
        }

        /// <summary>
        /// Create an instance of the service response dto type and inject it with the supplied responseStatus
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStatus"></param>
        /// <returns></returns>
        public static object CreateResponseDto(object request, ResponseStatus responseStatus)
        {
            // Predict the Response message type name
            var responseDtoType = WebRequestUtils.GetErrorResponseDtoType(request);
            var responseDto = responseDtoType.CreateInstance();
            if (responseDto == null)
                return null;

            // For faster serialization of exceptions, services should implement IHasResponseStatus
            var hasResponseStatus = responseDto as IHasResponseStatus;
            if (hasResponseStatus != null)
            {
                hasResponseStatus.ResponseStatus = responseStatus;
            }
            else
            {
                var responseStatusProperty = responseDtoType.GetProperty(ResponseStatusPropertyName);
                if (responseStatusProperty != null)
                {
                    // Set the ResponseStatus
                    ReflectionUtils.SetProperty(responseDto, responseStatusProperty, responseStatus);
                }
            }

            // Return an Error DTO with the exception populated
            return responseDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="request"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static object HandleException(IResolver iocResolver, object request, Exception ex)
        {
            if (EndpointHost.Config != null && EndpointHost.Config.ReturnsInnerException 
                && ex.InnerException != null && !(ex is IHttpError))
            {
                ex = ex.InnerException;
            }

            var responseStatus = ex.ToResponseStatus();

            if (EndpointHost.DebugMode)
            {
                // View stack trace in tests and on the client
                responseStatus.StackTrace = GetRequestErrorBody(request) + "\n" + ex;
            }

            Log.Error("ServiceBase<TRequest>::Service Exception", ex);

            if (iocResolver != null)
                LogErrorInRedisIfExists(iocResolver.TryResolve<IRedisClientsManager>(), request.GetType().Name, responseStatus);

            var errorResponse = CreateErrorResponse(request, ex, responseStatus);

            return errorResponse;
        }

        /// <summary>
        /// Service error logs are kept in 'urn:ServiceErrors:{ServiceName}'
        /// </summary>
        public const string UrnServiceErrorType = "ServiceErrors";

        /// <summary>
        /// Combined service error logs are maintained in 'urn:ServiceErrors:All'
        /// </summary>
        public const string CombinedServiceLogId = "All";

        /// <summary>Logs error in redis if exists.</summary>
        ///
        /// <param name="redisManager">  Manager for redis.</param>
        /// <param name="operationName"> Name of the operation.</param>
        /// <param name="responseStatus">.</param>
        public static void LogErrorInRedisIfExists(
            IRedisClientsManager redisManager, string operationName, ResponseStatus responseStatus)
        {
            //If Redis is configured, maintain rolling service error logs in Redis (an in-memory datastore)
            if (redisManager == null) return;
            try
            {
                //Get a thread-safe redis client from the client manager pool
                using (var client = redisManager.GetClient())
                {
                    //Get a client with a native interface for storing 'ResponseStatus' objects
                    var redis = client.GetTypedClient<ResponseStatus>();

                    //Store the errors in predictable Redis-named lists i.e. 
                    //'urn:ServiceErrors:{ServiceName}' and 'urn:ServiceErrors:All' 
                    var redisSeriviceErrorList = redis.Lists[UrnId.Create(UrnServiceErrorType, operationName)];
                    var redisCombinedErrorList = redis.Lists[UrnId.Create(UrnServiceErrorType, CombinedServiceLogId)];

                    //Append the error at the start of the service-specific and combined error logs.
                    redisSeriviceErrorList.Prepend(responseStatus);
                    redisCombinedErrorList.Prepend(responseStatus);

                    //Clip old error logs from the managed logs
                    const int rollingErrorCount = 1000;
                    redisSeriviceErrorList.Trim(0, rollingErrorCount);
                    redisCombinedErrorList.Trim(0, rollingErrorCount);
                }
            }
            catch (Exception suppressRedisException)
            {
                Log.Error("Could not append exception to redis service error logs", suppressRedisException);
            }
        }

        /// <summary>
        /// Override to provide additional/less context about the Service Exception. 
        /// By default the request is serialized and appended to the ResponseStatus StackTrace.
        /// </summary>
        public static string GetRequestErrorBody(object request)
        {
            var requestString = "";
            try
            {
                requestString = TypeSerializer.SerializeToString(request);
            }
            catch /*(Exception ignoreSerializationException)*/
            {
                //Serializing request successfully is not critical and only provides added error info
            }

            return String.Format("[{0}: {1}]:\n[REQUEST: {2}]", (request?? new object()).GetType().Name, DateTime.UtcNow, requestString);
        }
    }
}