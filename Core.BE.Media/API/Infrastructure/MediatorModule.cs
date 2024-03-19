using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Emeint.Core.BE.API.Application.Behaviors;
using MediatR;


namespace Emeint.Core.BE.Media.API.Infrastructure
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
            // Register all the Command classes (they implement IAsyncRequestHandler) in assembly holding the Commands
            //builder.RegisterAssemblyTypes(typeof(AddVehicleCommand).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IAsyncRequestHandler<,>));
           



            // Register the Command's Validators (Validators based on FluentValidation library)
            //builder
            //    .RegisterAssemblyTypes(typeof(AddVehicleCommandValidator).GetTypeInfo().Assembly)
            //    .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
            //    .AsImplementedInterfaces();


            //builder.Register<SingleInstanceFactory>(context =>
            //{
            //    var componentContext = context.Resolve<IComponentContext>();
            //    return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            //});


            //builder.Register<MultiInstanceFactory>(context =>
            //{
            //    var componentContext = context.Resolve<IComponentContext>();

            //    return t =>
            //    {
            //        var resolved = (IEnumerable<object>)componentContext.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            //        return resolved;
            //    };
            //});

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}
