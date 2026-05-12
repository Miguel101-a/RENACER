using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Data;

public class RenacerDbContext : DbContext
{
    public RenacerDbContext(DbContextOptions<RenacerDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PK compuesta para RolPermiso
        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => new { rp.IdRol, rp.IdPermiso });

        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Rol)
            .WithMany(r => r.RolPermisos)
            .HasForeignKey(rp => rp.IdRol);

        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Permiso)
            .WithMany(p => p.RolPermisos)
            .HasForeignKey(rp => rp.IdPermiso);

        // PK compuesta para UsuarioRol
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.IdUsuario, ur.IdRol });

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(ur => ur.IdUsuario);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(ur => ur.IdRol);

        // Auditoria — sin FK a Usuario para no romper registros históricos
        modelBuilder.Entity<Auditoria>()
            .Property(a => a.IdAuditoria)
            .ValueGeneratedOnAdd();
    }
}
