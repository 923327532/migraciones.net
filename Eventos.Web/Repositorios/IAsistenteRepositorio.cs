using Eventos.Web.Models;

namespace Eventos.Web.Repositorios;

public interface IAsistenteRepositorio
{
    Task<List<Asistente>> ListarAsync();
    Task<bool> DniExisteAsync(string dni);
    Task<bool> CorreoExisteAsync(string correo);
    Task CrearAsync(Asistente asistente);
}
