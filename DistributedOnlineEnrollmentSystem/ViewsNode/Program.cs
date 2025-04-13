using Microsoft.EntityFrameworkCore;  
using Microsoft.AspNetCore.Authentication.JwtBearer;  
using Microsoft.IdentityModel.Tokens;  
using Microsoft.OpenApi.Models;  
using System.Text;  
using System.Net.Sockets;  

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

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("http://authnode/api/");
});
builder.Services.AddHttpClient("BasicFacilitiesApi", client =>
{
    client.BaseAddress = new Uri("http://basicfacilitiesnode/api/");
});
builder.Services.AddHttpClient("InstructorApi", client =>
{
    client.BaseAddress = new Uri("http://instructornode/api/");
});
builder.Services.AddHttpClient("StudentApi", client =>
{
    client.BaseAddress = new Uri("http://studentnode/api/");
});


// Add JWT authentication  
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
    // Global exception handler middleware  
    app.UseExceptionHandler(errorApp =>  
    {  
        errorApp.Run(async context =>  
        {  
            // Prevent infinite redirection loops  
            if (context.Request.Path.StartsWithSegments("/Home/Error"))  
            {  
                // If the request is already for /Home/Error, do not redirect  
                context.Response.StatusCode = 500;  
                await context.Response.WriteAsync("An error occurred while processing your request.");  
                return;  
            }  

            // Log the error (optional)  
            var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;  

            if (error != null)  
            {  
                // Handle specific exceptions (like node connectivity issues)  
                if (error is HttpRequestException || error is SocketException)  
                {  
                    // Redirect to the error page  
                    context.Response.Redirect("/Home/Error");  
                    return;  
                }  
            }  

            // Redirect to the generic error page for other exceptions  
            context.Response.Redirect("/Home/Error");  
        });  
    });  

    app.UseHsts();  
}  

app.UseHttpsRedirection();  
app.UseStaticFiles();  
app.UseRouting();  

app.UseSession();  
app.UseAuthentication();  
app.UseAuthorization();  

// Default route for HomeController  
app.MapControllerRoute(  
    name: "default",  
    pattern: "{controller=Home}/{action=Index}/{id?}");  

app.Run();