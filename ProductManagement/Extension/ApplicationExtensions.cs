using ProductManagement.Cart;
using ProductManagement.CartItem;
using ProductManagement.Category;
using ProductManagement.Product;

namespace ProductManagement.Extension;

using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Auth;
using Config;
using Db;
using Exception;
using Middilewire;
using Permission;
using Role;
using Services.Email;
using Token;
using User;

public static class ApplicationExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        AuthSettings? authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();
        if (authSettings != null)
        {
            builder.Services.AddSingleton(authSettings);
        }

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.AccessTokenSecretKey)),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

        return services;
    }

    public static IServiceCollection AddHangFireServices(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        return services
            .AddHangfire(configuration => configuration.UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
            ).AddHangfireServer();
    }

    public static IServiceCollection AddGlobalExceptionHandlerServices(this IServiceCollection services)
    {
        return services.AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    public static IServiceCollection AddMiddleware(this IServiceCollection services)
    {
        services.AddScoped<TransactionMiddleware>();
        return services;
    }

    public static IServiceCollection AddEmailServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        return services.AddScoped<IEmailService, EmailService>()
            .Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    }


    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IRoleService, RoleService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IPermissionService, PermissionService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<ICartService, CartService>()
            .AddScoped<ICartItemService,CartItemService>();

        services.AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<IPermissionRepository, PermissionRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITokenRepository, TokenRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<ICartRepository, CartRepository>()
            .AddScoped<ICartItemRepository, CartItemRepository>();
        
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
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

        return services;
    }

    public static IServiceCollection InitializeDatabase(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
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
                var isAdminExisted =
                    dbContext.Set<User>().FirstOrDefault(user => user.Email.Equals("super.admin@omuk.com")) !=
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

        return services;
    }
}