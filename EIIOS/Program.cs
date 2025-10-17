using EIIOS.Data;
using EIIOS.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(EIIOS.Resources.SharedResource));
    })
    .AddViewLocalization();

// Add Session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure MySQL Entity Framework
builder.Services.AddDbContext<EIIOSDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 3, 0))
    ));

// Add Localization Services
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var enUS = new CultureInfo("en-US");
    options.DefaultRequestCulture = new RequestCulture(enUS, new CultureInfo("lv-LV"));
    options.SupportedCultures = new[] { enUS };
    options.SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("lv-LV") };
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
    CultureInfo.DefaultThreadCurrentCulture = enUS;
    CultureInfo.DefaultThreadCurrentUICulture = enUS;
});

// Configure Antiforgery for JSON requests
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.AddScoped<EIIOSDataQuery>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<InventoryConsumptionService>();

var app = builder.Build();

// Seed the database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EIIOSDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Checking database and seeding initial data...");

        // Ensure database exists
        await context.Database.EnsureCreatedAsync();

        // Seed initial data (only adds data that doesn't exist)
        await context.SeedDataAsync();

        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add Session middleware (BEFORE routing!)
app.UseSession();

// Add Localization Middleware
app.UseRequestLocalization();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();