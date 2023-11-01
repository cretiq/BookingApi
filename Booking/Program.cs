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