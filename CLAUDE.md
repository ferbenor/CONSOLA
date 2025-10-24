# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Descripción del Proyecto

Aplicación WinForms en C# (.NET 8.0) para conexión a bases de datos Informix utilizando el driver IBM DB2 (Net.IBM.Data.Db2) con sistema de actualizaciones automáticas mediante Velopack.

## Arquitectura

### Formularios principales

- **Program.cs**: Punto de entrada principal. Inicializa Velopack y abre FormLogin
- **FormLogin.cs**: Formulario de login con verificación automática de actualizaciones
  - Verifica actualizaciones al abrir y cada 5 minutos
  - Muestra botón "Click para Actualizar" cuando hay actualizaciones disponibles
  - Abre FormPrincipal al hacer clic en "Entrar"
- **FormPrincipal.cs**: Formulario principal de la aplicación
  - También verifica actualizaciones al abrir y cada 5 minutos
  - Tiene menú "Ayuda → Buscar Actualizaciones"
  - Tiene menú "Click para Actualizar" (visible solo cuando hay actualizaciones)
- **FormActualizacion.cs**: Formulario con barra de progreso para descarga de actualizaciones
- **UpdateManager.cs**: Clase que gestiona todo el proceso de actualizaciones con Velopack

### Base de datos

- **Libreria/**: Contiene la DLL nativa `IBM.Data.Informix.dll` (actualmente comentada en el código)
- El proyecto usa el paquete NuGet `Net.IBM.Data.Db2` versión 9.0.0.300 como método de conexión actual

### Conexión a Base de Datos

El proyecto está configurado para conectarse a Informix usando dos enfoques posibles:
1. **IBM.Data.Db2** (activo): Usando la cadena de conexión formato DB2
2. **IBM.Data.Informix** (comentado): Usando la cadena de conexión nativa de Informix

Formato de cadena de conexión DB2:
```
Database=nombre_bd;Server=ip:puerto;UserID=usuario;Password=contraseña;
```

Formato de cadena de conexión Informix (comentado):
```
host=ip;protocol=onsoctcp;service=puerto;server=servidor;dataBase=bd;uid=usuario;pwd=contraseña;delimident=y
```

## Comandos de Desarrollo

### Compilar el proyecto
```bash
dotnet build
```

### Ejecutar la aplicación
```bash
dotnet run
```

### Compilar en modo Release
```bash
dotnet build -c Release
```

### Publicar la aplicación
```bash
dotnet publish -c Release -o ./publicar
```

### Restaurar dependencias
```bash
dotnet restore
```

### Desplegar nueva versión con aumento automático
```bash
deploy.bat
```

Este script automáticamente:
- Incrementa la versión (ej: 1.0.1 → 1.0.2)
- Compila el proyecto
- Genera paquetes Velopack
- Copia archivos a IIS

## Instalador y Actualizaciones

El proyecto usa **Velopack** para distribución e instalación automática (reemplazo moderno de ClickOnce).

### Estructura de Carpetas

- `./publish/` - Contiene todos los archivos publicados (DLLs, EXE, dependencias) por separado
- `./instalador/` - Contiene el instalador y archivos para publicar en IIS/servidor web

### Crear instalador y archivos para IIS

```bash
# 1. Publicar la aplicación con archivos separados (no single-file)
rm -rf ./publish
dotnet publish -c Release -r win-x64 -o ./publish

# 2. Crear el instalador con Velopack en carpeta separada
mkdir -p ./instalador
vpk pack --packId CONSOLA --packVersion 1.0.0 --packDir ./publish --mainExe CONSOLA.exe --outputDir ./instalador
```

### Archivos en ./instalador/ (para IIS)

- `index.html` - Página web de instalación (similar a ClickOnce)
- `web.config` - Configuración de IIS con tipos MIME
- `CONSOLA-win-Setup.exe` - Instalador ejecutable (64 MB)
- `CONSOLA-win-Portable.zip` - Versión portable
- `CONSOLA-1.0.0-full.nupkg` - Paquete de actualización
- `releases.win.json` - Manifiesto de versiones para actualizaciones automáticas

### Configurar en IIS

1. Crear un nuevo sitio web o aplicación en IIS
2. Copiar todo el contenido de la carpeta `./instalador/` al directorio del sitio
3. Asegurarse que el pool de aplicaciones tenga permisos de lectura
4. Acceder a `http://tu-servidor/` para ver la página de instalación
5. Los usuarios pueden instalar haciendo clic en "Instalar CONSOLA"

### Actualizar versión

1. Cambiar `<Version>` en CONSOLA.csproj (ej: 1.0.1)
2. Ejecutar los comandos de publicación
3. Copiar los nuevos archivos `.nupkg` y `releases.win.json` al servidor IIS
4. La aplicación detectará y descargará la actualización automáticamente

## Gestión de Actualizaciones en el Código

### Clases principales

- **`UpdateManager.cs`** - Clase para gestionar actualizaciones con eventos de progreso
- **`FormLogin.cs`** - Formulario de login con botón "Click para Actualizar"
- **`FormPrincipal.cs`** - Formulario principal con menú de actualizaciones
- **`FormActualizacion.cs`** - Formulario con barra de progreso para descargas
- **`ACTUALIZACIONES.md`** - Documentación completa del sistema
- **`COMO_FUNCIONA.md`** - Diagramas visuales con Mermaid del proceso

### Características del Sistema de Actualizaciones

✅ **Verificación automática** cada 5 minutos (en FormLogin y FormPrincipal)
✅ **Verificación al iniciar** ambos formularios
✅ **Verificación manual** desde menú Ayuda → Buscar Actualizaciones (FormPrincipal)
✅ **Botón visual** "Click para Actualizar" (verde/amarillo) aparece cuando hay actualizaciones
✅ **Barra de progreso** durante la descarga
✅ **Notificación al usuario** antes de descargar
✅ **Delta updates** - solo descarga cambios

### Controles en FormLogin

```
FormLogin (400x400)
├── Panel Header (azul con versión)
├── txtUsuario (campo de texto)
├── txtPassword (campo de contraseña)
├── btnEntrar (abre FormPrincipal)
├── btnClickParaActualizar (verde/amarillo, oculto por defecto)
├── lblEstado (muestra estado de verificación)
└── Timer (5 minutos)
```

### Menús en FormPrincipal

El formulario principal tiene estos menús (creados con Windows Forms Designer):

```
MenuStrip
├── Ayuda
│   └── Buscar Actualizaciones (verifica manualmente)
└── Click para Actualizar (oculto por defecto, verde/amarillo)
```

### Flujo Completo de la Aplicación

1. **Usuario abre la aplicación** → Se muestra FormLogin
2. **FormLogin verifica actualizaciones** automáticamente
3. **Cada 5 minutos en FormLogin**: Verifica en segundo plano
4. **Si hay actualización en FormLogin**:
   - Muestra diálogo preguntando si desea descargar
   - Hace visible botón "Click para Actualizar"
   - Si acepta: muestra FormActualizacion con barra de progreso
   - Descarga la actualización
   - Aplica y reinicia la aplicación
5. **Usuario hace clic en "Entrar"** → Se abre FormPrincipal
6. **FormPrincipal verifica actualizaciones** automáticamente
7. **Cada 5 minutos en FormPrincipal**: Verifica en segundo plano
8. **Si hay actualización en FormPrincipal**: Mismo proceso que FormLogin

### Uso básico en aplicaciones

```csharp
// En Program.cs (SIEMPRE primero)
VelopackApp.Build().Run();

// La aplicación inicia con FormLogin
Application.Run(new FormLogin());
```

### Archivo clave: releases.win.json

Este archivo en el servidor IIS contiene la lista de versiones disponibles. Los clientes lo consultan para detectar actualizaciones.

**Ver `ACTUALIZACIONES.md` para documentación completa con ejemplos para WinForms.**
**Ver `COMO_FUNCIONA.md` para diagramas visuales del proceso completo.**

## Convenciones del Proyecto

- Los nombres de campos, funciones y variables se manejan en **español**
- Los textos en la aplicación deben estar en español
