using System.Web;

using CMS.Core;
using CMS.Helpers;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

using Xperience.Community.Rest.Attributes;
using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Models.Requests;
using Xperience.Community.Rest.Models.Responses;
using Xperience.Community.Rest.Services;

namespace Xperience.Community.Rest.Controllers
{
    /// <summary>
    /// The REST controller providing data retrieval and manipulation.
    /// </summary>
    [ApiController]
    [Route("rest")]
    [RestAuthentication]
    public class RestController(
        ITypeRetriever typeRetriever,
        IObjectRetriever objectRetriever,
        IObjectMapper mapper,
        ISettingsService settingsService) : ControllerBase
    {
        /// <summary>
        /// If <c>true</c>, the REST service is enabled.
        /// </summary>
        private bool IsEnabled => ValidationHelper.GetBoolean(settingsService[Constants.SETTINGS_KEY_ENABLED], false);


        /// <summary>
        /// An action at the base path which provides information about the REST service's configuration.
        /// </summary>
        [HttpGet]
        public ActionResult<IndexResponse> Index() =>
            Ok(new IndexResponse
            {
                Enabled = IsEnabled,
                EnabledObjects = typeRetriever.GetAllowedObjects(),
                EnabledForms = typeRetriever.GetAllowedForms()
            });


        /// <summary>
        /// An action which returns metadata for an object type.
        /// </summary>
        /// <param name="objectType">The object type to retrieve metadata for.</param>
        [HttpGet]
        [Route("metadata/{objectType}")]
        public ActionResult<ObjectMeta> GetMetadata(string objectType)
        {
            ValidateRequestOrThrow(objectType);

            return typeRetriever.GetMetadata(objectType);
        }


        /// <summary>
        /// An action which returns multiple objects and supports pagination.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="where">A SQL WHERE condition to restrict the returned objects.</param>
        /// <param name="columns">Column names of the object type to include in the returned objects.</param>
        /// <param name="orderBy">The column name to order the objects by. May include "asc" or "desc."</param>
        /// <param name="topN">The maximum number of objects returned.</param>
        /// <param name="pageSize">For pagination, indicates the number of objects in each page.</param>
        /// <param name="page">For pagination, indicates the page to retrieve objects from, where 0 is the first page.</param>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpGet]
        [Route("{objectType}/all")]
        public ActionResult<GetAllResponse> GetAll(
            string objectType,
            [FromQuery] string? where,
            [FromQuery] string? columns,
            [FromQuery] string? orderBy,
            [FromQuery] int? topN,
            [FromQuery] int? pageSize,
            [FromQuery] int? page)
        {
            ValidateRequestOrThrow(objectType);
            var settings = new GetAllSettings
            {
                ObjectType = objectType,
                Columns = columns,
                OrderBy = orderBy,
                Where = where,
                TopN = topN,
                PageSize = pageSize,
                Page = page
            };
            var rows = objectRetriever.GetAll(settings, out int totalRecords);
            var objs = rows.Select(mapper.MapToSimpleObject);
            var response = new GetAllResponse
            {
                TotalRecords = totalRecords,
                Objects = objs
            };

            // If paging and there are more records available, set next URL
            int? seenRecords = (pageSize * page) + pageSize;
            bool hasMoreRecords = settings.IsPagedQuery && seenRecords is not null && totalRecords > seenRecords;
            if (hasMoreRecords)
            {
                string url = Request.GetDisplayUrl();
                var uriBuilder = new UriBuilder(url);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query) ??
                    throw new InvalidOperationException("Unable to parse query string.");
                query[nameof(page)] = (page + 1).ToString();
                uriBuilder.Query = query.ToString();

                response.NextUrl = uriBuilder.ToString();
            }

            return Ok(response);
        }


