using System.Web;

using CMS.Core;
using CMS.DataEngine;
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
    [ApiController]
    [Route("rest")]
    [RestAutorization]
    public class RestController(IObjectRetriever objectRetriever, IObjectMapper mapper, ISettingsService settingsService) : ControllerBase
    {
        private bool IsEnabled => ValidationHelper.GetBoolean(settingsService[Constants.SETTINGS_KEY_ENABLED], false);


        private static IEnumerable<string> RegisteredTypes => ObjectTypeManager.RegisteredTypes.Select(t => t.ObjectType.ToLower());


        private IEnumerable<string> EnabledTypes
        {
            get
            {
                string? types = settingsService[Constants.SETTINGS_KEY_ALLOWEDTYPES];
                if (string.IsNullOrEmpty(types))
                {
                    return RegisteredTypes;
                }

                return types.ToLower().Split(';');
            }
        }


        [HttpGet]
        public ActionResult<IndexResponse> Index() =>
            Ok(new IndexResponse
            {
                Enabled = IsEnabled,
                EnabledObjectTypes = RegisteredTypes.Intersect(EnabledTypes)
            });


        [HttpGet]
        [Route("{objectType}/all")]
        public ActionResult<GetAllResponse> Get(
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


        [HttpGet]
        [Route("{objectType}/{id:int}")]
        public IActionResult Get(string objectType, int id)
        {
            ValidateRequestOrThrow(objectType);
            var obj = objectRetriever.GetById(objectType, id);
            if (obj is null)
            {
                return NotFound();
            }

            return Ok(mapper.MapToSimpleObject(obj));
        }


        [HttpGet]
        [Route("{objectType}/{codeName}")]
        public IActionResult Get(string objectType, string codeName)
        {
            ValidateRequestOrThrow(objectType);
            var obj = objectRetriever.GetByCodeName(objectType, codeName);
            if (obj is null)
            {
                return NotFound();
            }

            return Ok(mapper.MapToSimpleObject(obj));
        }


        [HttpGet]
        [Route("{objectType}/{guid:guid}")]
        public IActionResult Get(string objectType, Guid guid)
        {
            ValidateRequestOrThrow(objectType);
            var obj = objectRetriever.GetByGuid(objectType, guid);
            if (obj is null)
            {
                return NotFound();
            }

            return Ok(mapper.MapToSimpleObject(obj));
        }


        [HttpPost]
        public IActionResult Post([FromBody] CreateRequestBody body)
        {
            ValidateRequestOrThrow(body.ObjectType);
            var newObject = ModuleManager.GetObject(body.ObjectType, true);
            mapper.MapFieldsFromRequest(newObject, body);
            newObject.Insert();

            return Ok(mapper.MapToSimpleObject(newObject));
        }


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


        [HttpDelete]
        public IActionResult Delete([FromBody] DeleteRequestBody model)
        {
            ValidateRequestOrThrow(model.ObjectType);
            var existingObject = objectRetriever.GetExistingObject(model);
            if (existingObject is null)
            {
                return NotFound();
            }

            existingObject.Delete();

            return Ok(mapper.MapToSimpleObject(existingObject));
        }


        private void ValidateRequestOrThrow(string objectType)
        {
            if (!IsEnabled)
            {
                throw new InvalidOperationException("REST service disabled.");
            }

            string normalizedObjectType = objectType.ToLower();

            // Ensure type is registered
            _ = ObjectTypeManager.GetTypeInfo(normalizedObjectType) ??
                throw new InvalidOperationException($"Object type '{normalizedObjectType}' not registered.");

            // Ensure type is allowed in settings
            if (!EnabledTypes.Contains(normalizedObjectType))
            {
                throw new InvalidOperationException($"Object type '{normalizedObjectType}' not enabled by REST service.");
            }
        }
    }
}
