namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// The base request body used in any REST request.
    /// </summary>
    public class BaseRequestBody
    {
        /// <summary>
        /// The requested object type.
        /// </summary>
        public required string ObjectType { get; set; }
    }
}
