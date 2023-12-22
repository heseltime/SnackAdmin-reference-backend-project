using SnackAdmin.BusinessLogic;
using SnackAdmin.BusinessLogic.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Dal.Ado;
using SnackAdmin.Dtos;
using Dal.Common;
using SnackAdmin.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SnackAdmin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions
            .Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions
            .PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions
            .PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddXmlDataContractSerializerFormatters();

var configuration =
    new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

//IConnectionFactory connectionFactory = DefaultConnectionFactory.FromConfiguration(configuration, "SnackDbConnection");
//builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddSingleton<IConnectionFactory>(DefaultConnectionFactory.FromConfiguration(configuration, "SnackDbConnection"));


// Dao Services
builder.Services.AddSingleton<IRestaurantDao, AdoRestaurantDao>();
builder.Services.AddSingleton<IAddressDao, AdoAddressDao>();
builder.Services.AddSingleton<IOrderDao, AdoOrderDao>();
builder.Services.AddSingleton<IOrderItemDao, AdoOrderItemDao>();
builder.Services.AddSingleton<IMenuDao, AdoMenuDao>();
builder.Services.AddSingleton<IDeliveryCondition, AdoDeliveryConditionDao>();
builder.Services.AddSingleton<IOpeningHourDao, AdoOpeningHourDao>();

// Logic Services
builder.Services.AddSingleton<IRestaurantManagementLogic, RestaurantManagementLogic>();
builder.Services.AddSingleton<IOrderManagementLogic, OrderManagementLogic>();
builder.Services.AddSingleton<IMenuManagementLogic, MenuManagementLogic>();

builder.Services.AddSingleton<IBusinessManagementLogic, BusinessManagementLogic>();
builder.Services.AddSingleton<IBusinessRegistrationManagementLogic, BusinessRegistrationManagementLogic>();
builder.Services.AddSingleton<IAuthManagementLogic, AuthManagementLogic>();

builder.Services.AddAutoMapper(typeof(Program));

// Swagger
builder.Services.AddOpenApiDocument(settings =>
{
    settings.PostProcess = doc => doc.Info.Title = "Snacks API";
});


builder.Services.AddCors(builder =>
    builder.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

// Controllers
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});
builder.Services.AddTransient<WebHookController>();

builder.Services.AddControllers();

// Authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();

// Swagger
app.UseOpenApi();
app.UseSwaggerUi3(setting => setting.Path = "/swagger");

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();
