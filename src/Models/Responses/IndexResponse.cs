namespace Xperience.Community.Rest.Models.Responses
{
    /// <summary>
    /// The response returned by a request which provides information about the REST service's configuration.
    /// </summary>
    public class IndexResponse
    {
        /// <summary>
        /// If <c>true</c>, the REST service is enabled.
        /// </summary>
        public bool Enabled { get; set; }


        /// <summary>
        /// A list of objects allowed to be managed by the REST service.
        /// </summary>
        public IEnumerable<string> EnabledObjectTypes { get; set; } = [];
    }
}
