-- ============================================================
--  SISTEMA RENACER - Script Incremental Sprint 2 (v3)
-- ============================================================
--  Centro Psicoterapéutico RENACER
--  Lic. Ana Luisa Apaza - Santa Cruz, Bolivia
--
--  Proyecto:    Taller de Grado I
--  Autor:       Miguel Valencia Apaza - UDABOL, Santa Cruz
--  SQL Server:  2022
--  Instancia:   LAPTOP-6RTMMUVO\MSSQLSERVER2024
--
--  Versión:     3.0 (incremental, Sprint 2)
--  Depende de:  RENACER_v2.sql (ya ejecutado)
--
--  Objetivo:
--    Sprint 2 (Pacientes y Agenda) reutiliza el esquema definido
--    en v2 — todas las tablas y catálogos requeridos ya existen.
--    Este script es IDEMPOTENTE y sirve como verificación +
--    refuerzo de datos semilla mínimos por si se ejecuta sobre
--    una instalación parcial.
--
--  Cambios v3 (todos guardados con IF NOT EXISTS):
--    - Reasegura los 8 permisos del Sprint 2 (PAC_* y CITA_*).
--    - Reasigna esos permisos a los roles Responsable y Administrador.
--    - Reasegura catálogos mínimos (TipoCita, Servicio, Moneda BOB).
--    - NO recrea tablas, NO modifica datos existentes.
-- ============================================================

USE RENACER;
GO

PRINT N'== Sprint 2 - Verificación / Refuerzo de datos semilla ==';
GO

-- ============================================================
--  SECCIÓN 1: PERMISOS Sprint 2 (idempotente)
-- ============================================================

-- PAC_VER
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'PAC_VER')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('PAC_VER', 'Ver pacientes',
            'Permite consultar el listado de pacientes', 'Operativo');

-- PAC_CREAR
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'PAC_CREAR')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('PAC_CREAR', 'Registrar paciente',
            'Permite registrar nuevos pacientes', 'Operativo');

-- PAC_EDITAR
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'PAC_EDITAR')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('PAC_EDITAR', 'Editar paciente',
            'Permite modificar datos del paciente', 'Operativo');

-- PAC_ELIMINAR
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'PAC_ELIMINAR')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('PAC_ELIMINAR', 'Dar de baja paciente',
            'Permite desactivar un paciente', 'Operativo');

-- CITA_VER
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'CITA_VER')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('CITA_VER', 'Ver citas',
            'Permite consultar citas programadas', 'Operativo');

-- CITA_CREAR
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'CITA_CREAR')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('CITA_CREAR', 'Registrar cita',
            'Permite agendar nuevas citas', 'Operativo');

-- CITA_EDITAR
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'CITA_EDITAR')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('CITA_EDITAR', 'Modificar cita',
            'Permite reagendar o editar una cita', 'Operativo');

-- CITA_CANCELAR
IF NOT EXISTS (SELECT 1 FROM Permiso WHERE Codigo = 'CITA_CANCELAR')
    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria)
    VALUES ('CITA_CANCELAR', 'Cancelar cita',
            'Permite cancelar citas', 'Operativo');
GO

-- ============================================================
--  SECCIÓN 2: Asignación de permisos a roles (idempotente)
-- ============================================================

DECLARE @IdRolAdmin INT       = (SELECT IdRol FROM Rol WHERE Nombre = 'Administrador');
DECLARE @IdRolResponsable INT = (SELECT IdRol FROM Rol WHERE Nombre = 'Responsable');

-- Administrador: todos los permisos del Sprint 2
INSERT INTO RolPermiso (IdRol, IdPermiso)
    SELECT @IdRolAdmin, p.IdPermiso
    FROM Permiso p
    WHERE p.Codigo IN ('PAC_VER','PAC_CREAR','PAC_EDITAR','PAC_ELIMINAR',
                       'CITA_VER','CITA_CREAR','CITA_EDITAR','CITA_CANCELAR')
      AND NOT EXISTS (
          SELECT 1 FROM RolPermiso rp
          WHERE rp.IdRol = @IdRolAdmin AND rp.IdPermiso = p.IdPermiso);

