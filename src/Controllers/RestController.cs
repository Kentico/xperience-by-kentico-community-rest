using System.Web;

using CMS.DataEngine;

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
    public class RestController(IObjectRetriever objectRetriever, IObjectMapper mapper) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IndexResponse> Index()
        {
            // TODO: Get enabled value from settings key or appsettings
            bool enabled = true;
            // TODO: Allow enabling/disabling of specific object types
            var types = ObjectTypeManager.RegisteredTypes.Select(t => t.ObjectType);

            return Ok(new IndexResponse
            {
                Enabled = enabled,
                EnabledObjectTypes = types
            });
        }


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
            ValidateTypeOrThrow(objectType);
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
            ValidateTypeOrThrow(objectType);
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
            ValidateTypeOrThrow(objectType);
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
            ValidateTypeOrThrow(objectType);
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
            ValidateTypeOrThrow(body.ObjectType);
            var newObject = ModuleManager.GetObject(body.ObjectType, true);
            mapper.MapFieldsFromRequest(newObject, body);
            newObject.Insert();

            return Ok(mapper.MapToSimpleObject(newObject));
        }


        [HttpPatch]
        public IActionResult Patch([FromBody] UpdateRequestBody body)
        {
            ValidateTypeOrThrow(body.ObjectType);
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
            ValidateTypeOrThrow(model.ObjectType);
            var existingObject = objectRetriever.GetExistingObject(model);
            if (existingObject is null)
            {
                return NotFound();
            }

            existingObject.Delete();

            return Ok(mapper.MapToSimpleObject(existingObject));
        }


        private static void ValidateTypeOrThrow(string objectType) => _ = ObjectTypeManager.GetTypeInfo(objectType) ??
            throw new InvalidOperationException($"Object type '{objectType}' not registered.");
    }
}
