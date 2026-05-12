using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("RolPermiso")]
public class RolPermiso
{
    [Column("IdRol")]
    public int IdRol { get; set; }

    [Column("IdPermiso")]
    public int IdPermiso { get; set; }

    public Rol Rol { get; set; } = null!;
    public Permiso Permiso { get; set; } = null!;
}
