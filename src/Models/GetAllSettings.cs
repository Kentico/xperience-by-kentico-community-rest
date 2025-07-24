using Xperience.Community.Rest.Services;

namespace Xperience.Community.Rest.Models
{
    /// <summary>
    /// The configuration for <see cref="ObjectRetriever.GetAll(GetAllSettings, out int)"/>.
    /// </summary>
    public class GetAllSettings
    {
        /// <summary>
        /// The object type to retrieve.
        /// </summary>
        public required string ObjectType { get; set; }


        /// <summary>
        /// A SQL WHERE condition to restrict the returned objects.
        /// </summary>
        public string? Where { get; set; }


        /// <summary>
        /// The column name to order the objects by. May include "asc" or "desc."
        /// </summary>
        public string? OrderBy { get; set; }


        /// <summary>
        /// Column names of the object type to include in the returned objects.
        /// </summary>
        public string? Columns { get; set; }


        /// <summary>
        /// The maximum number of objects returned.
        /// </summary>
        public int? TopN { get; set; }


        /// <summary>
        /// For pagination, indicates the number of objects in each page.
        /// </summary>
        public int? PageSize { get; set; }


        /// <summary>
        /// For pagination, indicates the page to retrieve objects from, where 0 is the first page.
        /// </summary>
        public int? Page { get; set; }


        /// <summary>
        /// If <c>true</c>, the requested objects should be paged.
        /// </summary>
        public bool IsPagedQuery => Page is not null && PageSize is not null;
    }
}
