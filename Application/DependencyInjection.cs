using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Validations;
using Application.Abstractions.Behaviors;
using FluentValidation;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName!.StartsWith("EmployeeManagementSystem")).ToArray();
        
        services.AddMediatR(conf =>
        {
            // conf.RegisterServicesFromAssemblies(assemblies);
            conf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        // Register validators from all EmployeeMS assemblies
        // foreach (var assembly in assemblies)
        // {
        //     services.AddValidatorsFromAssembly(assembly);
        // }
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        
        return services;
    }
}