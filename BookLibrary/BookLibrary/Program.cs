using BookLibrary.Business.Extensions;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Repositories.Extensions;
using Serilog;
using Microsoft.OpenApi.Models;
using BookLibrary.API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using BookLibrary.API.Middleware;
using BookLibrary.Business.Mapper;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddRepositories(configuration);
builder.Services.AddBusinessRules();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
builder.Services.Configure<AuthenticationSettings>(configuration.GetSection("Authentication"));

var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
var generalMapper = new GeneralMapper();
generalMapper.Register(typeAdapterConfig);
builder.Services.AddSingleton<IMapper>(new Mapper(typeAdapterConfig));

builder.Services.AddCors(setup =>
{
    setup.AddPolicy(AllowWhitelistCorsPolicy.Name, AllowWhitelistCorsPolicy.Get(configuration["CorsWhitelist"].Split(',')));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],
        ValidateAudience = true,
        ValidAudience = configuration["Authentication:JwtBearer:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"]))
    };
});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Book Library API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AllowWhitelistCorsPolicy.Name);

app.UseMiddleware<Middleware>();

app.UseAuthentication(); // UseAuthentication should be called before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
