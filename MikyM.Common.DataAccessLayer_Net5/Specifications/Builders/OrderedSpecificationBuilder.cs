﻿namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Builders
{
    public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T> where T : class
    {
        public Specification<T> Specification { get; }
        public bool IsChainDiscarded { get; set; }

        public OrderedSpecificationBuilder(Specification<T> specification, bool isChainDiscarded = false)
        {
            this.Specification = specification;
            this.IsChainDiscarded = isChainDiscarded;
        }
    }
}