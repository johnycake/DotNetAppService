using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Azure.Identity;
using FluentValidation;
using DotNetCoreApp1.Models.Types;
using DotNetCoreApp1.Controllers.Types;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var keyValutUri = builder.Configuration["KeyValutConfig:KeyValutUri"] ?? "";

builder.Configuration.AddAzureKeyVault(new Uri(keyValutUri), new DefaultAzureCredential());

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"] ?? ""))
    };
});

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
    .MinimumLevel.Debug()
    .WriteTo.Console()
    );
}
else
{
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(new TelemetryConfiguration()
    {
        InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"],
    },
    TelemetryConverter.Traces));
}
builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:AzureDb"]);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("role_Id", "Admin");
    });
    options.AddPolicy("MustBeUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("role_Id", ["User", "Admin"]);
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddValidatorsFromAssemblyContaining<User>();
builder.Services.AddValidatorsFromAssemblyContaining<UserDto>();
builder.Services.AddValidatorsFromAssemblyContaining<Record>();
builder.Services.AddValidatorsFromAssemblyContaining<RecordDto>();
builder.Services.AddValidatorsFromAssemblyContaining<Book>();
builder.Services.AddValidatorsFromAssemblyContaining<BookDto>();
builder.Services.AddValidatorsFromAssemblyContaining<PasswordChange>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseForwardedHeaders();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
