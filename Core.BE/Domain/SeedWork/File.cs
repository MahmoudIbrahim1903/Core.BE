using System;
using System.Net.Http;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public class File
    {
        public File(string key, string nameAndExtension, StreamContent content)
        {
            Key = key;
            NameAndExtension = nameAndExtension;
            Content = content;
        }
        public string Key { get; set; }
        public string NameAndExtension { get; set; }
        public StreamContent Content { get; set; }
    }
}
