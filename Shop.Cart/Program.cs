using Microsoft.EntityFrameworkCore;
using Shop.Cart.Models;
using Shop.Common.Identity;
using Microsoft.OpenApi.Models;
using Shop.Common.Models;
using Shop.Common.Extensions;
using Shop.Common.Clients;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shop.Cart;
using Shop.Common;

var builder = WebApplication.CreateBuilder(args);

const string AllowedOriginSetting = "AllowedOrigin";
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<CartContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddJwtBearerAuthentication();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.Read, policy =>
    {
        policy.RequireRole(Roles.Admin, Roles.Consumer);
        policy.RequireClaim("appscopes", Claims.Cart_ReadAccess, Claims.Cart_FullAccess);
    });
    options.AddPolicy(Policies.Write, policy =>
    {
        policy.RequireRole(Roles.Admin, Roles.Consumer);
        policy.RequireClaim("appscopes", Claims.Cart_WriteAccess, Claims.Cart_FullAccess);
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop.Cart", Version = "v1"});
});

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
HttpClient<CatalogItem>.AddHttpClientHandler<CatalogItem>(builder.Services);
HttpClient<ApplicationUser>.AddHttpClientHandler<ApplicationUser>(builder.Services);
builder.Services.AddTransient<IHttpShopClient<CatalogItem>, HttpShopClient<CatalogItem>>();
builder.Services.AddTransient<IHttpShopClient<ApplicationUser>, HttpShopClient<ApplicationUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(option => {
            option.WithOrigins(builder?.Configuration[AllowedOriginSetting] ?? string.Empty)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
