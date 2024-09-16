﻿using Microsoft.OpenApi.Models;
using MinApiLivros.Entities;
using MinApiLivros.Services;

namespace MinApiLivros.Endpoints;

public static class LivrosEndpoints
{
    public static void RegisterLivrosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        //Criar novo livro
        endpoints.MapPost("/livros", async (Livro livro, ILivroService _livroService) =>
        {
            await _livroService.AddLivro(livro);
            return Results.Created($"{livro.Id}", livro);
        })
            .WithName("AddLivro")
            .RequireAuthorization()
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Incluir um livro",
                Description = "Inclui um novo livro na biblioteca.",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
            });

        //Buscar Todos os livros
        endpoints.MapGet("/livros", async (ILivroService _livroService) =>
            TypedResults.Ok(await _livroService.GetLivros()))
            .WithName("GetLivros")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Obtém todos os livros da biblioteca",
                Description = "Retorna informações sobre livros.",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
            });

        //Buscar livro por Id
        endpoints.MapGet("/livros/{id}", async (ILivroService _livroService, int id) =>
        {
            var livro = await _livroService.GetLivro(id);

            if (livro != null)
                return Results.Ok(livro);
            else
                return Results.NotFound();
        })
            .WithName("GetLivroPorId")
            .RequireAuthorization()
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Obtém um livro pelo seu Id",
                Description = "Retorna a informação de um livro.",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Livros" } }
            });

        //Deletar livro
        endpoints.MapDelete("/livros/{id}", async (int id, ILivroService _livroService) =>
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
    }
}