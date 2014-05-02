using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.Serialization;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.ServiceInterface.Validation;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>A registration.</summary>
    [DataContract]
    public class Registration : IReturn<RegistrationResponse>
    {
        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order = 1)] public string UserName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        [DataMember(Order = 2)] public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        [DataMember(Order = 3)] public string LastName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        [DataMember(Order = 4)] public string DisplayName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        [DataMember(Order = 5)] public string Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
        [DataMember(Order = 6)] public string Password { get; set; }

        /// <summary>Gets or sets the automatic login.</summary>
        ///
        /// <value>The automatic login.</value>
        [DataMember(Order = 7)] public bool? AutoLogin { get; set; }

        /// <summary>Gets or sets the continue.</summary>
        ///
        /// <value>The continue.</value>
        [DataMember(Order = 8)] public string Continue { get; set; }
    }

    /// <summary>A registration response.</summary>
    [DataContract]
    public class RegistrationResponse
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RegistrationResponse class.</summary>
        public RegistrationResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
        [DataMember(Order = 1)] public string UserId { get; set; }

        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
        [DataMember(Order = 2)] public string SessionId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order = 3)] public string UserName { get; set; }

        /// <summary>Gets or sets URL of the referrer.</summary>
        ///
        /// <value>The referrer URL.</value>
        [DataMember(Order = 4)] public string ReferrerUrl { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order = 5)] public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A full registration validator.</summary>
    public class FullRegistrationValidator : RegistrationValidator
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.FullRegistrationValidator class.</summary>
        public FullRegistrationValidator()
        {
            RuleSet(ApplyTo.Post, () => {
                RuleFor(x => x.DisplayName).NotEmpty();
            });
        }
    }

    /// <summary>A registration validator.</summary>
    public class RegistrationValidator : AbstractValidator<Registration>
    {
        /// <summary>Gets or sets the user authentication repo.</summary>
        ///
        /// <value>The user authentication repo.</value>
        public IUserAuthRepository UserAuthRepo { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RegistrationValidator class.</summary>
        public RegistrationValidator()
        {
            RuleSet(ApplyTo.Post, () => {
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty().When(x => x.Email.IsNullOrEmpty());
                RuleFor(x => x.Email).NotEmpty().EmailAddress().When(x => x.UserName.IsNullOrEmpty());
                RuleFor(x => x.UserName)
                    .Must(x => UserAuthRepo.GetUserAuthByUserName(x) == null)
                    .WithErrorCode("AlreadyExists")
                    .WithMessage("UserName already exists")
                    .When(x => !x.UserName.IsNullOrEmpty());
                RuleFor(x => x.Email)
                    .Must(x => x.IsNullOrEmpty() || UserAuthRepo.GetUserAuthByUserName(x) == null)
                    .WithErrorCode("AlreadyExists")
                    .WithMessage("Email already exists")
                    .When(x => !x.Email.IsNullOrEmpty());
            });
            RuleSet(ApplyTo.Put, () => {
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
            });
        }
    }

    /// <summary>A registration service.</summary>
    [DefaultRequest(typeof(Registration))]
    public class RegistrationService : Service
    {
        /// <summary>Gets or sets the user authentication repo.</summary>
        ///
        /// <value>The user authentication repo.</value>
        public IUserAuthRepository UserAuthRepo { get; set; }

        /// <summary>Gets or sets the validate function.</summary>
        ///
        /// <value>The validate function.</value>
        public static ValidateFn ValidateFn { get; set; }

        /// <summary>Gets or sets the registration validator.</summary>
        ///
        /// <value>The registration validator.</value>
        public IValidator<Registration> RegistrationValidator { get; set; }

        private void AssertUserAuthRepo()
        {
            if (UserAuthRepo == null)
                throw new ConfigurationErrorsException("No IUserAuthRepository has been registered in your AppHost.");
        }

        /// <summary>
        /// Create new Registration
        /// </summary>
        public object Post(Registration request)
        {
            if (EndpointHost.RequestFilters == null
                || !EndpointHost.RequestFilters.Contains(ValidationFilters.RequestFilter)) //Already gets run
                RegistrationValidator.ValidateAndThrow(request, ApplyTo.Post);

            AssertUserAuthRepo();

            if (ValidateFn != null)
            {
                var validateResponse = ValidateFn(this, HttpMethods.Post, request);
                if (validateResponse != null) return validateResponse;
            }

            RegistrationResponse response = null;
            var session = this.GetSession();
            var newUserAuth = ToUserAuth(request);
            var existingUser = UserAuthRepo.GetUserAuth(session, null);

            var registerNewUser = existingUser == null;
            var user = registerNewUser
                ? this.UserAuthRepo.CreateUserAuth(newUserAuth, request.Password)
                : this.UserAuthRepo.UpdateUserAuth(existingUser, newUserAuth, request.Password);

            if (registerNewUser)
            {
                session.OnRegistered(this);
            }

            if (request.AutoLogin.GetValueOrDefault())
            {
                using (var authService = base.ResolveService<AuthService>())
                {
                    var authResponse = authService.Post(new Auth {
                        UserName = request.UserName ?? request.Email,
                        Password = request.Password,
                        Continue = request.Continue
                    });

                    if (authResponse is IHttpError)
                        throw (Exception)authResponse;

                    var typedResponse = authResponse as AuthResponse;
                    if (typedResponse != null)
                    {
                        response = new RegistrationResponse {
                            SessionId = typedResponse.SessionId,
                            UserName = typedResponse.UserName,
                            ReferrerUrl = typedResponse.ReferrerUrl,
                            UserId = user.Id.ToString(CultureInfo.InvariantCulture),
                        };
                    }
                }
            }

            if (response == null)
            {
                response = new RegistrationResponse {
                    UserId = user.Id.ToString(CultureInfo.InvariantCulture),
                    ReferrerUrl = request.Continue
                };
            }

            var isHtml = base.RequestContext.ResponseContentType.MatchesContentType(ContentType.Html);
            if (isHtml)
            {
                if (string.IsNullOrEmpty(request.Continue))
                    return response;

                return new HttpResult(response)
                {
                    Location = request.Continue
                };
            }

            return response;
        }

        /// <summary>Converts a request to a user authentication.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>request as an UserAuth.</returns>
        public UserAuth ToUserAuth(Registration request)
        {
            var to = request.TranslateTo<UserAuth>();
            to.PrimaryEmail = request.Email;
            return to;
        }

        /// <summary>
        /// Logic to update UserAuth from Registration info, not enabled on OnPut because of security.
        /// </summary>
        public object UpdateUserAuth(Registration request)
        {
            if (EndpointHost.RequestFilters == null 
                || !EndpointHost.RequestFilters.Contains(ValidationFilters.RequestFilter))
                RegistrationValidator.ValidateAndThrow(request, ApplyTo.Put);

            if (ValidateFn != null)
            {
                var response = ValidateFn(this, HttpMethods.Put, request);
                if (response != null) return response;
            }

            var session = this.GetSession();
            var existingUser = UserAuthRepo.GetUserAuth(session, null);
            if (existingUser == null)
            {
                throw HttpError.NotFound("User does not exist");
            }

            var newUserAuth = ToUserAuth(request);
            UserAuthRepo.UpdateUserAuth(newUserAuth, existingUser, request.Password);

            return new RegistrationResponse {
                UserId = existingUser.Id.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}