using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// The request body used when updating an existing object via the REST service.
    /// </summary>
    public class UpdateRequestBody : IRequestBodyWithObjectType, IRequestBodyWithIdentifiers, IRequestBodyWithFields
    {
        public int? Id { get; set; }


        public string? CodeName { get; set; }


        public Guid? Guid { get; set; }


        public JsonObject Fields { get; set; } = [];


        public required string ObjectType { get; set; }
    }
}
