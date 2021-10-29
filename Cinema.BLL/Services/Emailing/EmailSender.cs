using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Cinema.BLL.Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IHostEnvironment _env;
        public EmailSender(IHostEnvironment env)
        {
            _env = env;
        }
        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            MailMessage mailMessage = new();

            if (_env.IsDevelopment())
            {
                mailMessage.From = new MailAddress("serveremail@ourhosting.com");
            }

            mailMessage.To.Add(new MailAddress(email));

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlMessage;

            SmtpClient client = new();

            if (_env.IsDevelopment())
            {
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                System.IO.Directory.CreateDirectory(@"C:\Test");
                client.PickupDirectoryLocation = @"C:\Test";
            }
            else if (_env.IsProduction())
            {
                // settings for emailing
            }

            await client.SendMailAsync(mailMessage);
        }
    }
}