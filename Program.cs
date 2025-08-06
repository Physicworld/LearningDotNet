using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
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
builder.Services.AddScoped<IRepositoryMovies, RepositoryMovies>();
builder.Services.AddScoped<IRepositoryComments, RepositoryComments>();
builder.Services.AddScoped<IRepositoryErrors, RepositoryErrors>();

builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();

// End services
var app = builder.Build();

// Start middlewares
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature.Error!;
    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.Message = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IRepositoryErrors>();
    await repository.Create(error);
}));

app.UseStatusCodePages();

app.UseStaticFiles();
app.UseCors();
app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "Free")]() => "Hello World!");
app.MapGet("/error", () => { throw new InvalidOperationException("Example Error"); });
app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();

// End middlewares

app.Run();