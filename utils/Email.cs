using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace ApiInmobiliariaAnNaTe.Services
{
    public class Email
    {
        private readonly string _smtpServer = "smtp-mail.outlook.com";//"smtp.gmail.com"; //servidor SMTP
        private readonly int _port = 587; //587 para gmail, outlook
        private readonly string _email = "toony1717@hotmail.com"; //email de salida
        private readonly string _password = "Amorina11"; // contraseña

        public async Task SendResetPasswordEmail(string email, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Andrea Natalia", _email));
            message.To.Add(new MailboxAddress("Encantador Destinatario", email));
            message.Subject = "Restablecimiento de contraseña";

            message.Body = new TextPart("plain")
            {
                Text = $"Haga clic en el siguiente enlace para restablecer su contraseña: http://localhost:5000/restablecer-contrasena?token={token}&email={email}"
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
