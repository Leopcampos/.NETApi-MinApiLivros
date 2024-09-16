using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinApiLivros.Context;
using MinApiLivros.Endpoints;
using MinApiLivros.Entities;
using MinApiLivros.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(optons =>
    optons.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<ILivroService, LivroService>();

builder.Services.AddAuthorization();

var app = builder.Build();

//Configura o meddleware de exceção
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
    .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Mapeamento dos endpoins do Identity
app.MapGroup("/identity").MapIdentityApi<IdentityUser>();

//Registrando os endpoints de Livros
app.RegisterLivrosEndpoints();

app.Run();