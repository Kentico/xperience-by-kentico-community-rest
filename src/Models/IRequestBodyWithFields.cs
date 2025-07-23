using System.Text.Json.Nodes;

namespace Xperience.Community.Rest.Models
{
    public interface IRequestBodyWithFields
    {
        public JsonObject Fields { get; set; }
    }
}
