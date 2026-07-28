using PoslovnaLogika;
using PoslovnaLogika.Servisi;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7025")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient<ParametriPoslovnihPravilaServis>(
    httpClient =>
    {
        string adresa =
            builder.Configuration[
                "AdreseServisa:ParametriPoslovnihPravila"
            ] ?? throw new InvalidOperationException(
                "Nije podešena adresa REST API-ja za poslovna pravila."
            );

        httpClient.BaseAddress = new Uri(adresa);
    });

builder.Services.AddScoped<ObradaPoslovnihPravila>();

builder.Services.AddScoped<UgovorRepo>(provider =>
{
    string konekcioniString =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Nije pronađen connection string DefaultConnection."
        );

    return new UgovorRepo(konekcioniString);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();