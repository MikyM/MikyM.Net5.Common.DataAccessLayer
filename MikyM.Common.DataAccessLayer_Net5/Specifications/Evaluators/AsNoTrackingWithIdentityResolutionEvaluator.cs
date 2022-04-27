using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators
{
    public class AsNoTrackingWithIdentityResolutionEvaluator : IEvaluator, IEvaluatorBase
    {
        private AsNoTrackingWithIdentityResolutionEvaluator()
        {
        }

        public static AsNoTrackingWithIdentityResolutionEvaluator Instance { get; } = new();

        public bool IsCriteriaEvaluator { get; } = true;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification.IsAsNoTrackingWithIdentityResolution) query = query.AsNoTrackingWithIdentityResolution();

            return query;
        }
    }
}