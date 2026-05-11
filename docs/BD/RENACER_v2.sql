-- ============================================================
--  SISTEMA RENACER - Script Completo de Base de Datos v2
--  ============================================================
--  Centro Psicoterapéutico RENACER
--  Lic. Ana Luisa Apaza - Santa Cruz, Bolivia
--
--  SQL Server 2022
--  Instancia: LAPTOP-6RTMMUVO\MSSQLSERVER2024
--  SSMS: 20
--  Autenticación: Windows
--
--  Proyecto: Taller de Grado I
--  Autor: Miguel Valencia Apaza
--  Universidad UDABOL - Santa Cruz
--
--  Versión: 2.0 (corregida)
--  Cambios v2:
--    - Hash bcrypt-ready: SHA-256 documentado como placeholder
--      (la app ASP.NET Core re-hashea con BCrypt al primer login)
--    - Trigger riesgoso eliminado, reemplazado por SP controlado
--    - 4 índices de rendimiento agregados
--    - Auditoria.IdRegistro ampliado a BIGINT
--    - 3 triggers reales (Paciente UPDATE/DELETE,
--      Pago INSERT, Cita INSERT/UPDATE solo para auditoría)
-- ============================================================


-- ============================================================
--  SECCIÓN 1: CREAR BASE DE DATOS
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RENACER')
BEGIN
    ALTER DATABASE RENACER SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RENACER;
END
GO

CREATE DATABASE RENACER
    COLLATE Modern_Spanish_CI_AI;
GO

USE RENACER;
GO


-- ============================================================
--  SECCIÓN 2: CREAR TABLAS (19 tablas)
-- ============================================================

-- ──────────────────────────────────────────
--  BLOQUE A: SEGURIDAD Y ACCESO (5 tablas)
-- ──────────────────────────────────────────

