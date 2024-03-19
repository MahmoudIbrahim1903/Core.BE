using Autofac;
using Emeint.Core.BE.Infrastructure.Idempotency;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Infrastructure;
using Emeint.Core.BE.Media.Infrastructure.Repositories;

namespace Emeint.Core.BE.Media.API.Infrastructure
{
    public class ApplicationModule
        : Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ImageRepository>()
                .As<IImageRepository>()
                .InstancePerLifetimeScope();

            //builder.RegisterType<VehicleMakeRepository>()
            //    .As<IVehicleMakeRepository>()
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<VehicleFeatureRepository>()
            //    .As<IVehicleFeatureRepository>()
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<VehicleAccessoryRepository>()
            //    .As<IVehicleAccessoryRepository>()
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<VehicleOdometerRangeRepository>()
            //    .As<IVehicleOdometerRangeRepository>()
            //    .InstancePerLifetimeScope();

            builder.RegisterType<CommandManager<MediaContext>>()
                .As<ICommandManager>()
                .InstancePerLifetimeScope();
        }
    }
}
