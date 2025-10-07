using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EIIOS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendVerificationEmailAsync(string toEmail, string toName, string verificationLink)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:FromName"],
                    _configuration["EmailSettings:FromEmail"]
                ));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = "Verify Your Email - EIIOS";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #333;'>Welcome to EIIOS!</h2>
                                <p>Hi {toName},</p>
                                <p>Thank you for registering. Please verify your email address by clicking the button below:</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{verificationLink}' 
                                       style='background-color: #007bff; color: white; padding: 12px 30px; 
                                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        Verify Email
                                    </a>
                                </div>
                                <p>Or copy and paste this link into your browser:</p>
                                <p style='word-break: break-all; color: #666;'>{verificationLink}</p>
                                <p>This link will expire in 24 hours.</p>
                                <p>If you didn't create an account, please ignore this email.</p>
                                <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                                <p style='color: #999; font-size: 12px;'>EIIOS - Ēdināšanas iestāžu iekšējās organizācijas sistēma</p>
                            </div>
                        </body>
                        </html>
                    "
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:Port"]),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Verification email sent to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to {Email}", toEmail);
                return false;
            }
        }
    }
}