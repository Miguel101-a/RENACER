using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Auditoria")]
public class Auditoria
{
    [Key]
    [Column("IdAuditoria")]
    public long IdAuditoria { get; set; }

    [Column("Tabla")]
    [MaxLength(100)]
    public string Tabla { get; set; } = string.Empty;

    [Column("Operacion")]
    [MaxLength(10)]
    public string Operacion { get; set; } = string.Empty;

    [Column("IdRegistro")]
    public long IdRegistro { get; set; }

    [Column("ValorAnterior")]
    public string? ValorAnterior { get; set; }

    [Column("ValorNuevo")]
    public string? ValorNuevo { get; set; }

    [Column("IdUsuario")]
    public int? IdUsuario { get; set; }

    [Column("Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column("IP")]
    [MaxLength(50)]
    public string? IP { get; set; }
}
