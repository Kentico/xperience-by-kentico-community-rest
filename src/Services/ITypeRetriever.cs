using CMS.DataEngine;

using Xperience.Community.Rest.Models;

namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Contains methods for retrieving object type information for the REST service.
    /// </summary>
    public interface ITypeRetriever
    {
        /// <summary>
        /// Gets the names of all allowed object types of type <see cref="ClassType.FORM"/>.
        /// </summary>
        public IEnumerable<string> GetAllowedForms();


        /// <summary>
        /// Gets the names of all allowed object types of type <see cref="ClassType.SYSTEM_TABLE"/> or <see cref="ClassType.OTHER"/>.
        /// </summary>
        public IEnumerable<string> GetAllowedObjects();


        /// <summary>
        /// Gets metadata for the provided object type and its fields.
        /// </summary>
        /// <param name="objectType">The object type to get metadata for.</param>
        public ObjectMeta GetMetadata(string objectType);
    }
}
