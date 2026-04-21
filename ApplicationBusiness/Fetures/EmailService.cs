using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Infrastructure.Abestraction;

namespace Service.Abestraction
{
    public class EmailService : IEmailService
    {
    


        public async Task<bool> SendEmail(string to, string subject, string body, bool isHtml = false)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(subject, "testforprojects99@gmail.com"));
            email.To.Add(new MailboxAddress(to, to));
            email.Subject = subject;

            var builder = new BodyBuilder();

            if (isHtml)
                builder.HtmlBody = body; // Use HTML template
            else
                builder.TextBody = body; // Plain text fallback

            email.Body = builder.ToMessageBody();

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("testforprojects99@gmail.com", "dnja yveb onyl ayzr"); // Use app password
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


    }
}
