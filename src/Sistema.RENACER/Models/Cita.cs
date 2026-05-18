using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.RENACER.Models;

[Table("Cita")]
public class Cita
{
    [Key]
    [Column("IdCita")]
    public int IdCita { get; set; }

    [Column("IdPaciente")]
    public int IdPaciente { get; set; }

    [Column("IdTipoCita")]
    public int IdTipoCita { get; set; }

    [Column("IdProfesional")]
    public int? IdProfesional { get; set; }

    [Column("FechaHora")]
    public DateTime FechaHora { get; set; }

    [Column("DuracionMinutos")]
    public int DuracionMinutos { get; set; } = 60;

    // Valores válidos por CHECK constraint en BD:
    //   'Programada' | 'Asistió' | 'No asistió' | 'Cancelada' | 'Reagendada'
    [Column("Estado")]
    [MaxLength(30)]
    public string Estado { get; set; } = CitaEstado.Programada;

    [Column("Observaciones")]
    [MaxLength(500)]
    public string? Observaciones { get; set; }

    [Column("FechaRegistro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Column("IdUsuarioRegistra")]
    public int? IdUsuarioRegistra { get; set; }

    // Navegación (no FK obligatoria en EF, se mapea por convención por IdXxx)
    [ForeignKey(nameof(IdPaciente))]
    public Paciente? Paciente { get; set; }

    [ForeignKey(nameof(IdTipoCita))]
    public TipoCita? TipoCita { get; set; }

    [ForeignKey(nameof(IdProfesional))]
    public ProfesionalExterno? Profesional { get; set; }
}

public static class CitaEstado
{
    public const string Programada = "Programada";
    public const string Asistio    = "Asistió";
    public const string NoAsistio  = "No asistió";
    public const string Cancelada  = "Cancelada";
    public const string Reagendada = "Reagendada";

    public static readonly string[] Todos =
    {
        Programada, Asistio, NoAsistio, Cancelada, Reagendada
    };
}
