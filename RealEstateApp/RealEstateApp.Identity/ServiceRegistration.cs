using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Identity.Contexts;
using RealEstateApp.Identity.Entities;
using RealEstateApp.Identity.Services;
using System.Text;

namespace RealEstateApp.Identity;

public static class ServiceRegistration
{
    
    public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddIdentityInfrastructure(services, configuration, useJwtAsDefault: false);
    }
    
    
    public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useJwtAsDefault)
    {
        services.AddDbContext<IdentityContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JWTSettings");
        var key = jwtSettings["Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("JWT Key is not configured in appsettings.json. Please add JWTSettings:Key to your configuration.");
        }

        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json. Please add JWTSettings:Issuer to your configuration.");
        }

        if (string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT Audience is not configured in appsettings.json. Please add JWTSettings:Audience to your configuration.");
        }

        if (useJwtAsDefault)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"JWT Token validated for: {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"JWT Challenge: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };
            });
        }
        else
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(24);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministradorRole", policy => 
                policy.RequireRole("Administrador"));
            options.AddPolicy("RequireAgentRole", policy => 
                policy.RequireRole("Agente"));
            options.AddPolicy("RequireClientRole", policy => 
                policy.RequireRole("Cliente"));
            options.AddPolicy("RequireDeveloperRole", policy => 
                policy.RequireRole("Desarrollador"));
            
            options.AddPolicy("WebAppAccess", policy => 
                policy.RequireRole("Administrador", "Agente", "Cliente"));
            options.AddPolicy("ApiAccess", policy => 
                policy.RequireRole("Administrador", "Desarrollador"));
        });

        services.AddScoped<IUserStatsService, UserStatsService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IJwtService, JwtService>();
    }
}
