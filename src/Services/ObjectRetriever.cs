using CMS;
using CMS.DataEngine;

using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Services;

[assembly: RegisterImplementation(typeof(IObjectRetriever), typeof(ObjectRetriever))]
namespace Xperience.Community.Rest.Services
{
    public class ObjectRetriever : IObjectRetriever
    {
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
