using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kachuwa.IdentityServerAdmin.Infrastructure
{
    /// <summary>
    /// 基类型<see cref="object"/>扩展辅助操作类
    /// </summary>
    public static class ObjectExtensions
    {
        #region 公共方法

        /// <summary>
        /// 把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
            {
                return null;
            }

            if (conversionType.IsNullableType())
            {
                conversionType = conversionType.GetUnNullableType();
            }

            if (conversionType.IsEnum)
            {
                return Enum.Parse(conversionType, value.ToString());
            }

            if (conversionType == typeof(Guid))
            {
                return Guid.Parse(value.ToString());
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T CastTo<T>(this object value)
        {
            if (value == null)
            {
                return default;
            }

            if (value.GetType() == typeof(T))
            {
                return (T) value;
            }

            object result = CastTo(value, typeof(T));
            return (T) result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return CastTo<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 判断当前值是否介于指定范围内
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 动态类型对象 </param>
        /// <param name="start"> 范围起点 </param>
        /// <param name="end"> 范围终点 </param>
        /// <param name="leftEqual"> 是否可等于上限（默认等于） </param>
        /// <param name="rightEqual"> 是否可等于下限（默认等于） </param>
        /// <returns> 是否介于 </returns>
        public static bool IsBetween<T>(this IComparable<T> value, T start, T end, bool leftEqual = true,
            bool rightEqual = true) where T : IComparable
        {
            bool flag = leftEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            return flag && (rightEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0);
        }

        /// <summary>
        /// 判断当前值是否介于指定范围内
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 动态类型对象 </param>
        /// <param name="min">范围小值</param>
        /// <param name="max">范围大值</param>
        /// <param name="minEqual">是否可等于小值（默认等于）</param>
        /// <param name="maxEqual">是否可等于大值（默认等于）</param>
        public static bool IsInRange<T>(this IComparable<T> value, T min, T max, bool minEqual = true,
            bool maxEqual = true) where T : IComparable
        {
            bool flag = minEqual ? value.CompareTo(min) >= 0 : value.CompareTo(min) > 0;
            return flag && (maxEqual ? value.CompareTo(max) <= 0 : value.CompareTo(max) < 0);
        }

        /// <summary>
        /// 是否存在于
        /// </summary>
        public static bool IsIn<T>(this T value, params T[] source)
        {
            return source.Contains(value);
        }

        /// <summary>
        /// 将对象[主要是匿名对象]转换为dynamic
        /// </summary>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            Type type = value.GetType();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var val = property.GetValue(value);
                if (property.PropertyType.FullName != null &&
                    property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
                {
                    dynamic dval = val.ToDynamic();
                    expando.Add(property.Name, dval);
                }
                else
                {
                    expando.Add(property.Name, val);
                }
            }

            return expando as ExpandoObject;
        }

        /// <summary>
        /// 对象深度拷贝，复制出一个数据一样，但地址不一样的新版本
        /// </summary>
        public static T DeepClone<T>(this T obj) where T : class
        {
            if (obj == null)
            {
                return default;
            }

            if (typeof(T).HasAttribute<SerializableAttribute>())
            {
                throw new NotSupportedException(
                    string.Format("当前对象未标记特性“{0}”，无法进行DeepClone操作", typeof(SerializableAttribute)));
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                ms.Seek(0L, SeekOrigin.Begin);
                return (T) formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T) obj;
        }

        #endregion
    }
}