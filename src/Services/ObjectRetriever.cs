using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.DataEngine.Internal;
using CMS.OnlineForms;

using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Models.Requests;
using Xperience.Community.Rest.Services;

[assembly: RegisterImplementation(typeof(IObjectRetriever), typeof(ObjectRetriever))]
namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Default implementation of <see cref="IObjectRetriever"/>.
    /// </summary>
    public class ObjectRetriever : IObjectRetriever
    {
        public IEnumerable<DataRow> GetAll(GetAllSettings settings, out int totalRecords)
        {
            var query = GetQuery(settings);
            DataSet result;

            // Use TransactionScope for security reasons when columns, where or orderby is defined
            bool useTransaction = !string.IsNullOrEmpty(settings.Where) &&
                !string.IsNullOrEmpty(query.OrderByColumns) &&
                !string.IsNullOrEmpty(settings.Columns);
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


        public BaseInfo GetNewObject(string objectType) => IsForm(objectType)
                ? BizFormItem.New(objectType)
                : ModuleManager.GetObject(objectType, true);


        public BaseInfo? GetExistingObject<TBody>(TBody body) where TBody : BaseRequestBody, IRequestBodyWithIdentifiers
        {
            if (!string.IsNullOrEmpty(body.CodeName))
            {
                return GetByCodeName(body.ObjectType, body.CodeName);
            }

            if (body.Guid is not null && !body.Guid.Equals(Guid.Empty))
            {
                return GetByGuid(body.ObjectType, (Guid)body.Guid);
            }

            if (body.Id > 0)
            {
                return GetById(body.ObjectType, (int)body.Id);
            }

            throw new InvalidOperationException("No identifier provided.");
        }


        public BaseInfo? GetById(string objectType, int id) =>
            GetNewObject(objectType).TypeInfo.ProviderObject.GetInfoById(id);


        public BaseInfo? GetByCodeName(string objectType, string codeName) =>
            GetNewObject(objectType).TypeInfo.ProviderObject.GetInfoByName(codeName);


        public BaseInfo? GetByGuid(string objectType, Guid guid) =>
            GetNewObject(objectType).TypeInfo.ProviderObject.GetInfoByGuid(guid);


        private IDataQuery GetQuery(GetAllSettings settings)
        {
            GeneralizedInfo info = GetNewObject(settings.ObjectType);
            string? orderBy = settings.OrderBy;
            if (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("##default##", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = info.DisplayNameColumn;
            }

            // If no order by was defined, set the default orderby
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = info.TypeInfo.DefaultOrderBy;
            }

            int topN = settings.IsPagedQuery ? 0 : settings.TopN ?? 0;
            var query = info.GetDataQuery(true, parameters =>
            {
                parameters
                    .TopN(topN)
                    .Where(settings.Where)
                    .OrderBy(orderBy)
                    .Columns(settings.Columns);

                if (settings.IsPagedQuery)
                {
                    parameters.Page(settings.Page ?? 0, settings.PageSize ?? 0);
                }
            });

            if (settings.IsPagedQuery)
            {
                query.GetTotalRecordsForPagedQuery = true;
            }

            return query;
        }


        private static bool IsForm(string objectType) => DataClassInfoProvider.GetDataClassInfo(objectType).IsForm();
    }
}
