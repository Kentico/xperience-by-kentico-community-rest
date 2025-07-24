using System.Data;

using CMS.DataEngine;

using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Maps data from various object types to other objects.
    /// </summary>
    public interface IObjectMapper
    {
        /// <summary>
        /// Maps the data provided in the <paramref name="body"/> to the <paramref name="infoObject"/>.
        /// </summary>
        /// <param name="infoObject">The object to alter the data of.</param>
        /// <param name="body">The body containing the new data for the object.</param>
        public void MapFieldsFromRequest<TBody>(BaseInfo infoObject, TBody body) where TBody : IRequestBodyWithFields;


        /// <summary>
        /// Maps a <see cref="BaseInfo"/> object to a dynamic object which only contains the public fields of the object type.
        /// </summary>
        /// <param name="infoObject">The object to map.</param>
        public dynamic MapToSimpleObject(BaseInfo infoObject);


        /// <summary>
        /// Maps a <see cref="DataRow"/> object to a dynamic object which only contains the columns present in the row.
        /// </summary>
        /// <param name="row">The row to map.</param>
        public dynamic MapToSimpleObject(DataRow row);
    }
}
