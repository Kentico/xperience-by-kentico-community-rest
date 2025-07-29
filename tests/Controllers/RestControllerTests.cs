using CMS.Core;
using CMS.Membership;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using NSubstitute;

using Xperience.Community.Rest.Models.Requests;
using Xperience.Community.Rest.Models.Responses;
using Xperience.Community.Rest.Services;

namespace Xperience.Community.Rest.Controllers
{
    /// <summary>
    /// Tests for <see cref="RestController"/>.
    /// </summary>
    /// <remarks>
    /// It is not possible to test <see cref="RestController.Delete(DeleteRequestBody)"/> as it appears to require a database connection.
    /// </remarks>
    internal class RestControllerTests() : TestWithFakes
    {
        private RestController? controller;
        private readonly ISettingsService settings = Substitute.For<ISettingsService>();


        [SetUp]
        public void SetUp()
        {
            settings[Constants.SETTINGS_KEY_ENABLED].Returns("true");
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns(string.Empty);

            var objectRetriever = new ObjectRetriever();
            controller = new RestController(new TypeRetriever(settings, objectRetriever), objectRetriever, new ObjectMapper(), settings);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void Index_EnabledSetting_ReturnsEnabledStatus(bool isEnabled)
        {
            settings[Constants.SETTINGS_KEY_ENABLED].Returns(isEnabled.ToString());

            var actionResult = controller!.Index();
            var okObjectResult = actionResult.Result as OkObjectResult;
            var indexResponse = okObjectResult?.Value as IndexResponse;

            Assert.Multiple(() =>
            {
                Assert.That(indexResponse, Is.Not.Null);
                Assert.That(indexResponse!.Enabled, Is.EqualTo(isEnabled));
            });
        }


        [TestCase("")]
        [TestCase(null)]
        public void Index_AllowedTypesSetting_NotSet_ReturnsAllTypes(string? types)
        {
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns(types);

            var actionResult = controller!.Index();
            var okObjectResult = actionResult.Result as OkObjectResult;
            var indexResponse = okObjectResult?.Value as IndexResponse;

            Assert.Multiple(() =>
            {
                Assert.That(indexResponse, Is.Not.Null);
                Assert.That(indexResponse!.EnabledObjects, Is.EqualTo(OBJECT_TYPES));
                Assert.That(indexResponse!.EnabledForms, Does.Contain(FORM_NAME));
            });
        }


        [Test]
        public void Index_AllowedTypesSetting_SetValue_ReturnsValidTypes()
        {
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns("om.contact;cms.user;invalid.type");

            var actionResult = controller!.Index();
            var okObjectResult = actionResult.Result as OkObjectResult;
            var indexResponse = okObjectResult?.Value as IndexResponse;

            Assert.Multiple(() =>
            {
                Assert.That(indexResponse, Is.Not.Null);
                Assert.That(indexResponse!.EnabledObjects.Count(), Is.EqualTo(2));
                Assert.That(indexResponse!.EnabledObjects, Does.Contain("cms.user"));
                Assert.That(indexResponse!.EnabledObjects, Does.Contain("om.contact"));
                Assert.That(indexResponse!.EnabledObjects, Does.Not.Contain("invalid.type"));
            });
        }


        [Test]
        public void Get_Metadata_ReturnsObjectMetadata()
        {
            var actionResult = controller!.GetMetadata(UserInfo.OBJECT_TYPE);
            var objectMeta = actionResult?.Value;

            Assert.Multiple(() =>
            {
                Assert.That(objectMeta, Is.Not.Null);
                Assert.That(objectMeta!.ObjectType, Is.EqualTo(UserInfo.OBJECT_TYPE));
                Assert.That(objectMeta!.Fields.Count(), Is.EqualTo(4));
                Assert.That(objectMeta!.IdColumn, Is.EqualTo(nameof(UserInfo.UserID)));
                Assert.That(objectMeta!.GuidColumn, Is.EqualTo(nameof(UserInfo.UserGUID)));
                Assert.That(objectMeta!.CodeNameColumn, Is.EqualTo(nameof(UserInfo.UserName)));
            });
        }


        [Test]
        public void Get_ServiceDisabled_Throws()
        {
            settings[Constants.SETTINGS_KEY_ENABLED].Returns("false");

            Assert.Throws<InvalidOperationException>(() => controller?.GetById(UserInfo.OBJECT_TYPE, 1), "REST service disabled.");
        }


        [Test]
        public void GetAll_ReturnsAllObjects()
        {
            var actionResult = controller!.GetAll(UserInfo.OBJECT_TYPE, null, null, null, null, null, null);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var getAllResponse = okObjectResult?.Value as GetAllResponse;

            Assert.Multiple(() =>
            {
                Assert.That(getAllResponse, Is.Not.Null);
                Assert.That(getAllResponse!.NextUrl, Is.Null);
                Assert.That(getAllResponse!.TotalRecords, Is.EqualTo(4));
                Assert.That(getAllResponse!.Objects.Count(), Is.EqualTo(4));
            });
        }


        [Test]
        public void GetAll_WhereAndOrder_ReturnsFilteredObjects()
        {
            string where = $"{nameof(UserInfo.FirstName)} IN ('UserA', 'UserB')";
            string orderBy = $"{nameof(UserInfo.FirstName)} asc";
            // Note: Columns parameter cannot be tested as it requires a database connection
            var actionResult = controller!.GetAll(UserInfo.OBJECT_TYPE, where, null, orderBy, null, null, null);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var getAllResponse = okObjectResult?.Value as GetAllResponse;

            var firstObject = getAllResponse?.Objects.ElementAt(0);
            var secondObject = getAllResponse?.Objects.ElementAt(1);

            Assert.Multiple(() =>
            {
                Assert.That(getAllResponse, Is.Not.Null);
                Assert.That(getAllResponse!.NextUrl, Is.Null);
                // Check WHERE condition worked
                Assert.That(getAllResponse!.TotalRecords, Is.EqualTo(2));
                Assert.That(getAllResponse!.Objects.Count(), Is.EqualTo(2));
                Assert.That(firstObject, Is.Not.Null);
                Assert.That(secondObject, Is.Not.Null);
                // Check ORDER BY condition worked
                Assert.That(firstObject!.FirstName, Is.EqualTo("UserA"));
                Assert.That(secondObject!.FirstName, Is.EqualTo("UserB"));
            });
        }


        [Test]
        public void GetAll_TopN_ReturnsFilteredObjects()
        {
            var actionResult = controller!.GetAll(UserInfo.OBJECT_TYPE, null, null, null, 2, null, null);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var getAllResponse = okObjectResult?.Value as GetAllResponse;

            Assert.Multiple(() =>
            {
                Assert.That(getAllResponse, Is.Not.Null);
                Assert.That(getAllResponse!.NextUrl, Is.Null);
                Assert.That(getAllResponse!.TotalRecords, Is.EqualTo(2));
                Assert.That(getAllResponse!.Objects.Count(), Is.EqualTo(2));
            });
        }


        [Test]
        public void GetAll_Paging_ReturnsPagedObjects()
        {
            int pageSize = 1;
            int currentPage = 0;
            string scheme = "https";
            string host = "test.com";
            string path = "/rest/all";
            string queryString = $"?page={currentPage}&pageSize={pageSize}";
            controller!.ControllerContext = new ControllerContext(ConfigureActionContext(host, scheme, path, "GET", queryString));

            var actionResult = controller.GetAll(UserInfo.OBJECT_TYPE, null, null, null, null, pageSize, currentPage);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var getAllResponse = okObjectResult?.Value as GetAllResponse;
            var expectedNextUrl = new UriBuilder
            {
                Host = host,
                Scheme = scheme,
                Port = 443,
                Path = path,
                Query = $"?page={currentPage + 1}&pageSize={pageSize}"
            };

            Assert.Multiple(() =>
            {
                Assert.That(getAllResponse, Is.Not.Null);
                Assert.That(getAllResponse!.NextUrl, Is.Not.Null);
                Assert.That(getAllResponse!.NextUrl, Is.EqualTo(expectedNextUrl.ToString()));
                Assert.That(getAllResponse!.TotalRecords, Is.EqualTo(4));
                Assert.That(getAllResponse!.Objects.Count(), Is.EqualTo(1));
            });
        }


        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetById_ReturnsSingleObject(int id)
        {
            var okObjectResult = controller!.GetById(UserInfo.OBJECT_TYPE, id) as OkObjectResult;
            dynamic? returnedObject = okObjectResult?.Value;

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject, Is.Not.Null);
                Assert.That(returnedObject!.UserID, Is.EqualTo(id));
            });
        }


        [TestCase("00000000-0000-0000-0000-000000000001")]
        [TestCase("00000000-0000-0000-0000-000000000002")]
        [TestCase("00000000-0000-0000-0000-000000000003")]
        public void GetByGuid_ReturnsSingleObject(string guid)
        {
            var okObjectResult = controller!.GetByGuid(UserInfo.OBJECT_TYPE, Guid.Parse(guid)) as OkObjectResult;
            dynamic? returnedObject = okObjectResult?.Value;

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject, Is.Not.Null);
                Assert.That(returnedObject!.UserGUID, Is.EqualTo(Guid.Parse(guid)));
            });
        }


        [TestCase("A")]
        [TestCase("B")]
        [TestCase("C")]
        public void GetByCodeName_ReturnsSingleObject(string codeName)
        {
            var okObjectResult = controller!.GetByCodeName(UserInfo.OBJECT_TYPE, codeName) as OkObjectResult;
            dynamic? returnedObject = okObjectResult?.Value;

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject, Is.Not.Null);
                Assert.That(returnedObject!.UserName, Is.EqualTo(codeName));
            });
        }


        [Test]
        public void Post_CreatesObject()
        {
            string newUserName = "UserD";
            var createBody = new CreateRequestBody
            {
                ObjectType = UserInfo.OBJECT_TYPE,
                Fields =
                {
                    { nameof(UserInfo.UserName), newUserName },
                }
            };
            var okObjectResult = controller!.Post(createBody) as OkObjectResult;
            dynamic? returnedObject = okObjectResult?.Value;
            var allUsers = UserInfo.Provider.Get();

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject, Is.Not.Null);
                Assert.That(returnedObject!.UserName, Is.EqualTo(newUserName));
                Assert.That(allUsers, Has.Count.EqualTo(5));
            });
        }


        [Test]
        public void Patch_UpdatesObject()
        {
            int targetId = 1;
            string newUserName = "UPDATEDNAME";
            var updateBody = new UpdateRequestBody
            {
                Id = targetId,
                ObjectType = UserInfo.OBJECT_TYPE,
                Fields =
                {
                    { nameof(UserInfo.UserName), newUserName },
                }
            };
            var okObjectResult = controller!.Patch(updateBody) as OkObjectResult;
            dynamic? returnedObject = okObjectResult?.Value;
            var userFromProvider = UserInfo.Provider.Get(targetId);

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject, Is.Not.Null);
                Assert.That(returnedObject!.UserName, Is.EqualTo(newUserName));
                Assert.That(userFromProvider.UserName, Is.EqualTo(newUserName));
            });
        }


        private static ActionContext ConfigureActionContext(string host, string scheme, string path, string method, string queryString)
        {
            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Method.Returns(method);
            httpRequest.Path.Returns(new PathString(path));
            httpRequest.Scheme.Returns(scheme);
            httpRequest.Host.Returns(new HostString(host));
            httpRequest.QueryString.Returns(new QueryString(queryString));

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Returns(httpRequest);

            return new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());
        }
    }
}
