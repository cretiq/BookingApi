using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Booking;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Register();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();


var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
var kvUri = "https://" + keyVaultName + ".vault.azure.net";
var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

var secret = await client.GetSecretAsync("AzureSqlConnectionPassword");

Console.WriteLine("THIS IS SECRET!!!");
Console.WriteLine(secret);


//Configuration
var connectionString = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()
    .GetValue<string>("ConnectionStrings:Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));


builder.Services.AddIdentityCore<MyUser>().AddEntityFrameworkStores<AppDbContext>().AddApiEndpoints();
builder.Services.AddControllers();

builder.Services.BuildServiceProvider().GetService<AppDbContext>().Database.Migrate();

var app = builder.Build();

app.MapIdentityApi<MyUser>();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseHttpsRedirection();
app.Run();