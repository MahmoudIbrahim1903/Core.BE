using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Mailing.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Emeint.Core.BE.Mailing.Domain.Enums;
using Emeint.Core.BE.Mailing.Domain.Exceptions;
using Emeint.Core.BE.Mailing.Domain.Managers;
using Emeint.Core.BE.Mailing.Infrastructure.Repositories;
using Emeint.Core.BE.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.API.Controllers.Server
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/server/mailing/v{version:apiVersion}")]
    public class MailingController : Controller
    {
        private IConfigurationManager _configurationManager;
        private readonly ILogger<SendGridEmailManager> _logger;
        private readonly IEmailManager _emailManager;
        private readonly string _typeName;
        private readonly IMailRepository _mailRepository;
        private readonly IHashingManager _hashingManager;

        public MailingController(IConfigurationManager configurationManager, ILogger<SendGridEmailManager> logger, IEmailManager emailManager,
            IHashingManager hashingManager
            , IMailRepository mailRepository)
        {
            _configurationManager = configurationManager;
            _logger = logger;
            _emailManager = emailManager;
            _typeName = GetType().Name;
            _mailRepository = mailRepository;
            _hashingManager = hashingManager;
        }

        /// <summary>
        /// Used to send text email (Deprecated)
        /// </summary>
        /// <param name="mail"></param>
        /// <response code="200 (error_code=800)">SendEmailFailed</response>

        [Obsolete]
        [Route("send_email")]
        [HttpPost]
        public async Task<BaseResponse> SendEmail([FromBody] SendMailViewModel mail)
        {
            if (mail == null)
            {
                throw new MissingParameterException("mail");
            }

            if (string.IsNullOrEmpty(mail.From))
            {
                throw new MissingParameterException("from_email");
            }

            if (mail.To.Length < 1)
            {
                throw new MissingParameterException("to_email");
            }

            if (string.IsNullOrEmpty(mail.Subject))
            {
                throw new MissingParameterException("email_subject");
            }

            if (string.IsNullOrEmpty(mail.Body) && (mail.ExtraParams == null || mail.ExtraParams.Count == 0))
            {
                throw new MissingParameterException("email_body");
            }

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            #region Validate Hashed request
            string expectedSign = Request.Headers["Signature"];
            var sign = _hashingManager.HashSendEmailRequest(mail);
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(expectedSign) || sign != expectedSign)
            {
                throw new HashedRequestMismatchSignatureException();
            }
            #endregion

            //Validate From mail:
            EmailUtility.ValidateEmail(mail.From);

            string receivers = "";
            if (mail.To != null && mail.To.Length > 0)
            {
                foreach (var receiver in mail.To)
                {
                    receivers += receiver + ",";
                    EmailUtility.ValidateEmail(receiver);
                }
            }
            string ccEmails = "";
            if (mail.Cc != null && mail.Cc.Length > 0)
            {
                foreach (var cc in mail.Cc)
                {
                    ccEmails += cc + ",";
                    EmailUtility.ValidateEmail(cc);
                }
            }
            //to ensure null Execption will not be thrown
            else
            {
                mail.Cc = new string[0];
            }

            bool? hasAttachments = mail.Attachments?.Count > 0 ? true : false;

            // create the entity that will be stored in db
            Mail mailData = new Mail(mail.From, receivers, ccEmails, mail.Subject, mail.Body, hasAttachments, mail.IsImportant
                , mail.Type, mail.FromUserId, null, null, Status.Pending);
            _mailRepository.Add(mailData);
            _mailRepository.SaveEntities();

            //send the email
            _logger.LogInformation($"{_typeName}.{methodName}: send Email");
             await _emailManager.SendEmail(mail.From, null, mail.To, mail.Cc, mail.Subject, mail.Body,
                  mail.Attachments, mail.IsImportant, mail.Type, mail.ExtraParams);

            return new BaseResponse();

            //if (result?.ErrorCode == 0)
            //{
            //    mailData.ErrorCode = "0";
            //    mailData.Status = Status.Sent;
            //    _mailRepository.Update(mailData);
            //    _mailRepository.SaveEntities();
            //    return new BaseResponse();
            //}
            //else
            //{
            //    mailData.ErrorCode = result.ErrorCode.ToString();
            //    mailData.ErrorMessage = result.ErrorDetails;
            //    mailData.Status = Status.Failed;
            //    _mailRepository.Update(mailData);
            //    _mailRepository.SaveEntities();

            //    _logger.LogInformation($"From: {mail.From}. To:{ mail.To[0]}. Subject:{ mail.Subject}. :Error:{result.ErrorDetails}");
            //    throw new SendEmailFailedException(result.ErrorDetails);
            //}
        }
    }
}