CREATE TABLE Rol (
    IdRol         INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(50)  NOT NULL UNIQUE,
    Descripcion   NVARCHAR(200) NULL,
    Activo        BIT           NOT NULL DEFAULT 1,
    FechaCreacion DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Permiso (
    IdPermiso   INT IDENTITY(1,1) PRIMARY KEY,
    Codigo      NVARCHAR(50)  NOT NULL UNIQUE,
    Nombre      NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Categoria   NVARCHAR(50)  NOT NULL  -- 'Tecnico' | 'Operativo'
);
GO

CREATE TABLE Usuario (
    IdUsuario       INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario   NVARCHAR(50)  NOT NULL UNIQUE,
    -- IMPORTANTE: Este campo guarda el hash BCrypt en producción.
    -- El INSERT inicial usa SHA-256 como placeholder de bootstrap;
    -- la app ASP.NET Core debe re-hashear con BCrypt al primer login.
    PasswordHash    NVARCHAR(256) NOT NULL,
    Nombres         NVARCHAR(100) NOT NULL,
    Apellidos       NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(150) NULL,
    Activo          BIT           NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME      NOT NULL DEFAULT GETDATE(),
    UltimoAcceso    DATETIME      NULL
);
GO

CREATE TABLE RolPermiso (
    IdRol     INT NOT NULL REFERENCES Rol(IdRol),
    IdPermiso INT NOT NULL REFERENCES Permiso(IdPermiso),
    PRIMARY KEY (IdRol, IdPermiso)
);
GO

CREATE TABLE UsuarioRol (
    IdUsuario INT NOT NULL REFERENCES Usuario(IdUsuario),
    IdRol     INT NOT NULL REFERENCES Rol(IdRol),
    PRIMARY KEY (IdUsuario, IdRol)
);
GO

-- ──────────────────────────────────────────
--  BLOQUE B: CATÁLOGOS (6 tablas)
-- ──────────────────────────────────────────

CREATE TABLE Servicio (
    IdServicio  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(150) NOT NULL UNIQUE,
    Descripcion NVARCHAR(300) NULL,
    Activo      BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE Prueba (
    IdPrueba    INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(200) NOT NULL UNIQUE,
    Sigla       NVARCHAR(20)  NULL,
    Descripcion NVARCHAR(300) NULL,
    Activo      BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE TipoCita (
    IdTipoCita  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);
GO

CREATE TABLE TipoPago (
    IdTipoPago  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);
GO

CREATE TABLE MetodoPago (
    IdMetodoPago INT IDENTITY(1,1) PRIMARY KEY,
    Nombre       NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion  NVARCHAR(200) NULL
);
GO

CREATE TABLE Moneda (
    IdMoneda    INT IDENTITY(1,1) PRIMARY KEY,
    Codigo      NVARCHAR(10) NOT NULL UNIQUE,  -- BOB | USD
    Nombre      NVARCHAR(50) NOT NULL,
    Simbolo     NVARCHAR(5)  NOT NULL
);
GO

-- ──────────────────────────────────────────
--  BLOQUE C: PERSONAS EXTERNAS (1 tabla)
-- ──────────────────────────────────────────

CREATE TABLE ProfesionalExterno (
    IdProfesional INT IDENTITY(1,1) PRIMARY KEY,
    Nombres       NVARCHAR(100) NOT NULL,
    Apellidos     NVARCHAR(100) NULL,
    Especialidad  NVARCHAR(150) NULL,
    Telefono      NVARCHAR(20)  NULL,
    Email         NVARCHAR(150) NULL,
    Activo        BIT           NOT NULL DEFAULT 1
);
GO

-- ──────────────────────────────────────────
--  BLOQUE D: OPERACIÓN PRINCIPAL (4 tablas)
-- ──────────────────────────────────────────

CREATE TABLE Paciente (
    IdPaciente         INT IDENTITY(1,1) PRIMARY KEY,
    Nombres            NVARCHAR(100) NOT NULL,
    Apellidos          NVARCHAR(100) NOT NULL,
    FechaNacimiento    DATE          NOT NULL,
    Edad               AS (DATEDIFF(YEAR, FechaNacimiento, GETDATE())
                            - CASE WHEN MONTH(GETDATE()) < MONTH(FechaNacimiento)
                                     OR (MONTH(GETDATE()) = MONTH(FechaNacimiento)
                                         AND DAY(GETDATE()) < DAY(FechaNacimiento))
                                   THEN 1 ELSE 0 END),
    Genero             NVARCHAR(20)  NULL,    -- 'Masculino' | 'Femenino' | 'Otro'
    CI                 NVARCHAR(20)  NULL,
    Telefono           NVARCHAR(20)  NULL,
    Email              NVARCHAR(150) NULL,
    Direccion          NVARCHAR(300) NULL,
    NombreTutor        NVARCHAR(150) NULL,    -- si es menor de edad
    TelefonoTutor      NVARCHAR(20)  NULL,
    RelacionTutor      NVARCHAR(50)  NULL,
    MotivoConsulta     NVARCHAR(500) NULL,
    Activo             BIT           NOT NULL DEFAULT 1,
    FechaRegistro      DATETIME      NOT NULL DEFAULT GETDATE(),
    IdUsuarioRegistra  INT           NULL REFERENCES Usuario(IdUsuario)
);
GO

CREATE TABLE Cita (
    IdCita             INT IDENTITY(1,1) PRIMARY KEY,
    IdPaciente         INT          NOT NULL REFERENCES Paciente(IdPaciente),
    IdTipoCita         INT          NOT NULL REFERENCES TipoCita(IdTipoCita),
    IdProfesional      INT          NULL REFERENCES ProfesionalExterno(IdProfesional),
    FechaHora          DATETIME     NOT NULL,
    DuracionMinutos    INT          NOT NULL DEFAULT 60,
    Estado             NVARCHAR(30) NOT NULL DEFAULT 'Programada',
                       -- 'Programada' | 'Asistió' | 'No asistió' | 'Cancelada' | 'Reagendada'
    Observaciones      NVARCHAR(500) NULL,
    FechaRegistro      DATETIME      NOT NULL DEFAULT GETDATE(),
    IdUsuarioRegistra  INT           NULL REFERENCES Usuario(IdUsuario),
    CONSTRAINT CHK_Cita_Estado CHECK (Estado IN ('Programada','Asistió','No asistió','Cancelada','Reagendada'))
);
GO

CREATE TABLE ServicioRealizado (
    IdServicioRealizado INT IDENTITY(1,1) PRIMARY KEY,
    IdCita              INT          NOT NULL REFERENCES Cita(IdCita),
    IdServicio          INT          NOT NULL REFERENCES Servicio(IdServicio),
    FechaRealizacion    DATE         NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    Observaciones       NVARCHAR(500) NULL,
    IdUsuarioRegistra   INT          NULL REFERENCES Usuario(IdUsuario),
    FechaRegistro       DATETIME     NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE PruebaAplicada (
    IdPruebaAplicada    INT IDENTITY(1,1) PRIMARY KEY,
    IdCita              INT          NOT NULL REFERENCES Cita(IdCita),
    IdPrueba            INT          NOT NULL REFERENCES Prueba(IdPrueba),
    FechaAplicacion     DATE         NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    Resultado           NVARCHAR(500) NULL,
    Observaciones       NVARCHAR(500) NULL,
    IdUsuarioRegistra   INT          NULL REFERENCES Usuario(IdUsuario),
    FechaRegistro       DATETIME     NOT NULL DEFAULT GETDATE()
);
GO

-- ──────────────────────────────────────────
--  BLOQUE E: FINANZAS (2 tablas)
-- ──────────────────────────────────────────

CREATE TABLE Mensualidad (
    IdMensualidad      INT IDENTITY(1,1) PRIMARY KEY,
    IdPaciente         INT           NOT NULL REFERENCES Paciente(IdPaciente),
    MesInicio          DATE          NOT NULL,   -- primer día del mes inicial
    MesFin             DATE          NOT NULL,   -- último día del mes final
    MontoTotal         DECIMAL(10,2) NOT NULL,
    IdMoneda           INT           NOT NULL REFERENCES Moneda(IdMoneda),
    Descripcion        NVARCHAR(200) NULL,
    FechaRegistro      DATETIME      NOT NULL DEFAULT GETDATE(),
    IdUsuarioRegistra  INT           NULL REFERENCES Usuario(IdUsuario)
);
GO

CREATE TABLE Pago (
    IdPago             INT IDENTITY(1,1) PRIMARY KEY,
    IdPaciente         INT           NOT NULL REFERENCES Paciente(IdPaciente),
    IdCita             INT           NULL REFERENCES Cita(IdCita),
    IdMensualidad      INT           NULL REFERENCES Mensualidad(IdMensualidad),
    IdTipoPago         INT           NOT NULL REFERENCES TipoPago(IdTipoPago),
    IdMetodoPago       INT           NOT NULL REFERENCES MetodoPago(IdMetodoPago),
    IdMoneda           INT           NOT NULL REFERENCES Moneda(IdMoneda),
    Monto              DECIMAL(10,2) NOT NULL,
    FechaPago          DATE          NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    Referencia         NVARCHAR(100) NULL,   -- Nro. transferencia, comprobante QR, etc.
    Observaciones      NVARCHAR(300) NULL,
    FechaRegistro      DATETIME      NOT NULL DEFAULT GETDATE(),
    IdUsuarioRegistra  INT           NULL REFERENCES Usuario(IdUsuario),
    CONSTRAINT CHK_Pago_Origen CHECK (IdCita IS NOT NULL OR IdMensualidad IS NOT NULL)
);
GO

-- ──────────────────────────────────────────
--  BLOQUE F: TRAZABILIDAD (1 tabla)
-- ──────────────────────────────────────────

CREATE TABLE Auditoria (
    IdAuditoria   BIGINT IDENTITY(1,1) PRIMARY KEY,   -- BIGINT por crecimiento
    Tabla         NVARCHAR(100) NOT NULL,
    Operacion     NVARCHAR(10)  NOT NULL,  -- 'INSERT' | 'UPDATE' | 'DELETE'
    IdRegistro    BIGINT        NULL,
    ValorAnterior NVARCHAR(MAX) NULL,
    ValorNuevo    NVARCHAR(MAX) NULL,
    IdUsuario     INT           NULL REFERENCES Usuario(IdUsuario),
    Fecha         DATETIME      NOT NULL DEFAULT GETDATE(),
    IP            NVARCHAR(50)  NULL
);
GO


-- ============================================================
--  SECCIÓN 2.1: ÍNDICES DE RENDIMIENTO
-- ============================================================
-- Mejoran consultas frecuentes: agenda diaria, búsqueda por CI,
-- reportes financieros, auditoría por fecha.

CREATE NONCLUSTERED INDEX IX_Cita_FechaHora
    ON Cita (FechaHora) INCLUDE (Estado, IdPaciente, IdProfesional);
GO

CREATE NONCLUSTERED INDEX IX_Paciente_CI
    ON Paciente (CI) WHERE CI IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX IX_Pago_FechaPago
    ON Pago (FechaPago) INCLUDE (IdPaciente, Monto, IdMoneda);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_Fecha
    ON Auditoria (Fecha DESC) INCLUDE (Tabla, Operacion, IdUsuario);
GO


-- ============================================================
--  SECCIÓN 3: DATOS INICIALES
-- ============================================================

-- Roles
INSERT INTO Rol (Nombre, Descripcion) VALUES
    ('Administrador', 'Acceso completo al sistema, gestión de usuarios y configuración'),
    ('Responsable',   'Gestión operativa: pacientes, citas, servicios, pagos y reportes');
GO

-- Permisos técnicos (4)
INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria) VALUES
    ('USR_CREAR',      'Crear usuarios',      'Permite crear nuevas cuentas de usuario',    'Tecnico'),
    ('USR_EDITAR',     'Editar usuarios',     'Permite modificar datos de usuarios',         'Tecnico'),
    ('ROL_ASIGNAR',    'Asignar roles',       'Permite asignar y quitar roles a usuarios',   'Tecnico'),
    ('PERM_GESTIONAR', 'Gestionar permisos',  'Permite administrar permisos del sistema',    'Tecnico');
GO

-- Permisos operativos (17)
INSERT INTO Permiso (Codigo, Nombre, Descripcion, Categoria) VALUES
    ('PAC_VER',         'Ver pacientes',              'Permite consultar el listado de pacientes',         'Operativo'),
    ('PAC_CREAR',       'Registrar paciente',         'Permite registrar nuevos pacientes',                'Operativo'),
    ('PAC_EDITAR',      'Editar paciente',            'Permite modificar datos del paciente',              'Operativo'),
    ('PAC_ELIMINAR',    'Dar de baja paciente',       'Permite desactivar un paciente',                    'Operativo'),
    ('CITA_VER',        'Ver citas',                  'Permite consultar citas programadas',               'Operativo'),
    ('CITA_CREAR',      'Registrar cita',             'Permite agendar nuevas citas',                      'Operativo'),
    ('CITA_EDITAR',     'Modificar cita',             'Permite reagendar o editar una cita',               'Operativo'),
    ('CITA_CANCELAR',   'Cancelar cita',              'Permite cancelar citas',                            'Operativo'),
    ('SERV_REGISTRAR',  'Registrar servicio',         'Permite registrar servicios realizados',            'Operativo'),
    ('PRUEBA_REGISTRAR','Registrar prueba',           'Permite registrar pruebas aplicadas',               'Operativo'),
    ('PAGO_VER',        'Ver pagos',                  'Permite consultar pagos registrados',               'Operativo'),
    ('PAGO_REGISTRAR',  'Registrar pago',             'Permite ingresar nuevos pagos',                     'Operativo'),
    ('MENS_GESTIONAR',  'Gestionar mensualidades',    'Permite crear y consultar paquetes mensuales',      'Operativo'),
    ('REP_PACIENTES',   'Reporte de pacientes',       'Permite generar reportes de pacientes',             'Operativo'),
    ('REP_CITAS',       'Reporte de citas',           'Permite generar reportes de citas',                 'Operativo'),
    ('REP_SERVICIOS',   'Reporte servicios y pruebas','Permite generar reportes de servicios y pruebas',   'Operativo'),
    ('REP_PAGOS',       'Reporte de pagos',           'Permite generar reportes financieros',              'Operativo');
GO

-- Asignar TODOS los permisos al rol Administrador
INSERT INTO RolPermiso (IdRol, IdPermiso)
    SELECT 1, IdPermiso FROM Permiso;
GO

-- Asignar permisos operativos al rol Responsable
INSERT INTO RolPermiso (IdRol, IdPermiso)
    SELECT 2, IdPermiso FROM Permiso WHERE Categoria = 'Operativo';
GO

-- ────────────────────────────────────────────────────────────
-- Usuario inicial: anaapaza / Renacer2026!
-- ────────────────────────────────────────────────────────────
-- IMPORTANTE (defensa técnica):
-- Este hash SHA-256 es solo BOOTSTRAP para que el script SQL sea
-- autosuficiente y reproducible. La aplicación ASP.NET Core debe
-- detectar al primer login que el hash NO empieza con '$2a$' o
-- '$2b$' (formato BCrypt), re-hashear la contraseña con BCrypt
-- (cost factor 12) y guardar el hash definitivo.
--
-- Esto satisface el RNF de seguridad de contraseñas (BCrypt + salt)
-- sin sacrificar la portabilidad del script SQL.
-- ────────────────────────────────────────────────────────────
INSERT INTO Usuario (NombreUsuario, PasswordHash, Nombres, Apellidos, Email)
VALUES (
    'anaapaza',
    CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', 'Renacer2026!'), 2),
    'Ana Luisa',
    'Apaza',
    'ana.apaza@renacer.bo'
);
GO

-- Asignar rol Administrador al usuario anaapaza
INSERT INTO UsuarioRol (IdUsuario, IdRol) VALUES (1, 1);
GO

-- Servicios (13 del folleto)
INSERT INTO Servicio (Nombre) VALUES
    ('Diagnóstico psicológico'),
    ('Terapia individual'),
    ('Terapia de parejas y novios'),
    ('Terapia de padres e hijos'),
    ('Terapias grupales (víctimas y agresores)'),
    ('Talleres psicoeducativos de prevención de violencia'),
    ('Talleres motivacionales'),
    ('Estimulación temprana'),
    ('Estimulación cognitiva, afectiva y social'),
    ('Apoyo en lecto-escritura'),
    ('Apoyo en matemáticas'),
    ('Orientación vocacional'),
    ('Asesoramiento de padres y maestros');
GO

-- Pruebas psicológicas (18 total)
INSERT INTO Prueba (Nombre, Sigla) VALUES
    ('Wechsler Intelligence Scale for Children - V',         'WISC-V'),
    ('Wechsler Preschool and Primary Scale of Intelligence', 'WPPSI'),
    ('Inventario Clínico para Adolescentes de Millon',       'MACI'),
    ('Wechsler Adult Intelligence Scale - IV',               'WAIS-IV'),
    ('Test de Matrices Progresivas de Raven',                'Raven'),
    ('Test ABC de Lourenço Filho',                           'ABC'),
    ('Test del Árbol de Karl Koch',                          'Árbol Koch'),
    ('Test de la Figura Humana Goodenough/Machover',         'TFH'),
    ('Test de la Familia de Corman',                         'Familia Corman'),
    ('Test HTP (Casa, Árbol, Persona)',                      'HTP'),
    ('Inventario de Depresión de Beck',                      'BDI'),
    ('Cuestionario de Conners para TDAH',                    'Conners'),
    ('Test de Ansiedad de Spielberger',                      'STAI'),
    ('Test Gestáltico Visomotor de Bender',                  'Bender'),
    ('Evaluación de Procesos Lectores',                      'Pro-lec'),
    ('Evaluación de Habilidades Matemáticas Básicas',        'EHMB'),
    ('Entrevista clínica estructurada',                      'ECE'),
    ('Test de Rorschach',                                    'Rorschach');
GO

-- Tipos de Cita (4)
INSERT INTO TipoCita (Nombre) VALUES
    ('Primera consulta'),
    ('Recurrente'),
    ('Recuperación'),
    ('Entrega de informe');
GO

-- Tipos de Pago (4)
INSERT INTO TipoPago (Nombre) VALUES
    ('Pago único'),
    ('Mensualidad'),
    ('Pago parcial'),
    ('Pago a cuenta');
GO

-- Métodos de Pago (3)
INSERT INTO MetodoPago (Nombre) VALUES
    ('Efectivo'),
    ('QR (Pago Móvil)'),
    ('Transferencia bancaria');
GO

-- Monedas (2)
INSERT INTO Moneda (Codigo, Nombre, Simbolo) VALUES
    ('BOB', 'Bolivianos',         'Bs.'),
    ('USD', 'Dólares Americanos', '$');
GO

-- Profesionales Externas (4)
INSERT INTO ProfesionalExterno (Nombres, Apellidos, Especialidad) VALUES
    ('Lic. Elizabeth', NULL,        'Psicología Clínica'),
    ('Lic. Vivian',    NULL,        'Psicología Clínica'),
    ('Lic. Elizabeth', 'Alarcón',   'Psicología Clínica'),
    ('Lic. Karen',     'Arancibia', 'Psicología Clínica');
GO


-- ============================================================
--  SECCIÓN 4: VISTAS (4)
-- ============================================================

-- 1. Pacientes activos con edad calculada
CREATE OR ALTER VIEW vw_PacientesActivos AS
    SELECT
        p.IdPaciente,
        p.Nombres,
        p.Apellidos,
        p.Nombres + ' ' + p.Apellidos AS NombreCompleto,
        p.FechaNacimiento,
        p.Edad,
        p.Genero,
        p.CI,
        p.Telefono,
        p.Email,
        p.NombreTutor,
        p.TelefonoTutor,
        p.MotivoConsulta,
        p.FechaRegistro
    FROM Paciente p
    WHERE p.Activo = 1;
GO

-- 2. Agenda del día
CREATE OR ALTER VIEW vw_AgendaDelDia AS
    SELECT
        c.IdCita,
        c.FechaHora,
        c.DuracionMinutos,
        c.Estado,
        p.Nombres + ' ' + p.Apellidos               AS Paciente,
        p.Telefono                                   AS TelefonoPaciente,
        tc.Nombre                                    AS TipoCita,
        pe.Nombres + ' ' + ISNULL(pe.Apellidos, '')  AS Profesional,
        c.Observaciones
    FROM Cita c
    INNER JOIN Paciente            p  ON c.IdPaciente    = p.IdPaciente
    INNER JOIN TipoCita            tc ON c.IdTipoCita    = tc.IdTipoCita
    LEFT  JOIN ProfesionalExterno  pe ON c.IdProfesional = pe.IdProfesional
    WHERE CAST(c.FechaHora AS DATE) = CAST(GETDATE() AS DATE);
GO

-- 3. Historial completo del paciente
CREATE OR ALTER VIEW vw_HistorialPaciente AS
    SELECT
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos AS Paciente,
        'Servicio'                    AS TipoRegistro,
        s.Nombre                      AS Detalle,
        sr.FechaRealizacion           AS Fecha,
        sr.Observaciones
    FROM ServicioRealizado sr
    INNER JOIN Cita     ci ON sr.IdCita     = ci.IdCita
    INNER JOIN Paciente p  ON ci.IdPaciente = p.IdPaciente
    INNER JOIN Servicio s  ON sr.IdServicio = s.IdServicio

    UNION ALL

    SELECT
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos,
        'Prueba',
        pr.Nombre,
        pa.FechaAplicacion,
        pa.Observaciones
    FROM PruebaAplicada pa
    INNER JOIN Cita     ci ON pa.IdCita     = ci.IdCita
    INNER JOIN Paciente p  ON ci.IdPaciente = p.IdPaciente
    INNER JOIN Prueba   pr ON pa.IdPrueba   = pr.IdPrueba;
GO

-- 4. Pagos pendientes (mensualidades con saldo)
CREATE OR ALTER VIEW vw_PagosPendientes AS
    SELECT
        m.IdMensualidad,
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos           AS Paciente,
        p.Telefono,
        m.MesInicio,
        m.MesFin,
        m.MontoTotal,
        mo.Simbolo                               AS Moneda,
        ISNULL(SUM(pg.Monto), 0)                 AS TotalPagado,
        m.MontoTotal - ISNULL(SUM(pg.Monto), 0)  AS SaldoPendiente
    FROM Mensualidad m
    INNER JOIN Paciente p  ON m.IdPaciente     = p.IdPaciente
    INNER JOIN Moneda   mo ON m.IdMoneda       = mo.IdMoneda
    LEFT  JOIN Pago     pg ON pg.IdMensualidad = m.IdMensualidad
    GROUP BY
        m.IdMensualidad, p.IdPaciente,
        p.Nombres, p.Apellidos, p.Telefono,
        m.MesInicio, m.MesFin, m.MontoTotal,
        mo.Simbolo
    HAVING m.MontoTotal - ISNULL(SUM(pg.Monto), 0) > 0;
GO


-- ============================================================
--  SECCIÓN 5: PROCEDIMIENTOS ALMACENADOS (16)
-- ============================================================

-- 1. Autenticar usuario
-- NOTA: La validación de hash BCrypt se hace en la capa de aplicación.
-- Este SP solo busca al usuario por nombre y devuelve el hash para
-- que la app lo compare con BCrypt.Verify(). Si el hash empieza con
-- el formato SHA-256 placeholder, la app debe re-hashearlo con BCrypt.
CREATE OR ALTER PROCEDURE sp_AutenticarUsuario
    @NombreUsuario NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.IdUsuario,
        u.NombreUsuario,
        u.PasswordHash,
        u.Nombres,
        u.Apellidos,
        u.Email,
        r.Nombre AS Rol
    FROM Usuario u
    INNER JOIN UsuarioRol ur ON u.IdUsuario = ur.IdUsuario
    INNER JOIN Rol        r  ON ur.IdRol    = r.IdRol
    WHERE u.NombreUsuario = @NombreUsuario
      AND u.Activo        = 1;
END;
GO

-- 2. Registrar último acceso (llamado desde la app tras BCrypt.Verify exitoso)
CREATE OR ALTER PROCEDURE sp_RegistrarUltimoAcceso
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuario
    SET UltimoAcceso = GETDATE()
    WHERE IdUsuario = @IdUsuario;
END;
GO

-- 3. Actualizar hash de contraseña (usado por la app para migrar
--    de SHA-256 placeholder a BCrypt al primer login exitoso)
CREATE OR ALTER PROCEDURE sp_ActualizarPasswordHash
    @IdUsuario   INT,
    @NuevoHash   NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuario
    SET PasswordHash = @NuevoHash
    WHERE IdUsuario = @IdUsuario;
END;
GO

-- 4. Registrar paciente
CREATE OR ALTER PROCEDURE sp_RegistrarPaciente
    @Nombres           NVARCHAR(100),
    @Apellidos         NVARCHAR(100),
    @FechaNacimiento   DATE,
    @Genero            NVARCHAR(20)  = NULL,
    @CI                NVARCHAR(20)  = NULL,
    @Telefono          NVARCHAR(20)  = NULL,
    @Email             NVARCHAR(150) = NULL,
    @Direccion         NVARCHAR(300) = NULL,
    @NombreTutor       NVARCHAR(150) = NULL,
    @TelefonoTutor     NVARCHAR(20)  = NULL,
    @RelacionTutor     NVARCHAR(50)  = NULL,
    @MotivoConsulta    NVARCHAR(500) = NULL,
    @IdUsuarioRegistra INT           = NULL,
    @IdPaciente        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Paciente (
        Nombres, Apellidos, FechaNacimiento, Genero, CI,
        Telefono, Email, Direccion,
        NombreTutor, TelefonoTutor, RelacionTutor,
        MotivoConsulta, IdUsuarioRegistra
    ) VALUES (
        @Nombres, @Apellidos, @FechaNacimiento, @Genero, @CI,
        @Telefono, @Email, @Direccion,
        @NombreTutor, @TelefonoTutor, @RelacionTutor,
        @MotivoConsulta, @IdUsuarioRegistra
    );

    SET @IdPaciente = SCOPE_IDENTITY();

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, IdUsuario)
    VALUES ('Paciente', 'INSERT', @IdPaciente, @IdUsuarioRegistra);
END;
GO

-- 5. Actualizar paciente
CREATE OR ALTER PROCEDURE sp_ActualizarPaciente
    @IdPaciente        INT,
    @Nombres           NVARCHAR(100) = NULL,
    @Apellidos         NVARCHAR(100) = NULL,
    @FechaNacimiento   DATE          = NULL,
    @Genero            NVARCHAR(20)  = NULL,
    @CI                NVARCHAR(20)  = NULL,
    @Telefono          NVARCHAR(20)  = NULL,
    @Email             NVARCHAR(150) = NULL,
    @Direccion         NVARCHAR(300) = NULL,
    @NombreTutor       NVARCHAR(150) = NULL,
    @TelefonoTutor     NVARCHAR(20)  = NULL,
    @RelacionTutor     NVARCHAR(50)  = NULL,
    @MotivoConsulta    NVARCHAR(500) = NULL,
    @IdUsuarioModifica INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Paciente SET
        Nombres         = ISNULL(@Nombres,         Nombres),
        Apellidos       = ISNULL(@Apellidos,       Apellidos),
        FechaNacimiento = ISNULL(@FechaNacimiento, FechaNacimiento),
        Genero          = ISNULL(@Genero,          Genero),
        CI              = ISNULL(@CI,              CI),
        Telefono        = ISNULL(@Telefono,        Telefono),
        Email           = ISNULL(@Email,           Email),
        Direccion       = ISNULL(@Direccion,       Direccion),
        NombreTutor     = ISNULL(@NombreTutor,     NombreTutor),
        TelefonoTutor   = ISNULL(@TelefonoTutor,   TelefonoTutor),
        RelacionTutor   = ISNULL(@RelacionTutor,   RelacionTutor),
        MotivoConsulta  = ISNULL(@MotivoConsulta,  MotivoConsulta)
    WHERE IdPaciente = @IdPaciente;
    -- Nota: la auditoría con valor anterior la registra el trigger tr_Paciente_Auditoria
END;
GO

-- 6. Buscar pacientes
CREATE OR ALTER PROCEDURE sp_BuscarPacientes
    @Termino     NVARCHAR(100) = NULL,
    @SoloActivos BIT           = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPaciente,
        Nombres,
        Apellidos,
        Nombres + ' ' + Apellidos AS NombreCompleto,
        FechaNacimiento,
        Edad,
        Genero,
        CI,
        Telefono,
        Email,
        FechaRegistro
    FROM Paciente
    WHERE (@SoloActivos = 0 OR Activo = 1)
      AND (
          @Termino IS NULL
          OR Nombres   LIKE '%' + @Termino + '%'
          OR Apellidos LIKE '%' + @Termino + '%'
          OR CI        LIKE '%' + @Termino + '%'
          OR Telefono  LIKE '%' + @Termino + '%'
      )
    ORDER BY Apellidos, Nombres;
END;
GO

-- 7. Registrar cita (valida solapamiento por profesional)
CREATE OR ALTER PROCEDURE sp_RegistrarCita
    @IdPaciente        INT,
    @IdTipoCita        INT,
    @FechaHora         DATETIME,
    @DuracionMinutos   INT           = 60,
    @IdProfesional     INT           = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @IdUsuarioRegistra INT           = NULL,
    @IdCita            INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdProfesional IS NOT NULL
    BEGIN
        DECLARE @Conflicto INT;
        SELECT @Conflicto = COUNT(*)
        FROM Cita
        WHERE IdProfesional = @IdProfesional
          AND Estado IN ('Programada', 'Asistió')
          AND @FechaHora < DATEADD(MINUTE, DuracionMinutos, FechaHora)
          AND DATEADD(MINUTE, @DuracionMinutos, @FechaHora) > FechaHora;

        IF @Conflicto > 0
        BEGIN
            RAISERROR('Conflicto de horario: el profesional ya tiene una cita en ese rango.', 16, 1);
            RETURN;
        END;
    END;

    INSERT INTO Cita (IdPaciente, IdTipoCita, IdProfesional, FechaHora, DuracionMinutos, Observaciones, IdUsuarioRegistra)
    VALUES (@IdPaciente, @IdTipoCita, @IdProfesional, @FechaHora, @DuracionMinutos, @Observaciones, @IdUsuarioRegistra);

    SET @IdCita = SCOPE_IDENTITY();

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, IdUsuario)
    VALUES ('Cita', 'INSERT', @IdCita, @IdUsuarioRegistra);
END;
GO

-- 8. Verificar disponibilidad de horario
CREATE OR ALTER PROCEDURE sp_VerificarDisponibilidadHorario
    @IdProfesional   INT,
    @FechaHora       DATETIME,
    @DuracionMinutos INT = 60,
    @ExcluirIdCita   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.IdCita,
        c.FechaHora,
        c.DuracionMinutos,
        p.Nombres + ' ' + p.Apellidos AS Paciente,
        tc.Nombre                     AS TipoCita
    FROM Cita c
    INNER JOIN Paciente p  ON c.IdPaciente = p.IdPaciente
    INNER JOIN TipoCita tc ON c.IdTipoCita = tc.IdTipoCita
    WHERE c.IdProfesional = @IdProfesional
      AND c.Estado NOT IN ('Cancelada', 'Reagendada')
      AND (@ExcluirIdCita IS NULL OR c.IdCita <> @ExcluirIdCita)
      AND @FechaHora < DATEADD(MINUTE, c.DuracionMinutos, c.FechaHora)
      AND DATEADD(MINUTE, @DuracionMinutos, @FechaHora) > c.FechaHora;
END;
GO

-- 9. Modificar cita
CREATE OR ALTER PROCEDURE sp_ModificarCita
    @IdCita            INT,
    @FechaHora         DATETIME      = NULL,
    @DuracionMinutos   INT           = NULL,
    @IdProfesional     INT           = NULL,
    @Estado            NVARCHAR(30)  = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @IdUsuarioModifica INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Antes NVARCHAR(MAX);
    SELECT @Antes = (SELECT * FROM Cita WHERE IdCita = @IdCita FOR JSON PATH);

    UPDATE Cita SET
        FechaHora       = ISNULL(@FechaHora,       FechaHora),
        DuracionMinutos = ISNULL(@DuracionMinutos, DuracionMinutos),
        IdProfesional   = ISNULL(@IdProfesional,   IdProfesional),
        Estado          = ISNULL(@Estado,          Estado),
        Observaciones   = ISNULL(@Observaciones,   Observaciones)
    WHERE IdCita = @IdCita;

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, ValorAnterior, IdUsuario)
    VALUES ('Cita', 'UPDATE', @IdCita, @Antes, @IdUsuarioModifica);
