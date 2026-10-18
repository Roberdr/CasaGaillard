# Publicación de CasaGaillard.Core

## Preparar el servidor

La aplicación requiere una instancia de SQL Server que contenga las bases de datos y tablas que ya utiliza Casa Gaillard. Configura estas variables en el entorno del proceso web; no guardes sus valores en el repositorio:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`: cadena de conexión a SQL Server. La aplicación no inicia si falta.
- `DataProtection__KeysPath`: ruta absoluta y persistente donde la identidad del proceso web pueda crear y leer el anillo de claves. Si se omite, ASP.NET Core usa su almacenamiento predeterminado, que puede no conservar las claves entre reinicios del proceso.

No copies `appsettings.Development.json` al servidor. El perfil de publicación lo excluye.

## Generar el paquete

Desde la raíz del repositorio:

```powershell
dotnet publish CasaGaillard.Core\CasaGaillard.Core.csproj -c Release --no-restore -p:PublishProfile=CasaGaillard.Core\Properties\PublishProfiles\Folder.pubxml
```

El perfil genera un paquete framework-dependent en `CasaGaillard.Core\bin\Release\net10.0\publish`. El servidor debe tener instalado el runtime de ASP.NET Core 10; para IIS, también necesita el módulo ASP.NET Core que procesa el `web.config` generado.

## Servir desde este PC a la red local

Este equipo tiene actualmente la dirección `192.168.1.91` y el nombre `LAPTOP-I1KUOS85`. La dirección puede cambiar con DHCP; para una URL estable, reserva la dirección en el router o usa el nombre DNS del equipo. No se encontró el servicio IIS `W3SVC`, por lo que la opción directa es Kestrel.

Para permitir inicio de sesión con seguridad, expón Kestrel por HTTPS y usa un certificado cuyo nombre DNS coincida con la URL que abrirán los demás equipos. Si no hay certificado de empresa, se puede usar uno autofirmado instalado en `CurrentUser\\My` del usuario que ejecuta Kestrel. Los equipos cliente tendrán que importar el certificado público en sus autoridades raíz de confianza; no se debe distribuir la clave privada. Configuración de ejemplo mediante variables de entorno:

- `Kestrel__Endpoints__Https__Url=https://0.0.0.0:5043`
- `Kestrel__Endpoints__Https__Certificate__Subject=LAPTOP-I1KUOS85`
- `Kestrel__Endpoints__Https__Certificate__Store=My`
- `Kestrel__Endpoints__Https__Certificate__Location=CurrentUser`
- `Kestrel__Endpoints__Https__Certificate__AllowInvalid=true` (necesario para el certificado autofirmado)
- `ConnectionStrings__DefaultConnection`: conexión SQL Server
- `DataProtection__KeysPath`: carpeta persistente para claves de autenticación

La regla del firewall debe limitar TCP 5043 al perfil **Privado**. Si la directiva de Windows mantiene la red en perfil Público, limita `RemoteAddress` a la subred local (por ejemplo, `192.168.1.0/24`) en lugar de abrir el puerto a cualquier origen. El proceso Kestrel debe ejecutarse con una cuenta de Windows que tenga permisos de lectura y escritura sobre el anillo de claves y acceso integrado a SQL Server. Los clientes deben usar el nombre DNS `LAPTOP-I1KUOS85` (o un DNS local estable que figure en el certificado), no una IP que pueda cambiar con DHCP.

Para iniciar Kestrel al iniciar sesión del usuario, registra una tarea del Programador de tareas con disparador **Al iniciar sesión**, nivel **Limitado** y acción `pwsh.exe -NoProfile -ExecutionPolicy Bypass -File "<ruta del proyecto>\CasaGaillard.Core\Start-LanServer.ps1"`. La tarea debe ejecutarse con la misma cuenta que tiene el certificado y acceso integrado a SQL Server. El script usa el binario Release del proyecto, la configuración local `appsettings.Lan.json` y escribe registros en `App_Data`.

## Comprobación antes del cambio de tráfico

1. Copia el contenido de `publish` al directorio de la aplicación del servidor.
2. Configura las variables anteriores y concede al proceso permisos de lectura y escritura sobre `DataProtection__KeysPath`.
3. Arranca la aplicación y confirma que carga la portada, permite iniciar sesión y puede consultar los módulos que acceden a SQL Server.
4. Mantén disponible el sitio anterior hasta completar esa verificación. No se ejecutan scripts de migración de base de datos durante la publicación.
