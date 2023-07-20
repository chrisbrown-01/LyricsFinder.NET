using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;

namespace LyricsFinder.NET.Services
{
    public class MailkitEmailSender : IEmailSender, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly SmtpClient _smtpClient;
        private readonly string _emailSender;

        public MailkitEmailSender(IConfiguration configuration)
        {
            _config = configuration;
            _emailSender = _config["EmailSenderService:EmailUsername"]!;

            var emailPassword = _config["EmailSenderService:EmailPassword"];
            var smtpHost = _config["EmailSenderService:SmtpHost"];
            var smtpPort = Int32.Parse(_config["EmailSenderService:SmtpPort"]!);

            _smtpClient = new();
            _smtpClient.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate(_emailSender, emailPassword);
        }

        public void Dispose()
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }

        /// <summary>
        /// Send email to user for registration, password resets, wrong song info notifications, etc.
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="message">Email message contents</param>
        /// <returns></returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mailkitEmail = new MimeMessage();

                mailkitEmail.From.Add(MailboxAddress.Parse(_emailSender));
                mailkitEmail.To.Add(MailboxAddress.Parse(email));

                mailkitEmail.Subject = subject;
                mailkitEmail.Body = new TextPart(TextFormat.Html) { Text = message };

                await _smtpClient.SendAsync(mailkitEmail);
            }
            catch
            {
                // TODO: proper logger message
                Console.WriteLine("Error sending email.");
            }
        }
    }
}
