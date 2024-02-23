using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class MusicasExtensions
    {

        public static void AddEndPointsMusicas(this WebApplication app)
        {

            app.MapGet("/musicas", ([FromServices] DAL<Musica> dal) =>
            {
                return Results.Ok(dal.Listar());

            });

            app.MapGet("/musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                var musica = dal.RecuperarListaPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
                if (musica is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(musica);
            });

            app.MapPost("/musicas", ([FromServices] DAL < Artista > dalArtista, [FromServices] DAL<Musica> dal, [FromBody] PostMusicasRequests musicaRequests) => {
                var artista = dalArtista.RecuperarPor(a => a.Id == musicaRequests.ArtistaId);
                var musica = new Musica(musicaRequests.Nome) {
                    Artista = artista is not null ? artista : null,
                    AnoLancamento = musicaRequests.AnoLancamento,
                    Generos = GeneroRequestConverter(musicaRequests.Generos) is not null ? GeneroRequestConverter(musicaRequests.Generos) : new List<Genero>()


                };
                dal.Adicionar(musica);
                return Results.Ok();
            });

            app.MapDelete("/musicas/{id}", ([FromServices] DAL<Musica> dal, int id) => {
                var musica = dal.RecuperarPor(a => a.Id == id);
                if (musica is null)
                {
                    return Results.NoContent();
                }
                dal.Deletar(musica);
                return Results.Ok();
            });

            app.MapPut("/musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL < Artista > dalArtista, [FromBody] PutMusicasRequests musica) => {

                var musicaAtualizar = dal.RecuperarPor(a => a.Id == musica.id);

                if (musicaAtualizar is null)
                {
                    return Results.NotFound();
                }
                musicaAtualizar.Nome = musica.nome;
                musicaAtualizar.AnoLancamento = musica.anoLancamento;

                
                var  recuperarArtista = dalArtista.RecuperarPor(a => a.Id == musica.artistaId);
                if (recuperarArtista != null)
                {
                    musicaAtualizar.Artista = recuperarArtista;
                }
                dal.Atualizar(musicaAtualizar);
                return Results.Ok();
            });


        }
        
        private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos)
        {
            return generos.Select(a => RequestToEntity(a)).ToList();
        }

        private static Genero RequestToEntity(GeneroRequest genero)
        {
            return new Genero(genero.Nome)
            {
                Nome = genero.Nome,
                Descricao = genero.Descricao
            } ;
        }
    }

}
