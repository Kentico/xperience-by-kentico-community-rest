using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models.Requests
{
    public class CreateRequestBody : BaseRequestBody, IRequestBodyWithFields
    {
        public JsonObject Fields { get; set; } = [];
    }
}
