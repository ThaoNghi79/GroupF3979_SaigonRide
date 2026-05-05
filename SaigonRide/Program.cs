using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Services;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ──────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── DATABASE ─────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── SERVICES (Dependency Injection) ──────────────────────────
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IRevenueReportService, RevenueReportService>();

// ── SESSION (cho Auth sau này) ────────────────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ── MIDDLEWARE ────────────────────────────────────────────────
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

// ── ROUTES ───────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// ── AUTO MIGRATE ON STARTUP ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();