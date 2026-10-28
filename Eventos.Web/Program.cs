using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EventosContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("EventosContext")));
builder.Services.AddScoped<IEventoRepositorio, EventoRepositorio>();
builder.Services.AddScoped<IAsistenteRepositorio, AsistenteRepositorio>();
builder.Services.AddScoped<IInscripcionRepositorio, InscripcionRepositorio>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// EF Tools necesita construir el host sin inicializar datos antes de las migraciones.
if (!EF.IsDesignTime)
{
    await using var alcance = app.Services.CreateAsyncScope();
    var contexto = alcance.ServiceProvider.GetRequiredService<EventosContext>();
    await contexto.Database.MigrateAsync();

    if (!await contexto.Categorias.AnyAsync())
    {
        contexto.Categorias.AddRange(
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Cultura" },
            new Categoria { Nombre = "Negocios" });
        await contexto.SaveChangesAsync();
    }

    if (!await contexto.Eventos.AnyAsync())
    {
        var categoriasActivas = await contexto.Categorias
            .AsNoTracking()
            .Where(categoria => categoria.Activo)
            .OrderBy(categoria => categoria.Id)
            .Select(categoria => categoria.Id)
            .ToListAsync();
        if (categoriasActivas.Count > 0)
        {
            string[] titulos =
            [
                "Taller de .NET",
                "Encuentro cultural",
                "Emprende Perú",
                "Introducción a la nube"
            ];

            var eventosIniciales = Enumerable.Range(0, titulos.Length)
            .Select(indice => new Evento
                {
                    Titulo = titulos[indice],
                    Fecha = DateTime.Today.AddDays(30 + indice * 7).AddHours(18),
                    Lugar = "Auditorio Lima",
                    Aforo = 50 + indice * 25,
                    Precio = indice * 15,
                    CategoriaId = categoriasActivas[indice % categoriasActivas.Count],
                    Modalidad = "Presencial"
                });

            contexto.Eventos.AddRange(eventosIniciales);
            await contexto.SaveChangesAsync();
        }
    }

    if (!await contexto.Asistentes.AnyAsync())
    {
        string[] nombres =
        [
            "Ana Torres",
            "Bruno Díaz",
            "Carla Rojas",
            "Diego Pérez",
            "Elena Ruiz"
        ];

        var asistentesIniciales = Enumerable.Range(1, nombres.Length)
            .Select(indice => new Asistente
            {
                Nombres = nombres[indice - 1],
                Dni = $"7000000{indice}",
                Correo = $"asistente{indice}@example.com"
            });

        contexto.Asistentes.AddRange(asistentesIniciales);
        await contexto.SaveChangesAsync();
    }
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Eventos}/{action=Index}/{id?}");
app.Run();
