using OutcomeHub.Api.Extensions;
using OutcomeHub.Api.Middleware;
using OutcomeHub.Api.Services;
using OutcomeHub.Application;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Services & Context Accessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();

// 2. Layer Registrations
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Controllers & API Configuration
builder.Services.AddControllers();
builder.Services.AddOutcomeHubSwagger();

var app = builder.Build();

// 4. Global Exception & Security Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// 5. Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OutcomeHub API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger at app root URL
    });
}

app.MapControllers();

app.Run();

