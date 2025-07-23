namespace Xperience.Community.Rest.Models.Responses
{
    public class IndexResponse
    {
        public bool Enabled { get; set; }


        public IEnumerable<string> EnabledObjectTypes { get; set; } = [];
    }
}
