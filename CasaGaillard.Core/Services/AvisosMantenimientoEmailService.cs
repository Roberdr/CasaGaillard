using System.Net;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class AvisosMantenimientoEmailService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AvisosMantenimientoEmailService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public AvisosMantenimientoEmailService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<AvisosMantenimientoEmailService> logger, IHttpClientFactory httpClientFactory)
    {
        _scopeFactory=scopeFactory; _configuration=configuration; _logger=logger; _httpClientFactory=httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now=DateTime.Now;
            var next=now.Date.AddHours(8);
            if(next<=now) next=next.AddDays(1);
            try { await Task.Delay(next-now,stoppingToken); }
            catch(OperationCanceledException) when(stoppingToken.IsCancellationRequested) { break; }
            try { await SendDigestAsync(stoppingToken); }
            catch(Exception ex) when(ex is not OperationCanceledException) { _logger.LogError(ex,"No se pudo enviar el resumen diario de mantenimiento."); }
        }
    }

    private async Task SendDigestAsync(CancellationToken cancellationToken)
    {
        var section=_configuration.GetSection("EmailAvisos");
        var from=section["Remitente"];
        var recipients=(section["Destinatarios"]??string.Empty).Split(';',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
        var provider=section["Proveedor"];
        var graph=string.Equals(provider,"MicrosoftGraph",StringComparison.OrdinalIgnoreCase);
        if(string.IsNullOrWhiteSpace(from)||recipients.Length==0||(!graph&&string.IsNullOrWhiteSpace(section["Host"]))||
            (graph&&(string.IsNullOrWhiteSpace(section["TenantId"])||string.IsNullOrWhiteSpace(section["ClientId"])||string.IsNullOrWhiteSpace(section["ClientSecret"]))))
        {
            _logger.LogInformation("Avisos por correo sin activar: configura el proveedor y sus credenciales junto con Remitente y Destinatarios.");
            return;
        }

        using var scope=_scopeFactory.CreateScope();
        var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var limite=DateTime.Today.AddDays(14);
        var tareas=await db.TareasMantenimiento.AsNoTracking()
            .Where(t=>t.FechaProgramada.HasValue&&t.FechaProgramada.Value<=limite&&t.Estado!= "Cerrada")
            .OrderBy(t=>t.FechaProgramada).ThenBy(t=>t.Prioridad)
            .Select(t=>new {t.Codigo,t.Titulo,t.Equipo,t.FechaProgramada,t.AsignadaA,t.Estado})
            .ToListAsync(cancellationToken);
        if(tareas.Count==0) return;

        using var message=new MailMessage { From=new MailAddress(from), Subject=$"Avisos de mantenimiento · {DateTime.Today:dd/MM/yyyy}", IsBodyHtml=false };
        foreach(var recipient in recipients) message.To.Add(new MailAddress(recipient));
        message.Body="Tareas vencidas o previstas en los próximos 14 días:\r\n\r\n"+string.Join("\r\n",tareas.Select(t=>$"{t.FechaProgramada:dd/MM/yyyy} | {t.Codigo} | {t.Equipo??t.Titulo} | {t.Estado} | Responsable: {t.AsignadaA??"Sin asignar"}"));
        if (graph)
            await SendWithMicrosoftGraphAsync(section, from, recipients, message.Subject, message.Body, cancellationToken);
        else
        {
            var port=int.TryParse(section["Puerto"],out var parsed)?parsed:587;
            using var client=new SmtpClient(section["Host"]!,port) { EnableSsl=bool.TryParse(section["UsarSsl"],out var ssl)?ssl:true };
            var username=section["Usuario"]; var password=section["Clave"];
            if(!string.IsNullOrWhiteSpace(username)) client.Credentials=new NetworkCredential(username,password);
            await client.SendMailAsync(message,cancellationToken);
        }
        _logger.LogInformation("Enviado resumen diario de mantenimiento a {NumeroDestinatarios} destinatario(s).",recipients.Length);
    }

    private async Task SendWithMicrosoftGraphAsync(IConfigurationSection section, string from, string[] recipients, string subject, string body, CancellationToken cancellationToken)
    {
        var tenant = Uri.EscapeDataString(section["TenantId"]!);
        var tokenEndpoint = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";
        using var tokenResponse = await _httpClientFactory.CreateClient().PostAsync(tokenEndpoint,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = section["ClientId"]!,
                ["client_secret"] = section["ClientSecret"]!,
                ["scope"] = "https://graph.microsoft.com/.default",
                ["grant_type"] = "client_credentials"
            }), cancellationToken);
        tokenResponse.EnsureSuccessStatusCode();
        await using var tokenStream = await tokenResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var tokenJson = await JsonDocument.ParseAsync(tokenStream, cancellationToken: cancellationToken);
        var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
        if (string.IsNullOrWhiteSpace(accessToken)) throw new InvalidOperationException("Microsoft Identity Platform no devolvió un token de acceso.");

        var payload = JsonSerializer.Serialize(new
        {
            message = new
            {
                subject,
                body = new { contentType = "Text", content = body },
                toRecipients = recipients.Select(address => new { emailAddress = new { address } })
            },
            saveToSentItems = true
        });
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(from)}/sendMail");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var response = await _httpClientFactory.CreateClient().SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
