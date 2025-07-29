using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;

using Xperience.Community.Rest.Models;
using Xperience.Community.Rest.Services;

[assembly: RegisterImplementation(typeof(ITypeRetriever), typeof(TypeRetriever))]
namespace Xperience.Community.Rest.Services
{
    public class TypeRetriever(ISettingsService settingsService, IObjectRetriever objectRetriever) : ITypeRetriever
    {
        public IEnumerable<string> GetAllowedForms() => GetAllowedDataClassesOfTypes(ClassType.FORM);


        public IEnumerable<string> GetAllowedObjects() => GetAllowedDataClassesOfTypes(ClassType.OTHER, ClassType.SYSTEM_TABLE);


        public ObjectMeta GetMetadata(string objectType)
        {
            var baseInfo = objectRetriever.GetNewObject(objectType);
            var dci = DataClassInfoProvider.GetDataClassInfo(objectType, true);
            string? idColumn = baseInfo.TypeInfo.IDColumn
                .Equals(ObjectTypeInfo.COLUMN_NAME_UNKNOWN, StringComparison.InvariantCultureIgnoreCase)
                    ? null : baseInfo.TypeInfo.IDColumn;
            string? guidColumn = baseInfo.TypeInfo.GUIDColumn
                .Equals(ObjectTypeInfo.COLUMN_NAME_UNKNOWN, StringComparison.InvariantCultureIgnoreCase)
                    ? null : baseInfo.TypeInfo.GUIDColumn;
            string? codeNameColumn = baseInfo.TypeInfo.CodeNameColumn
                .Equals(ObjectTypeInfo.COLUMN_NAME_UNKNOWN, StringComparison.InvariantCultureIgnoreCase)
                    ? null : baseInfo.TypeInfo.CodeNameColumn;
            var objectMeta = new ObjectMeta(objectType)
            {
                ClassType = dci.ClassType,
                DisplayName = dci.ClassDisplayName,
                IdColumn = idColumn,
                CodeNameColumn = codeNameColumn,
                GuidColumn = guidColumn,
            };

            var formInfo = new FormInfo(dci.ClassFormDefinition);
            var fields = formInfo.GetFields(true, true);
            objectMeta.Fields = fields.Select(f =>
                new FieldMeta(f.Name)
                {
                    Caption = f.Caption,
                    IsRequired = !f.AllowEmpty,
                    DataType = f.DataType,
                    Size = f.Size,
                    DefaultValue = f.DefaultValue,
                    IsUnique = f.IsUnique,
                }
            );

            return objectMeta;
        }


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
