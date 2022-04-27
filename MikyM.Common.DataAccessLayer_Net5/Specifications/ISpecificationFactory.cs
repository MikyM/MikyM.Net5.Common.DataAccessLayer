namespace MikyM.Common.DataAccessLayer_Net5.Specifications
{
    public interface ISpecificationFactory
    {
        TSpecification GetSpecification<TSpecification>() where TSpecification : ISpecification;
    }
}