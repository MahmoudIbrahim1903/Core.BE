using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
namespace Emeint.Core.BE.UnitTest
{
    public abstract class UnitTestBase
    {
        protected ServiceCollection Services { get; private set; }
        protected ServiceProvider ServiceProvider { get; private set; }
        protected IConfiguration Configuration { private set; get; }

        public UnitTestBase()
        {
            Services = new ServiceCollection();
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            RegisterServices();
            BuildServiceProvider();
            ConfigureServices();
        }

        public void BuildServiceProvider()
        {
            ServiceProvider = Services.BuildServiceProvider();
        }
        public virtual void ConfigureServices()
        {

        }
        public abstract void RegisterServices();
    }
}