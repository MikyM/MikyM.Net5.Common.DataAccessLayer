﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MikyM.Common.DataAccessLayer_Net5.Specifications;
using MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators;
using MikyM.Common.Domain_Net5.Entities;

namespace MikyM.Common.DataAccessLayer_Net5.Repositories
{
    /// <summary>
    /// Read-only repository
    /// </summary>
    /// <inheritdoc cref="IReadOnlyRepository{TEntity}"/>
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : AggregateRootEntity
    {
        /// <summary>
        /// Entity type that this repository was created for
        /// </summary>
        public Type EntityType => typeof(TEntity);
    
        /// <summary>
        /// Current <see cref="DbContext"/>
        /// </summary>
        protected readonly DbContext Context;

        /// <summary>
        /// Inner evaluator
        /// </summary>
        private readonly ISpecificationEvaluator _specificationEvaluator;

        private Type _entityType;

        internal ReadOnlyRepository(DbContext context, ISpecificationEvaluator specificationEvaluator)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this._specificationEvaluator = specificationEvaluator;
        }

        /// <inheritdoc />
        public virtual async ValueTask<TEntity?> GetAsync(params object[] keyValues)
        {
            return await this.Context.Set<TEntity>().FindAsync(keyValues);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification)
        {
            return await this.ApplySpecification(specification)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public virtual async Task<TProjectTo?> GetSingleBySpecAsync<TProjectTo>(
            ISpecification<TEntity, TProjectTo> specification) where TProjectTo : class
        {
            return await this.ApplySpecification(specification)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TProjectTo>> GetBySpecAsync<TProjectTo>(
            ISpecification<TEntity, TProjectTo> specification) where TProjectTo : class
        {
            var result = await this.ApplySpecification(specification).ToListAsync();
            return specification.PostProcessingAction is null
                ? result
                : specification.PostProcessingAction(result).ToList();
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification)
        {
            var result = await this.ApplySpecification(specification)
                .ToListAsync();
            return specification.PostProcessingAction is null
                ? result
                : specification.PostProcessingAction(result).ToList();
        }

        /// <inheritdoc />
        public virtual async Task<long> LongCountAsync(ISpecification<TEntity>? specification = null)
        {
            if (specification is null) return await Context.Set<TEntity>().LongCountAsync();

            return await this.ApplySpecification(specification)
                .LongCountAsync();
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>() where TProjectTo : class
        {
            return await this.ApplySpecification(new Specification<TEntity, TProjectTo>())
                .ToListAsync();
        }

        /// <summary>
        ///     Filters the entities  of <typeparamref name="TEntity" />, to those that match the encapsulated query logic of the
        ///     <paramref name="specification" />.
        /// </summary>
        /// <param name="specification">The encapsulated query logic.</param>
        /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
        /// <returns>The filtered entities as an <see cref="IQueryable{T}" />.</returns>
        protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity>? specification,
            bool evaluateCriteriaOnly = false)
        {
            if (specification is null) throw new ArgumentNullException(nameof(specification), "Specification is required");

            return this._specificationEvaluator.GetQuery(Context.Set<TEntity>().AsQueryable(), specification,
                evaluateCriteriaOnly);
        }

        /// <summary>
        ///     Filters all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
        ///     <paramref name="specification" />, from the database.
        ///     <para>
        ///         Projects each entity into a new form, being <typeparamref name="TResult" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
        /// <param name="specification">The encapsulated query logic.</param>
        /// <returns>The filtered projected entities as an <see cref="IQueryable{T}" />.</returns>
        protected virtual IQueryable<TResult> ApplySpecification<TResult>(
            ISpecification<TEntity, TResult>? specification) where TResult : class
        {
            if (specification is null) throw new ArgumentNullException(nameof(specification), "Specification is required");

            return this._specificationEvaluator.GetQuery(Context.Set<TEntity>().AsQueryable(), specification);
        }
    }
}