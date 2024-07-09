using BookLibrary.Business.Extensions;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Repositories.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen; 
using Microsoft.OpenApi.Models;
using BookLibrary.API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.WriteTo.Console()
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Day,
         rollOnFileSizeLimit: true)
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog(Log.Logger);

var configuration = builder.Configuration;


builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddRepositories(configuration);
builder.Services.AddBusinessRules();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<AppSettings>(builder.Configuration);
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibrary API", Version = "v1" });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
