using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var ocelotConfigFile =
    builder.Environment.IsProduction()
        ? "ocelot.container.json"
        : "ocelot.json";

builder.Configuration
    .AddJsonFile(ocelotConfigFile, optional: false, reloadOnChange: true);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services
    .AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseAuthentication();

await app.UseOcelot();

app.Run();