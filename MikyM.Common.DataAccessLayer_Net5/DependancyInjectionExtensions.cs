using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Options;
using MikyM.Autofac.Extensions_Net5;
using MikyM.Common.DataAccessLayer_Net5.Repositories;
using MikyM.Common.DataAccessLayer_Net5.Specifications;
using MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators;
using MikyM.Common.DataAccessLayer_Net5.Specifications.Validators;
using MikyM.Common.DataAccessLayer_Net5.UnitOfWork;

namespace MikyM.Common.DataAccessLayer_Net5
{
    /// <summary>
    /// DI extensions for <see cref="ContainerBuilder"/>
    /// </summary>
    public static class DependancyInjectionExtensions
    {
        /// <summary>
        /// Adds Data Access Layer to the application.
        /// </summary>
        /// <remarks>
        /// Automatically registers <see cref="IRepository{TEntity}"/>, <see cref="IReadOnlyRepository{TEntity}"/>, all base <see cref="IEvaluator"/> types, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork"/> with <see cref="ContainerBuilder"/>.
        /// </remarks>
        /// <param name="builder">Current instance of <see cref="ContainerBuilder"/></param>
        /// <param name="options"><see cref="Action"/> that configures DAL.</param>
        public static void AddDataAccessLayer(this ContainerBuilder builder, Action<DataAccessConfiguration>? options = null)
        {
            var config = new DataAccessConfiguration(builder);
            options?.Invoke(config);

            var ctorFinder = new AllConstructorsFinder();

            builder.Register(x => config).As<IOptions<DataAccessConfiguration>>().SingleInstance();

            builder.RegisterGeneric(typeof(ReadOnlyRepository<>))
                .As(typeof(IReadOnlyRepository<>))
                .FindConstructorsWith(ctorFinder)
                .InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .FindConstructorsWith(ctorFinder)
                .InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator))
                    .As<IEvaluator>()
                    .FindConstructorsWith(ctorFinder)
                    .SingleInstance();
            }

            builder.RegisterType<IncludeEvaluator>()
                .As<IEvaluator>()
                .UsingConstructor(typeof(bool))
                .FindConstructorsWith(ctorFinder)
                .WithParameter(new TypedParameter(typeof(bool), config.EnableIncludeCache))
                .SingleInstance();

            builder.RegisterType<ProjectionEvaluator>()
                .As<IProjectionEvaluator>()
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();

            builder.RegisterType<SpecificationEvaluator>()
                .As<ISpecificationEvaluator>()
                .UsingConstructor(typeof(IEnumerable<IEvaluator>), typeof(IProjectionEvaluator))
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();

            builder.RegisterType<SpecificationValidator>()
                .As<ISpecificationValidator>()
                .UsingConstructor(typeof(IEnumerable<IValidator>))
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();

            builder.RegisterType<InMemorySpecificationEvaluator>()
                .As<IInMemorySpecificationEvaluator>()
                .UsingConstructor(typeof(IEnumerable<IInMemoryEvaluator>))
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();
        }
    }
}