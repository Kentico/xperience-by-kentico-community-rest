using CMS.DataEngine;

using Xperience.Community.Rest.Models;

namespace Xperience.Community.Rest.Services
{
    public interface IObjectMapper
    {
        public void MapFieldsFromRequest<TBody>(BaseInfo infoObject, TBody body) where TBody : IRequestBodyWithFields;


        public dynamic MapToSimpleObject(BaseInfo infoObject);
    }
}
