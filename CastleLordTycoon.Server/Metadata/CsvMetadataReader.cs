using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace CastleLordTycoon.Server.Metadata;

public static class CsvMetadataReader
{
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        DetectDelimiter = false,
        HasHeaderRecord = true,
        BadDataFound = context =>
        {
            throw new InvalidDataException($"CSV 잘못된 데이터: {context.RawRecord}");
        },
        MissingFieldFound = null,
        TrimOptions = TrimOptions.Trim
    };

    public static IReadOnlyList<T> Load<T>(string filePath)
    {
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        using var csv = new CsvReader(reader, Configuration);
        var records = csv.GetRecords<T>().ToList();
        return records;
    }
}
