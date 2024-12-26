namespace LogTransformer.Api.Services;

public class LogConverterService
{
    public string ConverterParaAgora(string logEntrada)
    {
        var linhas = logEntrada.Split(Environment.NewLine);
        var logsConvertidos = new List<string>
        {
            "#Version: 1.0",
            "#Date: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            "#Fields: provider http-method status-code uri-path time-taken response-size cache-status"
        };

        foreach (var linha in linhas)
        {
            var campos = linha.Split('|');
            if (campos.Length != 5) continue;

            string provider = "MINHA CDN";
            string httpMethod = campos[3].Split(' ')[0].Trim('"');
            string statusCode = campos[1];
            string uriPath = campos[3].Split(' ')[1];
            string timeTaken = campos[4];
            string responseSize = campos[0];
            string cacheStatus = campos[2];

            logsConvertidos.Add($"\"{provider}\" {httpMethod} {statusCode} {uriPath} {timeTaken} {responseSize} {cacheStatus}");
        }

        return string.Join(Environment.NewLine, logsConvertidos);
    }
}

