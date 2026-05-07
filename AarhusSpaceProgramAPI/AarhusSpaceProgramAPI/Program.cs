using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgramAPI.Data;
using Scalar.AspNetCore;
using AarhusSpaceProgramAPI.Models;
using MongoDB.Driver;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;



var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});


// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"] ?? string.Empty);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin",
        cfg =>
        {
            cfg.AllowAnyOrigin();
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
        });
});

builder.Services.AddControllers(options =>
{
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
        (x) => $"The value '{x}' is invalid.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        (x) => $"The field {x} must be a number.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (x, y) => $"The value '{x}' is not valid for {y}.");
    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
        () => $"A value is required.");
});


builder.Services.AddIdentity<ApiUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
                options.DefaultScheme =
                    options.DefaultSignInScheme =
                        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["JWT:SigningKey"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExperimentCrud", policy => policy.RequireRole("Scientist", "Manager","true"));
    options.AddPolicy("GETOnly", policy => policy.RequireRole("Astronaut", "Manger", "Scientist","true"));
    options.AddPolicy("FullAccess", policy => policy.RequireRole("Manager","true"));
});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var addOpenApi = builder.Services.AddOpenApi();

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient("mongodb://mongodb:27017"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("SpaceProgramLogs");
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            return Task.CompletedTask;
        });
    });

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => { options.OpenApiRoutePattern = "/openapi/v1.json"; });
    }

    if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
        app.UseDeveloperExceptionPage();
    else
        app.UseExceptionHandler("/error");

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

// Minimal API
    app.MapGet("/error",
        [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
        (HttpContext context) =>
        {
            var exceptionHandler =
                context.Features.Get<IExceptionHandlerPathFeature>();

            var details = new ProblemDetails();
            details.Detail = exceptionHandler?.Error.Message;
            details.Extensions["traceId"] =
                System.Diagnostics.Activity.Current?.Id
                ?? context.TraceIdentifier;
            details.Type =
                "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            details.Status = StatusCodes.Status500InternalServerError;
            return Results.Problem(details);
        });

    app.MapGet("/error/test",
        [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
        () => { throw new Exception("test"); });

    app.MapGet("/cod/test",
        [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)]
        () =>
            Results.Text("<script>" +
                         "window.alert('Your client supports JavaScript!" +
                         "\\r\\n\\r\\n" +
                         $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
                         "\\r\\n" +
                         "Client time (UTC): ' + new Date().toISOString());" +
                         "</script>" +
                         "<noscript>Your client does not support JavaScript</noscript>",
                "text/html"));

    app.MapControllers().RequireCors("AnyOrigin");

    var retries = 0;
    while (retries < 20)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();
            SeedDb(context);

            Console.WriteLine("Database ready and seeded.");
            break;
        }
        catch (Exception ex)
        {
            retries++;
            Console.WriteLine($"Database not ready, retrying in 5 seconds... ({retries}/20)");
            Console.WriteLine(ex.Message);
            Thread.Sleep(5000);
        }
    }

    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApiUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedIdentityUsers(userManager, roleManager);
    }

    async Task SeedIdentityUsers(
        UserManager<ApiUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Astronaut", "Scientist", "Manager" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUser(
            userManager,
            email: "manager@test.com",
            fullName: "Manager Test",
            password: "Test1!",
            role: "Manager");

        await CreateUser(
            userManager,
            email: "scientist@test.com",
            fullName: "Scientist Test",
            password: "Test1!",
            role: "Scientist");

        await CreateUser(
            userManager,
            email: "astronaut@test.com",
            fullName: "Astronaut Test",
            password: "Test1!",
            role: "Astronaut");
    }

    async Task CreateUser(
        UserManager<ApiUser> userManager,
        string email,
        string fullName,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByNameAsync(email);

        if (existingUser != null)
        {
            return;
        }

        var user = new ApiUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
        else
        {
            throw new Exception(
                $"Could not create user {email}: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    void SeedDb(ApplicationDbContext context)
    {
        if (context.Astronauts.Any() || context.CelestialBodies.Any() || context.Missions.Any() ||
            context.Managers.Any() || context.Scientists.Any() || context.Rockets.Any())
        {
            return;
        }

        var earth = new CelestialBody
            { Name = "Earth", Distance = 2000, Composition = "Rock and acid", BodyType = "Planet" };
        var mars = new CelestialBody { Name = "Mars", Distance = 2570, Composition = "Red Rock", BodyType = "Planet" };
        var moon = new CelestialBody
            { Name = "Moon", Distance = 15000, Composition = "Moonrock", BodyType = "Satellite" };

        context.CelestialBodies.AddRange(earth, mars, moon);

        var pad1 = new LaunchPad { Location = "USA", MaxWeight = 50000, CurrentStatus = "Operational" };
        var pad2 = new LaunchPad { Location = "Europe", MaxWeight = 65000, CurrentStatus = "Operational" };

        context.LaunchPads.AddRange(pad1, pad2);

        var rocket1 = new Rocket { Model = "Falcon X", CrewCapacity = 5 };
        var rocket2 = new Rocket { Model = "StarLift", CrewCapacity = 3 };

        context.Rockets.AddRange(rocket1, rocket2);

        var scientist1 = new Scientist
            { Name = "Dr. Nova", HireDate = DateTime.Now.AddYears(-12), Title = "Doctor", Specialty = "Astrophysics" };
        var scientist2 = new Scientist
        {
            Name = "Dr. Quark", HireDate = DateTime.Now.AddYears(-7), Title = "Worse Doctor", Specialty = "Engineering"
        };

        context.Scientists.AddRange(scientist1, scientist2);

        var manager1 = new Manager
            { Name = "Alice Control", Department = "Landing", HireDate = DateTime.Now.AddYears(-15) };
        var manager2 = new Manager
            { Name = "Bob Command", Department = "Observation", HireDate = DateTime.Now.AddYears(-9) };

        context.Managers.AddRange(manager1, manager2);

        var mission1 = new Mission
        {
            MissionName = "Mars Exploration",
            LaunchDate = DateTime.Now.AddYears(+2),
            Duration = 175,
            Status = "Active",
            Type = "Observation",
            Rocket = rocket1,
            LaunchPad = pad2,
            Manager = manager1,
            TargetBody = mars
        };

        var mission2 = new Mission
        {
            MissionName = "Lunar Landing",
            LaunchDate = DateTime.Now.AddYears(+2),
            Duration = 265,
            Status = "Budgeting",
            Type = "Landing",
            Rocket = rocket2,
            LaunchPad = pad1,
            Manager = manager2,
            TargetBody = moon
        };

        context.Missions.AddRange(mission1, mission2);

        var astro1 = new Astronaut
        {
            Name = "John Star",
            HireDate = DateTime.Now.AddYears(-5),
            PayGrade = 1.2,
            Rank = "Commander",
            EXPInSim = 500,
            EXPInSpace = 200,
        };

        var astro2 = new Astronaut
        {
            Name = "Luna Sky",
            HireDate = DateTime.Now.AddYears(-3),
            PayGrade = 1.0,
            Rank = "Pilot",
            EXPInSim = 300,
            EXPInSpace = 120,
        };

        var astro3 = new Astronaut
        {
            Name = "Max Orbit",
            HireDate = DateTime.Now.AddYears(-2),
            PayGrade = 0.9,
            Rank = "Engineer",
            EXPInSim = 250,
            EXPInSpace = 80,
        };

        context.Astronauts.AddRange(astro1, astro2, astro3);
        context.SaveChanges();
    }

    try
    {

        app.Run();

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fatal error: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        Console.ReadLine();
    }
}
