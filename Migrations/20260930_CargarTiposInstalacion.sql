/* Carga inicial de categorías de activos; es seguro ejecutarlo más de una vez. */
DECLARE @Tipos TABLE (Nombre NVARCHAR(100) NOT NULL, Descripcion NVARCHAR(500) NOT NULL);

INSERT INTO @Tipos (Nombre, Descripcion)
VALUES
    (N'Vehículos y transporte', N'Vehículos, remolques y medios de transporte de la empresa.'),
    (N'Equipos de manutención', N'Carretillas, transpaletas, elevadores y equipos para mover cargas.'),
    (N'Instalaciones industriales', N'Instalaciones generales de proceso, servicios e infraestructura industrial.'),
    (N'Instalaciones de almacenamiento y trasiego', N'Depósitos, silos, sistemas de carga, descarga y trasiego.'),
    (N'Equipos de proceso / producción', N'Maquinaria y equipos utilizados directamente en producción.'),
    (N'Equipos de pesaje y metrología', N'Básculas, balanzas, instrumentos de medida y patrones.'),
    (N'Instalaciones del edificio', N'Instalaciones eléctricas, climatización, agua y otros servicios del edificio.'),
    (N'Equipamiento de oficina', N'Ordenadores, impresoras y otros equipos de oficina.'),
    (N'Equipamiento de servicios', N'Equipos auxiliares de limpieza, taller, laboratorio y servicios generales.'),
    (N'Seguridad y emergencias', N'Equipos e instalaciones de protección, detección y respuesta ante emergencias.');

INSERT INTO gaillard.TipoInstalacion (Nombre, Descripcion)
SELECT t.Nombre, t.Descripcion
FROM @Tipos t
WHERE NOT EXISTS
(
    SELECT 1 FROM gaillard.TipoInstalacion existente
    WHERE existente.Nombre = t.Nombre
);
