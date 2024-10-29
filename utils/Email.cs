using System.Linq.Expressions;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ApiInmobiliariaAnNaTe.Services
{
    public class Email
    {
        private readonly string _smtpServer = "smtp.gmail.com";//"smtp-mail.outlook.com";servidor SMTP
        private readonly int _port = 587; //587 para gmail, outlook
        private readonly string _email; //email de salida
        private readonly string _password; //contraseña de aplicacion


        public Email(IConfiguration configuration)
        {
            _email = configuration["miapp:email"]; //email config
            _password = configuration["miapp:pass_de_aplicacion"]; // contraseña config
        }

        public async Task SendResetPasswordEmail(string email, string token)
        {
            // Crear y enviar el correo
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Andrea Natalia", _email));
            message.To.Add(new MailboxAddress("Encantador Destinatario", email));
            message.Subject = "Restablecimiento de contraseña";

            //enlace para restablecer pass
            var resetLink = $"http://192.168.1.2:5000/api/propietario/restablecePass?token={token}&email={email}";

            message.Body = new TextPart("html")
            {
                Text = $@"
    <html>
        <body>
            <p>Haz solicitado restablecer tu password en la app Inmobiliaria AnNaTe?</p>
            <p>Si es así, haz clic en el siguiente botón para confirmar el restablecimiento:</p>
            <br>
            <a href='{resetLink}' style='padding: 10px; background-color: blue; color: white; text-decoration: none; display: inline-block;'>Restablecer Password</a>
            <br>
            <p>Si no has enviado la solicitud, puedes ignorar este correo.</p>

        </body>
    </html>"
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _port, false);
                    await client.AuthenticateAsync(_email, _password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error al enviar el correo: {e.Message}");
            }
        }
    }
}

