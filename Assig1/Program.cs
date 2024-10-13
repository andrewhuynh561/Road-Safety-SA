using Assig1;
using Microsoft.EntityFrameworkCore;

// Create Application
var builder = WebApplication.CreateBuilder(args);
Debug.SetActive(builder);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Set DB Context
builder.Services.AddDbContext<Assig1.Data.ExpiationsContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ExpiationsContext") ??
throw new InvalidOperationException("Connection String for Expiations DB not found")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseCors(b =>
{
    b.AllowAnyMethod();
    b.AllowAnyOrigin();
    b.AllowAnyHeader();
});
app.UseStaticFiles();

app.UseRouting();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
