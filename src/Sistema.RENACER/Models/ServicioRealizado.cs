using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("ServicioRealizado")]
public class ServicioRealizado
{
    [Key]
    [Column("IdServicioRealizado")]
    public int IdServicioRealizado { get; set; }

    [Column("IdCita")]
    public int IdCita { get; set; }

    [Column("IdServicio")]
    public int IdServicio { get; set; }

    [Column("FechaRealizacion", TypeName = "date")]
    public DateTime FechaRealizacion { get; set; } = DateTime.Today;

    [Column("Observaciones")]
    [MaxLength(500)]
    public string? Observaciones { get; set; }

    [Column("IdUsuarioRegistra")]
    public int? IdUsuarioRegistra { get; set; }

    [Column("FechaRegistro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
