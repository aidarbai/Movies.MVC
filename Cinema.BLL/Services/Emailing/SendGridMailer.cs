using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Cinema.BLL.Services.Emailing
{
    public class SendGridMailer : IEmailSender
    {
        private readonly ILogger<SendGridMailer> _logger;
        private readonly string sendGridApiKey;
        private readonly string senderEmail;
        private readonly string senderName;
        public SendGridMailer(ILogger<SendGridMailer> logger, IConfiguration configuration)
        {
            _logger = logger;
            sendGridApiKey = configuration["SendGrid:ApiKey"];
            senderEmail = configuration["SendGrid:SenderEmail"];
            senderName = configuration["SendGrid:SenderName"];
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(new SendGridClientOptions { ApiKey = sendGridApiKey, HttpErrorAsException = true });
            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(email);
            var plainTextContent = "Message from Cinema.MVC server";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlMessage);
            Response response = await client.SendEmailAsync(msg);
            var statusCode = response.StatusCode;
            _logger.LogInformation($"SendGrid responded with the code: {statusCode}");
        }
    }
}
