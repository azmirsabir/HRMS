using System.Text.Json;
using System.Text.Json.Serialization;
using API;
using API.converters;
using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using API.Middlewares;
using Core.Domain.Users;
using Infrastructure.Database.Context;
using Infrastructure.Database.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    x.JsonSerializerOptions.Converters.Add(new NullableFloatJsonConverter());
    x.JsonSerializerOptions.ReferenceHandler = null;
    x.JsonSerializerOptions.WriteIndented = true;
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase,
        allowIntegerValues: true));
});

builder.Services.AddApplication();
builder.Services.AddMemoryCache();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();

    c.SupportNonNullableReferenceTypes();

    // c.SwaggerDoc("v1", new OpenApiInfo { Title = "HRMS.Api", Version = "v1" });
    // c.ResolveConflictingActions(apiDescription => apiDescription.First());
    // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    // {
    //     Name = "Authorization",
    //     Type = SecuritySchemeType.Http,
    //     Scheme = "Bearer",
    //     BearerFormat = "JWT",
    //     In = ParameterLocation.Header,
    //     Description = "Enter your valid token in the text input below."
    // });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        // {
        //     new OpenApiSecurityScheme
        //     {
        //         Reference = new OpenApiReference
        //         {
        //             Type = ReferenceType.SecurityScheme,
        //             Id = "Bearer"
        //         }
        //     },
        //     new string[] {}
        // }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("Production", builder =>
//     {
//         builder
//             .WithOrigins("https://test.test", "https://www.test.test")
//             .AllowAnyMethod()
//             .AllowAnyHeader();
//     });
// });



var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName=="Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/swagger-ui/custom.css"); // Custom CSS
    });
}


if (app.Environment.IsProduction())
{   
    app.UseCors("Production");
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MainContext>();

    context.Database.Migrate();

    if (!context.Users.Any())
    {
        await UserSeeder.SeedAsync(services);
        await DepartmentSeed.SeedAsync(services);
    }
}

app.UseCors("CorsPolicy");

app.UseStaticFiles();
app.UseMiddleware<TokenInValidationMiddleware>();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();








// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
//
// app.UseHttpsRedirection();

