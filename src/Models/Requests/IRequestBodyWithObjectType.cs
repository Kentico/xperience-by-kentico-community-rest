namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// Represents a request body containing an object type.
    /// </summary>
    public interface IRequestBodyWithObjectType
    {
        /// <summary>
        /// The requested object type.
        /// </summary>
        public string ObjectType { get; set; }
    }
}
