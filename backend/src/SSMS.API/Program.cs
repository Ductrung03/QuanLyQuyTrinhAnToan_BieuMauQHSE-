using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SSMS.Infrastructure.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/ssms-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SSMS API - Ship Safety Management System",
        Version = "v1.0",
        Description = "API for managing QHSE procedures, submissions, and approvals",
        Contact = new OpenApiContact
        {
            Name = "LuckyBoiz",
            Email = "contact@ssms.com"
        }
    });

    // JWT Authentication configuration for Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:3001",
            "http://localhost:5173",
            "https://localhost:3000",
            "https://localhost:3001",
            "https://localhost:5173"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "SSMS-Default-Secret-Key-For-Development-Only-Change-In-Production";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SSMS-API",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "SSMS-Client",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Policy yêu cầu user thuộc cùng Unit
    options.AddPolicy("SameUnit", policy =>
        policy.Requirements.Add(new SSMS.Infrastructure.Identity.UnitRequirement(requireSameUnit: true)));

    // Policy cho Admin
    options.AddPolicy("AdminOnly", policy =>
        policy.Requirements.Add(new SSMS.Infrastructure.Identity.RoleRequirement("Admin")));

    // Policy cho Manager và Admin
    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.Requirements.Add(new SSMS.Infrastructure.Identity.RoleRequirement("Manager", "Admin")));
});

// Register Authorization Handlers
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, 
    SSMS.Infrastructure.Identity.UnitAuthorizationHandler>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, 
    SSMS.Infrastructure.Identity.RoleAuthorizationHandler>();

// Register Application Services
builder.Services.AddScoped<SSMS.Core.Interfaces.IUnitOfWork, SSMS.Infrastructure.Data.Repositories.UnitOfWork>();
builder.Services.AddScoped<SSMS.Infrastructure.Identity.MockAuthService>();
builder.Services.AddScoped<SSMS.Application.Services.IProcedureService, SSMS.Application.Services.ProcedureService>();
builder.Services.AddScoped<SSMS.Application.Services.ITemplateService, SSMS.Application.Services.TemplateService>();
builder.Services.AddScoped<SSMS.Application.Services.ISubmissionService, SSMS.Application.Services.SubmissionService>();
builder.Services.AddScoped<SSMS.Application.Services.IApprovalService, SSMS.Application.Services.ApprovalService>();

// Add HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SSMS API v1");
        options.RoutePrefix = "swagger";
    });
}

// Global Exception Handler Middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            Log.Error(ex, "Unhandled exception occurred");

            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = 500,
                Message = app.Environment.IsDevelopment()
                    ? ex.Message
                    : "An internal server error occurred.",
                Details = app.Environment.IsDevelopment()
                    ? ex.StackTrace
                    : null
            });
        }
    });
});

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Request logging middleware
app.UseSerilogRequestLogging();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
}))
.WithName("HealthCheck")
.WithTags("System");

Log.Information("SSMS API is starting...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
