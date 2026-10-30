using CasaGaillard.Core.Services;
using Xunit;

namespace CasaGaillard.Core.Tests;

public sealed class CsvReportFormatterTests
{
    [Theory]
    [InlineData("texto;con separador", "\"texto;con separador\"")]
    [InlineData("dijo \"hola\"", "\"dijo \"\"hola\"\"\"")]
    [InlineData("=HYPERLINK(\"https://example.invalid\")", "\"'=HYPERLINK(\"\"https://example.invalid\"\")\"")]
    [InlineData("+cmd|' /C calc'!A0", "\"'+cmd|' /C calc'!A0\"")]
    [InlineData(null, "\"\"")]
    public void Field_QuotesAndNeutralizesSpreadsheetFormulas(string? input, string expected)
    {
        Assert.Equal(expected, CsvReportFormatter.Field(input));
    }
}
