using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using Kachuwa.Data.Crud;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;
using EditableAttribute = Kachuwa.Data.Crud.Attribute.EditableAttribute;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

//using KeyAttribute = Kachuwa.Data.Crud.Attribute.KeyAttribute;

namespace Kachuwa.Data
{
    public abstract class QueryBuilder
    {
        private ISQLTemplate SqlTemplate { get; set; }
        private readonly IDictionary<Type, string> _tableNames = new Dictionary<Type, string>();
        private readonly IDictionary<string, string> _columnNames = new Dictionary<string, string>();

        private readonly ITableNameResolver _tableNameResolver;
        private readonly IColumnNameResolver _columnNameResolver;
        protected QueryBuilder(ISQLTemplate template, ITableNameResolver tblresolver, IColumnNameResolver colresolver)
        {
            SqlTemplate = template;
            _tableNameResolver = tblresolver;
            _columnNameResolver = colresolver;
        }
        protected QueryBuilder(ISQLTemplate template)
        {
            SqlTemplate = template;
            _tableNameResolver = new TableNameResolver(template, this);
            _columnNameResolver = new ColumnNameResolver(template, this);
        }

        public virtual string GetQuery()
        {
            return "";
        }
        public QueryRequest Get<T>(object id)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Get<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            sb.AppendFormat(" from {0}", name);
            sb.Append(" where " + GetColumnName(onlyKey) + " = @Id");

