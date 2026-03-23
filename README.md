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
  - Entidades: `Propiedad`, `Mejora`, `TipoPropiedad`, `TipoVenta`, `Oferta`, `Chat`
  - Interfaces de repositorios
  - Excepciones de dominio
  - Configuraciones y constantes

#### **Capa de Aplicación (`RealEstateApp.Application`)**
- **Responsabilidad:** Lógica de negocio, orquestación de casos de uso y contratos de servicio
- **Contenido:**
  - DTOs y ViewModels
  - Interfaces de servicios
  - Features (Commands y Queries — patrón CQRS con MediatR)
  - Behaviors de validación (FluentValidation)
  - Mappings con AutoMapper
  - Excepciones de aplicación

#### **Capa de Persistencia (`RealEstateApp.Persistence`)**
- **Responsabilidad:** Acceso a datos y persistencia con EF Core
- **Contenido:**
  - `ApplicationDbContext` (Entity Framework Core — Code First)
  - Implementación de repositorios genéricos
  - Configuraciones de entidades (Fluent API)
  - Migraciones de base de datos

#### **Capa de Identidad (`RealEstateApp.Identity`)**
- **Responsabilidad:** Autenticación, autorización y gestión de usuarios
- **Contenido:**
  - Configuración de ASP.NET Identity
  - Seeds de usuarios y roles por defecto
  - Servicios de autenticación JWT (para la API)
  - Contexto de identidad

#### **Capa Compartida (`RealEstateApp.Shared`)**
- **Responsabilidad:** Servicios transversales reutilizables
- **Contenido:**
  - Servicio de envío de correos electrónicos
  - Helpers y utilidades comunes

#### **Capa de Presentación WebApp (`RealEstateApp.WebApp`)**
- **Responsabilidad:** Interfaz de usuario MVC para clientes, agentes y administradores
- **Contenido:**
  - Controllers MVC
  - Razor Views con Bootstrap
  - ViewModels con validaciones
  - Middlewares y Handlers de errores

#### **Capa de Presentación API (`RealEstateApp.Api`)**
- **Responsabilidad:** Endpoints REST para consumo externo
- **Contenido:**
  - Controllers REST con JWT
  - Documentación Swagger/OpenAPI
  - Handlers de excepciones globales (Problem Details RFC 7807)
  - Patrón CQRS + Mediator en todos los endpoints

---

## 3. Base de Datos

### 3.1 Motor y Configuración
- **Motor:** SQL Server
- **ORM:** Entity Framework Core 9.0
- **Estrategia:** Code-First con migraciones

### 3.2 Diagrama de Entidades Principales

```
┌─────────────────┐         ┌──────────────────┐
│    Propiedad    │◄───────►│  TipoPropiedad   │
├─────────────────┤         ├──────────────────┤
│ Id (PK)         │         │ Id (PK)          │
│ Codigo (6 dig.) │         │ Nombre           │
│ Precio (DOP)    │         │ Descripcion      │
│ Descripcion     │         └──────────────────┘
│ TamanoMetros    │
│ Habitaciones    │         ┌──────────────────┐
│ Banos           │◄───────►│   TipoVenta      │
│ Estado          │         ├──────────────────┤
│ AgenteId (FK)   │         │ Id (PK)          │
│ TipoPropId (FK) │         │ Nombre           │
│ TipoVentaId(FK) │         │ Descripcion      │
└─────────────────┘         └──────────────────┘
         │
         ├──────────────────────────┐
         │                          │
         ▼                          ▼
┌─────────────────┐      ┌──────────────────────┐
│ PropiedadMejora │      │      Imagen           │
├─────────────────┤      ├──────────────────────┤
│ PropiedadId(FK) │      │ Id (PK)              │
│ MejoraId   (FK) │      │ Url                  │
└─────────────────┘      │ PropiedadId (FK)     │
         │               └──────────────────────┘
         ▼
┌─────────────────┐
│     Mejora      │
├─────────────────┤
│ Id (PK)         │
│ Nombre          │
│ Descripcion     │
└─────────────────┘

┌─────────────────┐         ┌──────────────────┐
│    Oferta       │         │      Chat        │
├─────────────────┤         ├─────────────────┤
│ Id (PK)         │         │ Id (PK)         │
│ Cifra           │         │ Mensaje         │
│ Fecha           │         │ Fecha           │
│ Estado          │         │ EsDelCliente    │
│ ClienteId (FK)  │         │ ClienteId (FK)  │
│ PropiedadId(FK) │         │ AgenteId  (FK)  │
└─────────────────┘         │ PropiedadId(FK) │
                            └─────────────────┘

┌─────────────────┐
│  Favorito       │
├─────────────────┤
│ Id (PK)         │
│ ClienteId (FK)  │
│ PropiedadId(FK) │
└─────────────────┘
```

