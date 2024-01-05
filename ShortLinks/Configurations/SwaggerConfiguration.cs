using Microsoft.OpenApi.Models;


namespace ShortLinks.Configurations;

public static class SwaggerConfiguration
{
    
    public static IServiceCollection ConfigureSwagger(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", CreateScheme());
            options.AddSecurityRequirement(CreateRequirement());
        });
    }
    
    private static OpenApiSecurityScheme CreateScheme()
    {
        return new OpenApiSecurityScheme()
        {
            Name = "JWT Bearer token",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Bearer token Authorization",
        };
    }
    
    private static OpenApiSecurityRequirement CreateRequirement()
    {
        return new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                },
                new string[] {}
            }
        };
    }
}