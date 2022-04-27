﻿using System.Collections.Generic;
using System.Linq;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators
{
    public class WhereEvaluator : IEvaluator, IInMemoryEvaluator, IEvaluatorBase
    {
        private WhereEvaluator() { }
        public static WhereEvaluator Instance { get; } = new WhereEvaluator();

        public bool IsCriteriaEvaluator { get; } = true;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.WhereExpressions is null) return query;

            foreach (var info in specification.WhereExpressions)
            {
                query = query.Where(info.Filter);
            }

            return query;
        }

        public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.WhereExpressions is null) return query;

            foreach (var info in specification.WhereExpressions)
            {
                query = query.Where(info.FilterFunc);
            }

            return query;
        }
    }
}