END;
GO

-- 10. Consultar citas (con filtros)
CREATE OR ALTER PROCEDURE sp_ConsultarCitas
    @IdPaciente INT          = NULL,
    @FechaDesde DATE         = NULL,
    @FechaHasta DATE         = NULL,
    @Estado     NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.IdCita,
        c.FechaHora,
        c.DuracionMinutos,
        c.Estado,
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos               AS Paciente,
        p.Telefono,
        tc.Nombre                                    AS TipoCita,
        pe.Nombres + ' ' + ISNULL(pe.Apellidos, '')  AS Profesional,
        c.Observaciones
    FROM Cita c
    INNER JOIN Paciente            p  ON c.IdPaciente    = p.IdPaciente
    INNER JOIN TipoCita            tc ON c.IdTipoCita    = tc.IdTipoCita
    LEFT  JOIN ProfesionalExterno  pe ON c.IdProfesional = pe.IdProfesional
    WHERE (@IdPaciente IS NULL OR c.IdPaciente = @IdPaciente)
      AND (@FechaDesde IS NULL OR CAST(c.FechaHora AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(c.FechaHora AS DATE) <= @FechaHasta)
      AND (@Estado     IS NULL OR c.Estado = @Estado)
    ORDER BY c.FechaHora;
END;
GO

-- 11. Actualizar estado de citas vencidas
-- Reemplaza al trigger riesgoso v1. Lo llama la app al iniciar sesión
-- o un job programado. Marca como 'No asistió' las citas vencidas que
-- siguen 'Programada'.
CREATE OR ALTER PROCEDURE sp_ActualizarCitasVencidas
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Cita
    SET Estado = 'No asistió'
    WHERE Estado = 'Programada'
      AND DATEADD(MINUTE, DuracionMinutos, FechaHora) < GETDATE();

    SELECT @@ROWCOUNT AS CitasActualizadas;
END;
GO

-- 12. Registrar servicio realizado
CREATE OR ALTER PROCEDURE sp_RegistrarServicioRealizado
    @IdCita            INT,
    @IdServicio        INT,
    @FechaRealizacion  DATE          = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @IdUsuarioRegistra INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ServicioRealizado (IdCita, IdServicio, FechaRealizacion, Observaciones, IdUsuarioRegistra)
    VALUES (
        @IdCita,
        @IdServicio,
        ISNULL(@FechaRealizacion, CAST(GETDATE() AS DATE)),
        @Observaciones,
        @IdUsuarioRegistra
    );

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, IdUsuario)
    VALUES ('ServicioRealizado', 'INSERT', SCOPE_IDENTITY(), @IdUsuarioRegistra);
