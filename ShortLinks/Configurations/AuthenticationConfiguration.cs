using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace ShortLinks.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var secretKey = Guard.Against.NullOrEmpty(configuration["Credentials:Secret"]);        
        serviceCollection.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return serviceCollection;
    }
}