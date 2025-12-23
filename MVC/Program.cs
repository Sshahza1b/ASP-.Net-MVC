//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using MVC.DataAccess.Data;
//using MVC.DataAccess.Repository;
//using MVC.DataAccess.Repository.IRepository;


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();
//builder.Services.AddDbContext<ApplicationDbContext>(options=> 
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IDbInitializer, DbInitializer>();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();
//// Cookie settings (Agar 500 error phir bhi aaye)
//builder.Services.ConfigureApplicationCookie(options => {
//    options.LoginPath = $"/Identity/Account/Login";
//    options.LogoutPath = $"/Identity/Account/Logout";
//    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
//});
////builder.Services.AddIdentity<IdentityUser, IdentityRole>()
////    .AddEntityFrameworkStores<ApplicationDbContext>();
//var app = builder.Build();


//app.MapRazorPages();


//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

//// SeedDatabase function ko call karein
//using (var scope = app.Services.CreateScope())
//{
//    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
//    dbInitializer.Initialize();
//}



//app.Run();


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository;
using MVC.DataAccess.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IDENTITY SETTINGS (Is line ko dhyan se dekhein, IdentityRole add kiya hai)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages(); // Identity ke liye zaroori hai

// Repository and DbInitializer Registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Login check karne ke liye
app.UseAuthorization();  // Role check karne ke liye

app.MapRazorPages(); // Logout/Login pages ke liye

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// DATABASE SEEDING (Ye project run hote hi Admin banayega)
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}

app.Run();