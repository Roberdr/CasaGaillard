IF OBJECT_ID(N'dbo.PlanificadorEstados', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PlanificadorEstados
    (
        Clave nvarchar(120) NOT NULL CONSTRAINT PK_PlanificadorEstados PRIMARY KEY,
        Json nvarchar(max) NOT NULL,
        ActualizadoUtc datetime2 NOT NULL,
        ActualizadoPor nvarchar(256) NULL
    );
END;
