@echo off
echo Creando solucion CONSOLA...

dotnet new sln -n CONSOLA --force
dotnet sln add CONSOLA.Contratos\CONSOLA.Contratos.csproj
dotnet sln add CONSOLA.MockDatos\CONSOLA.MockDatos.csproj
dotnet sln add CONSOLA.UI\CONSOLA.UI.csproj
dotnet sln add CONSOLA.Core\CONSOLA.Core.csproj

echo.
echo Solucion creada exitosamente.
echo.
echo IMPORTANTE: Copia el archivo icono.ico a la carpeta CONSOLA.UI\
echo   Origen:  "C:\sistemas\prueba conexion informix\CONSOLA\icono.ico"
echo   Destino: CONSOLA.UI\icono.ico
echo.
pause