        /// <summary>
        /// An action which returns a single object identified by its primary key.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="id">The primary key of the desired object.</param>
        [HttpGet]
        [Route("{objectType}/{id:int}")]
        public IActionResult GetById(string objectType, int id)
        {
            ValidateRequestOrThrow(objectType);
            var obj = objectRetriever.GetById(objectType, id);
            if (obj is null)
            {
                return NotFound();
            }

            return Ok(mapper.MapToSimpleObject(obj));
        }


        /// <summary>
        /// An action which returns a single object identified by its code name.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="codeName">The code name of the desired object.</param>
        [HttpGet]
        [Route("{objectType}/{codeName}")]
        public IActionResult GetByCodeName(string objectType, string codeName)
        {
            ValidateRequestOrThrow(objectType);
            var obj = objectRetriever.GetByCodeName(objectType, codeName);
            if (obj is null)
            {
                return NotFound();
            }

            return Ok(mapper.MapToSimpleObject(obj));
        }


        /// <summary>
        /// An action which returns a single object identified by its GUID.
        /// </summary>
        /// <param name="objectType">The object type to retrieve.</param>
        /// <param name="guid">The GUID of the desired object.</param>
        [HttpGet]
        [Route("{objectType}/{guid:guid}")]
        public IActionResult GetByGuid(string objectType, Guid guid)
        {
            ValidateRequestOrThrow(objectType);
            var obj = objectRetriever.GetByGuid(objectType, guid);
            if (obj is null)
            {
                return NotFound();
            }

            return Ok(mapper.MapToSimpleObject(obj));
        }


        /// <summary>
        /// An action which creates a new object.
        /// </summary>
        /// <param name="body">A model containing the object type and field values of the new object.</param>
        [HttpPost]
        public IActionResult Post([FromBody] CreateRequestBody body)
        {
            ValidateRequestOrThrow(body.ObjectType);
            var newObject = objectRetriever.GetNewObject(body.ObjectType);
            mapper.MapFieldsFromRequest(newObject, body);
            newObject.Insert();

            return Ok(mapper.MapToSimpleObject(newObject));
        }


        /// <summary>
        /// An action which partially updates an existing object.
        /// </summary>
        /// <param name="body">A model containing the object type and field values of the new object. Also contains multiple properties
        /// which can be used to identify the existing object.</param>
        [HttpPatch]
        public IActionResult Patch([FromBody] UpdateRequestBody body)
        {
            ValidateRequestOrThrow(body.ObjectType);
            var existingObject = objectRetriever.GetExistingObject(body);
            if (existingObject is null)
            {
                return NotFound();
            }

            mapper.MapFieldsFromRequest(existingObject, body);
            existingObject.Update();

            return Ok(mapper.MapToSimpleObject(existingObject));
        }


        /// <summary>
        /// An action which deletes an existing object.
        /// </summary>
        /// <param name="body">A model which contains the object type and identifier of the existing object.</param>
        [HttpDelete]
        public IActionResult Delete([FromBody] DeleteRequestBody body)
        {
            ValidateRequestOrThrow(body.ObjectType);
            var existingObject = objectRetriever.GetExistingObject(body);
            if (existingObject is null)
            {
                return NotFound();
            }

            existingObject.Delete();

            return Ok(mapper.MapToSimpleObject(existingObject));
        }


        /// <summary>
        /// Validates that the request may proceed based on the provided <paramref name="objectType"/> and the REST configuration.
        /// </summary>
        /// <param name="objectType">The object type provided in the request body.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private void ValidateRequestOrThrow(string objectType)
        {
            if (!IsEnabled)
            {
                throw new InvalidOperationException("REST service disabled.");
            }

            // Ensure type is allowed in settings
            if (!typeRetriever.GetAllowedForms().Contains(objectType, StringComparer.InvariantCultureIgnoreCase) &&
                !typeRetriever.GetAllowedObjects().Contains(objectType, StringComparer.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException($"Object type '{objectType}' not enabled by REST service.");
            }
        }
    }
}
