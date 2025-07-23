using CMS.DataEngine;

using Microsoft.AspNetCore.Mvc;

using Xperience.Community.Rest.Attributes;
using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Services;

namespace Xperience.Community.Rest.Controllers
{
    [ApiController]
    [Route("rest")]
    [RestAutorization]
    public class RestController(IObjectRetriever objectRetriever, IObjectMapper mapper) : ControllerBase
    {
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
