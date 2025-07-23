using System.Data;

using CMS.DataEngine;

using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    public interface IObjectMapper
    {
        public void MapFieldsFromRequest<TBody>(BaseInfo infoObject, TBody body) where TBody : IRequestBodyWithFields;


        public dynamic MapToSimpleObject(BaseInfo infoObject);


        public dynamic MapToSimpleObject(DataRow row);
    }
}
