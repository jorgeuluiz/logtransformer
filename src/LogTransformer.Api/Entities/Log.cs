namespace LogTransformer.Api.Entities;

public class Log
{
    public int Id { get; set; }
    public string LogOriginal { get; set; } = string.Empty;
    public string LogConvertido { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
}
