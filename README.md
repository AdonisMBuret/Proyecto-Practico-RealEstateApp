# Documentación del Proyecto RealEstateApp

**Estudiantes:**
- Jeremy Santiago
- Diomar Arianny Fleming Díaz
- Adonis Mercedes Buret (2021-2396)

---

## 1. Descripción General

**RealEstateApp** es una aplicación para el manejo de propiedades inmobiliarias desarrollada con **ASP.NET Core MVC (.NET 9)** aplicando **Onion Architecture** de forma consistente.  
El sistema permite la gestión completa de propiedades, agentes, clientes y administradores, con una API REST separada protegida por JWT.

### Objetivo del Proyecto
Proporcionar una plataforma inmobiliaria segura, escalable y fácil de usar que permita:
- Publicación y consulta de propiedades en estado disponible
- Gestión de usuarios con múltiples roles (Administrador, Agente, Cliente, Desarrollador)
- Sistema de ofertas y chat entre clientes y agentes
- Mantenimiento de tipos de propiedades, tipos de ventas y mejoras
- API REST documentada con Swagger para consumo externo

---

## 2. Arquitectura del Sistema (Onion Architecture)

### 2.1 Capas del Sistema

#### **Capa de Dominio (`RealEstateApp.Domain`)**
- **Responsabilidad:** Núcleo del sistema — entidades y contratos sin dependencias externas
- **Contenido:**
  - Entidades: `Propiedad`, `TipoPropiedad`, `TipoVenta`, `Mejora`, `ImagenPropiedad`, `PropiedadMejora`, `PropiedadFavorita`, `Oferta`, `Chat`, `Mensaje`
  - Enums: `EstadoPropiedad` (Disponible/Vendida), `EstadoOferta` (Pendiente/Aceptada/Rechazada)
  - Interfaces: `IRepositoryAsync<T>`, `IPropiedadRepository`, `IOfertaRepository`, `IChatRepository`, `IMensajeRepository`, `IFavoritoRepository`, `IMejoraRepository`, `ITipoPropiedadRepository`, `ITipoVentaRepository`, `IUsuarioRepository`
  - `BaseEntity` como clase base con propiedad `Id`

#### **Capa de Aplicación (`RealEstateApp.Application`)**
- **Responsabilidad:** Lógica de negocio, casos de uso y contratos de servicio
- **Contenido:**
  - DTOs y ViewModels por dominio (Propiedades, Agentes, Catálogos, Ofertas, Chat, Admin)
  - Interfaces de servicios: `IPropiedadService`, `IAgenteService`, `IOfertaService`, `IChatService`, `IFavoritoService`, `IMejoraService`, `ITipoPropiedadService`, `ITipoVentaService`, `IJwtService`, `IEmailService`, `IFileUploadService`
  - Features CQRS con MediatR para: Propiedades, Agentes, Mejoras, TipoPropiedades, TipoVentas, Ofertas, Chat, Favoritos, Admin (Dashboard)
  - `ValidationBehavior<TRequest, TResponse>` — pipeline de validación con FluentValidation
  - AutoMapper Profiles: `PropiedadApiProfile`, `AgenteApiProfile`, `MantenimientoApiProfile`, `PropiedadProfile`, `GeneralProfile`, `ChatViewModelProfile`, `OfertasViewModelProfile`, `PropiedadesViewModelProfile`, entre otros
  - Servicios: `PropiedadService`, `AgenteService`, `OfertaService`, `ChatService`, `FavoritoService`, `MejoraService`, `TipoPropiedadService`, `TipoVentaService`, `NotificacionService`

#### **Capa de Persistencia (`RealEstateApp.Persistence`)**
- **Responsabilidad:** Acceso a datos y persistencia con EF Core
- **Contenido:**
  - `ApplicationDbContext` con DbSets: `Propiedades`, `TiposPropiedades`, `TiposVentas`, `Mejoras`, `ImagenesPropiedades`, `PropiedadesMejoras`, `PropiedadesFavoritas`, `Ofertas`, `Chats`, `Mensajes`
  - `GenericRepositoryAsync<T>` — repositorio genérico base
  - Repositorios especializados: `PropiedadRepository`, `ChatRepository`, `OfertaRepository`, `MejoraRepository`, `MensajeRepository`, `PropiedadFavoritaRepository`, `TipoPropiedadRepository`, `TipoVentaRepository`, `UsuarioRepository`
  - Configuraciones Fluent API por entidad: índices únicos, restricciones, relaciones y comportamiento de eliminación
  - Seeds: `DefaultTipoPropiedades`, `DefaultTipoVentas`, `DefaultMejoras`

