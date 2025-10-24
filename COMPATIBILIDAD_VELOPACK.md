# 🔍 Compatibilidad de Velopack con VS 2015, .NET Framework 4.5 y SmartAssembly

---

## 📋 Tabla de Contenidos

1. [Velopack con .NET Framework 4.5](#velopack-con-net-framework-45)
2. [Velopack con SmartAssembly (Ofuscación)](#velopack-con-smartassembly-ofuscación)
3. [Alternativas para VS 2015 + .NET 4.5](#alternativas-para-vs-2015--net-45)
4. [Resumen de Compatibilidad](#resumen-de-compatibilidad)
5. [Problema Detectado: No Detecta Actualizaciones](#problema-detectado-no-detecta-actualizaciones)

---

## ❌ Velopack con .NET Framework 4.5

### Estado de Compatibilidad

**NO es compatible**. Velopack requiere:

- **.NET Framework 4.6.2** como mínimo (para Framework)
- **.NET 5.0+** para versiones modernas

**.NET Framework 4.5 está marcado como EOL (End of Life)** en la documentación de Velopack y no está soportado.

### Versiones Mínimas Soportadas

| Framework | Versión Mínima | Estado |
|-----------|----------------|--------|
| .NET Framework | 4.6.2 | ✅ Soportado |
| .NET Core | 5.0 | ✅ Soportado |
| .NET | 6.0, 7.0, 8.0 | ✅ Soportado |
| .NET Framework 4.5 | - | ❌ **NO Soportado (EOL)** |

### Opciones

1. **Actualizar a .NET Framework 4.6.2** (compatible con VS 2015)
2. **Migrar a .NET 5.0+** (recomendado, pero requiere VS 2019+)

---

## ⚠️ Velopack con SmartAssembly (Ofuscación)

### Estado de Compatibilidad

**No hay documentación oficial** sobre esta combinación, pero debería funcionar siguiendo el proceso correcto.

### ✅ Proceso Recomendado

```
1. Compilar proyecto
   ↓
2. Ofuscar con SmartAssembly
   ↓
3. dotnet publish (archivos ofuscados)
   ↓
4. vpk pack (empaquetar archivos ofuscados)
   ↓
5. Instalador con código ofuscado
```

### 📋 Pasos Detallados

#### 1. Compilar tu aplicación normalmente

```bash
dotnet build -c Release
```

#### 2. Ofuscar con SmartAssembly

- Genera DLLs/EXEs ofuscados
- Guarda en carpeta temporal (ej: `./obfuscated/`)

#### 3. Publicar los archivos ofuscados

```bash
dotnet publish -c Release -r win-x64 -o ./publish
```

Luego copia los archivos ofuscados a `./publish/` reemplazando los originales.

#### 4. Empaquetar con Velopack

```bash
vpk pack --packDir ./publish --mainExe TuApp.exe
```

---

### ⚠️ Consideraciones Importantes

#### Firma de código (Code Signing)

Velopack firma los ejecutables por defecto. Con SmartAssembly:

- ✅ Ofusca **ANTES** de empaquetar con Velopack
- ✅ Velopack firmará los archivos ya ofuscados
- ✅ Si usas firma digital, usa `--signParams` en vpk pack

#### Posibles Problemas

| Problema | Solución |
|----------|----------|
| **VelopackApp.Build().Run()** puede fallar si se ofusca | Excluir de ofuscación |
| SmartAssembly renombra métodos/clases que Velopack necesita | Usar atributo `[Obfuscation(Exclude = true)]` |
| Reflexión en código de Velopack puede romperse | Excluir namespaces `Velopack.*` |

---

### 🔧 Configuración SmartAssembly Recomendada

#### En SmartAssembly, excluir de ofuscación:

```
Velopack.*
VelopackApp
System.Reflection.*
```

#### En tu código, marcar métodos críticos:

```csharp
using System.Reflection;

internal class Program
{
    [Obfuscation(Exclude = true)]
    static void Main(string[] args)
    {
        // IMPORTANTE: NO ofuscar esta llamada
        VelopackApp.Build().Run();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormPrincipal());
    }
}
```

#### Excluir clases relacionadas con Velopack:

```csharp
[Obfuscation(Exclude = true)]
public class UpdateManager
{
    // Código relacionado con actualizaciones
}

[Obfuscation(Exclude = true)]
public class FormActualizacion : Form
{
    // Formulario de actualización
}
```

---

## 🎯 Alternativas para VS 2015 + .NET 4.5

Si **NO puedes actualizar** a .NET Framework 4.6.2:

### Comparación de Soluciones

| Solución | Compatible .NET 4.5 | Ofuscación | Delta Updates | VS 2015 |
|----------|---------------------|------------|---------------|---------|
| **ClickOnce** | ✅ | ⚠️ (problemas con DLLs nativas) | ❌ | ✅ |
| **Squirrel.Windows** | ✅ | ✅ | ✅ | ✅ |
| **WiX Toolset** | ✅ | ✅ | ❌ | ✅ |
| **Velopack** | ❌ (requiere 4.6.2+) | ⚠️ | ✅ | ✅ |

---

### Recomendación: Squirrel.Windows

Si estás atascado en .NET 4.5, usa **Squirrel.Windows**:

#### Instalación

```bash
Install-Package squirrel.windows
```

O desde NuGet Package Manager:
```
PM> Install-Package Squirrel.Windows
```

#### Compatibilidad

| Característica | Squirrel.Windows | Velopack |
|----------------|------------------|----------|
| .NET Framework 4.5 | ✅ | ❌ |
| .NET Framework 4.6.2+ | ✅ | ✅ |
| SmartAssembly | ✅ | ⚠️ |
| Visual Studio 2015 | ✅ | ✅ |
| Delta updates | ✅ | ✅ |
| Velocidad | ⚠️ (C#) | ✅ (Rust) |
| Mantenimiento | ⚠️ (menos activo) | ✅ (activo) |

#### Código de Ejemplo con Squirrel.Windows

```csharp
using Squirrel;

static async Task Main(string[] args)
{
    // IMPORTANTE: Antes de cualquier otra cosa
    SquirrelAwareApp.HandleEvents();

    using (var mgr = new UpdateManager("http://tu-servidor/instalador/"))
    {
        // Verificar actualizaciones
        var updateInfo = await mgr.CheckForUpdate();

        if (updateInfo.ReleasesToApply.Any())
        {
            // Descargar actualizaciones
            await mgr.UpdateApp();

            // Reiniciar
            UpdateManager.RestartApp();
        }
    }

    // Continuar con la aplicación normal
    Application.EnableVisualStyles();
    Application.Run(new FormPrincipal());
}
```

---

### ⚖️ Squirrel.Windows vs Velopack

#### ¿Qué tienen en común?

| Característica | Descripción |
|----------------|-------------|
| **Delta updates** | Ambos descargan solo los cambios (no el paquete completo) |
| **Auto-actualización** | Actualizaciones silenciosas en segundo plano |
| **IIS hosting** | Servidor web simple para distribuir actualizaciones |
| **NuGet packages** | Generan paquetes `.nupkg` |
| **Instalador** | Setup.exe para instalación inicial |

#### Diferencias clave

| Aspecto | Squirrel.Windows | Velopack |
|---------|------------------|----------|
| **Lenguaje** | C# | Rust (más rápido) |
| **.NET mínimo** | 4.0+ | 4.6.2+ |
| **Desarrollo activo** | ⚠️ Menos activo | ✅ Muy activo |
| **Documentación** | ⚠️ Antigua | ✅ Moderna |
| **Velocidad** | ⚠️ | ✅ Más rápido |
| **Compatibilidad legacy** | ✅ | ⚠️ |

#### ¿Son lo mismo?

**Sí y no:**

- **Squirrel.Windows** es el proyecto original (2013-2016)
- **Velopack** es un fork/sucesor moderno de Squirrel (2023+)
- Velopack es **básicamente Squirrel reescrito en Rust** con mejoras de velocidad y características modernas
- **Mismo concepto**, pero Velopack es más rápido, moderno y mantenido activamente

**Analogía:**
```
Squirrel.Windows  →  jQuery (antiguo pero funcional)
Velopack          →  React (moderno y mantenido)
```

---

## ✅ Resumen de Compatibilidad

### Tabla Completa

| Tecnología | VS 2015 | .NET 4.5 | .NET 4.6.2+ | .NET 8.0 | SmartAssembly |
|------------|---------|----------|-------------|----------|---------------|
| **Velopack** | ✅ | ❌ | ✅ | ✅ | ⚠️ (sin documentar, probable) |
| **Squirrel.Windows** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **ClickOnce** | ✅ | ✅ | ✅ | ⚠️ | ⚠️ |

### Mi Recomendación

#### Escenario 1: Puedes actualizar a .NET 4.6.2+

```
✅ Usa VELOPACK
   - Moderno
   - Rápido (Rust)
   - Delta updates optimizados
   - Documentación actualizada
   - Comunidad activa
```

#### Escenario 2: Atascado en .NET 4.5

```
✅ Usa SQUIRREL.WINDOWS
   - Compatible con .NET 4.5
   - Probado con SmartAssembly
   - Delta updates
   - VS 2015 compatible
   - Funcional (aunque menos mantenido)
```

---

## 🐛 Problema Detectado: No Detecta Actualizaciones

### Síntomas Reportados

> "He hecho pruebas instalando la versión inicial, luego creo la nueva versión pero no la detecta el aplicativo y solo dice que está en la última versión y actualizada"

### Posibles Causas

#### 1. ❌ URL del servidor incorrecta

**Problema:** La URL en el código no coincide con el servidor

**Verificar en `Program.cs`:**
```csharp
public static string rutaActualizaciones = "https://consultas.santarosa.gob.ec/consola/";
```

**Debe coincidir con:**
```
\\192.168.100.18\sitios$\consola\
```

**Solución:**
```csharp
// Si accedes por red local
public static string rutaActualizaciones = "http://192.168.100.18/consola/";

// Si accedes por dominio
public static string rutaActualizaciones = "https://consultas.santarosa.gob.ec/consola/";
```

---

#### 2. ❌ Archivo `releases.win.json` no actualizado

**Problema:** El servidor tiene la versión antigua de `releases.win.json`

**Verificar:**
```bash
# Abrir en navegador
http://192.168.100.18/consola/releases.win.json
```

**Debe mostrar la nueva versión:**
```json
{
  "Assets": [
    {
      "PackageId": "CONSOLA",
      "Version": "1.0.4",  ← Verificar que sea la nueva versión
      "Type": "Full",
      "FileName": "CONSOLA-1.0.4-full.nupkg"
    }
  ]
}
```

**Solución:**
```bash
# Volver a ejecutar deploy.bat
deploy.bat
```

---

#### 3. ❌ Versión en `.csproj` no incrementada

**Problema:** El `.csproj` sigue con la misma versión

**Verificar en `CONSOLA.csproj`:**
```xml
<Version>1.0.4</Version>  ← Debe ser mayor que la instalada
```

**Si instalaste versión 1.0.3, el `.csproj` debe tener 1.0.4 o superior**

**Solución:**
```bash
# deploy.bat lo incrementa automáticamente
# O manualmente:
<Version>1.0.5</Version>
```

---

#### 4. ❌ Caché del cliente

**Problema:** El cliente tiene caché de la respuesta HTTP

**Verificar en código:**
```csharp
var updateInfo = await _updateManager.VerificarActualizacionesAsync();
```

**Solución temporal (para pruebas):**
```csharp
// Agregar timestamp para evitar caché
public static string rutaActualizaciones =
    $"https://consultas.santarosa.gob.ec/consola/?t={DateTime.Now.Ticks}";
```

**Solución permanente:** Configurar `web.config` para no cachear:
```xml
<staticContent>
  <clientCache cacheControlMode="DisableCache" />
</staticContent>
```

---

#### 5. ❌ Firewall o antivirus bloqueando

**Problema:** El antivirus bloquea la descarga de `.nupkg`

**Verificar:**
- Deshabilitar temporalmente antivirus
- Revisar logs de Windows Defender
- Revisar firewall de Windows

**Solución:**
```
Agregar excepción en antivirus para:
- C:\Users\Usuario\AppData\Local\CONSOLA\
- *.nupkg
```

---

#### 6. ❌ Error en la comparación de versiones

**Problema:** Velopack no reconoce la versión como más nueva

**Verificar formato de versión:**
```
✅ Correcto: 1.0.4
✅ Correcto: 1.0.4-beta
❌ Incorrecto: v1.0.4
❌ Incorrecto: 1.0.4.0
```

**Solución:**
```xml
<Version>1.0.4</Version>  <!-- Sin prefijo 'v' -->
```

---

### 🔍 Diagnóstico Paso a Paso

#### Paso 1: Verificar versión instalada

```csharp
var versionActual = _updateManager.ObtenerVersionActual();
MessageBox.Show($"Versión instalada: {versionActual}");
```

#### Paso 2: Verificar URL del servidor

```csharp
MessageBox.Show($"URL de actualizaciones: {Program.rutaActualizaciones}");
```

#### Paso 3: Verificar acceso al servidor

Abrir en navegador:
```
http://192.168.100.18/consola/releases.win.json
```

Debe mostrar JSON con las versiones disponibles.

#### Paso 4: Verificar contenido de `releases.win.json`

```json
{
  "Assets": [
    {
      "Version": "1.0.4",  ← ¿Es mayor que la instalada?
      "FileName": "CONSOLA-1.0.4-full.nupkg"
    },
    {
      "Version": "1.0.4",
      "Type": "Delta",
      "FileName": "CONSOLA-1.0.4-delta.nupkg"
    }
  ]
}
```

#### Paso 5: Forzar verificación con logs

```csharp
try
{
    MessageBox.Show("Iniciando verificación...");

    var updateInfo = await _updateManager.VerificarActualizacionesAsync();

    if (updateInfo != null)
    {
        MessageBox.Show($"¡Actualización encontrada! Versión: {updateInfo.TargetFullRelease.Version}");
    }
    else
    {
        MessageBox.Show("No hay actualizaciones disponibles");
    }
}
catch (Exception ex)
{
    MessageBox.Show($"Error: {ex.Message}\n\nStack: {ex.StackTrace}");
}
```

---

### ✅ Checklist de Verificación

Antes de reportar un problema, verificar:

- [ ] ✅ Versión en `.csproj` es mayor que la instalada
- [ ] ✅ `deploy.bat` ejecutado exitosamente
- [ ] ✅ Archivos copiados al servidor (`\\192.168.100.18\sitios$\consola\`)
- [ ] ✅ `releases.win.json` accesible desde navegador
- [ ] ✅ `releases.win.json` contiene la nueva versión
- [ ] ✅ URL en `Program.cs` es correcta
- [ ] ✅ Firewall/antivirus no bloquea
- [ ] ✅ IIS configurado correctamente (tipos MIME)
- [ ] ✅ Cliente reiniciado después de la instalación inicial

---

## 📚 Referencias

- [Documentación oficial de Velopack](https://docs.velopack.io/)
- [Velopack en GitHub](https://github.com/velopack/velopack)
- [Squirrel.Windows en GitHub](https://github.com/Squirrel/Squirrel.Windows)
- [SmartAssembly Documentation](https://documentation.red-gate.com/sa/)

---

**Fecha de actualización:** 2025-10-23
