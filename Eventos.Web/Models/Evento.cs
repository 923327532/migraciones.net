using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Eventos.Web.Models;

public class Evento
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Titulo { get; set; } = "";

    public DateTime Fecha { get; set; }

    [Required, StringLength(100)]
    public string Lugar { get; set; } = "";

    [Range(1, 5000)]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)"), Range(typeof(decimal), "0", "99999999.99")]
    public decimal Precio { get; set; }

    [Range(1, int.MaxValue), Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [ValidateNever]
    public Categoria Categoria { get; set; } = null!;

    [Required, StringLength(20), RegularExpression("^(Presencial|Virtual)$")]
    public string Modalidad { get; set; } = "Presencial";

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
