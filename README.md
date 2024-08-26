
## Create Identity Microservice

**Step 1:** Create a new web app project for our identity microservice run:

```bash
dotnet new webapp --auth Individual -uld -o Shop.Identity
```

#### Command Breakdown

**`dotnet new webapp`**

This command uses the .NET CLI (dotnet) to create a new ASP.NET Core Web Application project template (webapp). The webapp template is typically used to create a web application with Razor Pages.

**`--auth Individual`**

This option specifies the authentication mechanism to use in the project. **Individual** means that the application will be set up with individual user accounts authentication. This typically includes using ASP.NET Core Identity, which is a membership system that adds login functionality to your application. With this option, users can register, log in, reset passwords, etc.

**`-uld`**

This flag stands for **"Use Local DB."** It configures the project to use a local database (such as SQL Server Express or SQLite, depending on your system configuration) for storing user accounts and related data. When this flag is included, the generated project is set up to connect to a local development database.

**`-o Shop.Identity`**

The **-o** option specifies the output directory where the project will be created. In this case, Shop.Identity is the name of the folder that will be created to contain your new web application. If this directory doesn't exist, it will be created.

**Step 2:** Add Microsoft.AspNetCore.Identity nuget package to the project

```bash
cd Shop.Identity
dotnet add package Microsoft.AspNetCore.Identity
```

**Step 3:** Create a new folder called Models and create a new class ApplicationUser.cs

```bash
md Models
cd Models
new-item ApplicationUser.cs
```
**Add the following code to ApplicationUser.cs file:**

```bash
using Microsoft.AspNetCore.Identity;
namespace Shop.Identity.Models;
public class ApplicationUser : IdentityUser<Guid>
{
    public decimal Budget { get; set; }
}

```

**Step 4:** Create a new class ApplicationRole.cs

```bash
new-item ApplicationRole.cs
```

**Add the following code to ApplicationRole.cs file:**

```bash
using Microsoft.AspNetCore.Identity;
namespace Shop.Identity.Models;
public class ApplicationRole : IdentityRole<Guid> {}
```
```bash
cd..
```
**Step 5:** Scaffold Register, Login, LogOut, and RegisterConfirmation

**Install needed packages for scaffolding:**
```bash
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet tool install --global dotnet-aspnet-codegenerator
```

**Scaffold Account files:**

```bash
dotnet-aspnet-codegenerator identity -dc Shop.Identity.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation"
```

#### Command Breakdown
**`dotnet aspnet-codegenerator identity`**

This command invokes the ASP.NET Core Identity code generator. The aspnet-codegenerator is a CLI tool that helps scaffold or generate Identity UI pages, controllers, and other necessary components for managing user authentication and authorization.

**`-dc Shop.Identity.Data.ApplicationDbContext`**

The **-dc** option specifies the DbContext class to use for the Identity database operations.
In this case, Shop.Identity.Data.ApplicationDbContext refers to the ApplicationDbContext class, which is typically defined in your project to manage Identity-related data (like users, roles, etc.). This class inherits from IdentityDbContext and is configured to interact with the underlying database.

**`--files "Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation"`**

- The **--files** option specifies the Identity-related pages or views to be generated or scaffolded. The pages listed in this option will be created (or overwritten if they exist) in your project.

- The semicolon-separated list **"Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation"** indicates that you want to generate the following Identity pages:
    1. **Account.Register**: The registration page where users can sign up for an account.
    2. **Account.Login**: The login page where users can sign in.
    3. **Account.Logout**: The logout functionality that allows users to sign out.
    4. **Account.RegisterConfirmation**: A page that confirms successful registration and typically provides next steps (like verifying email).

**Step 6:** Replace IdentityUser with Applicationuser

Replace IdentityUser with ApplicationUser in the following files:
- _LoginPartial.cshtml
- Register.cshtml
- Login.cshtml.cs
- Logout.cshtml.cs
- Register.cshtml.cs
- RegisterConfirmationModel.cshtml.cs
- Program.cs

**Step 7:** Scaffold User controller

