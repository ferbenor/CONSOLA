@echo off
setlocal enabledelayedexpansion

rem ========================================
rem Script de Despliegue Automático CONSOLA
rem ========================================

rem Obtener directorio del script
set SCRIPT_DIR=%~dp0

set PROJ=%SCRIPT_DIR%CONSOLA.csproj
set PUBLISH_DIR=%SCRIPT_DIR%publish
set INSTALLER_DIR=%SCRIPT_DIR%instalador
set IIS_DST=\\192.168.100.18\sitios$\consola
set ICON=%SCRIPT_DIR%icono.ico

rem Extraer nombre del proyecto del archivo .csproj
for %%F in ("%PROJ%") do set PACK_ID=%%~nF

echo ========================================
echo  DEPLOY CONSOLA con Velopack
echo ========================================
echo.

rem ========================================
rem 1. INCREMENTAR VERSION AUTOMATICAMENTE
rem ========================================
echo [1/5] Incrementando version...

rem Usar PowerShell para leer y actualizar version de forma confiable
for /f "usebackq delims=" %%v in (`powershell -NoProfile -Command "$xml = [xml](Get-Content '%PROJ%'); $xml.Project.PropertyGroup.Version"`) do set CURRENT_VERSION=%%v

echo Version actual: %CURRENT_VERSION%

rem Extraer numeros de la version (ejemplo: 1.0.1 -> 1 0 1)
for /f "tokens=1,2,3 delims=." %%a in ("%CURRENT_VERSION%") do (
    set MAJOR=%%a
    set MINOR=%%b
    set PATCH=%%c
)

rem Incrementar el PATCH en 1
set /a NEW_PATCH=%PATCH% + 1
set NEW_VERSION=%MAJOR%.%MINOR%.%NEW_PATCH%

echo Nueva version: %NEW_VERSION%

rem Actualizar la version en el .csproj (con respaldo de seguridad)
copy "%PROJ%" "%PROJ%.backup" >nul
powershell -NoProfile -Command "$xml = [xml](Get-Content '%PROJ%'); $xml.Project.PropertyGroup.Version = '%NEW_VERSION%'; $xml.Save('%PROJ%')"

if errorlevel 1 (
    echo ERROR: No se pudo actualizar la version en el .csproj
    echo Restaurando backup...
    copy "%PROJ%.backup" "%PROJ%" >nul
    del "%PROJ%.backup" >nul
    exit /b 1
)

del "%PROJ%.backup" >nul

echo OK: Version actualizada a %NEW_VERSION%
echo.

rem ========================================
rem 2. COMPILAR Y PUBLICAR
rem ========================================
echo [2/5] Publicando aplicacion...

rem Limpiar directorio de publicacion
if exist "%PUBLISH_DIR%" rmdir /s /q "%PUBLISH_DIR%"

dotnet publish "%PROJ%" -c Release -r win-x64 -o "%PUBLISH_DIR%"
if errorlevel 1 (
    echo ERROR: Fallo la publicacion.
    exit /b 1
)

echo OK: Aplicacion publicada
echo.

rem ========================================
rem 3. CREAR PAQUETES CON VELOPACK
rem ========================================
echo [3/5] Creando paquetes de actualizacion con Velopack...
echo Pack ID: %PACK_ID%
echo Icono: %ICON%

vpk pack --packId %PACK_ID% --packVersion %NEW_VERSION% --packDir "%PUBLISH_DIR%" --mainExe %PACK_ID%.exe --icon "%ICON%" --outputDir "%INSTALLER_DIR%"
if errorlevel 1 (
    echo ERROR: Fallo la creacion de paquetes con Velopack.
    exit /b 1
)

echo OK: Paquetes creados
echo.

rem ========================================
rem 4. COPIAR AL SERVIDOR IIS
rem ========================================
echo [4/5] Copiando archivos al servidor IIS...

rem Crear directorio IIS si no existe
if not exist "%IIS_DST%" mkdir "%IIS_DST%"

rem Poner el sitio OFFLINE (opcional)
if exist "%IIS_DST%\app_offline.htm" del "%IIS_DST%\app_offline.htm" >nul 2>&1
echo App en OFFLINE temporal... > "%IIS_DST%\app_offline.htm"

rem Copiar archivos del instalador al servidor IIS
echo Copiando instaladores y paquetes...
xcopy /Y "%INSTALLER_DIR%\*.nupkg" "%IIS_DST%\" >nul
xcopy /Y "%INSTALLER_DIR%\*.json" "%IIS_DST%\" >nul
xcopy /Y "%INSTALLER_DIR%\*.exe" "%IIS_DST%\" >nul
xcopy /Y "%INSTALLER_DIR%\*.zip" "%IIS_DST%\" >nul
xcopy /Y "%INSTALLER_DIR%\index.html" "%IIS_DST%\" >nul
xcopy /Y "%INSTALLER_DIR%\web.config" "%IIS_DST%\" >nul

if errorlevel 1 (
    echo ERROR: Fallo la copia al servidor IIS.
    del "%IIS_DST%\app_offline.htm" 2>nul
    exit /b 1
)

rem Poner el sitio ONLINE
del "%IIS_DST%\app_offline.htm" 2>nul

echo OK: Archivos copiados a IIS
echo.

rem ========================================
rem 5. RESUMEN
rem ========================================
echo [5/5] DEPLOY COMPLETADO
echo ========================================
echo Version desplegada: %NEW_VERSION%
echo Ubicacion IIS: %IIS_DST%
echo URL: http://localhost/CONSOLA/
echo.
echo Archivos desplegados:
echo   - CONSOLA-%NEW_VERSION%-full.nupkg
echo   - CONSOLA-%NEW_VERSION%-delta.nupkg (si aplica)
echo   - releases.win.json
echo   - CONSOLA-win-Setup.exe
echo   - index.html
echo ========================================
echo.
echo Los clientes detectaran la actualizacion automaticamente.
echo.

endlocal
pause
