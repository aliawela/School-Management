using Microsoft.EntityFrameworkCore;
using StudentHomework.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<StudentHomeworkDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(StudentHomework.Application.MappingProfile));
builder.Services.AddScoped<StudentHomework.Application.Managers.CourseManager>();
builder.Services.AddScoped<StudentHomework.Application.Managers.StudentManager>();
builder.Services.AddScoped<StudentHomework.Domain.Repositories.ICourseRepository, StudentHomework.Infrastructure.Repositories.CourseRepository>();
builder.Services.AddScoped<StudentHomework.Domain.Repositories.IStudentRepository, StudentHomework.Infrastructure.Repositories.StudentRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    StudentHomework.Infrastructure.Data.DbInitializer.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
