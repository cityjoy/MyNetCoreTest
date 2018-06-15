using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Identity.Services;

namespace User.Identity.Authentication
{
    public class SmSCodeValidator : IExtensionGrantValidator
    {
        private readonly IUserService userService;
        private readonly IAuthCodeService authCodeService;
        public SmSCodeValidator(IUserService userService, IAuthCodeService authCodeService)
        {
            this.userService = userService;
            this.authCodeService = authCodeService;
        }
        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var sms_code = context.Request.Raw["auth_code"];
            var validationResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(sms_code))
            {
                context.Result = validationResult;
                return;
            }
            if (!authCodeService.Validate(phone, sms_code))
            {
                context.Result = validationResult;
                return;

            }
            int userId = await userService.CheckOrCreate(phone);
            if (userId <= 0)
            {
                context.Result = validationResult;
                return;

            }

            context.Result = new GrantValidationResult(userId.ToString(), GrantType);
        }
    }
}
