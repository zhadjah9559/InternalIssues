using InternalIssues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser>
{   //Overload the constructor to access currently logged in user and IdentityOptions
    public AppUserClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
    {
    }
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        //when cookie is generated, add on these new claims
        var identity = await base.GenerateClaimsAsync(user);

        //Claims
        identity.AddClaim(new Claim("FullName", user.FullName));
        identity.AddClaim(new Claim("FirstName", user.FirstName));
        identity.AddClaim(new Claim("LastName", user.LastName));

        return identity;
    }
}