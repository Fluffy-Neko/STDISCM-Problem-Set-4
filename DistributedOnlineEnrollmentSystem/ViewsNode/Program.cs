using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add MVC and views
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromHours(2);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Set the port for the ViewsNode to run on
builder.WebHost.UseUrls("http://localhost:5000");

builder.Services.AddHttpClient("AuthApi", client =>
{
	client.BaseAddress = new Uri("http://localhost:5001/api/");
});
builder.Services.AddHttpClient("BasicFacilitiesApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/api/");
});
builder.Services.AddHttpClient("InstructorApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5003/api/");
});
builder.Services.AddHttpClient("StudentApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5004/api/");
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Default route for HomeController
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