#### **Capa de Identidad (`RealEstateApp.Identity`)**
- **Responsabilidad:** Autenticación, autorización y gestión de usuarios
- **Contenido:**
  - `IdentityContext` con `ApplicationUser` extendido (Nombre, Apellido, Cedula, UrlImagenPerfil, EsActivo, FechaCreacion)
  - Configuración dual: Cookies para WebApp / JWT Bearer para la API (parámetro `useJwtAsDefault`)
  - Seeds: `DefaultRoles`, `DefaultUsers`
  - Servicios: `JwtService`, `UserStatsService`, `UserManagementService`
  - Políticas de autorización: `RequireAdministradorRole`, `RequireAgentRole`, `RequireClientRole`, `RequireDeveloperRole`, `WebAppAccess`, `ApiAccess`

#### **Capa Compartida (`RealEstateApp.Shared`)**
- **Responsabilidad:** Servicios transversales reutilizables
- **Contenido:**
  - `EmailService` — SMTP Gmail con plantillas HTML (confirmación de cuenta, bienvenida)
  - `FileUploadService` — manejo de imágenes
  - `UserService` — consulta de agentes usando `UserManager`

#### **Capa de Presentación WebApp (`RealEstateApp.WebApp`)**
- **Responsabilidad:** Interfaz de usuario MVC con autenticación por Cookies
- **Controllers:** `HomeController`, `AgentesController`, `AgenteController`, `ClienteController`, `AdminController`, `MejorasController`, `TipoPropiedadesController`, `TipoVentasController`, `AccountController`
- **Puerto:** `https://localhost:7255` / `http://localhost:5020`

#### **Capa de Presentación API (`RealEstateApp.Api`)**
- **Responsabilidad:** Endpoints REST con autenticación JWT
- **Contenido:** Controllers REST, `GlobalExceptionHandler` con Problem Details RFC 7807, Swagger con Bearer integrado, seed automático al arrancar
- **Puerto:** `https://localhost:7112` / `http://localhost:5216`

---

## 3. Base de Datos

### 3.1 Motor y Configuración
- **Motor:** SQL Server (LocalDB en desarrollo)
- **ORM:** Entity Framework Core 9.0
- **Estrategia:** Code-First con migraciones
- **Dos contextos:** `ApplicationDbContext` (datos) + `IdentityContext` (usuarios) → misma base de datos

### 3.2 Diagrama de Entidades

```
┌─────────────────┐         ┌──────────────────┐
│    Propiedad    │◄───────►│  TipoPropiedad   │
├─────────────────┤         ├──────────────────┤
│ Id (PK)         │         │ Id (PK)          │
│ Codigo (PROPXXX)│         │ Nombre           │
│ Precio decimal  │         │ Descripcion      │
│ TamanoEnMetros  │         └──────────────────┘
│ Habitaciones    │
│ Banos           │         ┌──────────────────┐
│ Descripcion     │◄───────►│   TipoVenta      │
│ Estado (enum)   │         ├──────────────────┤
│ FechaCreacion   │         │ Id (PK)          │
│ AgenteId        │         │ Nombre           │
│ TipoPropId (FK) │         │ Descripcion      │
│ TipoVentaId(FK) │         └──────────────────┘
└─────────────────┘
    │    │    │    │
    │    │    │    └────────────────────────┐
    │    │    └───────────────┐             │
    │    └─────┐              │             │
    │          │              │             │
    ▼          ▼              ▼             ▼
┌──────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐
│ Mejora   │ │ImagenProp.│ │  Oferta   │ │   Chat    │
│ (N:N via │ ├───────────┤ ├───────────┤ ├───────────┤
│PropMejora│ │UrlImagen  │ │Monto(DOP) │ │ClienteId  │
└──────────┘ │EsPrincipal│ │Estado(enum│ │AgenteId   │
             └───────────┘ │ClienteId  │ └─────┬─────┘
                           └───────────┘       │
                                        ┌──────▼──────┐
                                        │   Mensaje   │
                                        ├─────────────┤
                                        │ Contenido   │
                                        │ FechaEnvio  │
                                        │ EmisorId    │
                                        │ ReceptorId  │
                                        │ EsLeido     │
                                        └─────────────┘

┌──────────────────┐
│ PropiedadFavorita│
├──────────────────┤
│ ClienteId        │
│ PropiedadId (FK) │
└──────────────────┘
```

