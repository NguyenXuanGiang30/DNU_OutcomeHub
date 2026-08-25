using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OutcomeHub.Api.Extensions;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddOutcomeHubSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OutcomeHub API (OBE Engine)",
                Version = "v1",
                Description = "Hệ thống Quản trị, Đo lường, Đánh giá và Cải tiến Chuẩn đầu ra theo OBE - Đại học Đại Nam.",
                Contact = new OpenApiContact
                {
                    Name = "OutcomeHub Development Team",
                    Email = "dev@outcomehub.dnu.edu.vn",
                },
            });

            // 1. JWT Bearer Definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Nhập 'Bearer {token}' để xác thực.",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });

            // 2. Custom Operation Filter for Dev Headers
            c.OperationFilter<DevHeadersOperationFilter>();
        });

        return services;
    }
}

public sealed class DevHeadersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Principal-Id",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Dev Test Actor Principal ID (e.g. 10000000-0000-7000-8000-000000000001)",
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" },
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Role-Name",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Dev Role Name (e.g. ADMIN, DEAN, LECTURER, STUDENT)",
            Schema = new OpenApiSchema { Type = "string" },
        });
    }
}