            var dynParms = new DynamicParameters();
            dynParms.Add("@id", id);

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = dynParms };
        }

        public QueryRequest Get<T>(string condition, object parameters = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Get<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            sb.AppendFormat(" from {0}", name);
            sb.Append(condition);
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }

        public QueryRequest GetList<T>(object whereConditions)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            //if (!idProps.Any())
            //    throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);
            var joins = currenttype.GetTypeInfo().GetCustomAttributes<JoinAttribute>();
            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            sb.AppendFormat(" from {0}", name);
            foreach (var join in joins)
            {
                string jointype = "";
                JoinType type = join.JoinType;
                switch (type)
                {
                    case JoinType.InnerJoin:
                        jointype = "inner join";
                        break;
                }
                string tableName = GetTableName(join.TableName);
                //[Join(TableName = typeof(Product), At = "{table}.ProductId={self}.ProductId")]
                //[Join(TableName = typeof(SEO), At = "{table}.ProductId={self}.ProductId")]
                // $"hello, {name}"
                // string on= string.Format(join.At,new {table=rootTableName,self=tableName});
                string on = join.At;
                sb.AppendFormat("{0} {1}", jointype, tableName);
                sb.AppendFormat("on {0}", on);
            }
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)), whereConditions);
            }
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = whereConditions };
        }
        public QueryRequest GetList<T>(string conditions, object parameters = null)
        {
            var currenttype = typeof(T);
            //primary key not required!
            //var idProps = GetIdProperties(currenttype).ToList();
            //if (!idProps.Any())
            //    throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);
            var joins = currenttype.GetTypeInfo().GetCustomAttributes<JoinAttribute>();
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            sb.AppendFormat(" from {0}", name);
            foreach (var join in joins)
            {
                string jointype = "";
                JoinType type = join.JoinType;
                switch (type)
                {
                    case JoinType.InnerJoin:
                        jointype = "inner join";
                        break;
                }
                string tableName = GetTableName(join.TableName);
                //[Join(TableName = typeof(Product), At = "{table}.ProductId={self}.ProductId")]
                //[Join(TableName = typeof(SEO), At = "{table}.ProductId={self}.ProductId")]
                // $"hello, {name}"
                // string on= string.Format(join.At,new {table=rootTableName,self=tableName});
                string on = join.At;
                sb.AppendFormat("{0} {1}", jointype, tableName);
                sb.AppendFormat("on {0}", on);
            }
            sb.Append(" " + conditions);
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }

        public QueryRequest GetJoinedList<T>(string conditions, object parameters = null)
        {
            var currenttype = typeof(T);
            //var idProps = GetIdProperties(currenttype).ToList();
            //if (!idProps.Any())
            //    throw new ArgumentException("Entity must have at least one [Key] property");

            var rootTableName = GetTableName(currenttype);

            var joins = currenttype.GetTypeInfo().GetCustomAttributes<JoinAttribute>();
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            sb.AppendFormat(" from {0}", rootTableName);

            foreach (var join in joins)
            {
                string jointype = "";
                JoinType type = join.JoinType;
                switch (type)
                {
                    case JoinType.InnerJoin:
                        jointype = "inner join";
                        break;
                }
                string tableName = GetTableName(join.TableName);
                //[Join(TableName = typeof(Product), At = "{table}.ProductId={self}.ProductId")]
                //[Join(TableName = typeof(SEO), At = "{table}.ProductId={self}.ProductId")]
                // $"hello, {name}"
                // string on= string.Format(join.At,new {table=rootTableName,self=tableName});
                string on = join.At;
                sb.AppendFormat("{0} {1}", jointype, tableName);
                sb.AppendFormat("on {0}", on);
            }
            sb.Append(" " + conditions);

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }



        public QueryRequest GetList<T>()
        {
            return GetList<T>(new { });
        }

        public QueryRequest GetPaginatedList<T>(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null)
        {
            if (string.IsNullOrEmpty(SqlTemplate.PaginatedSql))
                throw new Exception("GetListPage is not supported with the current SQL Dialect");

            if (pageNumber < 1)
                throw new Exception("Page must be greater than 0");

            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");
            var joins = currenttype.GetTypeInfo().GetCustomAttributes<JoinAttribute>();

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            var query = SqlTemplate.PaginatedSql;
            if (string.IsNullOrEmpty(orderby))
            {
                orderby = GetColumnName(idProps.First()) + " desc";
            }

            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            query = query.Replace("{SelectColumns}", sb.ToString());
            query = query.Replace("{TableName}", name);
            // query = query.Replace("{join}", name);
            if (!joins.Any())
            {
                query = query.Replace("{join}", "");
            }
            foreach (var join in joins)
            {
                string jointype = "";
                JoinType type = join.JoinType;
                switch (type)
                {
                    case JoinType.InnerJoin:
                        jointype = "inner join";
                        break;
                }
                string tableName = GetTableName(join.TableName);
                //[Join(TableName = typeof(Product), At = "{table}.ProductId={self}.ProductId")]
                //[Join(TableName = typeof(SEO), At = "{table}.ProductId={self}.ProductId")]
                // $"hello, {name}"
                // string on= string.Format(join.At,new {table=rootTableName,self=tableName});
                string on = join.At;
                //sb.AppendFormat("{0} {1}", jointype, tableName);
                //sb.AppendFormat("on {0}", on);
                query = query.Replace("{join}", jointype + tableName + " on " + on);
            }
            query = query.Replace("{PageNumber}", pageNumber.ToString());
            query = query.Replace("{RowsPerPage}", rowsPerPage.ToString());
            query = query.Replace("{OrderBy}", orderby);
            query = query.Replace("{WhereClause}", conditions);
            query = query.Replace("{Offset}", pageNumber.ToString());
            //query = query.Replace("{Offset}", ((pageNumber - 1) * rowsPerPage).ToString());

            return new QueryRequest { QuerySql = query, Parameters = parameters };
            //return connection.Query<T>(query, parameters, transaction, true, commandTimeout);
        }

        

        public QueryRequest Insert<TKey>(object entityToInsert)
        {
            var idProps = GetIdProperties(entityToInsert).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Insert<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Insert<T> only supports an entity with a single [Key] or Id property");

            var keyHasPredefinedValue = false;
            var baseType = typeof(TKey);
            var underlyingType = Nullable.GetUnderlyingType(baseType);
            var keytype = underlyingType ?? baseType;
            if (keytype != typeof(int) && keytype != typeof(uint) && keytype != typeof(long) && keytype != typeof(ulong) && keytype != typeof(short) && keytype != typeof(ushort) && keytype != typeof(Guid) && keytype != typeof(string))
            {
                throw new Exception("Invalid return type");
            }

            var name = GetTableName(entityToInsert);
            var sb = new StringBuilder();
            sb.AppendFormat("insert into {0}", name);
            sb.Append(" (");
            BuildInsertParameters(entityToInsert, sb);
            sb.Append(") ");
            sb.Append("values");
            sb.Append(" (");
            BuildInsertValues(entityToInsert, sb);
            sb.Append(")");
            var isGuidType = false;
            object Id = null;
            if (keytype == typeof(Guid))
            {
                isGuidType = true;
                var guidvalue = (Guid)idProps.First().GetValue(entityToInsert, null);
                if (guidvalue == Guid.Empty)
                {
                    var newguid = SequentialGuid();
                    idProps.First().SetValue(entityToInsert, newguid, null);
                }
                else
                {
                    keyHasPredefinedValue = true;
                    Id = (TKey)idProps.First().GetValue(entityToInsert, null);
                }
                sb.Append(";select '" + idProps.First().GetValue(entityToInsert, null) + "' as id");
            }

            if ((keytype == typeof(int) || keytype == typeof(long)) && Convert.ToInt64(idProps.First().GetValue(entityToInsert, null)) == 0)
            {
                sb.Append(";" + SqlTemplate.IdentitySql);
            }
            else
            {
                keyHasPredefinedValue = true;
                Id = (TKey)idProps.First().GetValue(entityToInsert, null);
            }

            return new QueryRequest
            {
                IsKeyGuidType = isGuidType,
                KeyHasPredefinedValue = keyHasPredefinedValue,
                Id = Id,
                QuerySql = sb.ToString(),
                Parameters = entityToInsert
            };

        }

        public QueryRequest Insert(object entityToInsert)
        {
            return Insert<int?>(entityToInsert);
        }
        public QueryRequest Update(object entityToUpdate)
        {
            var idProps = GetIdProperties(entityToUpdate).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToUpdate);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0}", name);

            sb.AppendFormat(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            sb.Append(" where ");
            BuildWhere(sb, idProps, entityToUpdate);

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = entityToUpdate };
        }

        public QueryRequest Update(object entityToUpdate, string condition, object parameters)
        {
            var idProps = GetIdProperties(entityToUpdate).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToUpdate);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0}", name);

            sb.AppendFormat(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            sb.Append(condition);
            //BuildWhere(sb, idProps, entityToUpdate);

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }
        public QueryRequest Delete<T>(T entityToDelete)
        {
            var idProps = GetIdProperties(entityToDelete).ToList();


            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToDelete);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0}", name);

            sb.Append(" where ");
            BuildWhere(sb, idProps, entityToDelete);
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = entityToDelete };
            // return connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
        }
        public QueryRequest Delete<T>(string condition, object parameters)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Get<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0}", name);
            //sb.Append(" where ");
            sb.Append(condition);
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }
        public QueryRequest Delete<T>(object id)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();


            if (!idProps.Any())
                throw new ArgumentException("Delete<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Delete<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0}", name);
            sb.Append(" where " + GetColumnName(onlyKey) + " = @Id");

            var dynParms = new DynamicParameters();
            dynParms.Add("@id", id);

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = dynParms };
        }
        public QueryRequest Delete<T>(int[] ids)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();


            if (!idProps.Any())
                throw new ArgumentException("Delete<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Delete<T> only supports an entity with a single [Key] or Id property");

            var onlyKey = idProps.First();
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0}", name);
            sb.Append(" where " + GetColumnName(onlyKey) + "in  @Ids");

            var param = new { Ids = ids };

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = param };
        }


        public QueryRequest DeleteList<T>(object whereConditions)
        {

            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.AppendFormat("Delete from {0}", name);
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)));
            }
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = whereConditions };
        }

        public QueryRequest DeleteList<T>(string conditions, object parameters = null)
        {
            if (string.IsNullOrEmpty(conditions))
                throw new ArgumentException("DeleteList<T> requires a where clause");
            if (!conditions.ToLower().Contains("where"))
                throw new ArgumentException("DeleteList<T> requires a where clause and must contain the WHERE keyword");

            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0}", name);
            sb.Append(" " + conditions);

            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }
        public QueryRequest RecordCount<T>(string conditions = "", object parameters = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select count(1)");
            sb.AppendFormat(" from {0}", name);
            sb.Append(" " + conditions);
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };
        }


        public QueryRequest RecordCount<T>(object whereConditions)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select count(1)");
            sb.AppendFormat(" from {0}", name);
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)));
            }
            return new QueryRequest { QuerySql = sb.ToString(), Parameters = whereConditions };
        }

        //build update statement based on list on an entity
        private void BuildUpdateSet(object entityToUpdate, StringBuilder sb)
        {
            var nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();

            for (var i = 0; i < nonIdProps.Length; i++)
            {
                var property = nonIdProps[i];

                sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
                if (i < nonIdProps.Length - 1)
                    sb.AppendFormat(", ");
            }
        }

        //build select clause based on list of properties skipping ones with the IgnoreSelect and NotMapped attribute
        private void BuildSelect(StringBuilder sb, IEnumerable<PropertyInfo> props)
        {
            var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
            var addedAny = false;
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreSelectAttribute).Name || attr.GetType().Name == typeof(NotMappedAttribute).Name || attr.GetType().Name == typeof(IgnoreAllAttribute).Name)) continue;

                if (addedAny)
                    sb.Append(",");
                var joinedtable = propertyInfos.ElementAt(i).GetCustomAttribute<GetFromAttribute>();
                if (joinedtable != null)
                {
                    var name = GetTableName(joinedtable.TableName);
                    var columnName = GetColumnName(propertyInfos.ElementAt(i));
                    sb.AppendFormat("{0}.{1}", name, columnName);
                }
                else
                {
                    sb.Append(GetColumnName(propertyInfos.ElementAt(i)));
                }
                //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) != null)
                    sb.Append(" as " + Encapsulate(propertyInfos.ElementAt(i).Name));
                addedAny = true;

            }
        }

        private void BuildWhere(StringBuilder sb, IEnumerable<PropertyInfo> idProps, object sourceEntity, object whereConditions = null)
        {
            var propertyInfos = idProps.ToArray();
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var useIsNull = false;

                //match up generic properties to source entity properties to allow fetching of the column attribute
                //the anonymous object used for search doesn't have the custom attributes attached to them so this allows us to build the correct where clause
                //by converting the model type to the database column name via the column attribute
                var propertyToUse = propertyInfos.ElementAt(i);
                var sourceProperties = GetScaffoldableProperties(sourceEntity).ToArray();
                for (var x = 0; x < sourceProperties.Count(); x++)
                {
                    if (sourceProperties.ElementAt(x).Name == propertyInfos.ElementAt(i).Name)
                    {
                        propertyToUse = sourceProperties.ElementAt(x);

                        if (whereConditions != null && propertyInfos.ElementAt(i).CanRead && (propertyInfos.ElementAt(i).GetValue(whereConditions, null) == null || propertyInfos.ElementAt(i).GetValue(whereConditions, null) == DBNull.Value))
                        {
                            useIsNull = true;
                        }
                        break;
                    }
                }
                sb.AppendFormat(
                    useIsNull ? "{0} is null" : "{0} = @{1}",
                    GetColumnName(propertyToUse),
                    propertyInfos.ElementAt(i).Name);

                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        //build insert values which include all properties in the class that are:
        //Not named Id
        //Not marked with the Editable(false) attribute
        //Not marked with the [Key] attribute (without required attribute)
        //Not marked with [IgnoreInsert]
        //Not marked with [NotMapped]
        private void BuildInsertValues(object entityToInsert, StringBuilder sb)
        {
            var props = GetScaffoldableProperties(entityToInsert).ToArray();
            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.PropertyType != typeof(Guid)
                    && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                    && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                    continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreInsertAttribute).Name || attr.GetType().Name == typeof(IgnoreAllAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;

                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                sb.AppendFormat("@{0}", property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

        }

        //build insert parameters which include all properties in the class that are not:
        //marked with the Editable(false) attribute
        //marked with the [Key] attribute
        //marked with [IgnoreInsert]
        //named Id
        //marked with [NotMapped]
        private void BuildInsertParameters(object entityToInsert, StringBuilder sb)
        {
            var props = GetScaffoldableProperties(entityToInsert).ToArray();

            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.PropertyType != typeof(Guid)
                    && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                    && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                    continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreInsertAttribute).Name || attr.GetType().Name == typeof(IgnoreAllAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;
                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                sb.Append(GetColumnName(property));
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
        }

        //Get all properties in an entity
        private IEnumerable<PropertyInfo> GetAllProperties(object entity)
        {
            if (entity == null) entity = new { };
            return entity.GetType().GetProperties();
        }

        //Get all properties that are not decorated with the Editable(false) attribute
        private IEnumerable<PropertyInfo> GetScaffoldableProperties(object entity)
        {
            var props = entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(EditableAttribute).Name && !IsEditable(p)) == false);
            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        //Determine if the Attribute has an AllowEdit key and return its boolean state
        //fake the funk and try to mimick EditableAttribute in System.ComponentModel.DataAnnotations 
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private bool IsEditable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Count() > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(EditableAttribute).Name);
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }


        //Determine if the Attribute has an IsReadOnly key and return its boolean state
        //fake the funk and try to mimick ReadOnlyAttribute in System.ComponentModel 
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private bool IsReadOnly(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Count() > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(ReadOnlyAttribute).Name);
                if (write != null)
                {
                    return write.IsReadOnly;
                }
            }
            return false;
        }

        //Get all properties that are:
        //Not named Id
        //Not marked with the Key attribute
        //Not marked ReadOnly
        //Not marked IgnoreInsert
        //Not marked NotMapped
        private IEnumerable<PropertyInfo> GetUpdateableProperties(object entity)
        {
            var updateableProperties = GetScaffoldableProperties(entity);
            //remove ones with ID
            updateableProperties = updateableProperties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            //remove ones with key attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name) == false);
            //remove ones that are readonly
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == typeof(ReadOnlyAttribute).Name) && IsReadOnly(p)) == false);
            //remove ones with IgnoreUpdate attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreUpdateAttribute).Name || attr.GetType().Name == typeof(IgnoreAllAttribute).Name) == false);
            //remove ones that are not mapped
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name) == false);

            return updateableProperties;
        }

        //Get all properties that are named Id or have the Key attribute
        //For Inserts and updates we have a whole entity so this method is used
        private IEnumerable<PropertyInfo> GetIdProperties(object entity)
        {
            var type = entity.GetType();
            return GetIdProperties(type);
        }

        //Get all properties that are named Id or have the Key attribute
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        private IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }

        //Gets the table name for this entity
        //For Inserts and updates we have a whole entity so this method is used
        //Uses class name by default and overrides if the class has a Table attribute
        private string GetTableName(object entity)
        {
            var type = entity.GetType();
            return GetTableName(type);
        }

        //Gets the table name for this type
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        //Use dynamic type to be able to handle both our Table-attribute and the DataAnnotation
        //Uses class name by default and overrides if the class has a Table attribute
        private string GetTableName(Type type)
        {
            string tableName;

            if (_tableNames.TryGetValue(type, out tableName))
                return tableName;

            tableName = _tableNameResolver.ResolveTableName(type);
            _tableNames[type] = tableName;

            return tableName;
        }

        private string GetColumnName(PropertyInfo propertyInfo)
        {
            string columnName, key = string.Format("{0}.{1}", propertyInfo.DeclaringType, propertyInfo.Name);

            if (_columnNames.TryGetValue(key, out columnName))
                return columnName;

            columnName = _columnNameResolver.ResolveColumnName(propertyInfo);
            _columnNames[key] = columnName;

            return columnName;
        }

        public string Encapsulate(string databaseword)
        {
            return string.Format(SqlTemplate.Encapsulation, databaseword);
        }
        /// <summary>
        /// Generates a guid based on the current date/time
        /// http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp
        /// </summary>
        /// <returns></returns>
        public Guid SequentialGuid()
        {
            var tempGuid = Guid.NewGuid();
            var bytes = tempGuid.ToByteArray();
            var time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }

        public IEnumerable<QueryRequest> GetDependents<T>(object parameters = null)
        {
            var currenttype = typeof(T);
            var depAtts = currenttype.GetProperties().Select(x =>
            {
                var z = x.GetCustomAttribute(typeof(DependentAttribute), false);
                return z;
            }).Where(y => y != null);
            var multitblQuery = new StringBuilder();
            foreach (DependentAttribute dependent in depAtts)
            {
                var type = dependent.Model;
                var tblname = GetTableName(type);
                var sb = new StringBuilder();
                sb.Append("Select ");
                sb.Append(dependent.Get);
                sb.AppendFormat(" from {0}", tblname);
                sb.Append(" " + dependent.Condition);
                sb.Append(";");

                yield return new QueryRequest { QuerySql = sb.ToString(), Parameters = parameters };

                //multitblQuery.Append(sb);

            }

            //return new QueryRequest { QuerySql = multitblQuery.ToString(), Parameters = parameters };

        }
    }
}