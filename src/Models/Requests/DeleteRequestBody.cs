namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// The request body used when deleting an object via the REST service.
    /// </summary>
    public class DeleteRequestBody : BaseRequestBody, IRequestBodyWithIdentifiers
    {
        public int Id { get; set; }


        public string? CodeName { get; set; }


        public Guid Guid { get; set; }
    }
}
