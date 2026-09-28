using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CBSESchool.Data;
using CBSESchool.Models;
using CBSESchool.Repositories;
using CBSESchool.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=CBSESchoolDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity Authentication & Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Dependency Injection - Repositories & Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGradeCalculatorService, GradeCalculatorService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<INoticeService, NoticeService>();
builder.Services.AddScoped<IFeeService, FeeService>();

// Add Controllers with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed Database asynchronously on startup
await DbInitializer.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
