using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// The request body used when creating a new object via the REST service.
    /// </summary>
    public class CreateRequestBody : IRequestBodyWithObjectType, IRequestBodyWithFields
    {
        public JsonObject Fields { get; set; } = [];


        public required string ObjectType { get; set; }
    }
}
