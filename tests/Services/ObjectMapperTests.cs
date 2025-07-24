using System.Data;
using System.Text.Json.Nodes;

using CMS.ContactManagement;
using CMS.Tests;

using Xperience.Community.Rest.Models.Requests;

namespace Xperience.Community.Rest.Services
{
    internal class ObjectMapperTests : UnitTests
    {
        private readonly ObjectMapper mapper = new();
        private const string CONTACT_EMAIL = "test@localhost.com";
        private const string CONTACT_FIRSTNAME = "Tester";
        private readonly JsonObject fields = new()
        {
            { nameof(ContactInfo.ContactEmail), CONTACT_EMAIL },
            { nameof(ContactInfo.ContactFirstName), CONTACT_FIRSTNAME },
        };


        [SetUp]
        public void SetUp() => Fake<ContactInfo, ContactInfoProvider>();


        [Test]
        public void MapFieldsFromRequest_SetsObjectValues()
        {
            var infoObject = new ContactInfo();
            var requestBody = new CreateRequestBody
            {
                ObjectType = "om.contact",
                Fields = fields
            };

            mapper.MapFieldsFromRequest(infoObject, requestBody);

            Assert.Multiple(() =>
            {
                Assert.That(infoObject.GetStringValue(nameof(ContactInfo.ContactLastName), string.Empty), Is.Empty);
                Assert.That(infoObject.GetStringValue(nameof(ContactInfo.ContactEmail), string.Empty), Is.EqualTo(CONTACT_EMAIL));
                Assert.That(infoObject.GetStringValue(nameof(ContactInfo.ContactFirstName), string.Empty), Is.EqualTo(CONTACT_FIRSTNAME));
            });
        }


        [Test]
        public void MapToSimpleObject_BaseInfo_SetsDynamicFields()
        {
            var infoObject = new ContactInfo()
            {
                ContactEmail = CONTACT_EMAIL,
                ContactFirstName = CONTACT_FIRSTNAME,
            };

            var returnedObject = mapper.MapToSimpleObject(infoObject);

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject.ContactLastName, Is.Null);
                Assert.That(returnedObject.ContactEmail, Is.EqualTo(CONTACT_EMAIL));
                Assert.That(returnedObject.ContactFirstName, Is.EqualTo(CONTACT_FIRSTNAME));
            });
        }


        [Test]
        public void MapToSimpleObject_DataRow_SetsDynamicFields()
        {
            var contactTable = new DataTable();
            contactTable.Columns.Add(nameof(ContactInfo.ContactEmail));
            contactTable.Columns.Add(nameof(ContactInfo.ContactFirstName));
            contactTable.Columns.Add(nameof(ContactInfo.ContactLastName));
            var contactRow = contactTable.NewRow();
            contactRow[nameof(ContactInfo.ContactEmail)] = CONTACT_EMAIL;
            contactRow[nameof(ContactInfo.ContactFirstName)] = CONTACT_FIRSTNAME;

            var returnedObject = mapper.MapToSimpleObject(contactRow);

            Assert.Multiple(() =>
            {
                Assert.That(returnedObject.ContactLastName, Is.Null);
                Assert.That(returnedObject.ContactEmail, Is.EqualTo(CONTACT_EMAIL));
                Assert.That(returnedObject.ContactFirstName, Is.EqualTo(CONTACT_FIRSTNAME));
            });
        }
    }
}
