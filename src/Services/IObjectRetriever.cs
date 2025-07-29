using System.Data;

using CMS.DataEngine;

using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Contains methods for retrieving objects from the Xperience by Kentico database.
    /// </summary>
    public interface IObjectRetriever
    {
        /// <summary>
        /// Gets a list of objects.
        /// </summary>
        /// <param name="settings">The configuration for data retrieval.</param>
        /// <param name="totalRecords">The total number of records in the database for the provided object type.</param>
        public IEnumerable<DataRow> GetAll(GetAllSettings settings, out int totalRecords);


        /// <summary>
        /// Gets a single object determined by the provided indentifier in <paramref name="body"/>.
        /// </summary>
        /// <param name="body">The request body which contains at least one valid identifier.</param>
        public BaseInfo? GetExistingObject<TBody>(TBody body) where TBody : IRequestBodyWithObjectType, IRequestBodyWithIdentifiers;


        /// <summary>
        /// Gets a single object by its primary key.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="id">The ID of the object to retrieve.</param>
        public BaseInfo? GetById(string objectType, int id);


        /// <summary>
        /// Gets a single object by its code name.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="codeName">The code name of the object to retrieve.</param>
        public BaseInfo? GetByCodeName(string objectType, string codeName);


        /// <summary>
        /// Gets a single object by its GUID.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="guid">The GUID of the object to retrieve.</param>
        public BaseInfo? GetByGuid(string objectType, Guid guid);


        /// <summary>
        /// Instantiates and returns a new object of the given type.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        public BaseInfo GetNewObject(string objectType);
    }
}
