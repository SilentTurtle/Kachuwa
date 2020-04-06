using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Web;
using Microsoft.AspNetCore.Localization;

namespace Kachuwa.Data.Extension
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
                    if (customAtt.HasFixedValue)
                    {

                        var typedDefValue = Convert.ChangeType(customAtt.DefaultValue, prop.PropertyType);
                        if (null != prop && prop.CanWrite)
                        {
                            prop.SetValue(obj, typedDefValue, null);
                        }


                    }
                    else
                    {
                        switch (customAtt.fillBy)
                        {
                            case AutoFillProperty.CurrentCulture:
                                if (ContextResolver.Context == null) {customAtt.DefaultValue="en-US"; break;}
                                var rqf = ContextResolver.Context?.Features.Get<IRequestCultureFeature>();
                                // Culture contains the information of the requested culture
                                var culture = rqf.RequestCulture.Culture.ToString();
                                customAtt.DefaultValue = culture;
                                break;
                            case AutoFillProperty.CurrentUser:
                                var claim=
                                    ContextResolver.Context.User.Claims.SingleOrDefault(c => c.Type == "name");
                                customAtt.DefaultValue = claim == null ? "" : claim.Value;


                                break;
                            case AutoFillProperty.CurrentDate:
                                customAtt.DefaultValue = DateTime.Now;

                                break;
                            case AutoFillProperty.CurrentUtcDate:
                                customAtt.DefaultValue = DateTime.UtcNow;
                                break;
                            //case AutoFillProperty.TheaterId:
                            //    var theaterId = ContextResolver.Context.User.Claims.SingleOrDefault(c => c.Type == "tid");
                            //    customAtt.DefaultValue = theaterId == null ? 0 : Convert.ToInt32(theaterId.Value.ToString());
                            //    break;

                        }
                        var typedDefValue = Convert.ChangeType(customAtt.DefaultValue, prop.PropertyType);
                        if (null != prop && prop.CanWrite)
                        {
                            prop.SetValue(obj, typedDefValue, null);
                        }
                    }

                    //System.TypeCode typeCode = System.Type.GetTypeCode(prop.PropertyType);
                    //if (customAtt.GetCurrentUser)
                    //{

                    //    //TODO:: fetch username
                    //    // customAtt.DefaultValue = UserHelper.UserName;
                    //    customAtt.DefaultValue = "Admin";// ContextResolver.Context.User.Identity.Name;
                    //}
                    //else
                    //    customAtt.DefaultValue = customAtt.IsDate ? DateTime.Now : customAtt.DefaultValue;



                 
                }
            }
        }
    }
}