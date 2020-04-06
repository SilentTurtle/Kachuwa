using System;

namespace Kachuwa.Data.Crud
{
    public interface ITableNameResolver
    {
        string ResolveTableName(Type type);
    }
}