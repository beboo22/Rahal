using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Infrastructure.Abestraction;

namespace Service.Abestraction
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            var emailservice = new MimeMessage();
            emailservice.From.Add(new MailboxAddress("Order Management System", "testforprojects99@gmail.com"));
            emailservice.To.Add(new MailboxAddress(to, to));
            emailservice.Subject = subject;
            emailservice.Body = new BodyBuilder()
            {
                TextBody = $"{body}"
            }.ToMessageBody();
            try
            {

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls); // Corrected method and port
                    client.Authenticate("testforprojects99@gmail.com", "dnja yveb onyl ayzr"); // Replace with actual credentials
                    await client.SendAsync(emailservice); // Corrected parameter
                    await client.DisconnectAsync(true); // Corrected method
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