### 3.3 Tablas Principales

| Tabla | Descripción | Campos Clave |
|-------|-------------|--------------|
| **Propiedades** | Propiedades inmobiliarias | Codigo (6 dígitos), Precio (DOP), Estado (Disponible/Vendida) |
| **TiposPropiedades** | Tipos como Casa, Apartamento, etc. | Nombre, Descripcion |
| **TiposVentas** | Tipos como Venta, Alquiler, etc. | Nombre, Descripcion |
| **Mejoras** | Amenidades: Piscina, Balcón, A/C, etc. | Nombre, Descripcion |
| **PropiedadesMejoras** | Relación N:N | PropiedadId, MejoraId |
| **Imagenes** | Fotos de propiedades (1 a 4) | Url, PropiedadId |
| **Ofertas** | Ofertas de clientes sobre propiedades | Cifra, Estado (Pendiente/Aceptada/Rechazada) |
| **Chats** | Mensajes entre cliente y agente | Mensaje, EsDelCliente |
| **Favoritos** | Propiedades favoritas de un cliente | ClienteId, PropiedadId |
| **AspNetUsers** | Usuarios del sistema (Identity) | Nombre, Apellido, Telefono, Foto, Rol |

---

## 4. Roles y Permisos

### 4.1 Tipos de Usuario

El sistema implementa **4 roles** en la WebApp y **2 roles** en la API:

#### 🔵 **Administrador (WebApp)**
- Dashboard con estadísticas: propiedades disponibles/vendidas, agentes y clientes activos/inactivos, desarrolladores
- Listado de agentes con opción de activar/inactivar y eliminar
- Mantenimiento completo de Administradores, Desarrolladores, Tipos de Propiedades, Tipos de Ventas y Mejoras
- No puede editar ni cambiar el estado de su propio usuario

#### 🟠 **Agente (WebApp)**
- Ver sus propiedades (disponibles y vendidas)
- Crear, editar y eliminar propiedades (genera código único de 6 dígitos)
- Responder ofertas: aceptar o rechazar (al aceptar, las demás quedan rechazadas y la propiedad pasa a Vendida)
- Chat con múltiples clientes por propiedad
- Editar su perfil (nombre, apellido, teléfono, foto)

#### 🟡 **Cliente (WebApp)**
- Ver propiedades disponibles con filtros
- Marcar/desmarcar propiedades como favoritas
- Ver "Mis Propiedades" (solo favoritas)
- Enviar ofertas y ver su estado (Pendiente / Aceptada / Rechazada)
- Chat con el agente de la propiedad
- Activación de cuenta vía correo electrónico

#### 🟣 **Desarrollador (API)**
- Acceso de solo lectura a endpoints de propiedades, agentes, tipos de propiedades, tipos de ventas y mejoras
- No puede usar endpoints de creación, edición o eliminación

#### 🔴 **Administrador (API)**
- Acceso completo a todos los endpoints
- Único rol que puede crear usuarios administrador en la API
- Puede cambiar el estado de agentes

---

## 5. Funcionalidades Principales

### 5.1 Home (Pantalla Pública)

- Listado de propiedades disponibles, ordenadas de la más reciente a la más antigua
- Datos mostrados por propiedad: tipo, imagen, código, tipo de venta, valor (DOP), habitaciones, baños, metros cuadrados
- Búsqueda por código de propiedad
- Filtros combinables: tipo de propiedad, precio mínimo/máximo, habitaciones, baños
- Menú: Home | Agentes | Únete a la app | Iniciar Sesión

### 5.2 Agentes (Pantalla Pública)

- Listado de agentes activos ordenados alfabéticamente
- Búsqueda de agente por nombre
- Al hacer clic en un agente: ver todas sus propiedades disponibles con los mismos filtros del Home
- Al hacer clic en una propiedad: ver detalle completo

### 5.3 Registro (`Únete a la App`)

```
Campos: nombre, apellido, teléfono, foto, usuario, correo, contraseña, confirmar contraseña, tipo (Cliente / Agente)

→ Cliente: se crea inactivo + se envía correo de activación
→ Agente:  se crea inactivo, activación la realiza un Administrador
→ En ambos casos: redirige al Login
```

### 5.4 Login

- Campos: correo o nombre de usuario + contraseña
- Redirección según rol al autenticarse correctamente
- Usuario tipo Desarrollador no puede iniciar sesión en la WebApp

### 5.5 Detalle de Propiedad

| Elemento | Visitante | Cliente | Agente |
|----------|-----------|---------|--------|
| Slider de imágenes | ✅ | ✅ | ✅ |
| Datos generales | ✅ | ✅ | ✅ |
| Info del agente | ✅ | ✅ | ✅ |
| Chat | ❌ | ✅ (enviar) | ✅ (responder) |
| Ofertas | ❌ | ✅ (crear/ver las propias) | ✅ (ver todas y responder) |

