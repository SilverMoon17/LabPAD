using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity.Services;

public class ProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactoryFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ProfileService
    (IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactoryFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _userClaimsPrincipalFactoryFactory = userClaimsPrincipalFactoryFactory;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userManager.FindByIdAsync(sub);
        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactoryFactory.CreateAsync(user);

        List<Claim> claims = userClaims.Claims.ToList();

        claims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
        
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.FirstName));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.LastName));
        if (_userManager.SupportsUserRole)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            foreach (var rolename in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, rolename));
                if (_roleManager.SupportsRoleClaims)
                {
                    IdentityRole role = await _roleManager.FindByNameAsync(rolename);
                    if (role is not null)
                    {
                        claims.AddRange(await _roleManager.GetClaimsAsync(role));
                    }
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string sub = context.Subject.GetSubjectId();

        ApplicationUser user = await _userManager.FindByIdAsync(sub);

        context.IsActive = user is not null;
    }
}