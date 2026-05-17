using Microsoft.EntityFrameworkCore;
using TECNOKARNY.Models;

namespace TECNOKARNY.Servicios
{
    public class ServicioEnvioAutomatico : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServicioEnvioAutomatico> _logger;

        public ServicioEnvioAutomatico(IServiceScopeFactory scopeFactory, ILogger<ServicioEnvioAutomatico> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de Envío Automático Iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando envío automático de correos...");

                    // Ejecuta el envío de correos
                    await ProcesarYEnviarCorreos();
                    
                    _logger.LogInformation("Envío automático de correos finalizado.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error en el ciclo de cobranza: {ex.Message}");
                }

                _logger.LogInformation("El servicio dormirá por 24 horas hasta la siguiente ráfaga de cobro.");
                
                // Espera exactamente 24 horas para volver a cobrarle a los que sigan debiendo
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task ProcesarYEnviarCorreos()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BdtecnokarnyContext>();
                var servicioCorreo = scope.ServiceProvider.GetRequiredService<ServicioCorreo>();

                // --- NUEVA LÓGICA DIRECTA ---
                // Trae TODAS las ventas a crédito donde el saldo sea mayor a 0, 
                // sin importar la fecha en la que se compró o venza.
                var ventasConAdeudo = await db.Ventas
                    .Include(v => v.IdClienteNavigation)
                    .Where(v => v.Saldo > 0 
                             && v.Tipo.ToLower() == "crédito" 
                             && v.IdClienteNavigation.Correo != null)
                    .ToListAsync();

                _logger.LogInformation($"Se encontraron {ventasConAdeudo.Count} cuentas con saldo pendiente en el sistema.");

                foreach (var venta in ventasConAdeudo)
                {
                    string correoCliente = venta.IdClienteNavigation.Correo.Trim();
                    string nombreCliente = $"{venta.IdClienteNavigation.Nombre} {venta.IdClienteNavigation.ApePat}";
                    string asunto = "Recordatorio de Saldo Pendiente - TECNOKARNY";
                    string montoFormateado = string.Format(new System.Globalization.CultureInfo("es-MX"), "{0:C}", venta.Saldo);

                    string cuerpoHtml = $@"
                        <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; max-width: 600px; margin: 0 auto;'>
                            <div style='background-color: #9a0000; color: white; padding: 10px; text-align: center;'>
                                <h2>TECNOKARNY</h2>
                            </div>
                            <div style='padding: 20px;'>
                                <p>Estimado(a) <strong>{nombreCliente}</strong>,</p>
                                <p>Le informamos que en nuestro sistema se registra un saldo pendiente de su cuenta a crédito correspondiente a la nota de venta <strong>#{venta.Id}</strong>.</p>
                                
                                <div style='background-color: #f8f9fa; border-left: 4px solid #9a0000; padding: 15px; margin: 20px 0;'>
                                    <p style='margin: 0; font-size: 16px;'><strong>Monto Pendiente Actual:</strong> <span style='color: #9a0000; font-weight: bold;'>{montoFormateado}</span></p>
                                </div>

                                <p>Le recordamos que este correo se le enviará de manera diaria como recordatorio automático hasta que la cuenta quede liquidada en su totalidad (Saldo: $0.00).</p>
                                <p>Le solicitamos atentamente pasar a la brevedad a regularizar su situación. Si ya realizó su pago, favor de hacer caso omiso.</p>
                            </div>
                        </div>";

                    try
                    {
                        await servicioCorreo.EnviarCorreoAsync(correoCliente, asunto, cuerpoHtml);
                        _logger.LogInformation($"Correo de cobro enviado con éxito a: {correoCliente}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"No se pudo enviar correo al cliente {nombreCliente}: {ex.Message}");
                        continue;
                    }
                }
            }
        }
    }
}