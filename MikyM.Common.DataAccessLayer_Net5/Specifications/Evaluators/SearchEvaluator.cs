﻿using System.Collections.Generic;
using System.Linq;
using MikyM.Common.DataAccessLayer_Net5.Specifications.Extensions;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators
{
    public class SearchEvaluator : IEvaluator, IInMemoryEvaluator, IEvaluatorBase
    {
        private SearchEvaluator() { }
        public static SearchEvaluator Instance { get; } = new SearchEvaluator();

        public bool IsCriteriaEvaluator { get; } = true;


        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.SearchCriterias is null) return query;

            foreach (var searchCriteria in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
            {
                query = query.Search(searchCriteria);
            }

            return query;
        }

        public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.SearchCriterias is null) return query;

            foreach (var searchGroup in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
            {
                query = query.Where(x => searchGroup.Any(c => c.SelectorFunc(x).Like(c.SearchTerm)));
            }

            return query;
        }
    }
}