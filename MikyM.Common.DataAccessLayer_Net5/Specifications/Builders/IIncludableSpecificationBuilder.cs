namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Builders
{
    public interface IIncludableSpecificationBuilder<T, out TProperty> : ISpecificationBuilder<T> where T : class
    {
        bool IsChainDiscarded { get; set; }
    }
}