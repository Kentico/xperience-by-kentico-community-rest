namespace Xperience.Community.Rest.Models.Responses
{
    public class GetAllResponse
    {
        public int TotalRecords { get; set; }


        public string? NextUrl { get; set; }


        public IEnumerable<dynamic> Objects { get; set; } = [];
    }
}
