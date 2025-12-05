using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;

var builder = WebApplication.CreateBuilder(args);

// Veri tabaný baðlantý dizesini yapýlandýrma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapýlandýrmasý
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Þifre kurallarý 
    options.Password.RequireDigit = false;           // Sayý zorunlu mu? Hayýr
    options.Password.RequireLowercase = false;       // Küçük harf zorunlu mu? Hayýr
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu mu? Hayýr
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (@,!) zorunlu mu? Hayýr
    options.Password.RequiredLength = 3;             // En az kaç karakter? 3 (Test kolaylýðý için)

    // Kullanýcý ayarlarý
    options.User.RequireUniqueEmail = true;          // Her email sadece bir kere kayýt olabilir
})
.AddEntityFrameworkStores<ApplicationDbContext>(); // Identity için veri tabaný deposu olarak ApplicationDbContext'i kullanmasý saðlanýr



// Add services to the container.
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

// Rol ve Admin Kullanýcýsý Oluþturma Kýsmý
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    // Rollerin Kontrolü ve Oluþturulmasý
    var roles = new[] { "Admin", "Member" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new AppRole { Name = role });
        }
    }

    // Admin Kullanýcýsýnýn Kontrolü ve Oluþturulmasý
    var adminEmail = "g231210035@sakarya.edu.tr"; // Admin kullanýcýsýnýn email adresi
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

        // Þifre: sau 
        var result = await userManager.CreateAsync(adminUser, "sau");

        if (result.Succeeded)
        {
            // Kullanýcýya Admin rolünün atanmasý
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.Run();
