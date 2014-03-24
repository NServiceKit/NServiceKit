using System;
using System.Net;
using System.Reflection;
using NServiceKit.Common.Utils;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
	/// <summary>
	/// Inherit from this class if you want to host your web services inside a 
	/// Console Application, Windows Service, etc.
	/// 
	/// Usage of HttpListener allows you to host webservices on the same port (:80) as IIS 
	/// however it requires admin user privillages.
	/// </summary>
	public abstract class AppHostHttpListenerBase 
		: HttpListenerBase
	{
		protected AppHostHttpListenerBase() {}

		protected AppHostHttpListenerBase(string serviceName, params Assembly[] assembliesWithServices)
			: base(serviceName, assembliesWithServices)
		{
			EndpointHostConfig.Instance.NServiceKitHandlerFactoryPath = null;
			EndpointHostConfig.Instance.MetadataRedirectPath = "metadata";
		}

		protected AppHostHttpListenerBase(string serviceName, string handlerPath, params Assembly[] assembliesWithServices)
			: base(serviceName, assembliesWithServices)
		{
			EndpointHostConfig.Instance.NServiceKitHandlerFactoryPath = string.IsNullOrEmpty(handlerPath)
				? null : handlerPath;			
			EndpointHostConfig.Instance.MetadataRedirectPath = handlerPath == null 
				? "metadata"
				: PathUtils.CombinePaths(handlerPath, "metadata");
		}

		protected override void ProcessRequest(HttpListenerContext context)
		{
			if (string.IsNullOrEmpty(context.Request.RawUrl)) return;

			var operationName = context.Request.GetOperationName();

			var httpReq = new HttpListenerRequestWrapper(operationName, context.Request);
			var httpRes = new HttpListenerResponseWrapper(context.Response);
			var handler = NServiceKitHttpHandlerFactory.GetHandler(httpReq);

			var NServiceKitHandler = handler as INServiceKitHttpHandler;
			if (NServiceKitHandler != null)
			{
				var restHandler = NServiceKitHandler as RestHandler;
				if (restHandler != null)
				{
					httpReq.OperationName = operationName = restHandler.RestPath.RequestType.Name;
				}
				NServiceKitHandler.ProcessRequest(httpReq, httpRes, operationName);
				httpRes.Close();
				return;
			}

			throw new NotImplementedException("Cannot execute handler: " + handler + " at PathInfo: " + httpReq.PathInfo);
		}

	}
}