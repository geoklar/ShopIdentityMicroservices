
# Shop Store

Before we dive into the course topics, let's review the fictional client application that your microservices will serve and which parts of the backend system architecture will be covered by this repository.

Imagine a store where the consumer can purchase different type of products for domestic use.

There will be a catalog of products that will allow the consumer to purchase items like vegetables, fruits or other similar.

These products have a price, and to purchase them, the consumer will need to hold the right amount of some sort of currency, which we will just call budget.

Once a consumer successfully purchases a product, it then goes into his shopping cart where it will be available for him to select for his final order.

Now, the client side of this application, the app or apps that run in the consumer's device, are already being built by your company's client team, so you don't have to worry about them.



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
- Add Razor Page support

```bash
builder.Services.AddRazorPages();
```

## SMTP Support


**Step 1:** Create a new class library project to use it as a common library:

```bash
dotnet new classlib -o Shop.Common
```

#### Command Breakdown
**`dotnet new`**

This is the general command used to create a new .NET project or file based on a specified template. It's part of the .NET CLI (Command-Line Interface) tools.

**`classlib`**

This specifies the type of project you want to create. In this case, **classlib** stands for "class library." A class library is a project that contains reusable code (like classes, methods, interfaces, etc.) that can be shared across multiple other projects, such as web applications, desktop apps, or other class libraries.

**`-o Shop.Common`**

The **-o** (short for --output) option specifies the directory where the new project will be created.
Shop.Common is the name of the directory that will be created. The project files and code for the class library will be placed in this directory. The name Shop.Common suggests that this class library is intended to hold common or shared code that could be used across different parts of the Shop application.

- Open into VS code

```bash
Code . -r Shop.Common
```

**Step 2:** Install the following nuget packages:

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity
dotnet add package SendGrid
```

**Step 3:** Create a new folder called Models and create a new file ApplicationUser.cs

```bash
md Models
cd Models
new-item ApplicationUser.cs
```
**Add the following code to ApplicationUser.cs file:**

```bash
using Microsoft.AspNetCore.Identity;
namespace Shop.Common.Models;
public class ApplicationUser : IdentityUser<Guid>
{
    public decimal Budget { get; set; }
}

```

**Step 4:** Create a new file ApplicationRole.cs

```bash
new-item ApplicationRole.cs
```

**Add the following code to ApplicationRole.cs file:**

```bash
using Microsoft.AspNetCore.Identity;
namespace Shop.Common.Models;
public class ApplicationRole : IdentityRole<Guid> {}
```
```bash
cd..
```

**Step 5:** Create a new folder called Services and create a new file AuthMessageSenderOptions.cs

```bash
md Services
cd Services
new-item AuthMessageSenderOptions.cs
```
**Add the following code to AuthMessageSenderOptions.cs file:**

```bash
namespace Shop.Common.Services
{
    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; }
    }
}

```
**Step 6:** Create a new file EmailSender.cs

```bash
new-item EmailSender.cs
```

**Add the following code to EmailSender.cs file:**

```bash
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shop.Common.Services;
namespace Play.Common.Services;
public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;
    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }
    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(Options.SendGridKey, subject, message, toEmail);
    }
    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("geoklar@hotmail.com", subject),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));
        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode 
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }
}
```
```bash
cd..
```

**Step 6:** Set-up user secrets for development 

```bash
dotnet user-secrets init
```

**Register SenGrid Key:**

```bash
dotnet user-secrets set SendGridKey SG.<#YPiKt74HTimHeGUMQQ0vrcg.aUMQQ0............#>
```
```bash
cd..
```

Secrets are stored on the following location:

`%APPDATA%/Microsoft/UserSecrets/<WebAppName-userSecretsId>`

**Step 7:** Build and create Shop.Common nuget package

```bash
dotnet build .\Shop.Common.csproj
```

```bash
dotnet pack .\Shop.Common.csproj -p:PackageVersion=1.0.0 -o ../Shop.Packages
```

**Step 8:** Configure local nuget source

```bash
dotnet nuget add source "<#AbolutePath#>\Play.Packages"
```
Nuget source information can be found on the following location:

`%appdata%\NuGet\NuGet.Config`

**Step 9:** Add Shop.Common package as dependency to Shop.Identity

```bash
dotnet add package Shop.Common
```

**Step 10:** Replace Shop.Identity.Models.ApplicationUser & Shop.Identity.Models.ApplicationRole references with the new created from Shop.Common.Models 

**Step 11:** Edit Program.cs file adding SMTP support

```bash
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
```

**Step 12:** Disable default account verification

- Edit /Areas/Identity/Pages/Account/RegisterConfirmation.cshtml.cs and update:

```bash
DisplayConfirmAccountLink = false
```


## Enable HTTPS and configure port

- Open and edit launchSettings.json (Ctrl+P):

```bash
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:53424",
      "sslPort": 44397
    }
  },
  "profiles": {
    "Play.Identity": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5214",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```


## Configure Debugging

- Create .vscode folder on the root project folder

```bash
md .vscode
cd .vscode
```
- Create launch.json file

```bash
new-item launch.json
```
- Add the following code to launch.json file

```bash
{
    "version": "0.2.0",
    "configurations": [
        {
            // Use IntelliSense to find out which attributes exist for C# debugging
            // Use hover for the description of the existing attributes
            // For further information visit https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger-launchjson.md
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            // If you have changed target frameworks, make sure to update the program path.
            "program": "${workspaceFolder}/bin/Debug/net8.0/Shop.Identity.dll",
            "args": [],
            "cwd": "${workspaceFolder}/",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickProcess}"
        }
    ]
}
```

- Create tasks.json file

```bash
new-item tasks.json
```
- Add the following code to tasks.json file

```bash
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/Shop.Identity.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        },
        {
            "label": "publish",
            "command": "dotnet",
            "type": "process",
            "args": [
                "publish",
                "${workspaceFolder}/Shop.Identity.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "watch",
            "command": "dotnet",
            "type": "process",
            "args": [
                "watch",
                "run",
                "${workspaceFolder}/Shop.Identity.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}
```

## Add .gitignore

- Create .gitignore file on the root project folder

```bash
new-item .gitignore
```

- Add the following code to .gitignore file

```bash
*.swp
*.*~
project.lock.json
.DS_Store
*.pyc
nupkg/

# Visual Studio Code
.vscode/

# Rider
.idea/

# Visual Studio
.vs/

# Fleet
.fleet/

# Code Rush
.cr/

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/
[Oo]ut/
msbuild.log
msbuild.err
msbuild.wrn

```
