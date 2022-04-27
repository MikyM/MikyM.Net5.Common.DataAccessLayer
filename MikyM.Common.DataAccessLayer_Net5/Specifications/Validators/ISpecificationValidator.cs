namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Validators
{
    public interface ISpecificationValidator
    {
        bool IsValid<T>(T entity, ISpecification<T> specification) where T : class;
    }
}