### 5.6 Mantenimiento de Propiedades (Agente)

```
Crear propiedad:
1. Seleccionar tipo de propiedad, tipo de venta, mejoras
2. Ingresar precio (DOP), descripción, metros, habitaciones, baños
3. Subir entre 1 y 4 imágenes
4. Se genera automáticamente un código único de 6 dígitos
5. La propiedad se crea en estado Disponible

Editar propiedad: mismos campos, las imágenes actuales se visualizan
Eliminar propiedad: confirmación antes de eliminar
```

### 5.7 Sistema de Ofertas

```
Estados posibles: Pendiente → Aceptada / Rechazada

Reglas:
- El botón de nueva oferta se deshabilita si ya existe una oferta aprobada
  sobre la propiedad (de cualquier cliente) o si el cliente tiene una pendiente
- Al aceptar una oferta: todas las demás pendientes se rechazan automáticamente
  y la propiedad cambia a estado Vendida
```

---

## 6. API REST

### 6.1 Autenticación

La API utiliza **JWT (JSON Web Tokens)**. Roles: `Administrador` y `Desarrollador`.

#### AccountController

| Endpoint | Método | Descripción | Roles |
|----------|--------|-------------|-------|
| `/api/Account/login` | POST | Autenticarse y obtener token JWT | Público |
| `/api/Account/register-developer` | POST | Registrar usuario desarrollador | Público |
| `/api/Account/register-admin` | POST | Registrar usuario administrador | Administrador |

### 6.2 Controlador de Propiedades

| Nombre | Método | Parámetros | Respuesta OK | Roles |
|--------|--------|------------|--------------|-------|
| List | GET | — | 200 (lista JSON) / 204 | Admin, Dev |
| GetById | GET | Id | 200 (JSON) / 204 | Admin, Dev |
| GetByCode | GET | Código | 200 (JSON) / 204 | Admin, Dev |

### 6.3 Controlador de Agentes

| Nombre | Método | Parámetros | Respuesta OK | Roles |
|--------|--------|------------|--------------|-------|
| List | GET | — | 200 / 204 | Admin, Dev |
| GetById | GET | Id | 200 / 204 | Admin, Dev |
| GetAgentProperty | GET | Id del agente | 200 / 204 | Admin, Dev |
| ChangeStatus | PATCH | Id, Estatus (bool) | 204 | Admin |

### 6.4 Controlador de Tipos de Propiedades

| Nombre | Método | Parámetros | Respuesta OK | Roles |
|--------|--------|------------|--------------|-------|
| Create | POST | Datos | 201 Created | Admin |
| Update | PUT | Código + Datos | 200 (JSON) | Admin |
| List | GET | — | 200 / 204 | Admin, Dev |
| GetById | GET | Código | 200 / 204 | Admin, Dev |
| Delete | DELETE | Código | 204 (elimina con sus propiedades) | Admin |

### 6.5 Controlador de Tipos de Ventas

| Nombre | Método | Parámetros | Respuesta OK | Roles |
|--------|--------|------------|--------------|-------|
| Create | POST | Datos | 201 Created | Admin |
| Update | PUT | Código + Datos | 200 (JSON) | Admin |
| List | GET | — | 200 / 204 | Admin, Dev |
| GetById | GET | Código | 200 / 204 | Admin, Dev |
| Delete | DELETE | Código | 204 (elimina con sus propiedades) | Admin |

### 6.6 Controlador de Mejoras

| Nombre | Método | Parámetros | Respuesta OK | Roles |
|--------|--------|------------|--------------|-------|
| Create | POST | Datos | 201 Created | Admin |
| Update | PUT | Código + Datos | 200 (JSON) | Admin |
| List | GET | — | 200 / 204 | Admin, Dev |
| GetById | GET | Código | 200 / 204 | Admin, Dev |
| Delete | DELETE | Código | 204 | Admin |

### 6.7 Manejo de Errores (Problem Details — RFC 7807)

| Código | Cuándo se usa |
|--------|---------------|
| **200 OK** | Petición exitosa con datos |
| **201 Created** | Recurso creado |
| **204 No Content** | Operación sin retorno (delete, change status) |
| **400 Bad Request** | Validación fallida |
| **401 Unauthorized** | Token ausente o inválido |
| **403 Forbidden** | Sin permisos suficientes |
| **500 Internal Server Error** | Error interno |

---

## 7. Patrones y Requerimientos Técnicos

### 7.1 Arquitectura
- ✅ **Onion Architecture** aplicada al 100% (Domain → Application → Infrastructure/Identity/Shared → Presentation)
- ✅ **CQRS + Mediator** en todos los endpoints de la API (MediatR)
- ✅ **Repositorios genéricos** y servicios genéricos

