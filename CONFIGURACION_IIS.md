# 🌐 Configuración de IIS para CONSOLA

## Requisitos Previos

- Windows Server o Windows 10/11 con IIS instalado
- Permisos de administrador
- .NET Runtime instalado en el servidor (no requerido para aplicación cliente)

---

## 📋 Flujo General de Configuración

```mermaid
graph TD
    A[🚀 Inicio] --> B[📁 Crear Directorio Físico]
    B --> C[🌐 Configurar IIS]
    C --> D[👥 Configurar Permisos]
    D --> E[📦 Copiar Archivos]
    E --> F[✅ Verificar Acceso]
    F --> G[🔄 Usar deploy.bat]

    style A fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
    style G fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

---

## 🗂️ Estructura de Directorios

```mermaid
graph LR
    A[💻 Servidor IIS] --> B[C:\inetpub\wwwroot\CONSOLA\]
    B --> C[📦 *.nupkg]
    B --> D[📄 *.json]
    B --> E[🔧 *.exe]
    B --> F[📦 *.zip]
    B --> G[🌐 index.html]
    B --> H[⚙️ web.config]

    style A fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style B fill:#ffd93d,stroke:#f9a825,stroke-width:2px
```

---

## 📋 Pasos para Configurar IIS

### 1. Crear Directorio Físico

```mermaid
flowchart LR
    A[Explorador Windows] --> B{¿Existe carpeta?}
    B -->|No| C[Crear C:\inetpub\wwwroot\CONSOLA]
    B -->|Sí| D[✅ Usar existente]
    C --> D

    style C fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
```

Crea la carpeta donde estarán los archivos del instalador:

```
C:\inetpub\wwwroot\CONSOLA\
```

O usa una ruta de red compartida:
```
\\localhost\Sitios$\CONSOLA\
```

### 2. Abrir Administrador de IIS

```mermaid
sequenceDiagram
    participant U as 👤 Usuario
    participant W as 🪟 Windows
    participant IIS as 🌐 IIS Manager

    U->>W: Windows + R
    U->>W: Escribe "inetmgr"
    W->>IIS: Abre Administrador IIS
    IIS-->>U: ✅ IIS Manager abierto
```

1. Presiona `Windows + R`
2. Escribe: `inetmgr`
3. Presiona Enter

### 3. Crear Sitio Web en IIS

```mermaid
flowchart TD
    A[Abrir IIS Manager] --> B[Click derecho en Sitios]
    B --> C[Agregar sitio web...]
    C --> D[Completar formulario]
    D --> E{Configuración}
    E --> F[Nombre: CONSOLA]
    E --> G[Ruta física: C:\inetpub\wwwroot\CONSOLA]
    E --> H[Puerto: 80]
    F --> I[Click Aceptar]
    G --> I
    H --> I
    I --> J[✅ Sitio creado]

    style A fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
    style J fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

**Pasos detallados:**

1. En **IIS Manager**, panel izquierdo
2. Click derecho en **"Sitios"**
3. Seleccionar **"Agregar sitio web..."**
4. En el formulario que aparece:
   ```
   Nombre del sitio:        CONSOLA
   Ruta de acceso física:   C:\inetpub\wwwroot\CONSOLA  [Buscar...]

   Enlace:
   Tipo:                    http
   Dirección IP:            Todas las no asignadas
   Puerto:                  80
   Nombre de host:          (dejar vacío)
   ```
5. Click **"Aceptar"**
6. El sitio aparecerá en la lista y se iniciará automáticamente

**Verificar**: En el panel derecho debe aparecer el sitio "CONSOLA" con estado "Iniciado" (luz verde)

### 4. Configurar Permisos (Importante)

```mermaid
flowchart LR
    A[Carpeta CONSOLA] --> B[Propiedades]
    B --> C[Seguridad]
    C --> D[Agregar IIS_IUSRS]
    D --> E[✅ Permisos Lectura]

    style A fill:#ffd93d,stroke:#f9a825,stroke-width:2px
    style E fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

**Pasos rápidos:**

1. Abrir `C:\inetpub\wwwroot\CONSOLA` en Explorador
2. Click derecho → **Propiedades** → Pestaña **Seguridad**
3. Click **Editar...** → **Agregar...**
4. Escribir: `IIS_IUSRS` → **Comprobar nombres** → **Aceptar**
5. Con `IIS_IUSRS` seleccionado, marcar:
   - ☑ **Lectura y ejecución**
   - ☑ **Mostrar el contenido de la carpeta**
   - ☑ **Leer**
6. **Aplicar** → **Aceptar**

### 5. Copiar Archivos al Servidor

```mermaid
flowchart LR
    A[./instalador/] --> B[Copiar todo]
    B --> C[C:\inetpub\wwwroot\CONSOLA\]

    style A fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
    style C fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

