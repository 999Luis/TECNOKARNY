using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace TECNOKARNY.Servicios
{
    public class ServicioCorreo
    {
        public async Task EnviarCorreoAsync(string correoDestino, string asunto, string mensajeHtml)
        {
            var email = new MimeMessage();
            
            // 1. Configurar el remitente (Quien envía)
            email.From.Add(new MailboxAddress("TECNOKARNY Sistema", "tecnokarny@gmail.com"));
            
            // 2. Configurar el destinatario (Quien recibe)
            email.To.Add(new MailboxAddress("", correoDestino));
            
            // 3. Asunto
            email.Subject = asunto;

            // 4. Cuerpo del mensaje (Acepta formato HTML)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = mensajeHtml
            };
            email.Body = bodyBuilder.ToMessageBody();

            // 5. Configurar el cliente SMTP para el envío
            using (var cliente = new SmtpClient())
            {
                // Para Gmail/Outlook usualmente usamos el puerto 587 con STARTTLS
                await cliente.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // IMPORTANTE: En el caso de Gmail, "tu-contraseña" debe ser una 
                // "Contraseña de aplicación" generada desde tu cuenta de Google.
                await cliente.AuthenticateAsync("tecnokarny@gmail.com", "baps evlq xrnk ewcd");

                // Enviar el correo electrónico
                await cliente.SendAsync(email);
                
                // Desconectarse limpiamente
                await cliente.DisconnectAsync(true);
            }
        }
    }
}