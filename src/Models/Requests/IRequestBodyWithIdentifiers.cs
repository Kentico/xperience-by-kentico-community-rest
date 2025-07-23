namespace Xperience.Community.Rest.Models.Requests
{
    public interface IRequestBodyWithIdentifiers
    {
        public int Id { get; set; }


        public string? CodeName { get; set; }


        public Guid Guid { get; set; }
    }
}
