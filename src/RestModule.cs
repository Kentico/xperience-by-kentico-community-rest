using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Modules;

using Microsoft.Extensions.DependencyInjection;

using Xperience.Community.Rest;

[assembly: RegisterModule(typeof(RestModule))]
namespace Xperience.Community.Rest
{
    /// <summary>
    /// An Xperience by Kentico <see cref="Module"/> which installs the REST module and related objects at startup.
    /// </summary>
    public class RestModule : Module
    {
        public RestModule() : base(nameof(RestModule))
        {
        }


        protected override void OnInit(ModuleInitParameters parameters)
        {
            base.OnInit();
            ApplicationEvents.Initialized.Execute += (s, e) => Install(parameters);
        }


        private static void Install(ModuleInitParameters parameters)
        {
            var resource = InitializeResource(parameters);
            var category = InstallSettingsCategories(parameters, resource);
            InstallSettingsKeys(parameters, category);
        }


        private static ResourceInfo InitializeResource(ModuleInitParameters parameters)
        {
            var resourceProvider = parameters.Services.GetRequiredService<IInfoProvider<ResourceInfo>>();
            var resource = resourceProvider.Get(Constants.RESOURCE_NAME) ?? new ResourceInfo();

            resource.ResourceDisplayName = "Xperience Community- REST";
            resource.ResourceName = Constants.RESOURCE_NAME;
            resource.ResourceDescription = "REST module";
            resource.ResourceIsInDevelopment = false;
            if (resource.HasChanged)
            {
                resourceProvider.Set(resource);
            }

            return resource;
        }


        private static SettingsCategoryInfo InstallSettingsCategories(ModuleInitParameters parameters, ResourceInfo resource)
        {
            var keyCategoryProvider = parameters.Services.GetRequiredService<IInfoProvider<SettingsCategoryInfo>>();
            var parentCategory = keyCategoryProvider.Get("CMS.Settings") ??
                throw new InvalidOperationException("Installation failed: CMS.Settings category not found");

            // Root node in tree view
            var rootCategory = keyCategoryProvider.Get(Constants.SETTINGS_ROOTCATEGORY_NAME) ?? new SettingsCategoryInfo();
            rootCategory.CategoryName = Constants.SETTINGS_ROOTCATEGORY_NAME;
            rootCategory.CategoryDisplayName = Constants.SETTINGS_ROOTCATEGORY_NAME;
            rootCategory.CategoryParentID = parentCategory.CategoryID;
            rootCategory.CategoryResourceID = resource.ResourceID;
            rootCategory.CategoryOrder = 999;
            rootCategory.CategoryIsGroup = false;
            if (rootCategory.HasChanged)
            {
                keyCategoryProvider.Set(rootCategory);
            }

            // "General" subcategory
            var generalCategory = keyCategoryProvider.Get(Constants.SETTINGS_GENERALCATEGORY_NAME) ?? new SettingsCategoryInfo();
            generalCategory.CategoryName = Constants.SETTINGS_GENERALCATEGORY_NAME;
            generalCategory.CategoryDisplayName = Constants.SETTINGS_GENERALCATEGORY_NAME;
            generalCategory.CategoryParentID = rootCategory.CategoryID;
            generalCategory.CategoryResourceID = resource.ResourceID;
            generalCategory.CategoryOrder = 1;
            generalCategory.CategoryIsGroup = true;
            if (generalCategory.HasChanged)
            {
                keyCategoryProvider.Set(generalCategory);
            }

            return generalCategory;
        }


        private static void InstallSettingsKeys(ModuleInitParameters parameters, SettingsCategoryInfo category)
        {
            var keyProvider = parameters.Services.GetRequiredService<IInfoProvider<SettingsKeyInfo>>();

            var enabledKey = keyProvider.Get(Constants.SETTINGS_KEY_ENABLED) ?? new SettingsKeyInfo();
            enabledKey.KeyName = Constants.SETTINGS_KEY_ENABLED;
            enabledKey.KeyDisplayName = "Enabled";
            enabledKey.KeyCategoryID = category.CategoryID;
            enabledKey.KeyType = nameof(Boolean);
            enabledKey.KeyOrder = 1;
            if (enabledKey.HasChanged)
            {
                keyProvider.Set(enabledKey);
            }

            var allowedTypesKey = keyProvider.Get(Constants.SETTINGS_KEY_ALLOWEDTYPES) ?? new SettingsKeyInfo();
            allowedTypesKey.KeyName = Constants.SETTINGS_KEY_ALLOWEDTYPES;
            allowedTypesKey.KeyDisplayName = "Allowed object types";
            allowedTypesKey.KeyDescription = "A list of object types than can be retrieved and manipulated by the REST service, separated "
                + "by semicolons. If empty, all object types are allowed";
            allowedTypesKey.KeyCategoryID = category.CategoryID;
            allowedTypesKey.KeyType = nameof(String);
            allowedTypesKey.KeyOrder = 2;
            if (allowedTypesKey.HasChanged)
            {
                keyProvider.Set(allowedTypesKey);
            }
        }
    }
}
