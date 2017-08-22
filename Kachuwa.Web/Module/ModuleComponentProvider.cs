using System;
using System.Collections.Generic;
using System.Reflection;
using Kachuwa.Web.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Module
{
    public class ModuleComponentDescription : IModuleComponentDescription
    {
        public string DisplayName { get; set; }
        public ViewComponentDescriptor ComponentDescriptor { get; set; }
        public bool IsVisibleOnUI { get; set; } = true;
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public bool HasSetting { get; set; }
        public string ModuleSettingComponent { get; set; }
}
    public class ModuleComponentProvider : IModuleComponentProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IViewComponentDescriptorCollectionProvider _componentDescriptorCollectionProvider;

        public ModuleComponentProvider(IServiceProvider serviceProvider, IHttpContextAccessor contextAccessor,
            IViewComponentDescriptorCollectionProvider componentDescriptorCollectionProvider)
        {
            _serviceProvider = serviceProvider;
            _contextAccessor = contextAccessor;
            _componentDescriptorCollectionProvider = componentDescriptorCollectionProvider;
            _modulesComponent = new Dictionary<string, List<ModuleComponentDescription>>();
            FilterComponents();
        }

        private Dictionary<string, List<ModuleComponentDescription>> _modulesComponent;

        private void FilterComponents()
        {
            foreach (var component in _componentDescriptorCollectionProvider.ViewComponents.Items)
            {
                if (component.TypeInfo.BaseType.GetTypeInfo().IsGenericType == true &&
                    component.TypeInfo.BaseType.GetTypeInfo().GetGenericTypeDefinition() ==
                    typeof(KachuwaModuleViewComponent<>))
                {
                    // var service2 = _contextAccessor.HttpContext.RequestServices.GetService(typeof(SeoViewComponent));
                    var viewComponentInstance =
                        _contextAccessor.HttpContext.RequestServices.GetService(component.TypeInfo.AsType());
                    if (viewComponentInstance != null)
                    {
                        var serviceType = viewComponentInstance.GetType();
                        var module =
                            (IModule)
                            ((FieldInfo) serviceType.GetMember("Module").GetValue(0)).GetValue(viewComponentInstance);
                        var IsVisibleOnUI =
                            (bool) serviceType.GetProperty("IsVisibleOnUI").GetValue(viewComponentInstance);
                        var DisplayName =
                            (string) serviceType.GetProperty("DisplayName").GetValue(viewComponentInstance);
                        var hasSetting =
                           (bool)serviceType.GetProperty("HasSetting").GetValue(viewComponentInstance);
                        var settingComponent =
                          (string)serviceType.GetProperty("ModuleSettingComponent").GetValue(viewComponentInstance);
                        if (_modulesComponent.ContainsKey(module.Name))
                        {
                            _modulesComponent[module.Name].Add(new ModuleComponentDescription()
                            {
                                ComponentDescriptor = component,
                                DisplayName = DisplayName,
                                IsVisibleOnUI = IsVisibleOnUI,
                                ShortName = component.ShortName,
                                FullName = component.FullName,
                                HasSetting= hasSetting,
                                ModuleSettingComponent= settingComponent

                            });
                        }
                        else
                        {
                            _modulesComponent.Add(module.Name, new List<ModuleComponentDescription>()
                            {
                                new ModuleComponentDescription()
                                {
                                    ComponentDescriptor = component,
                                    DisplayName = DisplayName,
                                    IsVisibleOnUI = IsVisibleOnUI,
                                    ShortName = component.ShortName,
                                    FullName = component.FullName
                                }
                            });
                        }
                    }


                }
                else
                {
                    if (_modulesComponent.ContainsKey("Other"))
                    {
                        _modulesComponent["Other"].Add(new ModuleComponentDescription()
                        {
                            ComponentDescriptor = component,
                            DisplayName = component.DisplayName,
                            ShortName = component.ShortName,
                            FullName = component.FullName
                        });
                    }
                    else
                    {
                        _modulesComponent.Add("Other", new List<ModuleComponentDescription>()
                        {
                            new ModuleComponentDescription()
                            {
                                DisplayName = component.DisplayName,
                                ComponentDescriptor = component,
                                ShortName = component.ShortName,
                                FullName = component.FullName
                            }
                        });
                    }

                }
            }
        }

        public IEnumerable<ModuleComponentDescription> GetComponents(string moduleName)
        {
            if (_modulesComponent == null)
                return null;
            return _modulesComponent[moduleName];
        }
        public Dictionary<string, List<ModuleComponentDescription>> GetComponents()
        {
            if (_modulesComponent == null)
                return null;
            return _modulesComponent;
        }


    }
}