### 3.3 Tablas Principales

| Tabla | Descripción | Configuraciones clave |
|-------|-------------|----------------------|
| **Propiedades** | Propiedades inmobiliarias | Codigo único (max 10), índice compuesto Estado+TipoPropiedad |
| **TiposPropiedades** | Casa, Apartamento, Terreno, etc. | Nombre único, `DeleteBehavior.Restrict` |
| **TiposVentas** | Venta, Alquiler, etc. | Nombre único, `DeleteBehavior.Restrict` |
| **Mejoras** | Piscina, Balcón, A/C, Jardín, etc. | Nombre único |
| **PropiedadesMejoras** | Relación N:N Propiedad↔Mejora | Índice único (PropiedadId + MejoraId), Cascade |
| **ImagenesPropiedades** | Fotos (1–4 por propiedad) | Índice filtrado para imagen principal |
| **Ofertas** | Ofertas de clientes sobre propiedades | Mín RD$1,000 / Máx RD$100,000,000 |
| **Chats** | Sesión de mensajes cliente↔agente | Índice único (ClienteId + AgenteId + PropiedadId) |
| **Mensajes** | Mensajes individuales del chat | Índice en (ChatId + FechaEnvio), (ReceptorId + EsLeido) |
| **PropiedadesFavoritas** | Favoritos del cliente | Índice único (ClienteId + PropiedadId) |
| **Usuarios** (Identity) | ApplicationUser extendido | EsActivo, Nombre, Apellido, Cedula, UrlImagenPerfil |

---

## 4. Roles y Permisos

#### 🔵 **Administrador (WebApp)**
- Dashboard con estadísticas: propiedades disponibles/vendidas, agentes/clientes/desarrolladores activos e inactivos
- Listado de agentes: activar/inactivar y eliminar (elimina también sus propiedades)
- Mantenimiento completo de Administradores, Desarrolladores, Tipos de Propiedades, Tipos de Ventas y Mejoras
- No puede editar ni cambiar el estado de su propio usuario

#### 🟠 **Agente (WebApp)**
- Ver todas sus propiedades (disponibles y vendidas con etiqueta)
- Crear, editar y eliminar propiedades (código generado automáticamente en formato `PROP001`)
- Responder ofertas: al aceptar, todas las demás quedan rechazadas y la propiedad pasa a Vendida
- Chat con múltiples clientes por propiedad
- Editar su perfil (nombre, apellido, teléfono, foto)

#### 🟡 **Cliente (WebApp)**
- Ver propiedades disponibles con filtros combinables
- Marcar/desmarcar propiedades como favoritas
- Ver "Mis Propiedades" (solo sus favoritas disponibles)
- Crear ofertas con mínimo RD$1,000 y máximo RD$100,000,000; solo una pendiente por propiedad
- Chat con el agente de la propiedad
- Activación de cuenta vía correo electrónico

#### 🟣 **Desarrollador (API)**
- Acceso de solo lectura a endpoints de propiedades, agentes, tipos y mejoras
- No puede crear, editar, eliminar ni cambiar estados

#### 🔴 **Administrador (API)**
- Acceso completo a todos los endpoints
- Único rol que puede registrar otros administradores en la API
- Puede cambiar el estado de agentes (`ChangeStatus`)

---

## 5. Funcionalidades Principales

### 5.1 Home (Pantalla Pública)

Listado de propiedades disponibles ordenadas de la más reciente a la más antigua, con búsqueda por código y filtros combinables: tipo de propiedad, precio mínimo/máximo, habitaciones y baños.

