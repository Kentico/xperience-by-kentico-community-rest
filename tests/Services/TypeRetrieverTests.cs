using CMS.ContactManagement;
using CMS.Core;
using CMS.Membership;

using NSubstitute;

namespace Xperience.Community.Rest.Services
{
    internal class TypeRetrieverTests : TestWithFakes
    {
        private ISettingsService settings;
        private TypeRetriever? typeRetriever;


        [SetUp]
        public void SetUp()
        {
            settings = Substitute.For<ISettingsService>();
            typeRetriever = new TypeRetriever(settings);
        }


        [TestCase("")]
        [TestCase(null)]
        public void GetAllowedForms_TypesNotSet_ReturnsRegisteredForm(string? allowedTypes)
        {
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns(allowedTypes);

            var allowedForms = typeRetriever!.GetAllowedForms();

            Assert.Multiple(() =>
            {
                Assert.That(allowedForms.Count(), Is.EqualTo(1));
                Assert.That(allowedForms, Does.Contain(FORM_NAME));
            });
        }


        [Test]
        public void GetAllowedForms_LimitedForms_DoesntReturnRegisteredForm()
        {
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns("other.form");

            var allowedForms = typeRetriever!.GetAllowedForms();

            Assert.Multiple(() =>
            {
                Assert.That(allowedForms.Count(), Is.EqualTo(0));
                Assert.That(allowedForms, Does.Not.Contain(FORM_NAME));
            });
        }


        [TestCase("")]
        [TestCase(null)]
        public void GetAllowedObjects_TypesNotSet_ReturnsRegisteredForm(string? allowedTypes)
        {
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns(allowedTypes);

            var allowedObjects = typeRetriever!.GetAllowedObjects();

            Assert.Multiple(() =>
            {
                Assert.That(allowedObjects.Count(), Is.EqualTo(2));
                Assert.That(allowedObjects, Is.EqualTo(OBJECT_TYPES));
            });
        }


        [Test]
        public void GetAllowedObjects_LimitedObjects_ReturnsAllowedObjects()
        {
            settings[Constants.SETTINGS_KEY_ALLOWEDTYPES].Returns(UserInfo.OBJECT_TYPE);

            var allowedForms = typeRetriever!.GetAllowedObjects();

            Assert.Multiple(() =>
            {
                Assert.That(allowedForms.Count(), Is.EqualTo(1));
                Assert.That(allowedForms, Does.Contain(UserInfo.OBJECT_TYPE));
                Assert.That(allowedForms, Does.Not.Contain(ContactInfo.OBJECT_TYPE));
            });
        }
    }
}
