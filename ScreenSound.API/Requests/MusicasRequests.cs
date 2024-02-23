using System.ComponentModel.DataAnnotations;

namespace ScreenSound.API.Requests
{
    public record PostMusicasRequests([Required] string Nome, [Required] int ArtistaId, int AnoLancamento, ICollection<GeneroRequest> Generos = null);
    public record PutMusicasRequests(int id, string nome, int artistaId, int anoLancamento);
}
