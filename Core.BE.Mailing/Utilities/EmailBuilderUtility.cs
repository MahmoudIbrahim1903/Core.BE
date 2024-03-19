using System;
using System.Collections.Generic;
using System.IO;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Emeint.Core.BE.Mailing.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Mailing.Utilities
{
    public class EmailBuilderUtility
    {
        private IConfigurationManager _configurationManager;
        private IHostingEnvironment _hostingEnvironment;
        private IMailTemplateRepository _mailTemplateRepository;

        public EmailBuilderUtility(IHostingEnvironment hostingEnvironment, IConfigurationManager configurationManager, IMailTemplateRepository mailTemplateRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _configurationManager = configurationManager;
            _mailTemplateRepository = mailTemplateRepository;
        }
        public string GenerateTemplate(Dictionary<string, string> extraParams, string type)
        {
            string htmlPath = _hostingEnvironment.WebRootPath + _configurationManager.GetHtmlPath();
            string assetsURL = _configurationManager.GetAssets() + "EmailTemplates/Assets/";
            string htmlContent;
            var language = Language.en;
            if (extraParams["Language"].IndexOf("ar", StringComparison.OrdinalIgnoreCase) >= 0)
                language = Language.ar;
            var template = _mailTemplateRepository.GetMailTemplateByMailType(type);
            if (template != null)
            {
                if (language == Language.en)
                {
                    htmlContent = File.ReadAllText(htmlPath + template.TemplateNameEn);
                }
                else
                {
                    htmlContent = File.ReadAllText(htmlPath + template.TemplateNameAr);
                }

                foreach (var item in extraParams)
                {
                    htmlContent = htmlContent.Replace($"{item.Key}", item.Value);
                }

                htmlContent = htmlContent.Replace("src=\"../assets/", assetsURL);
            }
            else htmlContent = "";
            return htmlContent;
        }

    }
}
