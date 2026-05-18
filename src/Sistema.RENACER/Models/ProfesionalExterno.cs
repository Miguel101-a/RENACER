using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("ProfesionalExterno")]
public class ProfesionalExterno
{
    [Key]
    [Column("IdProfesional")]
    public int IdProfesional { get; set; }

    [Column("Nombres")]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Column("Apellidos")]
    [MaxLength(100)]
    public string? Apellidos { get; set; }

    [Column("Especialidad")]
    [MaxLength(150)]
    public string? Especialidad { get; set; }

    [Column("Telefono")]
    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Column("Email")]
    [MaxLength(150)]
    public string? Email { get; set; }

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    [NotMapped]
    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
}
