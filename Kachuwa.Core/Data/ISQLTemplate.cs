namespace Kachuwa.Data
{
    public interface ISQLTemplate
    {
        string Select { get; }
        string IdentitySql { get;  }
        string PaginatedSql { get; }
        string Encapsulation { get; }
    }
}