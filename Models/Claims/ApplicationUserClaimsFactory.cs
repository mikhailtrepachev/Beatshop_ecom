using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Beatshop.Models.Claims;

public class ApplicationUserClaimsFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
	public ApplicationUserClaimsFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
		: base(userManager, optionsAccessor)
	{
	}

protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
	{
		var identity = await base.GenerateClaimsAsync(user);

		identity.AddClaim(new Claim("Nickname", user.Nickname));

		return identity;
	}

}

