using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// je cree mon serveur

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
    //ajoute l’outil “base de données sqlite” basé sur AppDbContext


builder.Services.AddCors(options =>
{//j’autorise Angular à me parler
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");// activer l autorisation

app.MapGet("/api/test", () => "Backend OK");
//test pour voir si la connection marche

app.Run(); //je lance le serveur