### 5.2 Detalle de Propiedad

| Elemento | Visitante | Cliente | Agente |
|----------|-----------|---------|--------|
| Slider de imágenes + datos | ✅ | ✅ | ✅ |
| Info del agente (cel, email, foto) | ✅ | ✅ | ✅ |
| Chat (enviar mensaje) | ❌ | ✅ | ✅ (ver lista de clientes) |
| Ofertas (crear / ver las propias) | ❌ | ✅ | ✅ (aceptar / rechazar) |
| Marcar como favorita | ❌ | ✅ | ❌ |

### 5.3 Sistema de Ofertas

```
Estados: Pendiente → Aceptada / Rechazada

Validaciones:
- Monto mínimo: RD$1,000 — Monto máximo: RD$100,000,000
- No se puede hacer oferta si ya existe una aprobada (de cualquier cliente)
  o si el propio cliente tiene una pendiente sobre esa propiedad

Al aceptar una oferta:
- Todas las demás ofertas pendientes de esa propiedad → Rechazadas automáticamente
- La propiedad pasa a estado Vendida
```

### 5.4 Creación de Propiedades (Agente)

```
1. Seleccionar tipo de propiedad, tipo de venta y mejoras (selección múltiple)
2. Ingresar precio (DOP), descripción, metros, habitaciones, baños
3. Subir entre 1 y 4 imágenes
4. Código generado automáticamente: PROP001, PROP002, PROP010... (formato PROP + 3 dígitos, único)
5. La propiedad se crea en estado Disponible
```

---

## 6. API REST

### 6.1 Autenticación

```
POST /api/Account/login
→ Retorna token JWT con claims de UserId, Email, Username y Roles

POST /api/Account/register-developer  (público)
POST /api/Account/register-admin      (solo Administrador)
```

### 6.2 Endpoints por Controlador

**Propiedades:** `List` (GET), `GetById` (GET), `GetByCode` (GET) — Admin + Dev

**Agentes:** `List` (GET), `GetById` (GET), `GetAgentProperty` (GET), `ChangeStatus` (PATCH Admin) — Admin + Dev

**TipoPropiedades:** `Create` (POST Admin), `Update` (PUT Admin), `List` (GET), `GetById` (GET), `Delete` (DELETE Admin)

**TipoVentas:** `Create` (POST Admin), `Update` (PUT Admin), `List` (GET), `GetById` (GET), `Delete` (DELETE Admin)

**Mejoras:** `Create` (POST Admin), `Update` (PUT Admin), `List` (GET), `GetById` (GET), `Delete` (DELETE Admin)

Todos los endpoints de listado retornan `200` con datos o `204` si no hay registros. Los errores se devuelven con `400`, `401`, `403`, `404` o `500` según el caso, siguiendo Problem Details RFC 7807.

### 6.3 GlobalExceptionHandler — Problem Details (RFC 7807)

El handler centraliza errores con campos extendidos: `timestamp`, `traceId`, `path`, `method`. En desarrollo agrega `machine` y `exception` type.

| Código | Excepción mapeada |
|--------|-------------------|
| 400 | `ValidationException`, `ArgumentException`, `ArgumentNullException` |
| 401 | `UnauthorizedAccessException` |
| 403 | `ForbiddenException` |
| 404 | `KeyNotFoundException`, `NotFoundException` |
| 409 | `InvalidOperationException`, `ConflictException` |
| 500 | Cualquier excepción no mapeada |

---

## 7. Patrones y Requerimientos Técnicos

- ✅ **Onion Architecture** con dependencias unidireccionales: Domain ← Application ← Infrastructure/Identity/Shared ← Presentation
- ✅ **CQRS + Mediator** con MediatR — Commands y Queries separados por dominio funcional
- ✅ **ValidationBehavior** como pipeline de MediatR para validar todos los Commands y Queries con FluentValidation
- ✅ **Repositorio Genérico** `GenericRepositoryAsync<T>` + repositorios especializados
- ✅ **AutoMapper** con múltiples perfiles separados por contexto (API, WebApp, Entities↔DTOs)
- ✅ **ASP.NET Identity** con `ApplicationUser` extendido y `IdentityContext` propio
- ✅ **JWT** para la API y **Cookies** para la WebApp (configuración dual en `ServiceRegistration`)
- ✅ **Swagger/OpenAPI** con autenticación Bearer integrada en la UI
- ✅ **GlobalExceptionHandler** centralizado con Problem Details RFC 7807
- ✅ **Pruebas unitarias** con xUnit + Moq + FluentAssertions
- ✅ **Pruebas de integración** con EF InMemory + FluentAssertions sobre repositorios reales

