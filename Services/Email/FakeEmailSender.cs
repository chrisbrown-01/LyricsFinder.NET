using Microsoft.AspNetCore.Identity.UI.Services;

namespace LyricsFinder.NET.Services.Email
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}