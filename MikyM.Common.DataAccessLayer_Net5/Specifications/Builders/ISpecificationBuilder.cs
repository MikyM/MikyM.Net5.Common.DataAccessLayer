﻿namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Builders
{
    public interface ISpecificationBuilder<T, TResult> : ISpecificationBuilder<T> where T : class where TResult : class
    {
        new Specification<T, TResult> Specification { get; }
    }

    public interface ISpecificationBuilder<T> where T : class
    {
        Specification<T> Specification { get; }
    }
}