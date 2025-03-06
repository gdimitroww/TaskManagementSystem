using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TaskManagementSystem.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _configuration;
        private readonly bool _useSmtp;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly bool _enableSsl;
        
        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            // Get all email settings
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            _senderEmail = _configuration["EmailSettings:SenderEmail"];
            _senderName = _configuration["EmailSettings:SenderName"] ?? "Task Management System";
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
            
            // Log the email configuration (but mask the password)
            _logger.LogInformation("Email settings configured: Server={Server}, Port={Port}, Username={Username}, Sender={Sender}",
                _smtpServer, _smtpPort, _smtpUsername, _senderEmail);
            
            // Check if SMTP settings are available, otherwise use logging fallback
            _useSmtp = !string.IsNullOrEmpty(_smtpServer) && 
                       !string.IsNullOrEmpty(_smtpUsername) && 
                       !string.IsNullOrEmpty(_smtpPassword);
            
            if (!_useSmtp)
            {
                _logger.LogWarning("SMTP settings are incomplete. Using logging fallback for emails.");
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Attempting to send email to {Email} with subject '{Subject}'", email, subject);
            Console.WriteLine($"[EMAIL DEBUG] Attempting to send email to {email} with subject '{subject}'");
            
            try
            {
                if (_useSmtp)
                {
                    Console.WriteLine($"[EMAIL DEBUG] Using SMTP with server: {_smtpServer}, port: {_smtpPort}, username: {_smtpUsername}");
                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        Console.WriteLine($"[EMAIL DEBUG] Creating MailMessage with sender: {_senderEmail}");
                        
                        client.EnableSsl = _enableSsl;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        
                        var message = new MailMessage
                        {
                            From = new MailAddress(_senderEmail, _senderName),
                            Subject = subject,
                            Body = htmlMessage,
                            IsBodyHtml = true
                        };
                        message.To.Add(email);
                        
                        Console.WriteLine($"[EMAIL DEBUG] About to send email, SSL: {_enableSsl}");
                        await client.SendMailAsync(message);
                        Console.WriteLine($"[EMAIL DEBUG] Email sent successfully to {email}");
                        _logger.LogInformation("Email sent successfully to {Email}", email);
                    }
                }
                else
                {
                    Console.WriteLine($"[EMAIL DEBUG] SMTP not configured, logging email content instead.");
                    LogEmailContent(email, subject, htmlMessage);
                }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                Console.WriteLine($"[EMAIL ERROR] Failed to send email: {ex.Message}");
                Console.WriteLine($"[EMAIL ERROR] Inner exception: {innerMessage}");
                Console.WriteLine($"[EMAIL CONFIG] SMTP Server: {_smtpServer}, Port: {_smtpPort}, Username: {_smtpUsername}, Sender: {_senderEmail}, SSL: {_enableSsl}");
                
                _logger.LogError(ex, "Failed to send email to {Email}. Error: {ErrorMessage}", email, ex.Message);
                
                // Log config values to help diagnose issues
                _logger.LogError("SMTP Configuration: Server={Server}, Port={Port}, Username={Username}, SenderEmail={SenderEmail}, SSL={EnableSsl}",
                    _smtpServer, _smtpPort, _smtpUsername, _senderEmail, _enableSsl);
                
                // Still log the content for debugging
                LogEmailContent(email, subject, htmlMessage);
            }
        }
        
        private void LogEmailContent(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation(
                "Email would be sent to: {Email}, Subject: {Subject}, Message (first 100 chars): {MessagePreview}...",
                email, subject, htmlMessage.Length > 100 ? htmlMessage.Substring(0, 100) : htmlMessage);
            
            // Also log to console for Docker environments
            Console.WriteLine($"EMAIL WOULD BE SENT TO: {email}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine($"MESSAGE PREVIEW: {(htmlMessage.Length > 100 ? htmlMessage.Substring(0, 100) + "..." : htmlMessage)}");
        }
    }
} 