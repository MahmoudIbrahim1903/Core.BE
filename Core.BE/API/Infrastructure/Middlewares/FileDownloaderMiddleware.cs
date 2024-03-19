using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.API.Infrastructure.Middlewares
{
    public class FileDownloaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileDownloaderMiddleware(RequestDelegate next,IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;
            if (path.Contains("Files"))
            {
                int fileNameIndex = path.Split('/').Length - 1;
                string fileName = path.Split('/')[fileNameIndex];
                string nameWithoutExtension = fileName.Split('.')[0];
                int dateStringIndex = nameWithoutExtension.Split('_').Length - 1;
                var dateString = nameWithoutExtension.Split('_')[dateStringIndex];
                DateTime date;
                if (DateTime.TryParseExact(dateString, "yyyyMMddHHmmssffff", null, DateTimeStyles.None, out date))
                {
                    string dailyRootFolder = _hostingEnvironment.WebRootPath + "\\Files\\Export\\" + date.Year + "\\"
                + date.Month + "\\" + date.Day;
                    FileInfo file = new FileInfo(Path.Combine(dailyRootFolder, fileName));
                    if (!file.Exists)
                        throw new FileNotFoundException();
                    await httpContext.Response.SendFileAsync(file.FullName, new System.Threading.CancellationToken());
                }
                else
                {
                    await _next.Invoke(httpContext);
                    return;
                }
            }
            else
            {
                await _next.Invoke(httpContext);
                return;
            }
        }
    }
}
