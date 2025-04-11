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
	client.BaseAddress = new Uri("http://localhost:5001/api/"); // AuthNode
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

// Routes for CoursesController
app.MapControllerRoute(
	name: "courses",
	pattern: "courses/{action=Index}/{id?}",
	defaults: new { controller = "Courses" }
);

// Routes for InstructorController
app.MapControllerRoute(
	name: "instructor",
	pattern: "{controller=Instructor}/{action=Index}/{id?}"
);

// Routes for StudentController
app.MapControllerRoute(
	name: "student",
	pattern: "{controller=Student}/{action=Index}/{id?}"
);

app.Run();
