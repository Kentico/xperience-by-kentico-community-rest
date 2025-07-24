using Xperience.Community.Rest.Services;

namespace Xperience.Community.Rest.Models.Requests
{
    /// <summary>
    /// Represents the properties required to manipulate individual objects via the REST service. Only one identifier is required and the
    /// order of processing is defined in <see cref="ObjectRetriever.GetExistingObject{TBody}(TBody)"/>.
    /// </summary>
    public interface IRequestBodyWithIdentifiers
    {
        /// <summary>
        /// The ID of the object.
        /// </summary>
        public int? Id { get; set; }


        /// <summary>
        /// The code name of the object.
        /// </summary>
        public string? CodeName { get; set; }


        /// <summary>
        /// The GUID of the object.
        /// </summary>
        public Guid? Guid { get; set; }
    }
}
