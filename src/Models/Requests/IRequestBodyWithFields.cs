using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models.Requests
{
    public interface IRequestBodyWithFields
    {
        public JsonObject Fields { get; set; }
    }
}
