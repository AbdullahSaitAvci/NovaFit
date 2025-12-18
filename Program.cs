using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;
using NovaFit.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Veri tabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Yapılandırması 
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Şifre kuralları
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; // TODO: Canlıya geçerken burayı düzelt
    options.Password.RequiredLength = 3;

    // Kullanıcı ayarları
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true; // Mail onayı açık
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Yol ve Çerez Ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Sahte Email Servisini Tanıtıyoruz
builder.Services.AddTransient<IEmailSender, EmailSender>();

// MVC ve Razor Pages Servisleri 
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Döngüye girdiğinde hata verme, döngüyü kes (IgnoreCycles)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddRazorPages();

// HuggingFace Servisini sisteme ekliyoruz
builder.Services.AddHttpClient<NovaFit.Services.HuggingFaceService>();

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

// Kimlik Doğrulama ve Yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Identity sayfaları için

// Rol ve Admin Kullanıcısı Oluşturma (Seeder)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Veritabanı yoksa oluşturur

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        // Roller
        var roles = new[] { "Admin", "Member", "Trainer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppRole { Name = role });
            }
        }

        // Admin Kullanıcısı
        var adminEmail = "g231210035@sakarya.edu.tr";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Sistem Yöneticisi",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "sau");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("HATA: Seed işlemi sırasında bir sorun oluştu: " + ex.Message);
    }
}

app.Run();