using System.Data;

using CMS.DataEngine;

using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    public interface IObjectRetriever
    {
        public IEnumerable<DataRow> GetAll(GetAllSettings settings, out int totalRecords);


        public BaseInfo? GetExistingObject<TBody>(TBody body) where TBody : BaseRequestBody, IRequestBodyWithIdentifiers;


        public BaseInfo? GetById(string objectType, int id);


        public BaseInfo? GetByCodeName(string objectType, string codeName);


        public BaseInfo? GetByGuid(string objectType, Guid guid);
    }
}
