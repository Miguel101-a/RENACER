<div align="center">

# 🌳 Sistema RENACER

### Centro de Desarrollo Integral y Psicoterapia
**Sistema web de gestión para el Centro Psicoterapéutico RENACER**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/license-Academic-blue.svg)](LICENSE)
[![Status](https://img.shields.io/badge/status-en%20desarrollo-yellow.svg)]()

</div>

---

## 📖 Acerca del Proyecto

Sistema web desarrollado como **Proyecto de Grado** para la **Universidad UDABOL (Santa Cruz, Bolivia)** en la materia **Taller de Grado I**. Tiene como objetivo digitalizar y optimizar la gestión operativa del **Centro Psicoterapéutico RENACER**, dirigido por la **Lic. Ana Luisa Apaza**, automatizando el registro de pacientes, agendamiento de citas, control de servicios psicológicos, aplicación de pruebas y gestión de pagos.

### 🎯 Objetivo General

Desarrollar un sistema informático web que mejore la gestión integral del Centro Psicoterapéutico RENACER, sustituyendo el registro manual por una solución digital centralizada, segura y accesible.

---

## ✨ Funcionalidades

| Módulo | Descripción |
|--------|-------------|
| 🔐 **Usuarios y Seguridad** | Autenticación, roles, permisos y recuperación de contraseña |
| 👥 **Gestión de Pacientes** | Registro, búsqueda, edición e historial clínico |
| 📅 **Agenda de Citas** | Programación, reprogramación, control de asistencia |
| 🧠 **Servicios y Pruebas** | Registro de terapias y pruebas psicológicas aplicadas |
| 📊 **Reportes** | Reportes de pacientes, citas, servicios y financieros |
| 💰 **Pagos** | Gestión de pagos únicos, mensualidades y pagos parciales |

---

## 🛠️ Stack Tecnológico

- **Backend:** ASP.NET Core MVC 8.0 (C#)
- **Base de Datos:** SQL Server 2022
- **ORM:** Entity Framework Core 8
- **Frontend:** Razor Views + Bootstrap 5 + jQuery
- **Autenticación:** ASP.NET Core Identity + BCrypt
- **IDE:** Visual Studio 2022
- **Control de Versiones:** Git + GitHub
- **Metodología:** SCRUM (4 sprints)

---

## 📐 Arquitectura

El sistema sigue el patrón **MVC (Modelo-Vista-Controlador)** con una arquitectura en capas:

```
┌─────────────────────────────────────────────────┐
│  Capa de Presentación (Views + Controllers)     │
├─────────────────────────────────────────────────┤
│  Capa de Lógica de Negocio (Services)           │
├─────────────────────────────────────────────────┤
│  Capa de Acceso a Datos (Repositorios + EF Core)│
├─────────────────────────────────────────────────┤
│  SQL Server 2022                                │
└─────────────────────────────────────────────────┘
```

---

## 🚀 Instalación y Ejecución

### Requisitos previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server) (Express o superior)
- [SSMS 20](https://learn.microsoft.com/sql/ssms/) o superior
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recomendado) o VS Code

### Pasos de instalación

**1. Clonar el repositorio**

```bash
git clone https://github.com/TU-USUARIO/RENACER.git
cd RENACER
```

**2. Crear la base de datos**

Abrir `docs/BD/RENACER_v2.sql` en SSMS y ejecutarlo contra tu instancia local. El script crea la BD `RENACER` con todas sus tablas, vistas, procedimientos almacenados, triggers e índices.

**3. Configurar la conexión**

Crear el archivo `src/RENACER.Web/appsettings.Development.json` (NO se sube al repo):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU-SERVIDOR\\TU-INSTANCIA;Database=RENACER;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**4. Restaurar paquetes y ejecutar**

```bash
cd src/RENACER.Web
dotnet restore
dotnet run
```

**5. Acceder al sistema**

Abrir el navegador en `https://localhost:7XXX` (el puerto lo indica la consola).

- **Usuario:** `anaapaza`
- **Contraseña:** `Renacer2026!`

---

## 📁 Estructura del Repositorio

```
RENACER/
├── README.md                     ← Este archivo
├── .gitignore                    ← Archivos ignorados por Git
├── LICENSE                       ← Licencia académica
├── docs/
│   ├── BD/
│   │   └── RENACER_v2.sql        ← Script completo de la base de datos
│   ├── diagramas/                ← Diagramas UML, ER, arquitectura
│   └── capturas/                 ← Capturas para Sprint Reviews
│       ├── sprint1/
│       ├── sprint2/
│       ├── sprint3/
│       └── sprint4/
└── src/
    └── RENACER.Web/              ← Código fuente (ASP.NET Core MVC)
```

---

## 📊 Modelo de Datos

La base de datos cuenta con **19 tablas** organizadas en 6 bloques funcionales:

- **Seguridad y Acceso** (5): `Rol`, `Permiso`, `Usuario`, `RolPermiso`, `UsuarioRol`
- **Catálogos** (6): `Servicio`, `Prueba`, `TipoCita`, `TipoPago`, `MetodoPago`, `Moneda`
- **Personas Externas** (1): `ProfesionalExterno`
- **Operación Principal** (4): `Paciente`, `Cita`, `ServicioRealizado`, `PruebaAplicada`
- **Finanzas** (2): `Mensualidad`, `Pago`
- **Trazabilidad** (1): `Auditoria`

**Plus:** 4 índices de rendimiento, 4 vistas, 19 procedimientos almacenados y 3 triggers de auditoría.

---

## 🏃 Metodología SCRUM

El proyecto se desarrolla en **4 sprints** con un total de **215 puntos de historia** distribuidos en **16 Historias de Usuario**:

| Sprint | Foco | HUs | Puntos | Duración |
|--------|------|-----|--------|----------|
| **Sprint 1** | Seguridad y Acceso | HU-01 a HU-05 | 47 | 11.75 días |
| **Sprint 2** | Pacientes y Agenda | HU-06, 07, 09, 10 | 56 | 14 días |
| **Sprint 3** | Servicios, Pruebas, Edición | HU-08, 11, 12, 13 | 66 | 16.5 días |
| **Sprint 4** | Reportes y Pagos | HU-14, 15, 16 | 46 | 11.5 días |

---

## 👤 Autor

**Miguel Valencia Apaza**
Estudiante de Ingeniería de Sistemas
Universidad UDABOL - Santa Cruz, Bolivia
Taller de Grado I - Gestión 2026

---

## 📄 Licencia

Este proyecto es de uso académico para la presentación del Proyecto de Grado en la Universidad UDABOL. Su distribución comercial requiere autorización expresa del autor y del Centro Psicoterapéutico RENACER.

---

## 🙏 Agradecimientos

- **Lic. Ana Luisa Apaza** - Responsable del Centro Psicoterapéutico RENACER, por brindar el contexto, los requerimientos y el acceso a la información del centro.
- **Tutores y docentes de UDABOL** - Por la guía académica.
- **Universidad UDABOL Santa Cruz** - Por el marco institucional del proyecto.

---

<div align="center">

**🌳 Sistema RENACER** - *Donde cada vida vuelve a florecer*

</div>
