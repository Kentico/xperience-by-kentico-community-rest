using System.Data;

using CMS;
using CMS.DataEngine;

using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Services;

[assembly: RegisterImplementation(typeof(IObjectRetriever), typeof(ObjectRetriever))]
namespace Xperience.Community.Rest.Services
{
    public class ObjectRetriever : IObjectRetriever
    {
        private const int TOPN = 50;


        public IEnumerable<DataRow> GetAll(
            string objectType,
            out int totalRecords,
            string? where = null,
            string? orderBy = null,
            string? columns = null,
            int? topN = null)
        {
            GeneralizedInfo info = ModuleManager.GetObject(objectType);
            if (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("##default##", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = info.DisplayNameColumn;
            }

            // If no order by was defined, set the default orderby
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = info.TypeInfo.DefaultOrderBy;
            }

            var query = info.GetDataQuery(true, parameters => parameters
                .TopN(topN ?? TOPN)
                .Where(where)
                .OrderBy(orderBy)
                .Columns(columns)
            );

            // Use TransactionScope for security reasons when columns, where or orderby is defined
            DataSet result;
            bool useTransaction = !string.IsNullOrEmpty(where) && !string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(columns);
            using (useTransaction ? new CMSTransactionScope() : null)
            {
                result = query.Result;
                totalRecords = query.TotalRecords;
            }

            if (result.Tables.Count == 0)
            {
                return [];
            }

            return result.Tables[0].Rows.OfType<DataRow>();
        }


        public BaseInfo? GetExistingObject<TBody>(TBody body) where TBody : BaseRequestBody, IRequestBodyWithIdentifiers
        {
            if (!string.IsNullOrEmpty(body.CodeName))
            {
                return GetByCodeName(body.ObjectType, body.CodeName);
            }

            if (!body.Guid.Equals(Guid.Empty))
            {
                return GetByGuid(body.ObjectType, body.Guid);
            }

            if (body.Id > 0)
            {
                return GetById(body.ObjectType, body.Id);
            }

            throw new InvalidOperationException("No identifier provided.");
        }


        public BaseInfo? GetById(string objectType, int id)
        {
            var typeInfo = ObjectTypeManager.GetTypeInfo(objectType, true);

            return typeInfo.ProviderObject.GetInfoById(id);
        }


        public BaseInfo? GetByCodeName(string objectType, string codeName)
        {
            var typeInfo = ObjectTypeManager.GetTypeInfo(objectType, true);

            return typeInfo.ProviderObject.GetInfoByName(codeName);
        }


        public BaseInfo? GetByGuid(string objectType, Guid guid)
        {
            var typeInfo = ObjectTypeManager.GetTypeInfo(objectType, true);

            return typeInfo.ProviderObject.GetInfoByGuid(guid);
        }
    }
}