### 7.2 Persistencia y Mapeo
- ✅ **Entity Framework Core** — Code First
- ✅ **AutoMapper** — mapeo entre Entities, DTOs y ViewModels

### 7.3 Validaciones
- ✅ **Data Annotations** en ViewModels (WebApp)
- ✅ **FluentValidation** con Behaviors para Commands y Queries (API)

### 7.4 Seguridad
- ✅ **ASP.NET Identity** para gestión de usuarios y roles
- ✅ **JWT** para protección de la API
- ✅ Usuarios tipo `Desarrollador` no pueden iniciar sesión en la WebApp
- ✅ Usuarios tipo `Cliente` o `Agente` no pueden consumir la API
- ✅ Redirección a pantalla de acceso denegado si el rol no tiene permisos

### 7.5 Documentación y Errores
- ✅ **Swagger / OpenAPI** para documentación de la API
- ✅ **Global Exception Handler** centralizado
- ✅ Respuestas de error con estándar **Problem Details (RFC 7807)**

### 7.6 Pruebas Automatizadas
- ✅ **Pruebas unitarias (xUnit)** — Commands, Queries y Servicios (`RealEstateApp.Unit.Tests`)
- ✅ **Pruebas de integración (xUnit)** — Repositorios con base de datos (`RealEstateApp.Integration.Test`)

---

## 8. Inicialización de Datos (Seed)

### Usuarios por defecto

| Usuario | Contraseña | Rol | Descripción |
|---------|------------|-----|-------------|
| `admin` | Admin123@ | Administrador (WebApp) | Usuario admin principal |
| `cliente` | Cliente123@ | Cliente | Cliente de prueba |
| `agente` | Agente123@ | Agente | Agente de prueba |
| `adminapi` | AdminApi123@ | Administrador (API) | Admin para la API |
| `developer` | Dev123@ | Desarrollador | Desarrollador para la API |

> Los seeds también crean los roles necesarios automáticamente al iniciar la aplicación.

---

## 9. Tecnologías y Librerías

### 9.1 Backend
- **.NET 9.0**
- **ASP.NET Core MVC 9**
- **Entity Framework Core 9**
- **ASP.NET Identity**
- **MediatR** (CQRS + Mediator)
- **AutoMapper**
- **FluentValidation**
- **JWT Bearer Authentication**
- **Swagger / Swashbuckle**

### 9.2 Frontend
- **Bootstrap 5** (diseño responsivo)
- **jQuery 3**
- **jQuery Validation Unobtrusive**

### 9.3 Pruebas
- **xUnit 2.9**
- **coverlet.collector** (cobertura de código)
- **Microsoft.NET.Test.Sdk**

### 9.4 Herramientas de Desarrollo
- **Visual Studio 2022**
- **SQL Server Management Studio**
- **Postman / Swagger UI** (para API testing)
- **Git / GitHub** (control de versiones)

---

## 10. Configuración del Proyecto

### 10.1 Requisitos Previos
- ✅ Visual Studio 2022 o superior
- ✅ .NET 9 SDK
- ✅ SQL Server 2019+
- ✅ Git

### 10.2 Configuración de Base de Datos
Editar `appsettings.json` en `RealEstateApp.WebApp` y `RealEstateApp.Api`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=RealEstateAppDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 10.3 Configuración JWT (API)
```json
{
  "JwtSettings": {
    "Key": "tu-clave-secreta-muy-larga-aqui",
    "Issuer": "RealEstateApp.Api",
    "Audience": "RealEstateApp.Client",
    "DurationInMinutes": 60
  }
}
```

### 10.4 Configuración de Email
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@realestateapp.com",
    "SenderName": "RealEstateApp",
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password"
  }
}
```

### 10.5 Ejecutar Migraciones
```bash
# Desde la raíz de la solución
dotnet ef database update --project RealEstateApp.Persistence --startup-project RealEstateApp.WebApp
```

---

## 11. Conclusión

**RealEstateApp** es una plataforma inmobiliaria completa que demuestra la implementación correcta de:

✅ **Onion Architecture** con separación estricta de capas y dependencias  
✅ **Doble capa de presentación:** WebApp MVC + API REST  
✅ **Autenticación dual:** Cookies para la WebApp + JWT para la API  
✅ **CQRS + Mediator** con MediatR en la capa de aplicación  
✅ **Validaciones robustas** con FluentValidation y Behaviors  
✅ **Repositorios y servicios genéricos** para reutilización del código  
✅ **Seguridad por roles** con ASP.NET Identity  
✅ **API REST documentada** con Swagger/OpenAPI  
✅ **Manejo centralizado de errores** con Problem Details (RFC 7807)  
✅ **Pruebas unitarias e integración** con xUnit  
✅ **AutoMapper** para mapeos limpios entre capas
