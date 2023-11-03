using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Booking;
using Booking.DataAccess;
using Booking.Helper;
using Booking.Models;
using Booking.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Register();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

string? connectionString;

#region Add BookingSettings
builder.Configuration.AddJsonFile("bookingSettings.json", optional: true);
builder.Services.Configure<BookingSettings>(builder.Configuration.GetSection("BookingSettings"));
#endregion

#region Get Connection String

var isOnAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_WEB_ENVIRONMENT"));
if (isOnAzure)
{
    //Get KeyVaultSecrets
    var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
    var kvUri = "https://" + keyVaultName + ".vault.azure.net";
    var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
    var secret = await client.GetSecretAsync("AzureSqlConnectionPassword"); 
    connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
    connectionString = connectionString?.Replace("PasswordPlaceholder", secret.Value.Value);
}
else
{
    connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Default");
}
#endregion

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddIdentityCore<MyUser>().AddEntityFrameworkStores<AppDbContext>().AddApiEndpoints();
builder.Services.AddControllers();

if (isOnAzure)
{
    builder.Services.BuildServiceProvider().GetService<AppDbContext>()?.Database.Migrate();
}

builder.Services.AddScoped<IValidator<DateTime>, DateTimeValidator>();

var app = builder.Build();

app.MapIdentityApi<MyUser>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseHttpsRedirection();
app.Run();