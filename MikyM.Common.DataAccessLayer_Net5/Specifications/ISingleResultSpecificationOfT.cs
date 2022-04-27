namespace MikyM.Common.DataAccessLayer_Net5.Specifications
{
    public interface ISingleResultSpecification<T> : ISpecification<T>, ISingleResultSpecification where T : class
    {
    }
}