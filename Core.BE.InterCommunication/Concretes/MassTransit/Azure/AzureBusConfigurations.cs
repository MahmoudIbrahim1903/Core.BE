using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.InterCommunication.Concretes.Azure
{
    public class AzureBusConfigurations : BusConfigurations
    {

        public AzureBusConfigurations(IConfiguration configuration) : base(configuration)
        {

        }
        public string AzureSbNamespace
        {
            get
            {
                return Configuration["AzureSbNamespace"];
            }
        }
        public string AzureSbPath
        {
            get
            {
                return Configuration["AzureSbPath"];
            }
        }
        public string AzureSbKeyName
        {
            get
            {
                return Configuration["AzureSbKeyName"];
            }
        }
        public string AzureSbSharedAccessKey
        {
            get
            {
                return Configuration["AzureSbSharedAccessKey"];
            }
        }

    }
}