**Comando rápido:**

```batch
xcopy /Y "C:\sistemas\prueba conexion informix\CONSOLA\instalador\*.*" "C:\inetpub\wwwroot\CONSOLA\"
```

**O manualmente:** Copiar todos los archivos de `./instalador/` a `C:\inetpub\wwwroot\CONSOLA\`

**Archivos que se copian:**
- ✅ `*.nupkg` (paquetes de actualización)
- ✅ `releases.win.json` (manifiesto de versiones)
- ✅ `CONSOLA-win-Setup.exe` (instalador)
- ✅ `index.html` y `web.config`

### 6. Verificar Tipos MIME (Opcional)

El archivo `web.config` ya incluye los tipos MIME necesarios. **Si tienes problemas de descarga**, verifica:

1. En IIS Manager, selecciona sitio **CONSOLA**
2. Doble click en **Tipos MIME**
3. Busca estas extensiones:

| Extensión | Tipo MIME | Estado |
|-----------|-----------|--------|
| `.nupkg` | `application/octet-stream` | Debe existir |
| `.json` | `application/json` | Debe existir |

Si no existen, agregar manualmente con **Agregar...** en el panel derecho

### 7. Probar que Funciona

Abre un navegador y prueba estas URLs:

```mermaid
flowchart LR
    A[Navegador] --> B[localhost/CONSOLA/]
    B --> C{¿Funciona?}
    C -->|Sí| D[✅ Página instalación]
    C -->|No| E[❌ Revisar pasos]

    style D fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
    style E fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
```

**Pruebas básicas:**

| Prueba | URL | Resultado esperado |
|--------|-----|-------------------|
| 1️⃣ Página principal | `http://localhost/CONSOLA/` | Muestra página de instalación |
| 2️⃣ Manifiesto | `http://localhost/CONSOLA/releases.win.json` | Muestra JSON con versiones |
| 3️⃣ Instalador | `http://localhost/CONSOLA/CONSOLA-win-Setup.exe` | Descarga archivo (100 MB) |

✅ **Si las 3 pruebas funcionan, IIS está correctamente configurado**

---

## 🔧 Configuración del web.config

El archivo `web.config` ya está en `./instalador/web.config`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
        <!-- Tipos MIME para archivos de Velopack -->
        <staticContent>
            <mimeMap fileExtension=".nupkg" mimeType="application/octet-stream" />
            <mimeMap fileExtension=".json" mimeType="application/json" />
        </staticContent>

        <!-- Habilitar CORS (opcional) -->
        <httpProtocol>
            <customHeaders>
                <add name="Access-Control-Allow-Origin" value="*" />
            </customHeaders>
        </httpProtocol>

        <!-- Compresión -->
        <urlCompression doStaticCompression="true" />
    </system.webServer>
</configuration>
```

---

## 🚀 Uso del Script de Deploy

```mermaid
flowchart TD
    A[🚀 Ejecutar deploy.bat] --> B[📊 Leer versión actual]
    B --> C[➕ Incrementar versión<br/>1.0.1 → 1.0.2]
    C --> D[🔨 dotnet publish]
    D --> E[📦 vpk pack]
    E --> F[📤 Copiar a IIS]
    F --> G[✅ Deploy completo]

    style A fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
    style G fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

### Configurar Rutas en deploy.bat

Edita `deploy.bat`:

```batch
set IIS_DST=C:\inetpub\wwwroot\CONSOLA
```

### Ejecutar Deploy

```batch
cd "C:\sistemas\prueba conexion informix\CONSOLA"
deploy.bat
```

El script automáticamente:
1. ✅ Incrementa versión automáticamente
2. ✅ Compila aplicación
3. ✅ Genera paquetes Velopack
4. ✅ Copia archivos a IIS
5. ✅ Muestra resumen

---

## 🌍 Acceso desde Otros Equipos

```mermaid
graph TD
    A[🖥️ Servidor IIS] --> B[🔥 Configurar Firewall]
    B --> C[Puerto 80 abierto]

    A --> D[🌐 URL desde red<br/>192.168.1.100/CONSOLA]

    D --> E[💻 Cliente 1]
    D --> F[💻 Cliente 2]
    D --> G[💻 Cliente N]

    E --> H[Detecta actualizaciones]
    F --> H
    G --> H

    style A fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style H fill:#6bcf7f,stroke:#2d9561,stroke-width:2px
```

