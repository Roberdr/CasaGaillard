# Publicación de CasaGaillard.Core

## Preparar el servidor

La aplicación requiere una instancia de SQL Server que contenga las bases de datos y tablas que ya utiliza Casa Gaillard. Configura estas variables en el entorno del proceso web; no guardes sus valores en el repositorio:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`: cadena de conexión a SQL Server. La aplicación no inicia si falta.
- `DataProtection__KeysPath`: ruta absoluta y persistente donde la identidad del proceso web pueda crear y leer el anillo de claves. Si se omite, ASP.NET Core usa su almacenamiento predeterminado, que puede no conservar las claves entre reinicios del proceso.

No copies `appsettings.Development.json` al servidor. El perfil de publicación lo excluye.

## Archivos de fotos y documentos

Fotos de accesorios, equipos, instalaciones y tareas, junto con documentos de instalaciones, se guardan bajo `App_Data` en la raíz física de la aplicación. Esta carpeta es persistente: no la incluyas en Git ni la sustituyas al publicar una versión nueva. Configura la copia de seguridad del servidor para incluirla. El perfil de publicación excluye `App_Data` para que los datos no se empaqueten ni se borren al regenerar `publish`.

En la migración desde el proyecto MVC anterior, las carpetas `App_Data\AccesoriosFotos`, `App_Data\InstalacionEquipoFotos`, `App_Data\InstalacionesDocumentos` y `App_Data\TareasMantenimientoFotos` se copian a `CasaGaillard.Core\App_Data`. Las rutas relativas ya guardadas en SQL siguen siendo válidas.

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

## Actualización del esquema para tareas programadas

La versión que añade fechas previstas necesita una columna nullable en la tabla existente `dbo.TareasMantenimiento`. Antes de desplegarla, genera y revisa el SQL de la migración `20261018120000_AddFechaProgramadaTarea` y aplícalo en SQL Server con el procedimiento habitual de administración de la base de datos. El cambio es equivalente a:

```sql
ALTER TABLE dbo.TareasMantenimiento ADD FechaProgramada date NULL;
```

Haz una copia de seguridad según el procedimiento operativo vigente antes de modificar el esquema. Las tareas actuales conservarán `FechaProgramada` vacía hasta que se editen.

## Calendario e historial de cambios

La versión que añade el historial de edición de tareas necesita la tabla `dbo.TareaMantenimientoHistorial`. Aplica y revisa la migración `20261018130000_AddTaskAuditHistory` antes de publicar la aplicación. El SQL equivalente es:

```sql
CREATE TABLE dbo.TareaMantenimientoHistorial (
    ID bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_TareaMantenimientoHistorial PRIMARY KEY,
    TareaMantenimientoID int NOT NULL,
    Usuario nvarchar(100) NULL,
    Accion nvarchar(20) NOT NULL,
    Fecha datetime2 NOT NULL,
    CambiosJson nvarchar(max) NOT NULL
);
CREATE INDEX IX_TareaMantenimientoHistorial_TareaMantenimientoID_Fecha
    ON dbo.TareaMantenimientoHistorial(TareaMantenimientoID, Fecha);
