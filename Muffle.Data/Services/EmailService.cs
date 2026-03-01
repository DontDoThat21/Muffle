using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Muffle.Data.Services
{
    public static class EmailService
    {
        /// <summary>
        /// Sends a password reset email containing a 6-digit verification code.
        /// </summary>
        /// <returns>True if the email was sent successfully.</returns>
        public static async Task<bool> SendPasswordResetEmailAsync(string toEmail, string code)
        {
            try
            {
                var settings = ConfigurationLoader.GetSmtpSettings();

                if (string.IsNullOrWhiteSpace(settings.SenderEmail) ||
                    string.IsNullOrWhiteSpace(settings.AppPassword))
                {
                    Console.WriteLine("SMTP credentials not configured in appsettings.json.");
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "Muffle - Password Reset Code";

                message.Body = new TextPart("html")
                {
                    Text = $@"
<div style=""font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; padding: 20px;"">
    <h2 style=""color: #5865f2;"">Muffle Password Reset</h2>
    <p>You requested a password reset for your Muffle account.</p>
    <p>Your verification code is:</p>
    <div style=""background-color: #2f3136; color: #43B581; font-size: 32px; font-weight: bold;
                letter-spacing: 6px; text-align: center; padding: 20px; border-radius: 8px; margin: 20px 0;"">
        {code}
    </div>
    <p style=""color: #888; font-size: 13px;"">This code expires in 15 minutes.</p>
    <p style=""color: #888; font-size: 13px;"">If you did not request this reset, you can safely ignore this email.</p>
</div>"
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(settings.SenderEmail, settings.AppPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return false;
            }
        }
    }
}
