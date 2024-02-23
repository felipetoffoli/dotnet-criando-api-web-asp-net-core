using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {

        public static void AddEndPointsArtistas(this WebApplication app)
        {

            app.MapGet("/artistas", ([FromServices] DAL<Artista> dal) =>
            {
                return Results.Ok(dal.Listar());

            });

            app.MapGet("/artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                var artista = dal.RecuperarListaPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
                if (artista is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(artista);
            });

            app.MapPost("/artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistasRequests artistaRequests) => {
                var artista = new Artista(artistaRequests.nome, artistaRequests.bio);
                dal.Adicionar(artista);
                return Results.Ok();
            });

            app.MapDelete("/artistas/{id}", ([FromServices] DAL<Artista> dal, int id) => {
                var artista = dal.RecuperarPor(a => a.Id == id);
                if (artista is null)
                {
                    return Results.NoContent();
                }
                dal.Deletar(artista);
                return Results.Ok();
            });

            app.MapPut("/artistas", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista) => {
                var artistaAtualizar = dal.RecuperarPor(a => a.Id == artista.Id);

                if (artistaAtualizar is null)
                {
                    return Results.NotFound();
                }
                artistaAtualizar.Nome = artista.Nome;
                artistaAtualizar.Bio = artista.Bio;

                artistaAtualizar.FotoPerfil = artista.FotoPerfil;
                dal.Atualizar(artistaAtualizar);
                return Results.Ok();
            });


        }
    }

}
