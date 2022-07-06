using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Data.SQLLite
{
   public class SQLLiteFactory
    {
    }
    public class SQLLiteTemplate : ISQLTemplate
    {

        public string Select => $"Select {0} from {1}";
        public string IdentitySql => $"SELECT LAST_INSERT_ROWID() AS id";
        public string PaginatedSql
            => "Select {SelectColumns},(SELECT count(1) FROM {TableName} {WhereClause} ) as RowTotal FROM {TableName} {join} {WhereClause}  " +
                " Order By {OrderBy} limit {RowsPerPage} offset {RowsPerPage} * ({PageNumber}-1);"
        ;
        public string Encapsulation => "\"{0}\"";

    }
}
