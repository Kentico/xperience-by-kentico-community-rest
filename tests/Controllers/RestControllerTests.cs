using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using CMS.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using NSubstitute;

using Xperience.Community.Rest.Models.Responses;
using Xperience.Community.Rest.Services;

namespace Xperience.Community.Rest.Controllers
{
    internal class RestControllerTests() : UnitTests
    {
        private RestController? controller;
        private readonly ISettingsService settings = Substitute.For<ISettingsService>();


        private static IEnumerable<string> RegisteredTypes => ObjectTypeManager.RegisteredTypes.Select(t => t.ObjectType.ToLower());


        [SetUp]
        public void SetUp()
        {
            settings[Constants.SETTINGS_KEY_ENABLED].Returns("true");
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns(string.Empty);

            controller = new RestController(new ObjectRetriever(), new ObjectMapper(), settings);

            Fake<UserInfo, UserInfoProvider>().WithData(
                new()
                {
                    UserID = 2,
                    UserName = "B",
                    FirstName = "UserB",
                },
                new()
                {
                    UserID = 1,
                    UserName = "A",
                    FirstName = "UserA",
                },
                new()
                {
                    UserID = 3,
                    UserName = "C",
                    FirstName = "UserC",
                });
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
                Assert.That(indexResponse!.EnabledObjectTypes, Is.EqualTo(RegisteredTypes));
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
                Assert.That(indexResponse!.EnabledObjectTypes.Count(), Is.EqualTo(2));
                Assert.That(indexResponse!.EnabledObjectTypes, Does.Contain("cms.user"));
                Assert.That(indexResponse!.EnabledObjectTypes, Does.Contain("om.contact"));
                Assert.That(indexResponse!.EnabledObjectTypes, Does.Not.Contain("invalid.type"));
            });
        }


        [Test]
        public void Get_ServiceDisabled_Throws()
        {
            settings[Constants.SETTINGS_KEY_ENABLED].Returns("false");

            Assert.Throws<InvalidOperationException>(() => controller?.Get(UserInfo.OBJECT_TYPE, 1), "REST service disabled.");
        }


        [Test]
        public void Get_All_ReturnsAllObjects()
        {
            var actionResult = controller!.Get(UserInfo.OBJECT_TYPE, null, null, null, null, null, null);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var getAllResponse = okObjectResult?.Value as GetAllResponse;

            Assert.Multiple(() =>
            {
                Assert.That(getAllResponse, Is.Not.Null);
                Assert.That(getAllResponse!.NextUrl, Is.Null);
                Assert.That(getAllResponse!.TotalRecords, Is.EqualTo(3));
                Assert.That(getAllResponse!.Objects.Count(), Is.EqualTo(3));
            });
        }


        [Test]
        public void Get_All_WhereAndOrder_ReturnsFilteredObjects()
        {
            string where = $"{nameof(UserInfo.FirstName)} IN ('UserA', 'UserB')";
            string orderBy = $"{nameof(UserInfo.FirstName)} asc";
            // Note: Columns parameter cannot be tested as it requires a database connection
            var actionResult = controller!.Get(UserInfo.OBJECT_TYPE, where, null, orderBy, null, null, null);
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
        public void Get_All_TopN_ReturnsFilteredObjects()
        {
            var actionResult = controller!.Get(UserInfo.OBJECT_TYPE, null, null, null, 2, null, null);
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
        public void Get_All_Paging_ReturnsPagedObjects()
        {
            int pageSize = 1;
            int currentPage = 0;
            string scheme = "https";
            string host = "test.com";
            string path = "/rest/all";
            string queryString = $"?page={currentPage}&pageSize={pageSize}";
            controller!.ControllerContext = new ControllerContext(ConfigureActionContext(host, scheme, path, "GET", queryString));

            var actionResult = controller.Get(UserInfo.OBJECT_TYPE, null, null, null, null, pageSize, currentPage);
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
                Assert.That(getAllResponse!.TotalRecords, Is.EqualTo(3));
                Assert.That(getAllResponse!.Objects.Count(), Is.EqualTo(1));
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
