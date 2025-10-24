# 📦 Cómo Funciona el Sistema de Actualizaciones con Velopack

## 🎯 Visión General

El sistema de actualizaciones Velopack permite que tus aplicaciones WinForms se actualicen automáticamente, similar a ClickOnce pero sin sus limitaciones.

```mermaid
graph LR
    A[👨‍💻 Desarrollador] -->|1. Publica nueva versión| B[🌐 Servidor IIS]
    B -->|2. Consulta updates| C[💻 Cliente 1]
    B -->|2. Consulta updates| D[💻 Cliente 2]
    B -->|2. Consulta updates| E[💻 Cliente N]
    C -->|3. Descarga| B
    D -->|3. Descarga| B
    E -->|3. Descarga| B
    C -->|4. Actualiza| C
    D -->|4. Actualiza| D
    E -->|4. Actualiza| E
```

---

## 📁 Estructura del Servidor IIS

```mermaid
graph TD
    A[🌐 Servidor IIS<br/>http://tu-servidor/consola/] --> B[📄 index.html<br/>Página de instalación]
    A --> C[⚙️ web.config<br/>Configuración IIS]
    A --> D[📦 CONSOLA-win-Setup.exe<br/>Instalador inicial 64 MB]
    A --> E[📦 CONSOLA-1.0.0-full.nupkg<br/>Paquete completo v1.0.0]
    A --> F[📦 CONSOLA-1.0.1-full.nupkg<br/>Paquete completo v1.0.1]
    A --> G[⚡ CONSOLA-1.0.1-delta.nupkg<br/>Solo cambios 5 MB]
    A --> H[🔑 releases.win.json<br/>ARCHIVO CLAVE Lista de versiones]

    style H fill:#ff6b6b,stroke:#c92a2a,stroke-width:3px,color:#fff
```

### Contenido de `releases.win.json` (Archivo Clave)

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

---

## 🚀 Proceso de Instalación Inicial

```mermaid
sequenceDiagram
    participant U as 👤 Usuario
    participant B as 🌐 Navegador
    participant S as 🖥️ Servidor IIS
    participant I as 💿 Instalador
    participant PC as 💻 PC Usuario

    U->>B: Abre http://tu-servidor/consola/
    B->>S: GET index.html
    S-->>B: Página de instalación
    B-->>U: Muestra página bonita
    U->>B: Clic en "Instalar CONSOLA"
    B->>S: Descarga CONSOLA-win-Setup.exe
    S-->>B: Archivo (64 MB)
    U->>I: Ejecuta Setup.exe
    I->>PC: Instala en AppData\Local\CONSOLA\
    I->>PC: Crea acceso directo
    PC-->>U: ✅ CONSOLA v1.0.0 instalada
```

---

## 🔄 Proceso de Actualización Automática

### Flujo Completo

```mermaid
sequenceDiagram
    participant U as 👤 Usuario
    participant A as 🖥️ App CONSOLA v1.0.0
    participant VM as 🔧 UpdateManager
    participant S as 🌐 Servidor IIS
    participant D as 💾 Disco Local

    U->>A: Abre CONSOLA.exe
    activate A
    A->>A: VelopackApp.Build().Run()
    Note over A: Inicializa sistema de updates

    A->>VM: CheckForUpdatesAsync()
    activate VM
    VM->>S: GET releases.win.json
    S-->>VM: JSON con versiones disponibles

    VM->>VM: Compara versión local (1.0.0)<br/>vs servidor (1.0.1)

    alt Hay nueva versión
        VM-->>A: UpdateInfo (v1.0.1)
        A->>U: 💬 "Nueva versión 1.0.1 disponible<br/>¿Actualizar ahora?"

        alt Usuario acepta
            U-->>A: Sí, actualizar
            A->>VM: DownloadUpdatesAsync()
            VM->>S: GET CONSOLA-1.0.1-delta.nupkg
            Note over VM,S: Solo descarga cambios (5 MB)
            S-->>VM: Archivo delta
            VM->>D: Guarda en AppData\Local\CONSOLA\
            VM-->>A: Descarga completa

            A->>VM: ApplyUpdatesAndRestart()
            VM->>D: Aplica actualización
            VM->>A: Cierra app
            VM->>A: Reinicia app
            A->>U: ✅ CONSOLA v1.0.1 iniciada
        else Usuario rechaza
            U-->>A: Más tarde
            A->>U: ℹ️ "Actualización pendiente"
        end
    else No hay actualizaciones
        VM-->>A: null (ya está actualizado)
        A->>U: ✅ Aplicación iniciada
    end
    deactivate VM
    deactivate A
```

### Opciones de Implementación

