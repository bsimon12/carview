using CarViewer.Data;
using CarViewer.Data.Services;
using CarViewer.Data.Services.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

const string DEFAULT_DB_CONNECTION_STRING = "Data Source = cars.db;";
var dbConnectionString = configuration.GetConnectionString("DefaultConnection") ?? DEFAULT_DB_CONNECTION_STRING;

builder.Services.AddDbContext<CarContext>(
    options => {
        options.UseSqlite(DEFAULT_DB_CONNECTION_STRING);
    }
);

builder.Services.AddTransient<ICarDataService, CarDataService>();
builder.Services.AddTransient<IVinService, VinService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
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

