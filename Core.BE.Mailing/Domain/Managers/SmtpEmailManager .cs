using Emeint.Core.BE.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.IO;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using System.Net.Mail;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Emeint.Core.BE.InterCommunication.ViewModels;

namespace Emeint.Core.BE.Mailing.Domain.Managers
{
    public class SmtpEmailManager : IEmailManager
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly ILogger _logger;

        public SmtpEmailManager(IConfigurationManager configurationManager, ILogger logger)
        {
            _configurationManager = configurationManager;
            _logger = logger;
        }
        public async Task SendEmail(string fromEmail, string fromDisplayName, string[] toEmails, string[] ccEmails, string subject, string body, List<AttachmentVM> attachments,
            bool? isImportant, string mailType, Dictionary<string, string> extraParams)
        {

            MailMessage message = new MailMessage();

            #region smtp implementation

            //// Receiver
            if (toEmails != null && toEmails.Length > 0)
            {
                foreach (var toEmail in toEmails)
                {
                    message.To.Add(new MailAddress(toEmail));
                }
            }
            // CC
            if (ccEmails != null && ccEmails.Length > 0)
            {
                foreach (var ccEmail in ccEmails)
                {
                    message.CC.Add(new MailAddress(ccEmail));
                }
            }

            // Sender
            message.From = new MailAddress(fromEmail);

            // Subject
            message.Subject = subject;

            // Body
            message.Body = body; // set body as a html template

            // Important
            if (isImportant != null && isImportant == true)
                message.Priority = MailPriority.High;

            message.IsBodyHtml = true;

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(new System.Net.Mail.Attachment(attachment.FileBase64));
                }
            }

            SmtpClient SmtpServer = new SmtpClient(_configurationManager.GetSMTPSmtpIP());

            SmtpServer.Port = _configurationManager.GetSMTPSmtpPort();
            SmtpServer.UseDefaultCredentials = _configurationManager.SMTPUseDefaultCredentials();
            if (!SmtpServer.UseDefaultCredentials)
            {
                string userName = _configurationManager.GetSMTPUserName();
                string password = _configurationManager.GetSMTPPassword();
                var myCredential = new NetworkCredential(_configurationManager.GetSMTPUserName(), _configurationManager.GetSMTPPassword());
                SmtpServer.Credentials = myCredential;
                message.From = new MailAddress(_configurationManager.GetSMTPFrom());
            }
            SmtpServer.EnableSsl = _configurationManager.SMTPEnableSSL();
            SmtpServer.Timeout = _configurationManager.GetSMTPSmtpTimeout();

            SmtpServer.Send(message);
            #endregion
        }

    }
}
