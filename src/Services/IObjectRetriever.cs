using System.Data;

using CMS.DataEngine;

using Xperience.Community.Rest.Models;

namespace Xperience.Community.Rest.Services
{
    public interface IObjectRetriever
    {
        public IEnumerable<DataRow> GetAll(
            string objectType,
            out int totalRecords,
            string? where = null,
            string? orderBy = null,
            string? columns = null,
            int? topN = null);


        public BaseInfo? GetExistingObject<TBody>(TBody body) where TBody : BaseRequestBody, IRequestBodyWithIdentifiers;


        public BaseInfo? GetById(string objectType, int id);


        public BaseInfo? GetByCodeName(string objectType, string codeName);


        public BaseInfo? GetByGuid(string objectType, Guid guid);
    }
}
