using CorporateTrainingSystem.Domain.Entities;
using CorporateTrainingSystem.Infrastructure.Data;
using CorporateTrainingSystem.Infrastructure.Identity;   
using Microsoft.AspNetCore.Identity;                      
using Microsoft.EntityFrameworkCore;
using CorporateTrainingSystem.Domain.Interfaces;
using CorporateTrainingSystem.Infrastructure.Repositories;
using CorporateTrainingSystem.Application.Features.Courses.CreateCourse;
using CorporateTrainingSystem.Application.Features.Courses.ListCourses;
using FluentValidation;
using CorporateTrainingSystem.Application.Features.Employees.CreateEmployee;
using CorporateTrainingSystem.Application.Features.Employees.ListEmployees;
using CorporateTrainingSystem.Application.Features.Sessions.CreateSession;
using CorporateTrainingSystem.Application.Features.Sessions.ListSessions;
using CorporateTrainingSystem.Application.Features.Enrollments.EnrollEmployee;
using CorporateTrainingSystem.Application.Features.Enrollments.CancelEnrollment;
using CorporateTrainingSystem.Application.Features.Enrollments.ListEnrollments;
using CorporateTrainingSystem.Application.Features.Enrollments.GetTrainingHistory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddControllersWithViews();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("/Features/{1}/Views/{0}.cshtml");
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<CreateCourseHandler>();
builder.Services.AddScoped<ListCoursesHandler>();
builder.Services.AddScoped<IValidator<CreateCourseCommand>, CreateCourseValidator>();
builder.Services.AddScoped<CreateEmployeeHandler>();
builder.Services.AddScoped<ListEmployeesHandler>();
builder.Services.AddScoped<IValidator<CreateEmployeeCommand>, CreateEmployeeValidator>();
builder.Services.AddScoped<CreateSessionHandler>();
builder.Services.AddScoped<ListSessionsHandler>();
builder.Services.AddScoped<IValidator<CreateSessionCommand>, CreateSessionValidator>();
builder.Services.AddScoped<EnrollEmployeeHandler>();
builder.Services.AddScoped<CancelEnrollmentHandler>();
builder.Services.AddScoped<ListEnrollmentsHandler>();
builder.Services.AddScoped<GetTrainingHistoryHandler>();
builder.Services.AddScoped<IValidator<EnrollEmployeeCommand>, EnrollEmployeeValidator>();
builder.Services.AddScoped<IValidator<CancelEnrollmentCommand>, CancelEnrollmentValidator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();