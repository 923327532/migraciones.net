using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio(EventosContext contexto) : IAsistenteRepositorio
{
    public async Task<List<Asistente>> ListarAsync()
    {
        return await contexto.Asistentes
            .AsNoTracking()
            .Where(asistente => asistente.Activo)
            .OrderBy(asistente => asistente.Nombres)
            .ToListAsync();
    }

    public async Task<bool> DniExisteAsync(string dni)
    {
        return await contexto.Asistentes
            .AsNoTracking()
            .AnyAsync(asistente => asistente.Dni == dni);
    }

    public async Task<bool> CorreoExisteAsync(string correo)
    {
        return await contexto.Asistentes
            .AsNoTracking()
            .AnyAsync(asistente => asistente.Correo == correo);
    }

    public async Task CrearAsync(Asistente asistente)
    {
        contexto.Asistentes.Add(asistente);
        await contexto.SaveChangesAsync();
    }
}
