
namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Validators
{
    public interface IValidator
    {
        bool IsValid<T>(T entity, ISpecification<T> specification) where T : class;
    }
}