using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Servicio")]
public class Servicio
{
    [Key]
    [Column("IdServicio")]
    public int IdServicio { get; set; }

    [Column("Nombre")]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Column("Descripcion")]
    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [Column("Activo")]
    public bool Activo { get; set; } = true;
}
