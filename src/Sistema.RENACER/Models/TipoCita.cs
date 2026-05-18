using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("TipoCita")]
public class TipoCita
{
    [Key]
    [Column("IdTipoCita")]
    public int IdTipoCita { get; set; }

    [Column("Nombre")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Column("Descripcion")]
    [MaxLength(200)]
    public string? Descripcion { get; set; }
}