```mermaid
graph TD
    A[🔄 Verificar Actualizaciones] --> B{¿Hay nueva versión?}

    B -->|No| C[✅ Continuar normalmente]

    B -->|Sí| D{Estrategia de actualización}

    D -->|Opción 1: Automática| E[Descargar en segundo plano]
    E --> F[Aplicar sin preguntar]
    F --> G[🔄 Reiniciar app]

    D -->|Opción 2: Con confirmación| H[Mostrar diálogo al usuario]
    H --> I{Usuario acepta?}
    I -->|Sí| J[Descargar y aplicar]
    J --> G
    I -->|No| K[Continuar con versión actual]

    D -->|Opción 3: Pasiva| L[Descargar en segundo plano]
    L --> M[Notificar al usuario]
    M --> N[Aplicar al cerrar app]

    style D fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
    style G fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
```

---

## 👨‍💻 Proceso de Publicación (Desarrollador)

```mermaid
flowchart TD
    A[🛠️ Desarrollador hace cambios] --> B[📝 Cambiar version en .csproj<br/>1.0.0 → 1.0.1]
    B --> C[💻 Ejecutar dotnet publish]
    C --> D[📦 Ejecutar vpk pack]
    D --> E{Archivos generados en ./instalador/}

    E --> F[📦 CONSOLA-1.0.1-full.nupkg<br/>62 MB - Paquete completo]
    E --> G[⚡ CONSOLA-1.0.1-delta.nupkg<br/>5 MB - Solo cambios desde 1.0.0]
    E --> H[🔑 releases.win.json<br/>Actualizado con v1.0.1]

    F --> I[📤 Copiar archivos al servidor IIS]
    G --> I
    H --> I

    I --> J[🌐 http://tu-servidor/consola/]
    J --> K[✅ Clientes detectarán v1.0.1<br/>automáticamente]

    style B fill:#ffd93d,stroke:#f9a825,stroke-width:2px
    style H fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style K fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

### Comandos para Publicar

```bash
# 1. Cambiar versión en CONSOLA.csproj
<Version>1.0.1</Version>

# 2. Publicar aplicación
dotnet publish -c Release -r win-x64 -o ./publish

# 3. Crear instalador y paquetes de actualización
vpk pack --packId CONSOLA --packVersion 1.0.1 \
         --packDir ./publish \
         --mainExe CONSOLA.exe \
         --outputDir ./instalador

# 4. Copiar archivos al servidor IIS
# - CONSOLA-1.0.1-full.nupkg
# - CONSOLA-1.0.1-delta.nupkg (si existe)
# - releases.win.json (¡IMPORTANTE!)
```

---

## 📊 Comparación de Tamaños de Descarga

```mermaid
graph LR
    A[📦 Actualización Completa<br/>FULL Package] -->|62 MB| B[💾 Disco]
    C[⚡ Actualización Delta<br/>DELTA Package] -->|5 MB| B

    style A fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style C fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

**Ventaja de Delta Updates:**
- Usuario con v1.0.0 → v1.0.1: Solo descarga **5 MB** (delta)
- Usuario nuevo o muy desactualizado: Descarga **62 MB** (full)

---

## 🔍 Detección de Actualizaciones - Timeline

```mermaid
gantt
    title Ciclo de Vida de una Actualización
    dateFormat  YYYY-MM-DD
    section Desarrollo
    Desarrollo v1.0.0           :done, dev1, 2025-01-01, 10d
    Release v1.0.0              :milestone, 2025-01-10, 0d
    section Servidor
    v1.0.0 en producción       :done, prod1, 2025-01-10, 15d
    Desarrollo v1.0.1          :active, dev2, 2025-01-15, 10d
    Release v1.0.1             :milestone, 2025-01-25, 0d
    v1.0.1 disponible          :crit, prod2, 2025-01-25, 30d
    section Clientes
    Cliente A usa v1.0.0       :done, cli1, 2025-01-10, 15d
    Cliente A detecta update   :crit, 2025-01-25, 1d
    Cliente A usa v1.0.1       :done, 2025-01-26, 29d
    Cliente B usa v1.0.0       :done, cli2, 2025-01-10, 18d
    Cliente B detecta update   :crit, 2025-01-28, 1d
    Cliente B usa v1.0.1       :done, 2025-01-29, 26d
```

---

## 💡 Ejemplos de Código

### Ejemplo 1: Actualización Automática (Silenciosa)

```csharp
static async Task Main(string[] args)
{
    // SIEMPRE primero
    VelopackApp.Build().Run();

    var updateManager = new UpdateManager("http://tu-servidor/consola/");

    try
    {
        var updateInfo = await updateManager.CheckForUpdatesAsync();
        if (updateInfo != null)
        {
            // Descargar y aplicar automáticamente
            await updateManager.DownloadUpdatesAsync(updateInfo);
            updateManager.ApplyUpdatesAndRestart(updateInfo);
            return; // Sale para reiniciar
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"No se pudo verificar actualizaciones: {ex.Message}");
    }

    // Tu aplicación continúa...
    Application.Run(new FormPrincipal());
}
```

### Ejemplo 2: Con Confirmación del Usuario

