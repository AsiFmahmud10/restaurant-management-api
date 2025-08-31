using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagement.Auth;
using ProductManagement.Config;
using ProductManagement.Db;
using ProductManagement.Exception;
using ProductManagement.Middilewire;
using ProductManagement.Permission;
using ProductManagement.Role;
using ProductManagement.Services.Email;
using ProductManagement.Token;
using ProductManagement.User;

var builder = WebApplication.CreateBuilder(args);
AuthSettings? authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();
if (authSettings != null)
{
    builder.Services.AddSingleton(authSettings);
}

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<TransactionMiddleware>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.AccessTokenSecretKey)),
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

builder.Services
    .AddHangfire(configuration => configuration.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();


builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }

    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    optionsBuilder.UseSeeding((dbContext, b) =>
    {
        // admin data seed
        var isAdminExisted = dbContext.Set<User>().FirstOrDefault(user => user.Email.Equals("super.admin@omuk.com")) !=
                             null;
        if (!isAdminExisted)
        {
            var superAdmin = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "super",
                LastName = "admin",
                Email = "super.admin@omuk.com",
                Password = "123"
            };

            var adminRole = new Role() { Id = Guid.NewGuid(), Name = "admin" };
            var adminPermission = new Permission() { Id = Guid.NewGuid(), Name = "admin_permission" };

            adminRole.Permissions.Add(adminPermission);
            superAdmin.Roles.Add(adminRole);
            dbContext.Set<User>().Add(superAdmin);
            dbContext.SaveChanges();
        }

        // customer role & permission seed
        var isCustomerRoleExist = dbContext.Set<Role>().FirstOrDefault(role => role.Name.Equals("customer")) !=
                                  null;
        if (!isCustomerRoleExist)
        {
            var customerPermission = new Permission() { Id = Guid.NewGuid(), Name = "customer_permission" };
            var customerRole = new Role() { Id = Guid.NewGuid(), Name = "customer" };
            customerRole.Permissions.Add(customerPermission);
            dbContext.Set<Role>().Add(customerRole);
            dbContext.SaveChanges();
        }
    });
});
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "Jwt Authorization header using the Bearer scheme...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();

app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
app.UseSwaggerUI();


app.UseRouting();
app.UseMiddleware<TransactionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();