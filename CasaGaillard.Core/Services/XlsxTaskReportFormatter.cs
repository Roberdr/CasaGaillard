using System.IO.Compression;
using System.Security;
using System.Text;

namespace CasaGaillard.Core.Services;

public sealed record TaskReportRow(string Codigo, string Titulo, string Equipo, string Ubicacion, string Prioridad,
    string Estado, string Responsable, string FechaProgramada, string FechaCreacion);

public static class XlsxTaskReportFormatter
{
    private static readonly (string Name, Func<ClassifiedTaskReportRow, bool> Filter)[] Sheets =
    [
        ("Pendientes", row => row.Clasificacion == "Pendiente"),
        ("Atrasadas", row => row.Clasificacion == "Atrasada"),
        ("Completadas", row => row.Clasificacion == "Completada")
    ];

    public static byte[] Create(IReadOnlyList<ClassifiedTaskReportRow> rows)
    {
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
        {
            Write(archive, "[Content_Types].xml", ContentTypes);
            Write(archive, "_rels/.rels", RootRelationships);
            Write(archive, "xl/workbook.xml", Workbook);
            Write(archive, "xl/_rels/workbook.xml.rels", WorkbookRelationships);
            for (var index = 0; index < Sheets.Length; index++)
                Write(archive, $"xl/worksheets/sheet{index + 1}.xml", BuildWorksheet(rows.Where(Sheets[index].Filter).Select(x => x.Row).ToList()));
        }
        return output.ToArray();
    }

    private static string BuildWorksheet(IReadOnlyList<TaskReportRow> rows)
    {
        var headers = new[] { "Código", "Tarea", "Equipo", "Ubicación", "Prioridad", "Estado", "Responsable", "Fecha prevista", "Fecha de creación" };
        var xml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetViews><sheetView workbookViewId=\"0\"><pane ySplit=\"1\" topLeftCell=\"A2\" activePane=\"bottomLeft\" state=\"frozen\"/></sheetView></sheetViews><sheetFormatPr defaultRowHeight=\"15\"/><sheetData>");
        AppendRow(xml, 1, headers);
        for (var index = 0; index < rows.Count; index++)
        {
            var row = rows[index];
            AppendRow(xml, index + 2, [row.Codigo, row.Titulo, row.Equipo, row.Ubicacion, row.Prioridad, row.Estado, row.Responsable, row.FechaProgramada, row.FechaCreacion]);
        }
        xml.Append("</sheetData><autoFilter ref=\"A1:I").Append(rows.Count + 1).Append("\"/></worksheet>");
        return xml.ToString();
    }

    private static void AppendRow(StringBuilder xml, int rowNumber, IReadOnlyList<string> values)
    {
        xml.Append("<row r=\"").Append(rowNumber).Append("\">");
        for (var index = 0; index < values.Count; index++)
        {
            var cell = $"{(char)('A' + index)}{rowNumber}";
            xml.Append("<c r=\"").Append(cell).Append("\" t=\"inlineStr\"><is><t xml:space=\"preserve\">")
                .Append(SecurityElement.Escape(values[index] ?? string.Empty)).Append("</t></is></c>");
        }
        xml.Append("</row>");
    }

    private static void Write(ZipArchive archive, string path, string content)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
        using var stream = entry.Open();
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        writer.Write(content);
    }

    private const string ContentTypes = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/><Override PartName=\"/xl/worksheets/sheet2.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/><Override PartName=\"/xl/worksheets/sheet3.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>";
    private const string RootRelationships = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>";
    private const string Workbook = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Pendientes\" sheetId=\"1\" r:id=\"rId1\"/><sheet name=\"Atrasadas\" sheetId=\"2\" r:id=\"rId2\"/><sheet name=\"Completadas\" sheetId=\"3\" r:id=\"rId3\"/></sheets></workbook>";
    private const string WorkbookRelationships = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/><Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet2.xml\"/><Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet3.xml\"/></Relationships>";
}

public sealed record ClassifiedTaskReportRow(string Clasificacion, TaskReportRow Row);
