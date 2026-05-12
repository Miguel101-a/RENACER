using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Usuario")]
public class Usuario
{
    [Key]
    [Column("IdUsuario")]
    public int IdUsuario { get; set; }

    [Column("NombreUsuario")]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Column("PasswordHash")]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("Nombres")]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Column("Apellidos")]
    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Column("Email")]
    [MaxLength(150)]
    public string? Email { get; set; }

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    [Column("FechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [Column("UltimoAcceso")]
    public DateTime? UltimoAcceso { get; set; }

    [NotMapped]
    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();

    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
}
