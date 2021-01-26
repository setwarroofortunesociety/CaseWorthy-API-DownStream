using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CW.ClientAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CW.ClientAPI.Services.General
{
    public partial class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettingsModel> _emailSettings;
        private readonly ILogger<EmailService> _logger ;

        public EmailService(IOptions<EmailSettingsModel> emailSettings, 
                            ILogger<EmailService> logger)

        {
            _emailSettings = emailSettings;
            _logger = logger;
        }
        public void Send(string msgSubject, string message)
        {
            try {

                   SmtpClient client = new SmtpClient
                   {
                        Host =_emailSettings.Value.SmtpHost,
                        Port = _emailSettings.Value.Port,
                        Credentials = new NetworkCredential(_emailSettings.Value.UserName, _emailSettings.Value.Password),
                        EnableSsl = true
                   };
                
                   MailMessage mailMessage = new MailMessage();

                   mailMessage.From = new MailAddress(string.Format("Web App <{0}>",_emailSettings.Value.MailAddress));

             

                    string[] emailRecipientTo = _emailSettings.Value.EmailRecipients.Split(',').Select(s => s.Trim()).ToArray();
                    foreach (var address in emailRecipientTo)
                    {
                        mailMessage.To.Add(new MailAddress(address, ""));
                    }

                    string[] emailRecipientCC = _emailSettings.Value.EmailRecipients.Split(',').Select(s => s.Trim()).ToArray();
                    foreach (var address in emailRecipientCC)
                    {
                         mailMessage.CC.Add(new MailAddress(address, ""));
                    }

                    mailMessage.Subject = msgSubject;
                    mailMessage.Body = message;

                    client.Send(mailMessage);
                    mailMessage.Dispose();
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(String.Format(CultureInfo.CurrentCulture,"Email service not working.  Error Message {0}.", ex));
            }

        }
    }

}
