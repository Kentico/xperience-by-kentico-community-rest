using CMS.DataEngine;

namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Contains methods for retrieving the object types allowed for the REST service.
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
    }
}
