using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Fga.Net.DependencyInjection;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Website.Api.Authorization;
using Website.Api.Features.IdentityManagement;
using Website.Api.Features.Library.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Arcticgizmo API", Version = "v1" });
    c.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = true;
    c.CustomSchemaIds(type => type.ToString());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            corsBuilder =>
            {
                if (builder.Environment.IsProduction())
                {
                    corsBuilder
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithOrigins(builder.Configuration["ClientUrl"]!);
                }
                else
                {
                    corsBuilder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                }

                corsBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Content-Disposition");
            });
    });

var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddOpenFgaClient(config =>
{
    config.ConfigureAuth0Fga(x =>
    {
        x.WithAuthentication(builder.Configuration["Auth0Fga:ClientId"]!, builder.Configuration["Auth0Fga:ClientSecret"]!);
    });

    config.SetStoreId(builder.Configuration["Auth0Fga:StoreId"]!);
});

builder.Services.AddOpenFgaMiddleware(config =>
{
    config.SetUserIdentifier("user", principal => principal.Identity!.Name!);
});

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, p => p.RequireAuthenticatedUser().AddFgaRequirement());

        foreach (var scope in WbesiteScopes.All())
        {
            options.AddPolicy(scope, p => p
                .RequireAuthenticatedUser()
                .AddRequirements(new ScopeAuthorizationRequirement(scope, domain))
                .RequireClaim("org_id", builder.Configuration["Auth0:OrgId"]!)
            );
        }
    });

builder.Services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();
builder.Services.AddSingleton<IAuth0ManagementApi, Auth0ManagementApi>();

builder.Services.Configure<LibraryDatabaseConfig>(builder.Configuration.GetSection("LibraryDatabase"));
builder.Services.AddSingleton<ILibraryService, LibraryService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
else
{
    app.UseHttpsRedirection();
}

app.UseCors();


app.MapHealthChecks("/healthz").AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