### En el Servidor

1. Firewall de Windows → **"Configuración avanzada"**
2. **"Reglas de entrada"** → Habilitar: **"World Wide Web Services (HTTP Traffic-In)"**

### En las PCs Cliente

Actualiza la URL en `FormLogin.cs` y `FormPrincipal.cs`:

```csharp
_updateManager = new UpdateManager("http://192.168.1.100/CONSOLA/");
```

---

## 🔍 Solución de Problemas

```mermaid
graph TD
    A{❌ Problema} --> B[Error 404]
    A --> C[Error 403]
    A --> D[No descarga .nupkg]
    A --> E[No detecta updates]

    B --> B1[✅ Verificar ruta física]
    B --> B2[✅ Verificar sitio iniciado]

    C --> C1[✅ Verificar permisos IIS_IUSRS]
    C --> C2[✅ Verificar index.html existe]

    D --> D1[✅ Verificar tipos MIME]
    D --> D2[✅ Verificar web.config]

    E --> E1[✅ Verificar releases.win.json]
    E --> E2[✅ Verificar URL en código]

    style B fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style C fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style D fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
    style E fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
```

### Error 404: No se encuentra la página
- Verifica ruta física correcta
- Verifica que el sitio esté iniciado

### Error 403: Acceso prohibido
- Verifica permisos de `IIS_IUSRS`
- Verifica que exista `index.html`

### No descarga archivos .nupkg
- Verifica tipos MIME en IIS
- Verifica que `web.config` esté presente

### Los clientes no detectan actualizaciones
- Verifica acceso a `releases.win.json`
- Verifica URL en el código
- Verifica firewall/antivirus

---

## 📊 Arquitectura Completa

```mermaid
graph TB
    subgraph "🌐 Servidor IIS"
        A[IIS Manager]
        B[Sitio CONSOLA<br/>Puerto 80]
        C[C:\inetpub\wwwroot\CONSOLA\]
        D[releases.win.json]
        E[*.nupkg]
    end

    subgraph "💻 PC Desarrollador"
        F[deploy.bat]
        G[Incrementa versión]
        H[dotnet publish]
        I[vpk pack]
    end

    subgraph "👥 Clientes"
        J[Cliente 1<br/>v1.0.0]
        K[Cliente 2<br/>v1.0.1]
        L[Cliente 3<br/>v1.0.2]
    end

    F --> G
    G --> H
    H --> I
    I --> C

    C --> D
    C --> E

    J --> D
    K --> D
    L --> D

    D --> M{¿Hay update?}
    M -->|Sí| N[Descargar delta.nupkg]
    M -->|No| O[Continuar]

    style A fill:#4ecdc4,stroke:#1a535c,stroke-width:2px
    style F fill:#ffd93d,stroke:#f9a825,stroke-width:2px
    style M fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px
```

---

## ✅ Checklist de Configuración

- [ ] IIS instalado y funcionando
- [ ] Carpeta física creada (C:\inetpub\wwwroot\CONSOLA)
- [ ] Sitio web o aplicación creada en IIS
- [ ] Permisos de IIS_IUSRS configurados
- [ ] Tipos MIME verificados (.nupkg, .json)
- [ ] Archivos copiados desde ./instalador/
- [ ] Prueba: http://localhost/CONSOLA/
- [ ] Prueba: http://localhost/CONSOLA/releases.win.json
- [ ] Firewall configurado (si acceso remoto)
- [ ] URL actualizada en código cliente
- [ ] deploy.bat configurado con ruta correcta

---

## 🎯 Resultado Final

```mermaid
sequenceDiagram
    participant D as 👨‍💻 Desarrollador
    participant S as 🖥️ deploy.bat
    participant IIS as 🌐 IIS Server
    participant C as 💻 Cliente

    D->>S: Ejecuta deploy.bat
    S->>S: Incrementa versión 1.0.1 → 1.0.2
    S->>S: Compila y empaqueta
    S->>IIS: Copia archivos
    IIS-->>S: ✅ Archivos copiados

    C->>IIS: Verificar actualizaciones
    IIS-->>C: releases.win.json (v1.0.2)
    C->>IIS: Descargar delta.nupkg
    IIS-->>C: ✅ 37 MB descargado
    C->>C: Aplicar y reiniciar
    C-->>C: ✅ Ahora v1.0.2
```

¡Listo! Sistema completamente configurado y funcional.
