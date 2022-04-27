﻿namespace MikyM.Common.DataAccessLayer_Net5.Filters
{
    /// <summary>
    /// Data transfer object for <see cref="PaginationFilter"/>
    /// </summary>
    public class PaginationFilterDto
    {
        /// <summary>
        /// Page number
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
    }
}