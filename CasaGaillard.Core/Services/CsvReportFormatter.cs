namespace CasaGaillard.Core.Services;

public static class CsvReportFormatter
{
    public static string Field(string? value)
    {
        var text=value??string.Empty;
        if(text.Length>0&&text[0] is '=' or '+' or '-' or '@' or '\t' or '\r') text="'"+text;
        return "\""+text.Replace("\"","\"\"")+"\"";
    }
}
