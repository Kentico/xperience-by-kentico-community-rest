namespace Xperience.Community.Rest.Models
{
    public class GetAllResponse
    {
        public int TotalRecords { get; set; }


        public IEnumerable<dynamic> Objects { get; set; } = [];
    }
}
