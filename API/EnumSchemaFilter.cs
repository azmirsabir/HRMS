using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Only modify enums
        if (context.Type.IsEnum)
        {
            schema.Type = "integer";  // Set the type to integer
            schema.Enum.Clear(); // Clear the enum values
            foreach (var field in context.Type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.IsStatic)
                {
                    var enumValue = (int)field.GetValue(null);
                    schema.Enum.Add(new OpenApiInteger(enumValue));  // Add enum values as integers
                }
            }
        }
    }
}