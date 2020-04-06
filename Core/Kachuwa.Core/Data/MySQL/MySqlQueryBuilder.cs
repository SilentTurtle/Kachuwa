using Kachuwa.Data.Crud;
namespace Kachuwa.Data
{
    public sealed class MySqlQueryBuilder : QueryBuilder
    {       
        public MySqlQueryBuilder(ISQLTemplate template, ITableNameResolver tblresolver, IColumnNameResolver colresolver) 
          : base(template, tblresolver, colresolver)
        {
        }
        public MySqlQueryBuilder(ISQLTemplate template)
         : base(template)
        {
        }
    }
}