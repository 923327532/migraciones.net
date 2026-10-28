using Eventos.Web.Models;
using Eventos.Web.ViewModels;

namespace Eventos.Web.Repositorios;

public interface IEventoRepositorio
{
    Task<List<EventoResumen>> ListarAsync(int? categoriaId);
    Task<EventoResumen?> ObtenerAsync(int id);
    Task<List<Categoria>> CategoriasAsync();
    Task<bool> CategoriaValidaAsync(int id);
    Task CrearAsync(Evento evento);
    Task<bool> EditarAsync(Evento evento);
    Task<bool> EliminarAsync(int id);
}
