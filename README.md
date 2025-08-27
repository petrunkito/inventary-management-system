# Inventary Management System

Este proyecto es una aplicación de gestión de inventario desarrollada con **.NET 8** y **PostgreSQL** bajo el enfoque **Database-First**.

## 🚀 Requisitos previos

Antes de iniciar, asegúrate de tener instalados los siguientes componentes:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [PostgreSQL](https://www.postgresql.org/download/) (proyecto probado con la versión **16.10**, aunque versiones posteriores también son compatibles)

## 📖 Enfoque Database-First

Existen dos enfoques comunes para trabajar con bases de datos en .NET:

- **Code-First**: Las entidades se crean desde el código y luego se generan las tablas en la base de datos.  
- **Database-First**: Las entidades se generan a partir de una base de datos existente.  

Este proyecto utiliza **Database-First**, es decir, las entidades fueron generadas a partir de una base de datos ya definida.

## ⚙️ Instalación y configuración

1. Clona este repositorio:
   ```
   git clone https://github.com/petrunkito/inventary-management-system.git
   cd inventary-management-system
   ```

2. Crea una base de datos en PostgreSQL llamada `inventary_management_system`.

   ### Opción 1: Desde la terminal
   (Sí, usas esta primera opción, recuerda que ya debiste haber creado la base de datos `inventary_management_system`)
   Ejecuta el script SQL en tu base de datos:
   ```
   psql -h <hostname> -U <user> -d <database_name> -f /ruta/al/database_script.sql
   ```

   Donde:
   - `<hostname>` → normalmente `localhost`  
   - `<user>` → el usuario de PostgreSQL (por defecto `postgres`)  
   - `<database_name>` → debe ser `inventary_management_system`  

   > 🔔 Asegúrate de poner correctamente la ruta hacia `database_script.sql`.

   ### Opción 2: Desde PgAdmin (Windows recomendado)
   - Crea una nueva base de datos llamada `inventary_management_system`.  
   - Abre la herramienta de scripts en PgAdmin.  
   - Copia y pega el contenido del archivo `database_script.sql`.  
   - Ejecuta el script.  

3. Configura la cadena de conexión en `appsettings.json`:

   Abre el archivo `appsettings.json` y localiza la sección `ConnectionStrings`. Verás algo similar a esto:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*",
     "ConnectionStrings": {
       "InventaryManagementSystemConnection": "Host=<hostname>;Port=<port>;Database=<database_name>;Username=postgres;Password=<password>"
     }
   }
   ```

   Reemplaza los valores por los de tu máquina local:
   - `<hostname>` → tu máquina local (generalmente `localhost`)  
   - `<port>` → el puerto donde corre PostgreSQL (por defecto `5432`)  
   - `<database_name>` → el nombre de la base de datos (debe ser `inventary_management_system`)  
   - `<password>` → la contraseña del usuario de la base de datos  

4. Ejecuta el proyecto:
   ```
   dotnet run --launch-profile "http"
   ```

   La aplicación estará disponible en:  
   👉 http://localhost:5136/

## 🎮 Uso

Una vez ejecutada la aplicación, ya puedes comenzar a interactuar con el sistema de inventario. 🚀  

---

✍️ Desarrollado por [Aurelio Antonio Obando Flores(petrunkito)](https://github.com/petrunkito)  