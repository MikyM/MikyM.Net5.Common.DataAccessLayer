using System.Linq;
using MikyM.Common.DataAccessLayer_Net5.Specifications.Extensions;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Validators
{
    public class SearchValidator : IValidator
    {
        private SearchValidator() { }
        public static SearchValidator Instance { get; } = new();

        public bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
        {
            if (specification.SearchCriterias is null) return true;

            foreach (var searchGroup in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
            {
                if (searchGroup.Any(c => c.SelectorFunc(entity).Like(c.SearchTerm)) == false) return false;
            }

            return true;
        }
    }
}