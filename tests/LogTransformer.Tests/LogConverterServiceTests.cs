using FluentAssertions;
using LogTransformer.Api.Services;

namespace LogTransformer.Tests;

public class LogConverterServiceTests
{
    private readonly LogConverterService _service;

    public LogConverterServiceTests()
    {
        _service = new LogConverterService();
    }

    [Fact]
    public void ConverterParaAgora_DeveConverterLogCorretamente()
    {
        var logEntrada = "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2";
        var logConvertido = _service.ConverterParaAgora(logEntrada);

        logConvertido.Should().Contain("\"MINHA CDN\" GET 200 /robots.txt 100.2 312 HIT");
    }
}