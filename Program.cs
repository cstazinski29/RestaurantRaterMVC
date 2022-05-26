using Microsoft.EntityFrameworkCore;
using RestaurantRaterMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Template from the swapi API modules
// builder.Services.AddHttpClient("swapi", client =>
// {
//     client.BaseAddress = new Uri("https://swapi.dev/api/");
// });

builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);

builder.Services.AddDbContext<RestaurantDbContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
