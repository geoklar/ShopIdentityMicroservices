using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Shop.Identity.Data;
using Shop.Identity.Dtos;
using Shop.Common.Models;
using Shop.Identity.Extensions;
namespace Shop.Identity.Controllers;

	public static class UserController
	{
	    public static void MapApplicationUserEndpoints (this IEndpointRouteBuilder routes)
	    {
	        var group = routes.MapGroup("/api/user").WithTags(nameof(UserDto));
	        group.MapGet("/", async (ApplicationDbContext db) =>
	        {
	            return await db.Users.ToListAsync();
	        })
	        .WithName("GetUsers")
	        .WithOpenApi();
	        group.MapGet("/{id}", async Task<Results<Ok<UserDto>, NotFound>> (Guid id, ApplicationDbContext db) =>
	        {
	            return await db.Users.AsNoTracking()
	                .FirstOrDefaultAsync(model => model.Id == id)
	                is ApplicationUser model
	                    ? TypedResults.Ok(model.AsUserDto())
	                    : TypedResults.NotFound();
	        })
	        .WithName("GetUserById")
	        .WithOpenApi();
	        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, UserDto applicationUser, ApplicationDbContext db) =>
	        {
	            var affected = await db.Users
	                .Where(model => model.Id == id)
	                .ExecuteUpdateAsync(setters => setters
	                    .SetProperty(m => m.Budget, applicationUser.Budget)
	                    .SetProperty(m => m.Id, applicationUser.Id)
	                    .SetProperty(m => m.UserName, applicationUser.Username)
	                    .SetProperty(m => m.Email, applicationUser.Email)
	                    );
	            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
	        })
	        .WithName("UpdateUser")
	        .WithOpenApi();
	        group.MapPost("/", async (UserDto userDto, ApplicationDbContext db) =>
	        {
	            ApplicationUser user = userDto.AsApplicationUser();
	            db.Users.Add(user);
	            await db.SaveChangesAsync();
	            return TypedResults.Created($"/api/user/{user.Id}",user.AsUserDto());
	        })
	        .WithName("CreateApplicationUser")
	        .WithOpenApi();
	        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
	        {
	            var affected = await db.Users
	                .Where(model => model.Id == id)
	                .ExecuteDeleteAsync();
	            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
	        })
	        .WithName("DeleteUser")
	        .WithOpenApi();
	    }
	}

