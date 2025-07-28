using CMS;
using CMS.Core;
using CMS.DataEngine;

using Xperience.Community.Rest.Services;

[assembly: RegisterImplementation(typeof(ITypeRetriever), typeof(TypeRetriever))]
namespace Xperience.Community.Rest.Services
{
    public class TypeRetriever(ISettingsService settingsService) : ITypeRetriever
    {
        public IEnumerable<string> GetAllowedForms() => GetAllowedDataClassesOfTypes(ClassType.FORM);


        public IEnumerable<string> GetAllowedObjects() => GetAllowedDataClassesOfTypes(ClassType.OTHER, ClassType.SYSTEM_TABLE);


        private IEnumerable<string> GetAllowedDataClassesOfTypes(params string[] types)
        {
            var classesOfType = DataClassInfoProvider.GetClasses()
                .WhereIn(nameof(DataClassInfo.ClassType), types)
                .AsSingleColumn(nameof(DataClassInfo.ClassName))
                .GetListResult<string>()
                .Select(t => t.ToLower());

            string? allowedTypes = settingsService[Constants.SETTINGS_KEY_ALLOWEDTYPES];
            if (string.IsNullOrEmpty(allowedTypes))
            {
                return classesOfType;
            }

            IEnumerable<string> allowedTypeArray = allowedTypes.ToLower().Split(';');

            return classesOfType.Intersect(allowedTypeArray);
        }
    }
}
