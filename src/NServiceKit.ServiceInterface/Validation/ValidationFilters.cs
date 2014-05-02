using System.Linq;
using NServiceKit.ServiceHost;
using NServiceKit.FluentValidation;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceInterface.Validation
{
    /// <summary>A validation filters.</summary>
    public static class ValidationFilters
    {
        /// <summary>Request filter.</summary>
        ///
        /// <param name="req">       The request.</param>
        /// <param name="res">       The resource.</param>
        /// <param name="requestDto">The request dto.</param>
        public static void RequestFilter(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            var validator = ValidatorCache.GetValidator(req, requestDto.GetType());
            if (validator == null) return;

            var validatorWithHttpRequest = validator as IRequiresHttpRequest;
            if (validatorWithHttpRequest != null)
                validatorWithHttpRequest.HttpRequest = req;

            var ruleSet = req.HttpMethod;
            var validationResult = validator.Validate(
                new ValidationContext(requestDto, null, new MultiRuleSetValidatorSelector(ruleSet)));

            if (validationResult.IsValid) return;

            var errorResponse = DtoUtils.CreateErrorResponse(
                requestDto, validationResult.ToErrorResult());

            var validationFeature = EndpointHost.GetPlugin<ValidationFeature>();
            if (validationFeature != null && validationFeature.ErrorResponseFilter != null)
            {
                errorResponse = validationFeature.ErrorResponseFilter(validationResult, errorResponse);
            }

            res.WriteToResponse(req, errorResponse);
        }
    }
}
