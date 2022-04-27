﻿using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators
{
    public class AsNoTrackingEvaluator : IEvaluator, IEvaluatorBase
    {
        private AsNoTrackingEvaluator()
        {
        }

        public static AsNoTrackingEvaluator Instance { get; } = new();

        public bool IsCriteriaEvaluator { get; } = true;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.IsAsNoTracking) query = query.AsNoTracking();

            return query;
        }
    }
}
