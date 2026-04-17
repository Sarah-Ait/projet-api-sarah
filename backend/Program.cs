using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Interfaces;
using backend.Repositories;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// je crée mon serveur

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
// ajoute l’outil “base de données sqlite” basé sur AppDbContext

builder.Services.AddCors(options =>
{
    // j’autorise Angular à me parler
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
//addscoped pour garder la meme instance pendant une requete
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKanbanColumnRepository, KanbanColumnRepository>();
builder.Services.AddScoped<IKanbanColumnService, KanbanColumnService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular"); // activer l’autorisation

app.MapControllers();

app.Run(); // je lance le serveur