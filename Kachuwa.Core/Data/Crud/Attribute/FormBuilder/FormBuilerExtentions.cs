using System;
using System.Reflection;

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
                    System.TypeCode typeCode = System.Type.GetTypeCode(prop.PropertyType);
                    if (customAtt.GetCurrentUser)
                    {

                        //TODO:: fetch username
                        // customAtt.DefaultValue = UserHelper.UserName;

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