using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Paciente")]
public class Paciente
{
    [Key]
    [Column("IdPaciente")]
    public int IdPaciente { get; set; }

    [Column("Nombres")]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Column("Apellidos")]
    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Column("FechaNacimiento", TypeName = "date")]
    public DateTime FechaNacimiento { get; set; }

    // Columna calculada por SQL Server — no debe insertarse/actualizarse desde EF.
    [Column("Edad")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Edad { get; set; }

    [Column("Genero")]
    [MaxLength(20)]
    public string? Genero { get; set; }

    [Column("CI")]
    [MaxLength(20)]
    public string? CI { get; set; }

    [Column("Telefono")]
    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Column("Email")]
    [MaxLength(150)]
    public string? Email { get; set; }

    [Column("Direccion")]
    [MaxLength(300)]
    public string? Direccion { get; set; }

    [Column("NombreTutor")]
    [MaxLength(150)]
    public string? NombreTutor { get; set; }

    [Column("TelefonoTutor")]
    [MaxLength(20)]
    public string? TelefonoTutor { get; set; }

    [Column("RelacionTutor")]
    [MaxLength(50)]
    public string? RelacionTutor { get; set; }

    [Column("MotivoConsulta")]
    [MaxLength(500)]
    public string? MotivoConsulta { get; set; }

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    [Column("FechaRegistro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Column("IdUsuarioRegistra")]
    public int? IdUsuarioRegistra { get; set; }

    [NotMapped]
    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
}
