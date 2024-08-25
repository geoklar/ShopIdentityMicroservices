using Microsoft.EntityFrameworkCore;
using Shop.Catalog.Models;
using Shop.Common.Identity;
using Microsoft.OpenApi.Models;
using Shop.Catalog;
using Shop.Common;

var builder = WebApplication.CreateBuilder(args);

const string AllowedOriginSetting = "AllowedOrigin";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Add services to the container.
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.Read, policy =>
    {
        policy.RequireRole(Roles.Admin, Roles.Consumer);
        policy.RequireClaim("appscopes", Claims.Catalog_ReadAccess, Claims.Catalog_FullAccess);
    });
    options.AddPolicy(Policies.Write, policy =>
    {
        policy.RequireRole(Roles.Admin, Roles.Consumer);
        policy.RequireClaim("appscopes", Claims.Catalog_WriteAccess, Claims.Catalog_FullAccess);
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop.Catalog", Version = "v1"});
});

builder.Services.AddJwtBearerAuthentication();

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
