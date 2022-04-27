namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Builders
{
    public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
    {
        bool IsChainDiscarded { get; set; }
    }
}