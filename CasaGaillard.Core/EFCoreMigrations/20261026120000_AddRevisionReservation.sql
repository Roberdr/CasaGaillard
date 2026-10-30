IF OBJECT_ID(N'gaillard.Revision', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'gaillard.Revision', N'DiaReservado') IS NULL
        ALTER TABLE gaillard.Revision ADD DiaReservado bit NOT NULL CONSTRAINT DF_Revision_DiaReservado DEFAULT(0);
    IF COL_LENGTH(N'gaillard.Revision', N'FechaReservada') IS NULL
        ALTER TABLE gaillard.Revision ADD FechaReservada date NULL;
END;

IF OBJECT_ID(N'gaillard.RevisionVehiculo', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'gaillard.RevisionVehiculo', N'DiaReservado') IS NULL
        ALTER TABLE gaillard.RevisionVehiculo ADD DiaReservado bit NOT NULL CONSTRAINT DF_RevisionVehiculo_DiaReservado DEFAULT(0);
    IF COL_LENGTH(N'gaillard.RevisionVehiculo', N'FechaReservada') IS NULL
        ALTER TABLE gaillard.RevisionVehiculo ADD FechaReservada date NULL;
END;
