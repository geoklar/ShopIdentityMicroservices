using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Identity.Data;
using Shop.Identity;
using Shop.Common.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Shop.Common.Services;
using Play.Common.Services;
using Play.Identity.Settings;
using Shop.Identity.Settings;
using System.Configuration;
using Microsoft.OpenApi.Models;
using Shop.Identity.HostedServices;
using Shop.Identity.Controllers;

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

builder.Services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
                .AddInMemoryApiResources(identityServerSettings.ApiResources)
                .AddInMemoryClients(identityServerSettings.Clients)
                .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
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

builder.Services.AddAuthentication()
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
