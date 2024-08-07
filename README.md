# ACB Prueba Técnica de Àlex Andreu Borrell

## Descripción
Este proyecto es una prueba técnica para ACB. Está desarrollado en .NET 8 con C# y utiliza Entity Framework para la gestión de la base de datos SQLite y xUnit para las pruebas unitarias.

## Requisitos Previos
Antes de clonar y ejecutar el proyecto, asegúrate de tener instalados los siguientes componentes:
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)

## Instalación

### Clonar el repositorio desde Visual Studio 2022
Se puede clonar con la url: https://github.com/anbooco/ACB_Prueba_Tecnica.git
En Visual Studio verificar que las Dependencias estan todas bien o ejecutar ``` Update-Package ```

### Preparar la Base de Datos local
Ejecutar ``` Update-database ```. Esto generará un archivo *.db

### Ejecutar la API
##### Con Swagger
1. Levantar el proyecto con el profile https y acceder a https://localhost:7004/swagger/index.html
2. Generar un Token con el endpoint
3. Autenticarse en Swagger con el simbolo del  condado y escribir "Bearer <token>"
4. Probar los endpoints
##### Con Postman
1. En Postman desactivar la comprobacion SSL
2. Generar un Token con el endpoint con https://localhost:7004/api/Admin/generate-token
3. En los demás endpoints usar el token generado para generar la Autorizacion en Postman
4. Probar los endpoints
