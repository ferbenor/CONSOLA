# Guía de Actualizaciones con Velopack

## Cómo Funciona el Sistema de Actualizaciones

### 1. Configuración Inicial (Una Sola Vez)

#### En el servidor IIS:
```
http://tu-servidor/consola/
├── index.html                      # Página de instalación
├── web.config                      # Configuración IIS
├── CONSOLA-win-Setup.exe          # Instalador
├── CONSOLA-1.0.0-full.nupkg       # Paquete versión 1.0.0
├── releases.win.json               # ¡IMPORTANTE! Manifiesto de versiones
└── assets.win.json
```

**El archivo `releases.win.json` es clave**: contiene la lista de versiones disponibles.

### 2. En la Aplicación Cliente

Cada aplicación instalada verifica automáticamente si hay actualizaciones:

```csharp
// Al iniciar la aplicación
VelopackApp.Build().Run();  // SIEMPRE primero

// Luego verificar actualizaciones
var updateManager = new UpdateManager("http://tu-servidor/consola/");
var updateInfo = await updateManager.CheckForUpdatesAsync();

if (updateInfo != null)
{
    // Hay actualización disponible
    await updateManager.DownloadUpdatesAsync(updateInfo);  // Descarga
    updateManager.ApplyUpdatesAndRestart(updateInfo);      // Aplica y reinicia
}
```

## Proceso de Actualización Paso a Paso

### Paso 1: Crear Nueva Versión

1. Cambiar versión en `CONSOLA.csproj`:
```xml
<Version>1.0.1</Version>  <!-- Era 1.0.0 -->
```

2. Publicar y crear instalador:
```bash
dotnet publish -c Release -r win-x64 -o ./publish
vpk pack --packId CONSOLA --packVersion 1.0.1 --packDir ./publish --mainExe CONSOLA.exe --outputDir ./instalador
```

### Paso 2: Subir al Servidor IIS

Después de generar la versión 1.0.1, en `./instalador/` tendrás:

```
CONSOLA-1.0.0-full.nupkg   # Versión anterior (mantener)
CONSOLA-1.0.1-full.nupkg   # Versión nueva (agregar)
CONSOLA-1.0.1-delta.nupkg  # Actualización incremental (si existe)
releases.win.json           # Actualizado con ambas versiones
```

**Copiar estos archivos al servidor IIS** (en la misma carpeta donde está index.html)

### Paso 3: Detección Automática en Clientes

Cuando un usuario con la versión 1.0.0 inicia la aplicación:

1. ✅ La aplicación se conecta a `http://tu-servidor/consola/releases.win.json`
2. ✅ Lee que hay versión 1.0.1 disponible
3. ✅ Descarga `CONSOLA-1.0.1-delta.nupkg` (solo cambios) o `CONSOLA-1.0.1-full.nupkg`
4. ✅ Muestra mensaje al usuario (según tu implementación)
5. ✅ Aplica la actualización al reiniciar

## Ejemplos de Implementación

### Opción 1: Actualización Silenciosa (Automática)

```csharp
static async Task Main(string[] args)
{
    VelopackApp.Build().Run();

    var updateManager = new UpdateManager("http://tu-servidor/consola/");

    try
    {
        var updateInfo = await updateManager.CheckForUpdatesAsync();
        if (updateInfo != null)
        {
            // Descargar y aplicar sin preguntar
            await updateManager.DownloadUpdatesAsync(updateInfo);
            updateManager.ApplyUpdatesAndRestart(updateInfo);
            return; // Sale antes de continuar
        }
    }
    catch { /* Continuar si falla */ }

    // Resto de tu aplicación...
}
```

### Opción 2: Preguntar al Usuario (Recomendado)

```csharp
static async Task Main(string[] args)
{
    VelopackApp.Build().Run();

    var updateManager = new UpdateManager("http://tu-servidor/consola/");

    var updateInfo = await updateManager.CheckForUpdatesAsync();
    if (updateInfo != null)
    {
        var result = MessageBox.Show(
            $"Nueva versión {updateInfo.TargetFullRelease.Version} disponible.\n¿Actualizar ahora?",
            "Actualización",
            MessageBoxButtons.YesNo
        );

        if (result == DialogResult.Yes)
        {
            await updateManager.DownloadUpdatesAsync(updateInfo);
            updateManager.ApplyUpdatesAndRestart(updateInfo);
            return;
        }
    }

    // Resto de tu aplicación...
}
```