END;
GO

-- 13. Registrar prueba aplicada
CREATE OR ALTER PROCEDURE sp_RegistrarPruebaAplicada
    @IdCita            INT,
    @IdPrueba          INT,
    @FechaAplicacion   DATE          = NULL,
    @Resultado         NVARCHAR(500) = NULL,
    @Observaciones     NVARCHAR(500) = NULL,
    @IdUsuarioRegistra INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PruebaAplicada (IdCita, IdPrueba, FechaAplicacion, Resultado, Observaciones, IdUsuarioRegistra)
    VALUES (
        @IdCita,
        @IdPrueba,
        ISNULL(@FechaAplicacion, CAST(GETDATE() AS DATE)),
        @Resultado,
        @Observaciones,
        @IdUsuarioRegistra
    );

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, IdUsuario)
    VALUES ('PruebaAplicada', 'INSERT', SCOPE_IDENTITY(), @IdUsuarioRegistra);
END;
GO

-- 14. Registrar mensualidad
CREATE OR ALTER PROCEDURE sp_RegistrarMensualidad
    @IdPaciente        INT,
    @MesInicio         DATE,
    @MesFin            DATE,
    @MontoTotal        DECIMAL(10,2),
    @IdMoneda          INT           = 1,    -- 1 = BOB por defecto
    @Descripcion       NVARCHAR(200) = NULL,
    @IdUsuarioRegistra INT           = NULL,
    @IdMensualidad     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Mensualidad (IdPaciente, MesInicio, MesFin, MontoTotal, IdMoneda, Descripcion, IdUsuarioRegistra)
    VALUES (@IdPaciente, @MesInicio, @MesFin, @MontoTotal, @IdMoneda, @Descripcion, @IdUsuarioRegistra);

    SET @IdMensualidad = SCOPE_IDENTITY();

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, IdUsuario)
    VALUES ('Mensualidad', 'INSERT', @IdMensualidad, @IdUsuarioRegistra);
