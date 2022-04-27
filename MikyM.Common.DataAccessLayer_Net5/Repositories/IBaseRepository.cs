using System;

namespace MikyM.Common.DataAccessLayer_Net5.Repositories
{
    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IBaseRepository
    {
        /// <summary>
        /// Entity type
        /// </summary>
        Type EntityType { get; }
    }
}