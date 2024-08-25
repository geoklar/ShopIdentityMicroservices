using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Identity.Data;
using Shop.Identity;
using Shop.Common.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Shop.Common.Services;
using Play.Common.Services;
using Microsoft.OpenApi.Models;
using Shop.Identity.HostedServices;
using Shop.Identity.Controllers;
using Shop.Common.Settings;
using Shop.Identity.Settings;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder?.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder?.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder?.Services.AddDatabaseDeveloperPageExceptionFilter();

var identitySettings = builder?.Configuration.GetSection(nameof(IdentitySettings));
if (identitySettings != null)
{
    builder?.Services.Configure<IdentitySettings>(identitySettings)
        .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
}

IdentityServerSettings? identityServerSettings = builder?.Configuration?.GetSection(nameof(IdentityServerSettings))?.Get<IdentityServerSettings>();

builder?.Services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryApiScopes(identityServerSettings?.ApiScopes ?? [])
                .AddInMemoryApiResources(identityServerSettings?.ApiResources ?? [])
                .AddInMemoryClients(identityServerSettings?.Clients ?? [])
                .AddInMemoryIdentityResources(identityServerSettings?.IdentityResources ?? [])
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();



builder?.Services.AddLocalApiAuthentication();
builder?.Services.AddControllers();

builder?.Services.AddHostedService<IdentitySeedHostedService>();

builder?.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop.Identity", Version = "v1"});
});


var googleClientId = builder?.Configuration["GoogleClientId"];
var googleClientSecret = builder?.Configuration["GoogleClientSecret"];
var microsoftClientId = builder?.Configuration["MicrosoftClientId"];
var microsoftClientSecret = builder?.Configuration["MicrosoftClientSecret"];
var facebookClientId = builder?.Configuration["FacebookClientId"];
var facebookClientSecret = builder?.Configuration["FacebookClientSecret"];
var azureClientId = builder?.Configuration["AzureClientId"];
var azureTenantId = builder?.Configuration["AzureTenantId"];
var azureClientSecretId = builder?.Configuration["AzureClientSecretId "];

ServiceSettings? serviceSettings = builder?.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder?.Services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.ClientId = googleClientId ?? string.Empty;
                options.ClientSecret = googleClientSecret ?? string.Empty;
            })
            .AddMicrosoftAccount(options => {
                options.ClientId = microsoftClientId ?? string.Empty;
                options.ClientSecret = microsoftClientSecret ?? string.Empty;
            }).AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = facebookClientId ?? string.Empty;
                facebookOptions.AppSecret = facebookClientSecret ?? string.Empty;
            })
            .AddOpenIdConnect("aad", "Azure AD", options =>
            {
                // options.Authority = "https://login.microsoftonline.com/common";
                options.Authority = $"https://login.windows.net/{azureTenantId}";
                options.ClientId = azureClientId;
                options.ClientSecret = azureClientSecretId;

                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.ResponseType = "id_token";
                options.CallbackPath = "/signin-aad";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });



builder?.Services.AddRazorPages();

builder?.Services.AddEndpointsApiExplorer();

builder?.Services.AddTransient<IEmailSender, EmailSender>();

if (builder?.Configuration != null)
    builder?.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
                
builder?.Services.AddSwaggerGen();

// builder?.Services.Configure<CookiePolicyOptions>(options =>
//     {
//         options.Secure = CookieSecurePolicy.Always;
//     });

var app = builder?.Build();

if (app != null)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Identity.Service v1"));
    };

    // app.UseCookiePolicy(new CookiePolicyOptions()
    // {
    //     MinimumSameSitePolicy = SameSiteMode.None
    // });

    // app.UseCookiePolicy();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseIdentityServer();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
                {
                    if (endpoints != null)
                    {
                        endpoints.MapControllers();
                        endpoints.MapRazorPages();
                    }
                });

    app.MapApplicationUserEndpoints();

    app.Run();
}