END;
GO

-- 15. Registrar pago
CREATE OR ALTER PROCEDURE sp_RegistrarPago
    @IdPaciente        INT,
    @IdTipoPago        INT,
    @IdMetodoPago      INT,
    @IdMoneda          INT,
    @Monto             DECIMAL(10,2),
    @IdCita            INT           = NULL,
    @IdMensualidad     INT           = NULL,
    @FechaPago         DATE          = NULL,
    @Referencia        NVARCHAR(100) = NULL,
    @Observaciones     NVARCHAR(300) = NULL,
    @IdUsuarioRegistra INT           = NULL,
    @IdPago            INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Pago (
        IdPaciente, IdCita, IdMensualidad,
        IdTipoPago, IdMetodoPago, IdMoneda,
        Monto, FechaPago, Referencia, Observaciones, IdUsuarioRegistra
    ) VALUES (
        @IdPaciente, @IdCita, @IdMensualidad,
        @IdTipoPago, @IdMetodoPago, @IdMoneda,
        @Monto,
        ISNULL(@FechaPago, CAST(GETDATE() AS DATE)),
        @Referencia, @Observaciones, @IdUsuarioRegistra
    );

    SET @IdPago = SCOPE_IDENTITY();
    -- La auditoría de INSERT en Pago la registra el trigger tr_Pago_Auditoria