**Install needed packages for scaffolding:**
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.OpenApi
```

**Scaffold User Controller:**

```bash
dotnet-aspnet-codegenerator minimalapi -dc ApplicationDbContext -e UserController -m Shop.Identity.Models.ApplicationUser -outDir Controllers -namespace Shop.Identity.Controllers
```

#### Command Breakdown
**`dotnet-aspnet-codegenerator minimalapi`**

This command invokes the ASP.NET Core code generator to scaffold a minimal API. A minimal API is a simplified way to build HTTP APIs with ASP.NET Core, focusing on minimal setup and code.

**`-dc ApplicationDbContext`**

The **-dc** option specifies the DbContext class that will be used for data access.
ApplicationDbContext is the name of the DbContext class in your project that manages the connection to the database and performs operations related to your entities. This class typically inherits from DbContext or IdentityDbContext and is configured with your database context.

**`-e UserController`**

The **-e** option specifies the name of the controller to be generated or the entity that the minimal API will manage.
In this case, UserController is the name of the controller class that will be generated, which will handle API requests related to user data.

**`-m Shop.Identity.Models.
ApplicationUser`**

The **-m** option specifies the model class that the minimal API will be based on.
Shop.Identity.Models.ApplicationUser is the model class representing the user entity in your application. This class usually extends IdentityUser and includes additional properties specific to your application's users.

**`-outDir Controllers`**

The **-outDir** option specifies the output directory where the generated files will be placed.
Controllers is the directory where the generated UserController will be placed. If this directory doesn't exist, it will be created.

**`-namespace Shop.Identity.Controllers`**

The **-namespace** option specifies the namespace to use for the generated controller.
Shop.Identity.Controllers is the namespace that will be applied to the generated UserController. This is important for organizing and referencing your code correctly within the project.

**Step 8:** Create and run migrations

- Create migrations.
*Delete if exists any under folder Data\Migrations

```bash
dotnet ef migrations add Initialize -o Data\Migrations
```

- Run migrations to update database.

```bash
dotnet ef database update
```

**Step 9:** Create User DTO

```bash
md DTOs
cd DTOs
new-item UserDto.cs
```

**Add the following code to UserDto.cs file:**

```bash
using System.ComponentModel.DataAnnotations;
namespace Shop.Identity.DTOs
{
    public record UserDto(
        Guid Id,
        string Username,
        string Email,
        decimal Budget
    );
    public record UpdateUserDto(
        [Required][EmailAddress] string Email,
        [Range(0, 1000000)] decimal Budget
    );
}
```

**Step 10:** Create extension methods for UserController

**Create new folder Extensions and file UserDtoExtension.cs:**
```bash
md Extensions
cd Extensions
new-item ApplicationUserExtension.cs
```

**Add the following code to ApplicationUserExtension.cs file:**

```bash
namespace Shop.Identity.Extensions;
public static class ApplicationUserExtension
{
    public static UserDto AsUserDto(this ApplicationUser user)
    {
        return new UserDto
        (
            user.Id,
            user?.UserName,
            user?.Email,
            user.Budget
        );
    }
}
```
**Create new file UserDtoExtension.cs:**
```bash
new-item UserDtoExtension.cs
```

**Add the following code to UserDtoExtension.cs file:**

```bash
namespace Shop.Identity.Extensions;
public static class UserDtoExtension
{
    public static ApplicationUser AsApplicationUser(this UserDto user)
    {
        return new ApplicationUser
        {
            UserName = user.Username,
            Email = user.Email,
            Budget = user.Budget
        };
    }
}
```

**Step 11:** Update code to UserController.cs

```bash
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
```

**Step 12:** Add initial Budget to newly created user

- Update Register.cshtml.cs file

**Declare Budget variable**:

```bash
private const decimal StartingBudget = 100;
```

**Update OnPostAsync method**:

```bash
user.Budget = StartingBudget;
```

**Step 13:** Update code to Program.cs

- Add Swagger support

```bash
builder.Services.AddSwaggerGen(c =>
          {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Identity.Service", Version = "v1" });
          });
```

```bash
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Identity.Service v1"));

```

- Add controller route configuration

```bash
app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
        });
```
