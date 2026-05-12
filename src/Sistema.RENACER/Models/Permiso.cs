using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Permiso")]
public class Permiso
{
    [Key]
    [Column("IdPermiso")]
    public int IdPermiso { get; set; }

    [Column("Codigo")]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Column("Nombre")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Column("Descripcion")]
    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [Column("Categoria")]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}
