using CMS.Membership;

using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    /// <summary>
    /// Tests for <see cref="ObjectRetriever"/>.
    /// </summary>
    internal class ObjectRetrieverTests : TestWithFakes
    {
        private readonly ObjectRetriever retriever = new();
        private readonly Guid userGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");


        [Test]
        public void GetAll_GetsAllObjects()
        {
            var objects = retriever.GetAll(new() { ObjectType = UserInfo.OBJECT_TYPE }, out int total);

            Assert.Multiple(() =>
            {
                Assert.That(total, Is.EqualTo(4));
                Assert.That(objects.Count(), Is.EqualTo(4));
            });
        }


        [Test]
        public void GetById_GetsInfoObject()
        {
            var infoObject = retriever.GetById(UserInfo.OBJECT_TYPE, 66);

            Assert.That(infoObject?.GetIntegerValue(nameof(UserInfo.UserID), 0), Is.EqualTo(66));
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
            var infoObject = retriever.GetByCodeName(UserInfo.OBJECT_TYPE, "public");

            Assert.That(infoObject?.GetStringValue(nameof(UserInfo.UserName), string.Empty), Is.EqualTo("public"));
        }


        [Test]
        public void GetExistingObject_GetsInfoObject()
        {
            var infoObjectById = retriever.GetExistingObject(new UpdateRequestBody
            {
                Id = 66,
                ObjectType = UserInfo.OBJECT_TYPE
            });
            var infoObjectByGuid = retriever.GetExistingObject(new UpdateRequestBody
            {
                Guid = userGuid,
                ObjectType = UserInfo.OBJECT_TYPE
            });
            var infoObjectByName = retriever.GetExistingObject(new UpdateRequestBody
            {
                CodeName = "B",
                ObjectType = UserInfo.OBJECT_TYPE
            });

            Assert.Multiple(() =>
            {
                Assert.That(infoObjectById?.GetIntegerValue(nameof(UserInfo.UserID), 0), Is.EqualTo(66));
                Assert.That(infoObjectByGuid?.GetGuidValue(nameof(UserInfo.UserGUID), Guid.Empty), Is.EqualTo(userGuid));
                Assert.That(infoObjectByName?.GetStringValue(nameof(UserInfo.UserName), string.Empty), Is.EqualTo("B"));
            });
        }


        [Test]
        public void GetNewObject_GetsObject()
        {
            var obj = retriever.GetNewObject(UserInfo.OBJECT_TYPE);

            Assert.Multiple(() =>
            {
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj.TypeInfo.ObjectType, Is.EqualTo(UserInfo.OBJECT_TYPE));
                Assert.That(obj.GetIntegerValue(nameof(UserInfo.UserID), 0), Is.EqualTo(0));
            });
        }
    }
}
