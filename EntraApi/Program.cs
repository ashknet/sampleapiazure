using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EntraApi", Version = "v1" });

    var instance = configuration["AzureAd:Instance"]?.TrimEnd('/') ?? "https://login.microsoftonline.com";
    var tenantId = configuration["AzureAd:TenantId"] ?? string.Empty;

    if (!string.IsNullOrWhiteSpace(tenantId))
    {
        var authority = $"{instance}/{tenantId}/v2.0";
        var authorizationUrl = new Uri($"{authority}/oauth2/v2.0/authorize");
        var tokenUrl = new Uri($"{authority}/oauth2/v2.0/token");

        var scope = configuration["AzureAd:Swagger:Scope"];
        if (string.IsNullOrWhiteSpace(scope))
        {
            var audience = configuration["AzureAd:Audience"] ?? configuration["AzureAd:ClientId"] ?? "api://default";
            scope = $"{audience}/user_impersonation";
        }

        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = authorizationUrl,
                    TokenUrl = tokenUrl,
                    Scopes = new Dictionary<string, string>
                    {
                        { scope, "Access the API as a user" }
                    }
                }
            }
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new[] { scope }
            }
        });
    }
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(jwtOptions =>
    {
        configuration.Bind("AzureAd", jwtOptions);
        var audience = configuration["AzureAd:Audience"];
        var clientId = configuration["AzureAd:ClientId"];
        if (!string.IsNullOrWhiteSpace(audience) || !string.IsNullOrWhiteSpace(clientId))
        {
            jwtOptions.TokenValidationParameters.ValidAudiences = new[] { audience, clientId };
        }
    },
    identityOptions =>
    {
        configuration.Bind("AzureAd", identityOptions);
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<EntraApi.Data.AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EntraApi v1");
    var swaggerClientId = configuration["AzureAd:Swagger:ClientId"];
    if (!string.IsNullOrWhiteSpace(swaggerClientId))
    {
        c.OAuthClientId(swaggerClientId);
        c.OAuthUsePkce();
        var scope = configuration["AzureAd:Swagger:Scope"];
        if (!string.IsNullOrWhiteSpace(scope))
        {
            c.OAuthScopes(scope.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
