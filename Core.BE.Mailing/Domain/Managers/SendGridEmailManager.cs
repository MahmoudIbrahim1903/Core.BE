using Emeint.Core.BE.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.IO;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Mailing.Utilities;
using Microsoft.AspNetCore.Hosting;
using Emeint.Core.BE.Mailing.Domain.Enums;
using Emeint.Core.BE.Utilities;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using System.Net;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Emeint.Core.BE.Mailing.Infrastructure.Repositories;
using Emeint.Core.BE.InterCommunication.ViewModels;
using System.Linq;

namespace Emeint.Core.BE.Mailing.Domain.Managers
{
    public class SendGridEmailManager : IEmailManager
    {
        private EmailBuilderUtility _emailBuilder;
        private IConfigurationManager _configurationManager;
        private IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<SendGridEmailManager> _logger;
        private IMailTemplateRepository _mailTemplateRepository;
        private readonly IMailRepository _mailRepository;

        public SendGridEmailManager(IConfigurationManager configurationManager, IHostingEnvironment hostingEnvironment, ILogger<SendGridEmailManager> logger,
                                    IMailTemplateRepository mailTemplateRepository, IMailRepository mailRepository)
        {
            _configurationManager = configurationManager;
            _hostingEnvironment = hostingEnvironment;
            _emailBuilder = new EmailBuilderUtility(_hostingEnvironment, configurationManager, mailTemplateRepository);
            _logger = logger;
            _mailRepository = mailRepository;
        }

        public async Task SendEmail(string from, string fromDisplayName, string[] to, string[] cc, string subject, string body, List<AttachmentVM> attachments,
            bool? isImportant, string type, Dictionary<string, string> extraParams)
        {

            #region SendGrid
            var apikey = _configurationManager.GetSendGridapiKey();
            var client = new SendGridClient(apikey);
            var mailsubject = subject;
            List<EmailAddress> tos = new List<EmailAddress>();
            List<EmailAddress> ccs = new List<EmailAddress>();
            var htmlContent = "";
            var plainTextContent = "";

            if (StringUtility.IsHTML(body) && !string.IsNullOrEmpty(body))
            {
                htmlContent = body;
            }
            else
            {
                if (!string.IsNullOrEmpty(type) && extraParams != null && extraParams.Count > 0)
                {
                    htmlContent = _emailBuilder.GenerateTemplate(extraParams, type);
                    if (string.IsNullOrEmpty(htmlContent))
                    {
                        plainTextContent = body;
                    }
                }
                else
                {
                    plainTextContent = body;
                }
            }
            //check if no body sent and no template found then use a Dummy Text
            if (string.IsNullOrEmpty(plainTextContent))
                plainTextContent = ".";

            // Receivers
            if (to != null && to.Length > 0)
            {
                foreach (var toEmail in to)
                {
                    if (!string.IsNullOrEmpty(toEmail))
                    {
                        tos.Add(new EmailAddress(toEmail, toEmail));
                    }
                }
            }
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(from, fromDisplayName), tos, subject, plainTextContent, htmlContent);
            //add attachment
            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    var file = attachment;
                    msg.AddAttachment(Path.GetFileName(attachment.FileName), attachment.FileBase64);
                }
            }

            // is important 
            if (isImportant != null && isImportant == true)
            {
                msg.Headers = new Dictionary<string, string>();
                msg.Headers.Add("Priority", "Urgent");
            }
            //ccs
            if (cc != null && cc.Length > 0)
            {
                foreach (var ccEmail in cc)
                {
                    if (!string.IsNullOrEmpty(ccEmail))
                    {
                        var email = (new EmailAddress(ccEmail, ccEmail));
                        ccs.Add(email);
                    }
                }
                if (ccs.Count > 0)
                {
                    msg.AddCcs(ccs);
                }
            }

            // create the entity that will be stored in db
            Mail mailData = new Mail(from, string.Join(',', tos.Select(t => t.Email).ToArray()), string.Join(',', ccs.Select(c => c.Email).ToArray()), subject, htmlContent, attachments?.Count > 0, isImportant,
                                     type, null, null, null, Status.Pending);
            _mailRepository.Add(mailData);
            _mailRepository.SaveEntities();

            var result = await client.SendEmailAsync(msg);
            if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
            {
                mailData.ErrorCode = result.StatusCode.ToString();
                mailData.Status = Status.Failed;
                _mailRepository.Update(mailData);
                _mailRepository.SaveEntities();
            }
            else
            {
                mailData.Status = Status.Sent;
                _mailRepository.Update(mailData);
                _mailRepository.SaveEntities();
            }
            #endregion
        }
    }
}
