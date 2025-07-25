using CMS.Membership;
using CMS.Tests;

using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Tests for <see cref="ObjectRetriever"/>.
    /// </summary>
    internal class ObjectRetrieverTests : UnitTests
    {
        private const int USER_ID = 99;
        private const string USER_NAME = "tester";
        private readonly Guid userGuid = Guid.NewGuid();
        private readonly ObjectRetriever retriever = new();


        [SetUp]
        public void SetUp() => Fake<UserInfo, UserInfoProvider>().WithData(
            new()
            {
                UserID = USER_ID
            },
            new()
            {
                UserName = USER_NAME
            },
            new()
            {
                UserGUID = userGuid
            });


        [Test]
        public void GetById_GetsInfoObject()
        {
            var infoObject = retriever.GetById(UserInfo.OBJECT_TYPE, USER_ID);

            Assert.That(infoObject?.GetIntegerValue(nameof(UserInfo.UserID), 0), Is.EqualTo(USER_ID));
        }


        [Test]
        public void GetByGuid_GetsInfoObject()
        {
            var infoObject = retriever.GetByGuid(UserInfo.OBJECT_TYPE, userGuid);

            Assert.That(infoObject?.GetGuidValue(nameof(UserInfo.UserGUID), Guid.Empty), Is.EqualTo(userGuid));
        }


        [Test]
        public void GetByCodeName_GetsInfoObject()
        {
            var infoObject = retriever.GetByCodeName(UserInfo.OBJECT_TYPE, USER_NAME);

            Assert.That(infoObject?.GetStringValue(nameof(UserInfo.UserName), string.Empty), Is.EqualTo(USER_NAME));
        }


        [Test]
        public void GetExistingObject_GetsInfoObject()
        {
            var infoObjectById = retriever.GetExistingObject(new UpdateRequestBody
            {
                Id = USER_ID,
                ObjectType = UserInfo.OBJECT_TYPE
            });
            var infoObjectByGuid = retriever.GetExistingObject(new UpdateRequestBody
            {
                Guid = userGuid,
                ObjectType = UserInfo.OBJECT_TYPE
            });
            var infoObjectByName = retriever.GetExistingObject(new UpdateRequestBody
            {
                CodeName = USER_NAME,
                ObjectType = UserInfo.OBJECT_TYPE
            });

            Assert.Multiple(() =>
            {
                Assert.That(infoObjectById?.GetIntegerValue(nameof(UserInfo.UserID), 0), Is.EqualTo(USER_ID));
                Assert.That(infoObjectByGuid?.GetGuidValue(nameof(UserInfo.UserGUID), Guid.Empty), Is.EqualTo(userGuid));
                Assert.That(infoObjectByName?.GetStringValue(nameof(UserInfo.UserName), string.Empty), Is.EqualTo(USER_NAME));
            });
        }
    }
}
