# ACBNetBackend

## Requisitos
- .NET 8 SDK
- Visual Studio 2019 o superior

## Instrucciones para ejecutar
1. Clona el repositorio.
2. Abre el proyecto en Visual Studio.
3. Restaura las dependencias con `dotnet restore`.
4. Ejecuta la aplicación con `dotnet run`.

## Token
1. Se debe generar un Token con el endpoint /api/Admin/generate-token
2. Poner el Token en Postman o bien en el mismo Swagger para poder acceder a la API

## Pruebas
Ejecuta las pruebas unitarias con el siguiente comando:
```bash
dotnet test
