using MailKit.Net.Smtp;
using MimeKit;

namespace ApiInmobiliariaAnNaTe.Services
{
    public class Email
    {
        private readonly string _smtpServer = "smtp-mail.outlook.com";//"smtp.gmail.com"; //servidor SMTP
        private readonly int _port = 587; //587 para gmail, outlook
        private readonly string _email = "toony1717@hotmail.com"; //email de salida
        private readonly string _password = "xsip fkbb oiqw zosc"; //contraseña de aplicacion

        public async Task SendResetPasswordEmail(string email, string token)
        {
            // Crear y enviar el correo
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Andrea Natalia", _email));
            message.To.Add(new MailboxAddress("Encantador Destinatario", email));
            message.Subject = "Restablecimiento de contraseña";

            /*  message.Body = new TextPart("plain")
             {
                 Text = $"Haga clic en el siguiente enlace para restablecer su contraseña: http://localhost:5000/restablecer-contrasena?token={token}&email={email}"
             }; */

            //enlace para el restablecimiento de la contraseña
            var resetLink = $"http://localhost:5000/restablecerPass?token={token}&email={email}";
            message.Body = new TextPart("html")
            {
                Text = $@"
        <html>
        <body>
            <p>Haz solicitado restablecer tu contraseña? Si es así, haz clic en el siguiente botón para confirmar el restablecimiento de tu contraseña:</p>
            <a href='{resetLink}' style='padding: 10px; background-color: blue; color: white; text-decoration: none;'>Sí, restablecer contraseña</a>

            <p>Si no has solicitado restablecer tu contraseña, puedes ignorar este correo.</p>
        </body>
        </html>"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _port, false);
                await client.AuthenticateAsync(_email, _password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
