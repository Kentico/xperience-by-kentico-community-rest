using CMS.DataEngine;

using Xperience.Community.Rest.Models;

namespace Xperience.Community.Rest.Services
{
    public interface IObjectRetriever
    {
        public BaseInfo? GetExistingObject<TBody>(TBody body) where TBody : BaseRequestBody, IRequestBodyWithIdentifiers;


        public BaseInfo? GetById(string objectType, int id);


        public BaseInfo? GetByCodeName(string objectType, string codeName);


        public BaseInfo? GetByGuid(string objectType, Guid guid);
    }
}