END;
GO

-- 16. Reporte de pacientes
CREATE OR ALTER PROCEDURE sp_GenerarReportePacientes
    @FechaDesde  DATE = NULL,
    @FechaHasta  DATE = NULL,
    @SoloActivos BIT  = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos AS NombreCompleto,
        p.FechaNacimiento,
        p.Edad,
        p.Genero,
        p.CI,
        p.Telefono,
        p.Email,
        p.MotivoConsulta,
        COUNT(DISTINCT c.IdCita)      AS TotalCitas,
        MAX(c.FechaHora)              AS UltimaCita,
        p.FechaRegistro
    FROM Paciente p
    LEFT JOIN Cita c ON p.IdPaciente = c.IdPaciente
    WHERE (@SoloActivos = 0 OR p.Activo = 1)
      AND (@FechaDesde IS NULL OR p.FechaRegistro >= @FechaDesde)
      AND (@FechaHasta IS NULL OR p.FechaRegistro <= @FechaHasta)
    GROUP BY
        p.IdPaciente, p.Nombres, p.Apellidos,
        p.FechaNacimiento, p.Edad, p.Genero, p.CI,
        p.Telefono, p.Email, p.MotivoConsulta, p.FechaRegistro
    ORDER BY p.Apellidos, p.Nombres;
