using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinApiLivros.Context;
using MinApiLivros.Entities;
using MinApiLivros.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(optons => 
    optons.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<ILivroService, LivroService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Endpoints Livros
//Criar novo livro
app.MapPost("/livros", async (Livro livro, ILivroService _livroService) =>
{
    await _livroService.AddLivro(livro);
    return Results.Created($"{livro.Id}", livro);
})
    .WithName("AddLivro")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Incluir um livro",
        Description = "Inclui um novo livro na biblioteca.",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
    });

//Buscar Todos os livros
app.MapGet("/livros", async (ILivroService _livroService) =>
    TypedResults.Ok(await _livroService.GetLivros()))
    .WithName("GetLivros")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Obtém todos os livros da biblioteca",
        Description = "Retorna informações sobre livros.",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
    });

//Buscar livro por Id
app.MapGet("/livros/{id}", async (ILivroService _livroService, int id) =>
{
    var livro = await _livroService.GetLivro(id);

    if (livro != null)
        return Results.Ok(livro);
    else
        return Results.NotFound();
})
    .WithName("GetLivroPorId")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Obtém um livro pelo seu Id",
        Description = "Retorna a informação de um livro.",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
    });

//Deletar livro
app.MapDelete("/livros/{id}", async (int id, ILivroService _livroService) =>
{
    var livro = await _livroService.DeleteLivro(id);
    return Results.Ok($"Livro de id={id} deletado");
})
    .WithName("DeleteLivroPorId")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Deleta um livro pelo seu Id",
        Description = "Deleta um livro da biblioteca.",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
    });

#endregion


app.Run();