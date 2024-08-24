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
using IdentityServer.LdapExtension.Extensions;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer.LdapExtension;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityServer4;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var identitySettings = builder.Configuration.GetSection(nameof(IdentitySettings));
builder.Services.Configure<IdentitySettings>(identitySettings)
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

IdentityServerSettings identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
var identityServerLdap = builder.Configuration.GetSection(nameof(IdentityServerLdap));

builder.Services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
                .AddInMemoryApiResources(identityServerSettings.ApiResources)
                .AddInMemoryClients(identityServerSettings.Clients)
                .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
                .AddProfileService<ProfileService>()
                // [START of Usage of LDAP]
                // .AddLdapUsers<OpenLdapAppUser>(identityServerLdap, UserStore.InMemory)
                // [END of usage of LDAP]
                .AddDeveloperSigningCredential();



builder.Services.AddLocalApiAuthentication();
builder.Services.AddControllers();

builder.Services.AddHostedService<IdentitySeedHostedService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop.Identity", Version = "v1"});
});


var googleClientId = builder.Configuration["GoogleClientId"];
var googleClientSecret = builder.Configuration["GoogleClientSecret"];
var microsoftClientId = builder.Configuration["MicrosoftClientId"];
var microsoftClientSecret = builder.Configuration["MicrosoftClientSecret"];
var facebookClientId = builder.Configuration["FacebookClientId"];
var facebookClientSecret = builder.Configuration["FacebookClientSecret"];
ServiceSettings serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddAuthentication()
            .AddOpenIdConnect("aad", "Azure AD", options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;//SignoutScheme;

                options.Authority = "https://login.windows.net/829b600a-a85f-4f39-b5a3-a9e8a479642e";
                options.ClientId = "a01766da-2993-4a8a-a52b-db6da55d47e7";
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.CallbackPath = "/signin-aad";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            })
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
            })
            .AddMicrosoftAccount(options => {
                options.ClientId = microsoftClientId;
                options.ClientSecret = microsoftClientSecret;
            }).AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = facebookClientId;
                facebookOptions.AppSecret = facebookClientSecret;
            });


builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
                
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

app.MapApplicationUserEndpoints();

app.Run();
