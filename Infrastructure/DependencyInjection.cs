using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Abstractions.Data;
using Application.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Database.Context;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Users;
using Infrastructure.Authorizations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSetting>(configuration.GetSection("JWTSettings"));
            services.AddDatabase(configuration);
            services.AddIdentityUnitOfWork();
            services.AddAuthentication(configuration);
            services.AddAuthorization();
            services.AddServices();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen();
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            });
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<MainContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MainContext>()
                .AddDefaultTokenProviders();
        }

        private static IServiceCollection AddIdentityUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContext, MainContext>();
            return services;
        }

        private static void AddAuthorization(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

            // services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public static void  AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {

           
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Takeel Api", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            
        }

        private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JWTSettings:Audience"],
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWTSettings:Key"]!))
                };
            });
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.Configure<ImageProcessingSettings>(options =>
            {
                options.MaxImageWidth = 1920;
                options.MaxImageHeight = 1080;
                options.JpegQuality = 85;
                options.SupportedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            });
            
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<ISalaryCalculator, SalaryCalculator>();
        }
}