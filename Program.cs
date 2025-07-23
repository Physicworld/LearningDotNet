using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Services;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins");
// Start services
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("name=DefaultConnection"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });

    options.AddPolicy("Free", configuration => { configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepositoryGenres, RepositoryGenres>();
builder.Services.AddScoped<IRepositoryActors, RepositoryActors>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));

// End services
var app = builder.Build();

// Start middlewares
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors();
app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "Free")]() => "Hello World!");
app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();

// End middlewares

app.Run();

