namespace Xperience.Community.Rest.Models
{
    public class DeleteRequestBody : BaseRequestBody, IRequestBodyWithIdentifiers
    {
        public int Id { get; set; }


        public string? CodeName { get; set; }


        public Guid Guid { get; set; }
    }
}
