namespace Kachuwa.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class MySQLTemplate : ISQLTemplate
    {

        public string Select => $"Select {0}from {1}";
        public string IdentitySql => $"SELECT LAST_INSERT_ID() AS id";
        // public string PaginatedSql=>"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
        public string PaginatedSql
            =>
                "Select  {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}"
            ;
        public string Encapsulation => "`{0}`";



    }
}