-- Responsable: los mismos permisos del Sprint 2
INSERT INTO RolPermiso (IdRol, IdPermiso)
    SELECT @IdRolResponsable, p.IdPermiso
    FROM Permiso p
    WHERE p.Codigo IN ('PAC_VER','PAC_CREAR','PAC_EDITAR','PAC_ELIMINAR',
                       'CITA_VER','CITA_CREAR','CITA_EDITAR','CITA_CANCELAR')
      AND NOT EXISTS (
          SELECT 1 FROM RolPermiso rp
          WHERE rp.IdRol = @IdRolResponsable AND rp.IdPermiso = p.IdPermiso);
GO

-- ============================================================
--  SECCIÓN 3: Catálogos mínimos (idempotente)
-- ============================================================

-- TipoCita: refuerza los 3 tipos requeridos por el prompt
IF NOT EXISTS (SELECT 1 FROM TipoCita WHERE Nombre = 'Psicológica')
    INSERT INTO TipoCita (Nombre, Descripcion)
    VALUES ('Psicológica', 'Cita de atención psicológica clínica');

IF NOT EXISTS (SELECT 1 FROM TipoCita WHERE Nombre = 'Psicopedagógica')
    INSERT INTO TipoCita (Nombre, Descripcion)
    VALUES ('Psicopedagógica', 'Cita de apoyo psicopedagógico');

IF NOT EXISTS (SELECT 1 FROM TipoCita WHERE Nombre = 'Evaluación')
    INSERT INTO TipoCita (Nombre, Descripcion)
    VALUES ('Evaluación', 'Aplicación de pruebas psicológicas');

-- Servicio: refuerza los 4 servicios requeridos por el prompt
IF NOT EXISTS (SELECT 1 FROM Servicio WHERE Nombre = 'Terapia individual')
    INSERT INTO Servicio (Nombre) VALUES ('Terapia individual');

IF NOT EXISTS (SELECT 1 FROM Servicio WHERE Nombre = 'Terapia de pareja')
    INSERT INTO Servicio (Nombre) VALUES ('Terapia de pareja');

IF NOT EXISTS (SELECT 1 FROM Servicio WHERE Nombre = 'Apoyo escolar')
    INSERT INTO Servicio (Nombre) VALUES ('Apoyo escolar');

IF NOT EXISTS (SELECT 1 FROM Servicio WHERE Nombre = 'Evaluación psicológica')
    INSERT INTO Servicio (Nombre) VALUES ('Evaluación psicológica');

-- Moneda BOB
IF NOT EXISTS (SELECT 1 FROM Moneda WHERE Codigo = 'BOB')
    INSERT INTO Moneda (Codigo, Nombre, Simbolo) VALUES ('BOB', 'Bolivianos', 'Bs.');
GO

-- ============================================================
--  SECCIÓN 4: Verificación final
-- ============================================================

PRINT N'-- Permisos Sprint 2 (8 esperados) --';
SELECT Codigo, Nombre
FROM Permiso
WHERE Codigo IN ('PAC_VER','PAC_CREAR','PAC_EDITAR','PAC_ELIMINAR',
                 'CITA_VER','CITA_CREAR','CITA_EDITAR','CITA_CANCELAR')
ORDER BY Codigo;

PRINT N'-- Asignaciones de permisos Sprint 2 a roles --';
SELECT r.Nombre AS Rol, p.Codigo, p.Nombre AS Permiso
FROM RolPermiso rp
INNER JOIN Rol     r ON rp.IdRol     = r.IdRol
INNER JOIN Permiso p ON rp.IdPermiso = p.IdPermiso
WHERE p.Codigo IN ('PAC_VER','PAC_CREAR','PAC_EDITAR','PAC_ELIMINAR',
                   'CITA_VER','CITA_CREAR','CITA_EDITAR','CITA_CANCELAR')
ORDER BY r.Nombre, p.Codigo;

PRINT N'-- Catálogos Sprint 2 --';
SELECT 'TipoCita'  AS Catalogo, COUNT(*) AS Total FROM TipoCita UNION ALL
SELECT 'Servicio',              COUNT(*)         FROM Servicio  UNION ALL
SELECT 'ProfesionalExterno',    COUNT(*)         FROM ProfesionalExterno UNION ALL
SELECT 'Moneda',                COUNT(*)         FROM Moneda;
GO

-- ============================================================
--  FIN DEL SCRIPT v3 (Sprint 2)
--  Esquema reutilizado: 0 tablas nuevas
--  Permisos asegurados: 8 (PAC_* + CITA_*)
--  Catálogos reforzados: TipoCita, Servicio, Moneda
-- ============================================================
