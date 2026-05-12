using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("UsuarioRol")]
public class UsuarioRol
{
    [Column("IdUsuario")]
    public int IdUsuario { get; set; }

    [Column("IdRol")]
    public int IdRol { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Rol Rol { get; set; } = null!;
}