```csharp
protected override async void OnLoad(EventArgs e)
{
    base.OnLoad(e);

    var updateManager = new UpdateManager("http://tu-servidor/consola/");
    var updateInfo = await updateManager.CheckForUpdatesAsync();

    if (updateInfo != null)
    {
        var version = updateInfo.TargetFullRelease.Version;
        var result = MessageBox.Show(
            $"Nueva versión {version} disponible.\n¿Actualizar ahora?",
            "Actualización Disponible",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information
        );

        if (result == DialogResult.Yes)
        {
            await updateManager.DownloadUpdatesAsync(updateInfo);
            updateManager.ApplyUpdatesAndRestart(updateInfo);
        }
    }
}
```

### Ejemplo 3: Descarga en Segundo Plano

```csharp
private async Task DescargarActualizacionEnSegundoPlano()
{
    var updateManager = new UpdateManager("http://tu-servidor/consola/");
    var updateInfo = await updateManager.CheckForUpdatesAsync();

    if (updateInfo != null)
    {
        // Descargar sin interrumpir al usuario
        await updateManager.DownloadUpdatesAsync(updateInfo);

        // Mostrar notificación discreta
        notifyIcon1.ShowBalloonTip(
            3000,
            "Actualización Descargada",
            "Se aplicará al cerrar la aplicación",
            ToolTipIcon.Info
        );

        // Aplicar al cerrar
        Application.ApplicationExit += (s, e) => {
            updateManager.ApplyUpdatesAndExit();
        };
    }
}
```

---

## ✅ Ventajas de Velopack vs ClickOnce

```mermaid
graph TD
    A[🎯 Velopack] --> B[✅ Compatible con paquetes nativos<br/>IBM DB2, DLLs nativas]
    A --> C[⚡ Delta Updates<br/>Solo descarga cambios]
    A --> D[🚀 Más rápido<br/>Escrito en Rust]
    A --> E[📦 Sin problemas de manifests<br/>No error MSB3094]
    A --> F[🌐 Fácil IIS<br/>Solo copiar archivos]
    A --> G[🔕 Actualizaciones silenciosas<br/>Opcionales]
    A --> H[📚 Múltiples DLLs<br/>Sin limitaciones]

    style A fill:#4ecdc4,stroke:#1a535c,stroke-width:3px,color:#fff
```

---

## 🎓 Resumen para Desarrolladores

### 🎯 Lo que necesitas saber:

1. **Primera configuración** (una sola vez):
   - Copiar carpeta `./instalador/` a servidor IIS
   - Configurar URL en tu aplicación

2. **Para cada nueva versión**:
   ```bash
   # Paso 1: Cambiar versión
   <Version>1.0.X</Version>

   # Paso 2: Compilar y empaquetar
   dotnet publish -c Release -r win-x64 -o ./publish
   vpk pack --packId CONSOLA --packVersion 1.0.X --packDir ./publish --mainExe CONSOLA.exe --outputDir ./instalador

   # Paso 3: Copiar al servidor
   # - *.nupkg
   # - releases.win.json
   ```

3. **Los clientes se actualizan solos** ✨

### 🔑 Archivos Clave

| Archivo | Propósito | Ubicación |
|---------|-----------|-----------|
| `releases.win.json` | 🔑 Lista de versiones (consultado por clientes) | Servidor IIS |
| `*.nupkg` | 📦 Paquetes de actualización | Servidor IIS |
| `UpdateManager.cs` | 🔧 Gestiona actualizaciones en la app | Código de la app |
| `VelopackApp.Build().Run()` | ⚙️ Inicializa sistema | Primera línea de Main() |

---

## 🆘 Solución de Problemas

```mermaid
graph TD
    A{❌ Problema} --> B[No detecta actualizaciones]
    A --> C[Error al descargar]
    A --> D[No se puede conectar]

    B --> B1[✅ Verificar releases.win.json<br/>accesible en navegador]
    B --> B2[✅ Verificar versión en archivo<br/>es mayor que instalada]

    C --> C1[✅ Verificar tipos MIME<br/>en web.config]
    C --> C2[✅ Verificar permisos<br/>de lectura en IIS]

    D --> D1[✅ Verificar URL correcta<br/>con slash al final]
    D --> D2[✅ Verificar firewall/<br/>antivirus]

    style B fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style C fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style D fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
```

---

## 📞 Recursos Adicionales

- 📄 **ACTUALIZACIONES.md** - Documentación completa con todos los ejemplos
- 💻 **UpdateManager.cs** - Clase reutilizable para gestionar updates
- 🖼️ **FormPrincipal.cs** - Ejemplo de implementación en WinForms
- 📋 **CLAUDE.md** - Guía general del proyecto

---

**¡Eso es todo!** 🎉

El sistema está diseñado para ser simple:
1. Publicas nueva versión al servidor
2. Los clientes detectan y descargan automáticamente
3. Usuario reinicia y tiene la última versión

**No necesitas hacer nada más.** ✨
