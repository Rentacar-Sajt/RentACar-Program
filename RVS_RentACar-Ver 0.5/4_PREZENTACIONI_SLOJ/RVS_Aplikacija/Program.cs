using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using QuestPDF.Infrastructure;
using RVS_Aplikacija.Servisi.Pdf;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;


// Dodavanje MVC podrške
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<
    IPdfUgovorServis,
    PdfUgovorServis
>();

// Registracija DbContext-a (Entity Framework) sa SQL Server konekcijom
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        "Server=DESKTOP-FI5BVBV\\SQLEXPRESS;Database=RVS2025;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"));

// Registracija repozitorijuma za Dependency Injection
builder.Services.AddScoped<KonekcijaKlasa>(servisProvajder =>
{
    var konfiguracija = servisProvajder.GetRequiredService<IConfiguration>();
    var konekcioniString = konfiguracija.GetConnectionString("KonekcioniString");
    var konekcijaObjekat = new KonekcijaKlasa(konekcioniString);
    konekcijaObjekat.OtvoriKonekciju();
    return konekcijaObjekat;
});

builder.Services.AddScoped<TehnologijaRepo>();
builder.Services.AddScoped<KorisnikRepo>();
builder.Services.AddScoped<KursRepo>();

// Dodavanje cookie autentifikacije
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Nalog/Prijava";
        options.LogoutPath = "/Nalog/OdjaviSe";
        options.AccessDeniedPath = "/Nalog/Prijava";
    });

//Za sesiju
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("RestCrudServis", httpClient =>
{
    string adresa =
        builder.Configuration["AdreseServisa:RestCrudServis"]
        ?? throw new InvalidOperationException(
            "Nije podešena adresa REST CRUD servisa."
        );

    httpClient.BaseAddress = new Uri(adresa);
});

builder.Services.AddHttpClient("AutentikacijaServis", httpClient =>
{
    string adresa =
        builder.Configuration["AdreseServisa:AutentikacijaServis"]
        ?? throw new InvalidOperationException(
            "Nije podešena adresa servisa za autentikaciju."
        );

    httpClient.BaseAddress = new Uri(adresa);
});

var app = builder.Build();

// Konfiguracija middleware-a
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Pocetna/Greska");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Obavezno zbog autentifikacije / logina
app.UseAuthentication();
app.UseAuthorization();

// Mapiranje ruta
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pocetna}/{action=Index}/{id?}");

app.Run();