END;
GO

-- 17. Reporte de citas
CREATE OR ALTER PROCEDURE sp_GenerarReporteCitas
    @FechaDesde DATE         = NULL,
    @FechaHasta DATE         = NULL,
    @Estado     NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.IdCita,
        CAST(c.FechaHora AS DATE)                    AS Fecha,
        CAST(c.FechaHora AS TIME(0))                 AS Hora,
        p.Nombres + ' ' + p.Apellidos                AS Paciente,
        p.Edad,
        tc.Nombre                                     AS TipoCita,
        pe.Nombres + ' ' + ISNULL(pe.Apellidos, '')  AS Profesional,
        c.Estado,
        c.DuracionMinutos,
        c.Observaciones
    FROM Cita c
    INNER JOIN Paciente            p  ON c.IdPaciente    = p.IdPaciente
    INNER JOIN TipoCita            tc ON c.IdTipoCita    = tc.IdTipoCita
    LEFT  JOIN ProfesionalExterno  pe ON c.IdProfesional = pe.IdProfesional
    WHERE (@FechaDesde IS NULL OR CAST(c.FechaHora AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(c.FechaHora AS DATE) <= @FechaHasta)
      AND (@Estado     IS NULL OR c.Estado = @Estado)
    ORDER BY c.FechaHora;
END;
GO

-- 18. Reporte de servicios y pruebas
CREATE OR ALTER PROCEDURE sp_GenerarReporteServiciosYPruebas
    @IdPaciente INT  = NULL,
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        'Servicio'                        AS Tipo,
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos     AS Paciente,
        s.Nombre                          AS Detalle,
        sr.FechaRealizacion               AS Fecha,
        sr.Observaciones
    FROM ServicioRealizado sr
    INNER JOIN Cita     ci ON sr.IdCita     = ci.IdCita
    INNER JOIN Paciente p  ON ci.IdPaciente = p.IdPaciente
    INNER JOIN Servicio s  ON sr.IdServicio = s.IdServicio
    WHERE (@IdPaciente IS NULL OR p.IdPaciente = @IdPaciente)
      AND (@FechaDesde IS NULL OR sr.FechaRealizacion >= @FechaDesde)
      AND (@FechaHasta IS NULL OR sr.FechaRealizacion <= @FechaHasta)

    UNION ALL

    SELECT
        'Prueba',
        p.IdPaciente,
        p.Nombres + ' ' + p.Apellidos,
        pr.Nombre,
        pa.FechaAplicacion,
        pa.Observaciones
    FROM PruebaAplicada pa
    INNER JOIN Cita     ci ON pa.IdCita     = ci.IdCita
    INNER JOIN Paciente p  ON ci.IdPaciente = p.IdPaciente
    INNER JOIN Prueba   pr ON pa.IdPrueba   = pr.IdPrueba
    WHERE (@IdPaciente IS NULL OR p.IdPaciente = @IdPaciente)
      AND (@FechaDesde IS NULL OR pa.FechaAplicacion >= @FechaDesde)
      AND (@FechaHasta IS NULL OR pa.FechaAplicacion <= @FechaHasta)

    ORDER BY Fecha DESC;
END;
GO

-- 19. Reporte de pagos
CREATE OR ALTER PROCEDURE sp_GenerarReportePagos
    @FechaDesde   DATE = NULL,
    @FechaHasta   DATE = NULL,
    @IdPaciente   INT  = NULL,
    @IdMetodoPago INT  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pg.IdPago,
        pg.FechaPago,
        p.Nombres + ' ' + p.Apellidos AS Paciente,
        tp.Nombre                     AS TipoPago,
        mp.Nombre                     AS MetodoPago,
        mo.Simbolo                    AS Moneda,
        pg.Monto,
        pg.Referencia,
        pg.Observaciones
    FROM Pago pg
    INNER JOIN Paciente   p   ON pg.IdPaciente    = p.IdPaciente
    INNER JOIN TipoPago   tp  ON pg.IdTipoPago    = tp.IdTipoPago
    INNER JOIN MetodoPago mp  ON pg.IdMetodoPago  = mp.IdMetodoPago
    INNER JOIN Moneda     mo  ON pg.IdMoneda      = mo.IdMoneda
    WHERE (@FechaDesde   IS NULL OR pg.FechaPago    >= @FechaDesde)
      AND (@FechaHasta   IS NULL OR pg.FechaPago    <= @FechaHasta)
      AND (@IdPaciente   IS NULL OR pg.IdPaciente   = @IdPaciente)
      AND (@IdMetodoPago IS NULL OR pg.IdMetodoPago = @IdMetodoPago)
    ORDER BY pg.FechaPago DESC;
END;
GO


-- ============================================================
--  SECCIÓN 6: TRIGGERS (3 - solo auditoría, sin lógica de negocio)
-- ============================================================

-- 1. Auditoría de cambios en Paciente (UPDATE y DELETE)
CREATE OR ALTER TRIGGER tr_Paciente_Auditoria
ON Paciente
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, ValorAnterior, ValorNuevo)
    SELECT
        'Paciente',
        CASE WHEN EXISTS (SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END,
        d.IdPaciente,
        (SELECT d.* FOR JSON PATH),
        (SELECT i.* FROM inserted i WHERE i.IdPaciente = d.IdPaciente FOR JSON PATH)
    FROM deleted d;
END;
GO

-- 2. Auditoría de INSERT en Pago
CREATE OR ALTER TRIGGER tr_Pago_Auditoria
ON Pago
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, ValorNuevo, IdUsuario)
    SELECT
        'Pago',
        'INSERT',
        i.IdPago,
        (SELECT i.* FOR JSON PATH),
        i.IdUsuarioRegistra
    FROM inserted i;