---

## 8. Pruebas Automatizadas

### Pruebas Unitarias (`RealEstateApp.Unit.Tests`) — xUnit + Moq + FluentAssertions

| Clase de Test | Tests | Qué cubre |
|---------------|-------|-----------|
| `AceptarOfertaCommandTests` | 6 | Oferta válida, no encontrada, propiedad inexistente, agente no autorizado, oferta ya procesada, rechazo automático de pendientes |
| `DeletePropiedadCommandTests` | 4 | Eliminar válido, no encontrada, independiente del estado, excepción en repositorio |
| `MejoraCommandTests` | 5 | Crear, error repo, actualizar campos parciales, entidad no encontrada, eliminar idempotente |
| `MejoraQueryTests` | 4 | Listar, lista vacía, por ID encontrada, por ID no encontrada |
| `TipoPropiedadCommandTests` | 5 | Crear, excepción repo, actualizar campos parciales, no encontrado, eliminar idempotente |
| `TipoPropiedadQueryTests` | 4 | Listar, lista vacía, por ID encontrado, por ID no encontrado |
| `TipoVentaCommandTests` | 5 | Crear, error repo, actualizar campos parciales, no encontrado, eliminar idempotente |
| `TipoVentaQueryTests` | 4 | Listar, lista vacía, por ID encontrado, por ID no encontrado |
| `GetAllDisponiblesQueryTests` | 3 | Solo disponibles, lista vacía, datos correctos |
| `GetByCodigoQueryTests` | 2 | Código existente, código inválido/nulo |
| `GetOfertasByPropiedadQueryTests` | 3 | Lista válida, sin ofertas, ordenadas por fecha desc |
| `ChatQueryTests` | 4 | GetById null, GetByPropiedad ordenado, fallback sin mensajes, GetMensajesByChat |

### Pruebas de Integración (`RealEstateApp.Integration.Test`) — xUnit + EF InMemory + FluentAssertions

| Clase de Test | Tests | Qué cubre |
|---------------|-------|-----------|
| `PropiedadRepositoryTests` | 18 | Disponibles, filtros combinados, por código, por agente, detalle, estadísticas, generación de código, ViewModel projections, gestión de mejoras, eliminar todas por agente |
| `TipoPropiedadRepositoryTests` | 5 | Con propiedades incluidas, nombre case-insensitive, cantidad, tipos con propiedades, nulo cuando no existe |
| `TipoVentaRepositoryTests` | 5 | Con propiedades, nombre case-insensitive, cantidad, nulo, cero sin asignaciones |
| `OfertaRepositoryTests` | 2 | Ofertas del cliente ordenadas con propiedad incluida, `HasAcceptedOferta` |
| `PropiedadFavoritaRepositoryTests` | 3 | Es favorito, lista detallada ordenada con relaciones, cantidad por cliente |
| `ChatRepositoryTests` | 4 | Crear chat nuevo, reusar existente independiente del orden, mensajes ordenados, consultas por propiedad |

---

## 9. Inicialización de Datos (Seed)

### Roles creados por defecto
`Administrador`, `Agente`, `Cliente`, `Desarrollador`

### Usuarios por defecto

| Usuario | Contraseña | Email | Rol |
|---------|------------|-------|-----|
| `admin` | Admin123! | admin@realestate.com | Administrador |
| `cliente` | Cliente123! | cliente@realestate.com | Cliente |
| `agente` | Agente123! | agente@realestate.com | Agente |
| `developer` | Developer123! | developer@realestate.com | Desarrollador |

