using System.Collections.Generic;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Evaluators
{
    public interface IInMemoryEvaluator
    {
        IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class;
    }
}