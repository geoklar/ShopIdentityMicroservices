
using System.Collections.Generic;
using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Common;
using Shop.Common.Models;
using Shop.Identity.Data;
using Shop.Identity.Settings;

namespace Shop.Identity.HostedServices;

public class IdentitySeedHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IdentitySettings _identitySettings;

    public IdentitySeedHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<IdentitySettings> identitySettings)
    {
        _identitySettings = identitySettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await CreateRoleIfNotExistsAsync(Roles.Admin, roleManager);
        await CreateRoleIfNotExistsAsync(Roles.Consumer, roleManager);

        if (!string.IsNullOrEmpty(_identitySettings.AdminUserEmail) && !string.IsNullOrEmpty(_identitySettings.AdminUserPassword))
        {
            var adminUser = await userManager.FindByEmailAsync(_identitySettings.AdminUserEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = _identitySettings.AdminUserEmail,
                    Email = _identitySettings.AdminUserEmail
                };

                await userManager.CreateAsync(adminUser, _identitySettings.AdminUserPassword);
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                await userManager.AddClaimsAsync(adminUser, new Claim[]
                { 
                    new Claim(Auditor.Cart, UserClaims.Cart_FullAccess), 
                    new Claim(Auditor.Catalog, UserClaims.Catalog_FullAccess)
                });
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task CreateRoleIfNotExistsAsync(string role, RoleManager<ApplicationRole> roleManager)
    {
        var roleExists = await roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }
    }
}

public class ProfileService : IProfileService
{
    protected UserManager<ApplicationUser> _userManager;
    private readonly IdentitySettings _identitySettings;

    public ProfileService(UserManager<ApplicationUser> userManager, IOptions<IdentitySettings> identitySettings)
    {
        _userManager = userManager;
        _identitySettings = identitySettings.Value;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var userId = context?.Subject?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {

                var adminUserRole = await _userManager.GetRolesAsync(user);
                IList<Claim> claims = await _userManager.GetClaimsAsync(user);

                if (claims?.Count > 0)
                    context?.IssuedClaims.AddRange(claims);

                if (adminUserRole.Count > 0)
                {
                    Claim roleClaim = new Claim (JwtClaimTypes.Role, adminUserRole?.FirstOrDefault() ?? string.Empty);
                    context?.IssuedClaims.Add(roleClaim);
                }
            }
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        //>Processing
        var user = await _userManager.GetUserAsync(context.Subject);
        
        context.IsActive = (user != null);
    }
}