namespace Kachuwa.Data
{
    public class NpgSQLTemplate : ISQLTemplate
    {

        public string Select => $"Select {0} from {1}";
        public string IdentitySql => $"SELECT LASTVAL() AS id";
        // public string PaginatedSql=>"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
        public string PaginatedSql
            => "WITH  a AS (select {SelectColumns}, count(1) over (range unbounded preceding) as RowTotal FROM {TableName} {join} {WhereClause} ) "+
                "SELECT * from a Order By {OrderBy} limit {RowsPerPage} offset {RowsPerPage} * ({PageNumber}-1);"
        ;
        public string Encapsulation => " {0} ";//"\"{0}\"";

    }
}