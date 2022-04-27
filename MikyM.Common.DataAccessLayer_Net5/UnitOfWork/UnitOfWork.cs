﻿using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using MikyM.Common.DataAccessLayer_Net5.Helpers;
using MikyM.Common.DataAccessLayer_Net5.Repositories;
using MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators;

namespace MikyM.Common.DataAccessLayer_Net5.UnitOfWork
{
    /// <summary>
    /// Unit of work implementation
    /// </summary>
    /// <inheritdoc cref="IUnitOfWork{TContext}"/>
    public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : AuditableDbContext
    {
        /// <summary>
        /// Inner <see cref="ISpecificationEvaluator"/>
        /// </summary>
        private readonly ISpecificationEvaluator _specificationEvaluator;
        /// <summary>
        /// Configuration
        /// </summary>
        private readonly IOptions<DataAccessConfiguration> _options;

        // To detect redundant calls
        private bool _disposed;
        // ReSharper disable once InconsistentNaming

        /// <summary>
        /// Repository cache
        /// </summary>
        private ConcurrentDictionary<string, Lazy<IBaseRepository>>? _repositories;
        /// <summary>
        /// Repository entity type cache
        /// </summary>
        private ConcurrentDictionary<string, string>? _entityTypesOfRepositories;

        /// <summary>
        /// Inner <see cref="IDbContextTransaction"/>
        /// </summary>
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>
        /// </summary>
        /// <param name="context"><see cref="DbContext"/> to be used</param>
        /// <param name="specificationEvaluator">Specification evaluator to be used</param>
        public UnitOfWork(TContext context, ISpecificationEvaluator specificationEvaluator, IOptions<DataAccessConfiguration> options)
        {
            Context = context;
            _specificationEvaluator = specificationEvaluator;
            _options = options;
        }

        /// <inheritdoc />
        public TContext Context { get; }

        /// <inheritdoc />
        public async Task UseTransactionAsync()
            => _transaction ??= await Context.Database.BeginTransactionAsync();

        /// <inheritdoc />
        public TRepository GetRepository<TRepository>() where TRepository : class, IBaseRepository
        {
            _repositories ??= new ConcurrentDictionary<string, Lazy<IBaseRepository>>();
            _entityTypesOfRepositories ??= new ConcurrentDictionary<string, string>();

            var type = typeof(TRepository);
            string name = type.FullName ?? type.Name;
            var entityType = type.GetGenericArguments().FirstOrDefault();
            if (entityType is null)
                throw new ArgumentException("Couldn't retrieve entity type from generic arguments on repository type");
            var entityTypeName = entityType.FullName ?? entityType.Name;
        
            if (type.IsInterface)
            {
                if (!UoFCache.CachedRepositoryInterfaceImplTypes.TryGetValue(type, out var implType))
                    throw new InvalidOperationException($"Couldn't find a non-abstract implementation of {name}");

                type = implType;
                name = implType.FullName ?? throw new InvalidOperationException();
            }

            // lazy to avoid creating whole repository and then discarding it due to the fact that GetOrAdd isn't atomic, creation of Lazy<T> is very cheap
            var lazyRepository = _repositories.GetOrAdd(name, _ =>
            {
                if (!_entityTypesOfRepositories.TryAdd(entityTypeName, entityTypeName))
                    throw new InvalidOperationException(
                        "Seems like you tried to create a different type of repository (ie. both read-only and crud) for same entity type within same unit of work instance - it is not supported as it may lead to unexpected results");

                return new Lazy<IBaseRepository>(() =>
                {
                    var instance = Activator.CreateInstance(type,
                        BindingFlags.NonPublic | BindingFlags.Instance, null, new object[]
                        {
                            Context, _specificationEvaluator
                        }, CultureInfo.InvariantCulture);

                    if (instance is null) throw new InvalidOperationException($"Couldn't create an instance of {name}");

                    return (TRepository)instance;
                });
            });

            return (TRepository)lazyRepository.Value;
        }

        /// <inheritdoc />
        public async Task RollbackAsync()
        {
            if (_transaction is not null) await _transaction.RollbackAsync();
        }

        /// <inheritdoc />
        public async Task<int> CommitAsync()
        {
            if (_options.Value.OnBeforeSaveChangesActions is not null &&
                _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
                await action.Invoke(this);

            int result = await Context.SaveChangesAsync();
            if (_transaction is not null) await _transaction.CommitAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<int> CommitAsync(string? userId)
        {
            if (_options.Value.OnBeforeSaveChangesActions is not null &&
                _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
                await action.Invoke(this);
        
            int result = await Context.SaveChangesAsync(userId);
            if (_transaction is not null) await _transaction.CommitAsync();
            return result;
        }

        // Public implementation of Dispose pattern callable by consumers.
        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Context.Dispose();
                _transaction?.Dispose();
            }

            _repositories = null;
            _entityTypesOfRepositories = null;

            _disposed = true;
        }
    }
}