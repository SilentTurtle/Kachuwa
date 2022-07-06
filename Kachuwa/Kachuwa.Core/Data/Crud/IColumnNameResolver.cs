using System.Reflection;

namespace Kachuwa.Data.Crud
{
    public interface IColumnNameResolver
    {
        string ResolveColumnName(PropertyInfo propertyInfo);
    }
}