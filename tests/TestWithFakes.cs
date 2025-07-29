using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.FormEngine;
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
                    c.ClassFormDefinition = GetUserFormDefinition();
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


        private static string GetUserFormDefinition()
        {
            var formInfo = FormHelper.GetBasicFormDefinition(nameof(UserInfo.UserID));
            AddFields(formInfo);

            return formInfo.GetXmlDefinition();
        }


        private static void AddFields(FormInfo formInfo)
        {
            var userName = new FormFieldInfo()
            {
                Name = nameof(UserInfo.UserName),
                DataType = FieldDataType.Text,
                PrimaryKey = false,
                System = false,
                Visible = true,
                IsUnique = true,
            };
            var firstName = new FormFieldInfo()
            {
                Name = nameof(UserInfo.FirstName),
                DataType = FieldDataType.Text,
                PrimaryKey = false,
                System = false,
                Visible = true,
            };
            var userGuid = new FormFieldInfo()
            {
                Name = nameof(UserInfo.UserGUID),
                DataType = FieldDataType.Guid,
                PrimaryKey = false,
                System = false,
                Visible = true,
                IsUnique = true,
            };

            formInfo.AddFormItem(userName);
            formInfo.AddFormItem(firstName);
            formInfo.AddFormItem(userGuid);
        }
    }
}
