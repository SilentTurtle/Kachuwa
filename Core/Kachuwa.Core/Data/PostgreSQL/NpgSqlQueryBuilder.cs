using Kachuwa.Data.Crud;

namespace Kachuwa.Data
{
    public sealed class NpgSqlQueryBuilder : QueryBuilder
    {
        public NpgSqlQueryBuilder(ISQLTemplate template, ITableNameResolver tblresolver, IColumnNameResolver colresolver)
            : base(template, tblresolver, colresolver)
        {
        }
        public NpgSqlQueryBuilder(ISQLTemplate template)
            : base(template)
        {
        }
    }
}