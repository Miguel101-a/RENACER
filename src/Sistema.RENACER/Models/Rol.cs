using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Rol")]
public class Rol
{
    [Key]
    [Column("IdRol")]
    public int IdRol { get; set; }

    [Column("Nombre")]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Column("Descripcion")]
    [MaxLength(200)]
    public string? Descripcion { get; set; }

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    [Column("FechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
}