```

El historial conserva los eventos aunque se elimine una tarea; por eso la tabla no tiene una clave foránea con borrado en cascada. Los indicadores de costes y la exportación usan las tablas de intervenciones existentes.

## Email de personas y cuentas

Antes de publicar la gestión de usuarios vinculada a personas, aplica la migración `20261020120000_AddPersonaEmail` (archivo `EFCoreMigrations/20261020120000_AddPersonaEmail.sql`). Añade el email opcional a `gaillard.Persona` y un índice único para evitar duplicados. Al crear una cuenta, la aplicación vincula una persona que ya tenga ese email; si no la encuentra, solicita nombre, apellidos y NIF y crea la persona asociada.

El resumen diario por correo admite SMTP y Microsoft Graph. Para SMTP configura `EmailAvisos__Proveedor=Smtp`, `EmailAvisos__Host`, `EmailAvisos__Puerto`, `EmailAvisos__UsarSsl`, `EmailAvisos__Remitente` y `EmailAvisos__Destinatarios`; si el proveedor lo requiere, añade `EmailAvisos__Usuario` y `EmailAvisos__Clave`. Los destinatarios se separan con punto y coma.

Para Microsoft 365 configura `EmailAvisos__Proveedor=MicrosoftGraph`, `EmailAvisos__TenantId`, `EmailAvisos__ClientId`, `EmailAvisos__ClientSecret`, `EmailAvisos__Remitente` (buzón autorizado) y `EmailAvisos__Destinatarios`. Registra una aplicación en Microsoft Entra ID, concede el permiso de aplicación Microsoft Graph `Mail.Send` y otorga consentimiento de administrador. El servicio obtiene un token OAuth 2.0 con el flujo de credenciales de cliente y envía por `/users/{remitente}/sendMail`; el secreto debe almacenarse solo como secreto de despliegue o variable de entorno. Sin la configuración completa, no se envía correo.

## Teléfonos de personas y entidades

`CasaGaillard.Core` usa la tabla común `gaillard.Telefono`. Un teléfono personal queda vinculado a una persona; uno de trabajo queda vinculado a la entidad y a la persona que lo usa. Para las bases que aún no tienen la tabla, aplica primero `EFCoreMigrations/20261021150000_UnifyOrganizationPhones.sql` y después `EFCoreMigrations/20261021160000_LinkWorkPhonesToPeople.sql`.

Para guardar pies de foto de imágenes en activos, accesorios y tareas de mantenimiento, aplica `EFCoreMigrations/20261021170000_AddPhotoObservations.sql` antes de publicar la versión que incluye esos campos.

Las fotos existentes de cubas se importan al almacenamiento común la primera vez que se abre su galería. Aplica `EFCoreMigrations/20261021180000_AddCubaPhotoObservations.sql` para importar también las observaciones existentes y `EFCoreMigrations/20261021190000_AddCubaPhotoImportTracking.sql` para evitar volver a importar archivos que se eliminen después. Las nuevas imágenes de cuba se guardan directamente en `dbo.InstalacionEquipoFotos`.

Los equipos móviles pueden asociarse a un vehículo en vez de a una instalación. Antes de publicar esa versión, aplica `EFCoreMigrations/20261022100000_LinkEquipmentToVehicles.sql`: añade `VehiculoId` a `gaillard.Equipo`, permite que `InstalacionId` quede vacío y exige que cada equipo tenga exactamente una ubicación.

Para registrar los días reservados de las revisiones de cubas y vehículos, aplica `EFCoreMigrations/20261026120000_AddRevisionReservation.sql` antes de publicar la versión que muestra esos campos en los listados.

## Planificador de reparto

Antes de habilitar el área `/Repartos/Planificador`, aplica `EFCoreMigrations/20261024120000_AddPlanificadorEstados.sql`. El módulo guarda los repartos, el stock, los movimientos, las ubicaciones de clientes y el histórico en `dbo.PlanificadorEstados`; restringe la interfaz y sus endpoints a usuarios con rol Administrador o Mantenimiento. Al abrir esta versión desde un navegador que ya tenga datos locales, estos se copian al servidor cuando la clave correspondiente aún no existe. Para traer los datos del navegador anterior, también puedes exportar una copia JSON desde el planificador antiguo y usar «Restaurar copia». El formato de copia antiguo incluye pedidos, ubicaciones de clientes e histórico, pero no el stock de cisternas ni el registro de movimientos; vuelve a importar desde Excel esos datos auxiliares si los necesitas. Conserva el archivo original como respaldo hasta revisar la migración.

La primera migración copia los datos de las tablas anteriores y ambas migraciones conservan esas tablas para que la aplicación MVC antigua siga pudiendo ejecutarse durante la transición. Los teléfonos de entidad existentes se copian sin persona asignada porque las tablas antiguas no guardan ese vínculo; se pueden asignar desde la ficha de la entidad en Core. Una vez aplicada la segunda migración, Core permite teléfonos vinculados a persona y entidad. Los cambios hechos desde la aplicación antigua no se sincronizan con `gaillard.Telefono`; aplica las migraciones en el corte hacia Core o vuelve a ejecutar una sincronización revisada antes de retirar la aplicación anterior.
