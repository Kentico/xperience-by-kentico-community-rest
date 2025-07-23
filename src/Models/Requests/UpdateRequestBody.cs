using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models.Requests
{
    public class UpdateRequestBody : BaseRequestBody, IRequestBodyWithIdentifiers, IRequestBodyWithFields
    {
        public int Id { get; set; }


        public string? CodeName { get; set; }


        public Guid Guid { get; set; }


        public JsonObject Fields { get; set; } = [];
    }
}
