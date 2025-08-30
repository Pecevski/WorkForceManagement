using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Common;
using WorkforceManagement.Services.Interfaces;

namespace WorkforceManagement.WebApi.Configuration
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IAuthenticationService _authhenticationService;

        public PasswordValidator(IAuthenticationService authenticationService)
        {
            _authhenticationService = authenticationService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var loggedWihtEmail = false;
            var user = await _authhenticationService.FindByNameAsync(context.UserName);

            if (user == null)
            {
                loggedWihtEmail = true;
                user = await _authhenticationService.FindByEmailAsync(context.UserName);
            }

            if (user != null)
            {
                bool authResult = await _authhenticationService.ValidateUserCredentials(user, context.Password);

                if (authResult)
                {
                    List<string> roles = await _authhenticationService.GetUserRolesAsync(user);

                    if (!loggedWihtEmail && !roles.Contains(GlobalConstants.AdministratorRoleName))
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
                        return;
                    }
                    List<Claim> claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    context.Result = new GrantValidationResult(subject: user.Id, authenticationMethod: "password", claims: claims);
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
                }

                return;
            }
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
        }
    }
}