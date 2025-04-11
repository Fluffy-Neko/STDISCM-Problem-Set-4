using OnlineEnrollmentSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext with MySQL connection
// builder.Services.AddDbContext<AppDbContext>(options =>
// 	options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri("http://api-machine:5001/api/");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();  // Enforces HTTP Strict Transport Security (HSTS) in production
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Serve static files such as CSS, JS, Images, etc.
app.UseRouting();

app.UseAuthorization();

// Map default controller route
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "courses",
	pattern: "{controller=Courses}/{action=Index}/{id?}"
);

app.Run();