> Los seeds validan si los roles/usuarios ya existen antes de crearlos, evitando duplicados en cada reinicio. El seed de la API se ejecuta al arrancar la aplicación; el de la WebApp se ejecuta a través de `SeedDatabase` en startup.

### Catálogos sembrados
Al iniciar también se crean tipos de propiedades, tipos de ventas y mejoras por defecto (`DefaultTipoPropiedades`, `DefaultTipoVentas`, `DefaultMejoras`) para que el sistema funcione desde el primer arranque sin configuración manual.

---

## 10. Tecnologías y Librerías

### 10.1 Backend
- **.NET 9.0** / **ASP.NET Core MVC 9**
- **Entity Framework Core 9.0** (`Microsoft.EntityFrameworkCore.SqlServer`)
- **ASP.NET Identity** (`Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0`)
- **MediatR 13.1** (CQRS + Mediator + Pipeline Behaviors)
- **AutoMapper 12.0** (`AutoMapper.Extensions.Microsoft.DependencyInjection`)
- **FluentValidation 12.1** (`FluentValidation.DependencyInjectionExtensions`)
- **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer 9.0`)
- **Swagger** (`Swashbuckle.AspNetCore 7.2`)

### 10.2 Frontend
- **Bootstrap 5** + **Bootstrap Icons** (`bi-*`)
- **jQuery 3** + jQuery Validation Unobtrusive

### 10.3 Pruebas
- **xUnit 2.9** — framework de pruebas
- **Moq 4.20** — mocking para pruebas unitarias
- **FluentAssertions 8.8** — assertions expresivas en ambos proyectos de prueba
- **Microsoft.EntityFrameworkCore.InMemory 9.0** — base de datos en memoria para integración
- **coverlet.collector 6.0** — cobertura de código

### 10.4 Herramientas de Desarrollo
- **Visual Studio 2022**
- **SQL Server Management Studio**
- **Postman / Swagger UI**
- **Git / GitHub**

---

## 11. Configuración del Proyecto

### 11.1 Cadenas de Conexión (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RealEstateAppDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "IdentityConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RealEstateAppDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "JWTSettings": {
    "Key": "8K9mN2pQ4rS6tU8vW0xY2zA4bC6dE8fG0hI2jK4lM6nO8pQ0rS2tU4vW6xY8zA0b",
    "Issuer": "RealEstateApp",
    "Audience": "RealEstateAppUsers"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu-correo@gmail.com",
    "SmtpPass": "tu-app-password",
    "FromEmail": "tu-correo@gmail.com",
    "FromName": "Real Estate App"
  }
}
```

### 11.2 Migraciones
```bash
# Migración de datos de la aplicación
dotnet ef database update --project RealEstateApp.Persistence --startup-project RealEstateApp.Api

# Migración de identidad
dotnet ef database update --project RealEstateApp.Identity --startup-project RealEstateApp.Api
```

### 11.3 Ejecutar Pruebas
```bash
dotnet test RealEstateApp.Unit.Tests
dotnet test RealEstateApp.Integration.Test
```

---

## 12. Conclusión

**RealEstateApp** es una plataforma inmobiliaria completa que demuestra la implementación correcta de:

✅ **Onion Architecture** con separación estricta de capas y dependencias unidireccionales  
✅ **Doble capa de presentación:** WebApp MVC (Cookies) + API REST (JWT)  
✅ **CQRS + Mediator** con MediatR — Commands y Queries por cada dominio funcional  
✅ **Pipeline de validación** con FluentValidation y `ValidationBehavior`  
✅ **Repositorio Genérico** `GenericRepositoryAsync<T>` + repositorios especializados  
✅ **AutoMapper** con múltiples perfiles separados por contexto de uso  
✅ **ASP.NET Identity** extendido con campos de negocio  
✅ **JWT** para protección de la API + **Cookies** para la WebApp (configuración dual)  
✅ **Swagger/OpenAPI** con autenticación Bearer integrada en la UI  
✅ **GlobalExceptionHandler** centralizado con Problem Details RFC 7807  
✅ **Pruebas unitarias** con xUnit + Moq + FluentAssertions (~44 tests)  
✅ **Pruebas de integración** con EF InMemory + FluentAssertions (~37 tests sobre repositorios reales)
