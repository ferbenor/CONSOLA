# CONSOLA - Aplicación de Conexión a Informix

## Descripción

Aplicación WinForms .NET 8.0 con:
- Conexión a base de datos Informix mediante IBM.Data.Db2
- Sistema de actualizaciones automáticas con Velopack
- Interfaz de login y formulario principal
- Despliegue automatizado a servidor IIS

## Requisitos

- .NET 8.0 SDK
- Velopack CLI (`dotnet tool install -g vpk`)
- Servidor IIS (para despliegue)
- IBM Data Server Driver Package

## Compilación

```bash
dotnet build CONSOLA.csproj -c Release
```

## Despliegue

Ejecutar el script automatizado:

```bash
deploy.bat
```

El script automáticamente:
1. Incrementa la versión del proyecto (PATCH)
2. Crea backup de seguridad del .csproj
3. Compila y publica la aplicación
4. Genera paquetes Velopack
5. Copia archivos al servidor IIS

## Estructura del Proyecto

```
CONSOLA/
├── FormLogin.cs              # Formulario de login con verificación de updates
├── FormPrincipal.cs          # Formulario principal con prueba de Informix
├── FormActualizacion.cs      # Formulario de progreso de actualización
├── UpdateManager.cs          # Wrapper de Velopack
├── Program.cs                # Punto de entrada
├── deploy.bat                # Script de despliegue automatizado
├── CONSOLA.csproj            # Archivo de proyecto
├── CLAUDE.md                 # Documentación técnica
├── ACTUALIZACIONES.md        # Guía del sistema de actualizaciones
└── CONFIGURACION_IIS.md      # Guía de configuración IIS
```

## Configuración

### Servidor de Actualizaciones

Editar en `FormLogin.cs` y `FormPrincipal.cs`:

```csharp
_updateManager = new UpdateManager("http://tu-servidor/instalador/");
```

### Conexión Informix

Editar cadena de conexión en `FormPrincipal.cs:178`:

```csharp
string cadenaConexion = "Database=exgadmsr;Server=192.168.100.18:28157;UserID=informix;Password=informix;";
```

## Control de Versiones (Git)

### Recuperación ante Errores

Si el archivo `.csproj` se corrompe o se vacía:

```bash
# Ver el último commit válido
git log --oneline

# Restaurar archivo específico
git checkout HEAD -- CONSOLA.csproj

# O restaurar desde commit específico
git checkout c347708 -- CONSOLA.csproj
```

### Commits Importantes

```bash
c347708 - Commit inicial del proyecto CONSOLA
0ab0650 - Corregir advertencias de nullable en FormActualizacion
```

### Verificar Estado

```bash
git status
git diff
```

## Solución de Problemas

### Error: CONSOLA.csproj vacío

**Causa**: El script PowerShell en `deploy.bat` falló durante la actualización de versión

**Solución**:
```bash
git checkout HEAD -- CONSOLA.csproj
```

El nuevo `deploy.bat` crea backups automáticos para prevenir este problema.

### Error: No compila

```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

### Error: Velopack no encontrado

```bash
dotnet tool install -g vpk
```

## Versión Actual

**1.0.1** (ver `CONSOLA.csproj`)

## Autor

Desarrollado con asistencia técnica.

## Notas Importantes

- **SIEMPRE** hacer commit antes de ejecutar `deploy.bat`
- El script `deploy.bat` ahora crea backups automáticos del `.csproj`
- En caso de error durante deploy, el backup se restaura automáticamente
- Los archivos compilados (`bin/`, `obj/`, `publish/`) están en `.gitignore`
