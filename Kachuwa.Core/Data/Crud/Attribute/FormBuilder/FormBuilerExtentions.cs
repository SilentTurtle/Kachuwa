using System;
using System.Reflection;
using Kachuwa.Web;
using Microsoft.AspNetCore.Localization;

namespace Kachuwa.Data.Crud.FormBuilder
{
    public static class FormBuilerExtentions
    {
        public static void AutoFill(this object obj)
        {
            if (!obj.GetType().GetTypeInfo().IsClass)
                return;
            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {

                var customAtt = prop.GetCustomAttribute<AutoFillAttribute>();
                if (customAtt != null)
                {
                    if (customAtt.fillBy != null)
                    {


                        switch (customAtt.fillBy)
                        {
                            case AutoFillProperty.CurrentCulture:
                                var rqf = ContextResolver.Context.Features.Get<IRequestCultureFeature>();
                                // Culture contains the information of the requested culture
                                var culture = rqf.RequestCulture.Culture.ToString();
                                customAtt.DefaultValue = culture;
                                break;
                            case AutoFillProperty.CurrentUser:
                                customAtt.DefaultValue = "Admin";// ContextResolver.Context.User.Identity.Name;
                                break;
                            case AutoFillProperty.CurrentDate:
                                customAtt.DefaultValue = DateTime.Now;

                                break;

                        }
                      
                    }

                    System.TypeCode typeCode = System.Type.GetTypeCode(prop.PropertyType);
                    if (customAtt.GetCurrentUser)
                    {

                        //TODO:: fetch username
                        // customAtt.DefaultValue = UserHelper.UserName;
                        customAtt.DefaultValue = "Admin";// ContextResolver.Context.User.Identity.Name;
                    }
                    else
                        customAtt.DefaultValue = customAtt.IsDate ? DateTime.Now : customAtt.DefaultValue;



                    var typedDefValue = Convert.ChangeType(customAtt.DefaultValue, prop.PropertyType);
                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(obj, typedDefValue, null);
                    }
                }
            }
        }
    }
}