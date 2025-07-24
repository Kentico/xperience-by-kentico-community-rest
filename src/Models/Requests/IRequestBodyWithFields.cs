using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// Represents a request body containing an objects field names and values.
    /// </summary>
    public interface IRequestBodyWithFields
    {
        /// <summary>
        /// A collection of field names and values.
        /// </summary>
        public JsonObject Fields { get; set; }
    }
}
