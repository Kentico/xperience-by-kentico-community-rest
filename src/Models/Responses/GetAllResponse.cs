namespace Xperience.Community.Rest.Models.Responses
{
    /// <summary>
    /// The response returned by a request which lists multiple objects.
    /// </summary>
    public class GetAllResponse
    {
        /// <summary>
        /// The number of total records in the database.
        /// </summary>
        public int TotalRecords { get; set; }


        /// <summary>
        /// The absolute URL of the next page of results, or <c>null</c> if paging is disabled or there are no more results.
        /// </summary>
        public string? NextUrl { get; set; }


        /// <summary>
        /// The list of returned objects.
        /// </summary>
        public IEnumerable<dynamic> Objects { get; set; } = [];
    }
}
