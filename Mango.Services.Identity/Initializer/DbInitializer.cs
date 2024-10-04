using System.Security.Claims;
using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
        }
        else
        {
            return;
        }

        var adminUser = new ApplicationUser()
        {
            UserName = "Admin",
            Email = "admin@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "11111111",
            FirstName = "Ben",
            LastName = "Admin"
        };

        _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();;
        _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

        var tmp1 = _userManager.AddClaimsAsync(adminUser, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
            new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Admin)
        }).Result;
        
        var customerUser = new ApplicationUser()
        {
            UserName = "Customer",
            Email = "customer@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "11111111",
            FirstName = "Alex",
            LastName = "Customer"
        };

        _userManager.CreateAsync(customerUser, "Customer123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

        var tmp2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, customerUser.FirstName + " " + customerUser.LastName),
            new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Customer)
        }).Result;
    }

}