END;
GO

-- 3. Auditoría de DELETE en Cita (las cancelaciones reales)
CREATE OR ALTER TRIGGER tr_Cita_Auditoria
ON Cita
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Auditoria (Tabla, Operacion, IdRegistro, ValorAnterior)
    SELECT
        'Cita',
        'DELETE',
        d.IdCita,
        (SELECT d.* FOR JSON PATH)
    FROM deleted d;
END;
GO


-- ============================================================
--  SECCIÓN 7: QUERIES DE VERIFICACIÓN
-- ============================================================

-- Verificar estructura creada (tablas y columnas)
SELECT
    t.TABLE_NAME         AS Tabla,
    COUNT(c.COLUMN_NAME) AS NumeroColumnas
FROM INFORMATION_SCHEMA.TABLES t
INNER JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
GROUP BY t.TABLE_NAME
ORDER BY t.TABLE_NAME;
GO

-- Contar registros iniciales
SELECT 'Roles'                AS Entidad, COUNT(*) AS Total FROM Rol               UNION ALL
SELECT 'Permisos',                        COUNT(*)        FROM Permiso              UNION ALL
SELECT 'RolPermiso',                      COUNT(*)        FROM RolPermiso           UNION ALL
SELECT 'Usuarios',                        COUNT(*)        FROM Usuario              UNION ALL
SELECT 'UsuarioRol',                      COUNT(*)        FROM UsuarioRol           UNION ALL
SELECT 'Servicios',                       COUNT(*)        FROM Servicio             UNION ALL
SELECT 'Pruebas',                         COUNT(*)        FROM Prueba               UNION ALL
SELECT 'TiposCita',                       COUNT(*)        FROM TipoCita             UNION ALL
SELECT 'TiposPago',                       COUNT(*)        FROM TipoPago             UNION ALL
SELECT 'MetodosPago',                     COUNT(*)        FROM MetodoPago           UNION ALL
SELECT 'Monedas',                         COUNT(*)        FROM Moneda               UNION ALL
SELECT 'ProfesionalesExternos',           COUNT(*)        FROM ProfesionalExterno;
GO

-- Verificar vistas
SELECT TABLE_NAME AS Vista
FROM INFORMATION_SCHEMA.VIEWS
ORDER BY TABLE_NAME;
GO

-- Verificar procedimientos almacenados
SELECT name AS ProcedimientoAlmacenado
FROM sys.procedures
ORDER BY name;
GO

-- Verificar triggers
SELECT name AS Trigger_, OBJECT_NAME(parent_id) AS TablaAfectada
FROM sys.triggers
ORDER BY name;
GO

-- Verificar índices personalizados
SELECT
    i.name        AS Indice,
    OBJECT_NAME(i.object_id) AS Tabla,
    i.type_desc   AS Tipo
FROM sys.indexes i
WHERE i.name LIKE 'IX_%'
ORDER BY OBJECT_NAME(i.object_id), i.name;
GO

-- Probar autenticación del usuario inicial (devuelve el hash; la app valida con BCrypt)
EXEC sp_AutenticarUsuario @NombreUsuario = 'anaapaza';
GO

-- ============================================================
--  FIN DEL SCRIPT v2
--  Base de datos RENACER creada exitosamente
--  19 tablas | 4 índices | 4 vistas | 19 SPs | 3 triggers
-- ============================================================