### Opción 3: Descargar en Segundo Plano

```csharp
static async Task Main(string[] args)
{
    VelopackApp.Build().Run();

    var updateManager = new UpdateManager("http://tu-servidor/consola/");

    var updateInfo = await updateManager.CheckForUpdatesAsync();
    if (updateInfo != null)
    {
        // Descargar en segundo plano
        await updateManager.DownloadUpdatesAsync(updateInfo);

        MessageBox.Show(
            "Actualización descargada. Se aplicará al cerrar la aplicación.",
            "Actualización"
        );

        // Al cerrar la aplicación:
        Application.ApplicationExit += (s, e) => {
            updateManager.ApplyUpdatesAndExit();
        };
    }

    // Resto de tu aplicación...
}
```

## Para Aplicaciones WinForms

### En el FormPrincipal:

```csharp
public partial class FormPrincipal : Form
{
    private UpdateManager _updateManager;

    public FormPrincipal()
    {
        InitializeComponent();
        _updateManager = new UpdateManager("http://tu-servidor/consola/");
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await VerificarActualizacionesAsync();
    }

    private async Task VerificarActualizacionesAsync()
    {
        var hayActualizacion = await _updateManager.VerificarYAplicarActualizacionAsync();

        if (hayActualizacion)
        {
            var result = MessageBox.Show(
                "Nueva versión disponible. ¿Reiniciar para actualizar?",
                "Actualización",
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
            {
                _updateManager.AplicarYReiniciar();
            }
        }
    }

    // Botón manual de verificación
    private async void btnActualizar_Click(object sender, EventArgs e)
    {
        await VerificarActualizacionesAsync();
    }
}
```

## Archivo releases.win.json

Ejemplo de cómo se ve este archivo después de varias versiones:

```json
{
  "releases": [
    {
      "version": "1.0.0",
      "filename": "CONSOLA-1.0.0-full.nupkg",
      "size": 65011234,
      "sha256": "abc123..."
    },
    {
      "version": "1.0.1",
      "filename": "CONSOLA-1.0.1-full.nupkg",
      "size": 65123456,
      "sha256": "def456...",
      "delta": "CONSOLA-1.0.1-delta.nupkg"
    }
  ]
}
```

## Flujo Completo del Usuario

1. **Primera Instalación**: Usuario descarga e instala desde `http://tu-servidor/consola/`
2. **Uso Normal**: Usuario ejecuta la aplicación normalmente
3. **Nueva Versión**: Tú publicas versión 1.0.1 al servidor
4. **Detección**: Al abrir la app, detecta la versión 1.0.1
5. **Descarga**: Descarga solo los cambios (delta) o todo (full)
6. **Reinicio**: Usuario reinicia y tiene la versión 1.0.1
7. **Siguiente Actualización**: Proceso se repite para versión 1.0.2, etc.

## URLs del Servidor

Asegúrate de que la URL esté correctamente configurada:

```csharp
// Correcto - con slash al final
new UpdateManager("http://tu-servidor/consola/")

// También funciona
new UpdateManager("https://tu-servidor:8080/apps/consola/")

// Local (para pruebas)
new UpdateManager("http://localhost/consola/")
```

## Verificación de Configuración

Para verificar que todo está bien:

1. Abrir en navegador: `http://tu-servidor/consola/releases.win.json`
   - Debe mostrar el JSON con las versiones

2. Abrir en navegador: `http://tu-servidor/consola/CONSOLA-1.0.0-full.nupkg`
   - Debe descargar el archivo

Si estos dos archivos son accesibles, las actualizaciones funcionarán.

## Solución de Problemas

### "No se puede conectar al servidor"
- Verificar que la URL es accesible desde el cliente
- Verificar firewall/antivirus
- Verificar que IIS está corriendo

### "No detecta actualizaciones"
- Verificar que `releases.win.json` esté actualizado
- Verificar que la versión en el archivo sea mayor que la instalada
- Ver logs en consola/debug

### "Error al descargar"
- Verificar tipos MIME en web.config
- Verificar permisos de lectura en IIS
- Verificar tamaño máximo de archivos en IIS

## Resumen

✅ **VelopackApp.Build().Run()** - Siempre primero en Main()
✅ **UpdateManager** - Para verificar y aplicar actualizaciones
✅ **releases.win.json** - Archivo clave en el servidor
✅ **Delta updates** - Solo descarga cambios (eficiente)
✅ **Automático** - No requiere intervención del usuario (opcional)
