using Eventos.Web.Models;

namespace Eventos.Web.Repositorios;

public interface IInscripcionRepositorio
{
    Task<List<Inscripcion>> ListarAsync();
    Task<string?> CrearAsync(Inscripcion inscripcion);
    Task<bool> CancelarAsync(int id);
}
