# Taller 2 - CRUD de Productos con Entity Framework Core y MySQL

API REST en ASP.NET Core (.NET 10) que realiza las operaciones CRUD sobre la tabla
`Producto` de una base de datos MySQL, usando Entity Framework Core, migraciones,
repositorios, interfaces, controladores y operaciones asincronas.

## Integrantes

1. Luis Silva
2. Fabian Orlando

## Estructura del proyecto

```
EcommerceApi/
  Controllers/
    ProductoController.cs      Endpoints HTTP
  DB/
    AppDbContext.cs            DbContext de Entity Framework Core
  Interfaces/
    IProductoRepository.cs     Contrato del repositorio
  Models/
    Producto.cs                Entidad
    Dtos/ProductoDto.cs        Datos que llegan en el body
  Repository/
    ProductoRepository.cs      Acceso a datos y validaciones
  Migrations/                  Migracion InitialCreate
  Program.cs                   Registro del DbContext y del repositorio
  appsettings.json             Cadena de conexion (sin credenciales reales)
```

## Modelo Producto

| Propiedad   | Tipo    | Columna en MySQL |
|-------------|---------|------------------|
| Id          | int     | int, auto incremental, PK |
| Nombre      | string  | varchar(150), NOT NULL |
| Descripcion | string? | varchar(500), NULL |
| Precio      | decimal | decimal(18,2) |
| Stock       | int     | int |

En esta version la entidad es independiente: no tiene `UsuarioId`, `CategoriaId`,
`Imagen` ni navegaciones hacia otras entidades.

## Configuracion

1. Crear la base de datos en MySQL:

   ```sql
   CREATE DATABASE EcommerceDB CHARACTER SET utf8mb4;
   ```

   La tabla `Producto` NO se crea a mano: la crea la migracion.

2. Configurar la cadena de conexion **sin subir credenciales a GitHub**. Dos opciones:

   - Copiar `EcommerceApi/appsettings.Development.json.ejemplo` a
     `EcommerceApi/appsettings.Development.json` y poner ahi el usuario y la
     contrasena reales. Ese archivo esta en el `.gitignore`.
   - O usar los User Secrets de .NET:

     ```bash
     dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=EcommerceDB;User ID=root;Password=tu_password"
     ```

   El `appsettings.json` versionado deja los valores como `CAMBIAR_USUARIO` /
   `CAMBIAR_PASSWORD` a proposito.

3. Restaurar paquetes:

   ```bash
   dotnet restore
   ```

## Migraciones

La migracion `InitialCreate` ya esta incluida en `EcommerceApi/Migrations/`.
Para aplicarla a la base de datos:

```bash
dotnet ef database update --project EcommerceApi
```

Si se necesita regenerarla desde cero (borrando antes la carpeta `Migrations`):

```bash
dotnet ef migrations add InitialCreate --project EcommerceApi
dotnet ef database update --project EcommerceApi
```

Si el comando `dotnet ef` no esta instalado:

```bash
dotnet tool install --global dotnet-ef
```

## Ejecutar

```bash
dotnet run --project EcommerceApi
```

La API queda en `http://localhost:5094` (perfil `http` de `launchSettings.json`).

## Endpoints

Cada operacion responde en dos rutas equivalentes: la REST que pide el taller
y la de estilo nombrado usada en el ejemplo de clase.

| Metodo | Ruta REST             | Ruta estilo ejemplo             | Descripcion |
|--------|-----------------------|---------------------------------|-------------|
| GET    | `/api/producto`       | `/Producto/GetProductos`        | Devuelve todos los productos |
| POST   | `/api/producto`       | `/Producto/CreateProducto`      | Crea un producto |
| PUT    | `/api/producto/{id}`  | `/Producto/UpdateProducto/{id}` | Actualiza un producto existente |
| DELETE | `/api/producto/{id}`  | `/Producto/DeleteProducto/{id}` | Elimina un producto |

Ejemplo de body para POST y PUT:

```json
{
  "nombre": "Laptop Lenovo",
  "descripcion": "Laptop para trabajo y estudio",
  "precio": 2500000,
  "stock": 10,
  "imagenBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ..."
}
```

`imagenBase64` es opcional. En el PUT, si no se envia, se conserva la imagen actual.

## Imagenes con Cloudinary

Las imagenes no se guardan en MySQL:

1. El cliente envia la imagen en Base64 (`imagenBase64`).
2. `CloudinaryService` (`Services/`) la convierte a bytes y la sube a la carpeta `productos` de Cloudinary.
3. Cloudinary devuelve una URL segura, que se guarda en `Producto.ImagenUrl`.

Credenciales: seccion `CloudinarySettings` (clase `Configuration/CloudinarySettings.cs`).
En `appsettings.json` solo hay valores de ejemplo; las reales van en
`appsettings.Development.json` (ignorado por git, ver `appsettings.Development.json.ejemplo`):

```json
"CloudinarySettings": {
  "CloudName": "TU_CLOUD_NAME",
  "ApiKey": "TU_API_KEY",
  "ApiSecret": "TU_API_SECRET"
}
```

La migracion `AddImagenUrlToProducto` agrega la columna `ImagenUrl`:

```bash
dotnet ef database update --project EcommerceApi
```

### Validaciones

En POST y PUT se valida que:

1. El nombre no puede estar vacio.
2. El precio no puede ser negativo.
3. El stock no puede ser negativo.
4. Si se envia `imagenBase64`, debe ser un Base64 valido.

Si una validacion falla, la API responde `400 Bad Request` con un mensaje.
Si el producto del PUT o del DELETE no existe, responde `404 Not Found`.

### Pruebas rapidas

El archivo `EcommerceApi/EcommerceApi.http` trae las peticiones listas para
ejecutar desde Visual Studio o desde la extension REST Client de VS Code,
incluyendo los casos que deben fallar con 400 y 404.

Tambien esta la coleccion `Taller2-CRUD-Productos.postman_collection.json`,
lista para importar en Postman (Import > File).

Despues de cada POST, PUT o DELETE se puede comprobar en MySQL que el cambio
quedo guardado:

```sql
SELECT * FROM EcommerceDB.Producto;
```
