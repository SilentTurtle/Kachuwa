using System;

namespace Kachuwa.Data.Crud.Attribute
{
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = true)]

    public class JoinAttribute : System.Attribute
    {
        public Type TableName { get; set; }
        public string At { get; set; }
        public JoinType JoinType { get; set; } 
    }

    public enum JoinType
    {
        InnerJoin,CrossJoin,LeftJoin,RightJoin,LeftOuterJoin,RightOuterJoin
    }
    public class GetFromAttribute : System.Attribute
    {
        public Type TableName { get; set; }
    }
    /// <summary>
    /// Optional Table attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify the table name of a poco
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : System.Attribute
    {
        /// <summary>
        /// Optional Table attribute.
        /// </summary>
        /// <param name="tableName"></param>
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }
        /// <summary>
        /// Name of the table
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Name of the schema
        /// </summary>
        public string Schema { get; set; }
    }
}