using LogTransformer.Api.Data;
using LogTransformer.Api.Entities;
using LogTransformer.Api.Messages;
using LogTransformer.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogTransformer.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly LogDbContext _context;
    private readonly LogConverterService _logConverterService;
    private readonly IHttpClientFactory _httpClientFactory;

    public LogController(LogDbContext context, LogConverterService logConverterService, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _logConverterService = logConverterService;
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpPost("transformar")]
    public async Task<IActionResult> TransformarLogPorUrl([FromBody] string url, [FromQuery] bool salvarArquivo = false)
    {
        var logEntrada = await ObterLogPorUrl(url); 
        if (string.IsNullOrWhiteSpace(logEntrada))        
            return NotFound(ExceptionMessages.LogNaoEncontrado);
        

        var logConvertido = _logConverterService.ConverterParaAgora(logEntrada);

        if (salvarArquivo)
        {
            var diretorioLogs = Path.Combine(Directory.GetCurrentDirectory(), "ConvertedLogs");

            if (!Directory.Exists(diretorioLogs))            
                Directory.CreateDirectory(diretorioLogs);            

            var path = Path.Combine(Directory.GetCurrentDirectory(), "ConvertedLogs", $"log_{Guid.NewGuid()}.txt");


            await System.IO.File.WriteAllTextAsync(path, logConvertido);
            return Ok(new { path });
        }

        return Ok(new { logConvertido });
    }
    
    [HttpGet("logs/salvos")]
    public async Task<IActionResult> BuscarLogsSalvos()
    {
        var logs = await _context.Logs.ToListAsync();
        return Ok(logs);
    }
    
    [HttpGet("log/salvo/{id}")]
    public async Task<IActionResult> BuscarLogSalvoPorId(int id)
    {
        var log = await _context.Logs.FindAsync(id);
        if (log is null)        
            return NotFound(ExceptionMessages.LogNaoEncontrado);
        
        return Ok(log);
    }
    
    [HttpGet("log/transformado/{id}")]
    public async Task<IActionResult> BuscarLogTransformadoPorId(int id)
    {
        var log = await _context.Logs.FindAsync(id);
        if (log is null)        
            return NotFound(ExceptionMessages.LogNaoEncontrado);
        
        return Ok(new { log.LogOriginal, log.LogConvertido });
    }
    
    [HttpPost("salvar")]
    public async Task<IActionResult> SalvarLog([FromBody] string url)
    {
        if (string.IsNullOrWhiteSpace(url))        
            return BadRequest(ExceptionMessages.UrlNaoValida);

        var logEntrada = await ObterLogPorUrl(url);
        
        if (string.IsNullOrWhiteSpace(logEntrada))        
            return BadRequest(ExceptionMessages.UrlNaoEncontrada);        

        var logConvertido = _logConverterService.ConverterParaAgora(logEntrada);

        var log = new Log
        {
            LogOriginal = logEntrada,
            LogConvertido = logConvertido
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        return Ok(new { log.Id });
    }

    private async Task<string> ObterLogPorUrl(string url)
    {
        var client = _httpClientFactory.CreateClient();

        try
        {            
            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {                
                var logConteudo = await response.Content.ReadAsStringAsync();
                return logConteudo;
            }
            else
                return $"Erro ao obter log: {response.StatusCode}";
            
        }
        catch (Exception ex)
        {            
            return $"Erro ao fazer requisição: {ex.Message}";
        }
    }
}
