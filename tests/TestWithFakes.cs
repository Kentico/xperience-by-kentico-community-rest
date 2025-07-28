using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Membership;
using CMS.Tests;

namespace Xperience.Community.Rest
{
    internal class TestWithFakes : UnitTests
    {
        protected const string FORM_NAME = "bizform.contactus";
        protected string[] OBJECT_TYPES = ["cms.user", "om.contact"];


        [SetUp]
        public void TestWithFakes_SetUp()
        {
            Fake<ContactInfo, ContactInfoProvider>();

            Fake<DataClassInfo, DataClassInfoProvider>().WithData(
                DataClassInfo.New(c =>
                {
                    c.ClassName = "cms.user";
                    c.ClassType = ClassType.SYSTEM_TABLE;
                }),
                DataClassInfo.New(c =>
                {
                    c.ClassName = "om.contact";
                    c.ClassType = ClassType.OTHER;
                }),
                DataClassInfo.New(c =>
                {
                    c.ClassName = "bizform.contactus";
                    c.ClassType = ClassType.FORM;
                }));


            Fake<UserInfo, UserInfoProvider>().WithData(
                new()
                {
                    UserID = 2,
                    UserName = "B",
                    FirstName = "UserB",
                    UserGUID = Guid.Parse("00000000-0000-0000-0000-000000000002")
                },
                new()
                {
                    UserID = 1,
                    UserName = "A",
                    FirstName = "UserA",
                    UserGUID = Guid.Parse("00000000-0000-0000-0000-000000000001")
                },
                new()
                {
                    UserID = 3,
                    UserName = "C",
                    FirstName = "UserC",
                    UserGUID = Guid.Parse("00000000-0000-0000-0000-000000000003")
                },
                new()
                {
                    UserID = 66,
                    UserName = "public"
                });
        }
    }
}
