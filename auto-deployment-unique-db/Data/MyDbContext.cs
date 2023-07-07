using Microsoft.EntityFrameworkCore;
using auto_deployment_unique_db.Models;

//see StructureExample.json for example of DynamicTableRequest
namespace auto_deployment_unique_db.Data
{
    public class MyDbContext : DbContext
    {
        private readonly string? _connectionString;

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public MyDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString != null)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        public void CreateTablesAndColumns(DynamicTableRequest dynamicTableRequest)
        {
            foreach (var table in dynamicTableRequest.Tables)
            {
                string tableName = table.TableName;
                List<DynamicColumn> columns = table.Columns;

                string sql = $"CREATE TABLE \"{tableName}\" (";

                foreach (var column in columns)
                {
                    string columnName = column.ColumnName;
                    string dataType = column.DataType;

                    sql += $"\"{columnName}\" {dataType},";
                }

                sql = sql.Remove(sql.Length - 1);
                sql += ")";

                Database.ExecuteSqlRaw(sql);
            }
        }
        public void AddPrimaryKeys(DynamicTableRequest dynamicTableRequest)
        {
            foreach (var table in dynamicTableRequest.Tables)
            {
                string tableName = table.TableName;
                List<DynamicColumn> columns = table.Columns;

                string sql = $"ALTER TABLE \"{tableName}\" ADD CONSTRAINT PK_{tableName} PRIMARY KEY (";

                foreach (var column in columns)
                {
                    if (column.IsPrimaryKey)
                    {
                        string columnName = column.ColumnName;
                        sql += $"\"{columnName}\",";
                    }
                }

                sql = sql.Remove(sql.Length - 1);
                sql += ")";

                Database.ExecuteSqlRaw(sql);
            }
        }
        public void AddForeignKeys(DynamicTableRequest dynamicTableRequest)
        {
            foreach (var table in dynamicTableRequest.Tables)
            {
                string tableName = table.TableName;
                List<DynamicColumn> columns = table.Columns;

                foreach (var column in columns)
                {
                    if (column.IsForeignKey)
                    {
                        string columnName = column.ColumnName;
                        string referencedTable = column.ReferencedTable;
                        string referencedColumn = column.ReferencedColumn;

                        string sql = $"ALTER TABLE \"{tableName}\" ADD CONSTRAINT FK_{tableName}_{referencedTable} FOREIGN KEY (\"{columnName}\") REFERENCES \"{referencedTable}\" (\"{referencedColumn}\")";

                        Database.ExecuteSqlRaw(sql);
                    }
                }
            }
        }
        public void AddIsNullable(DynamicTableRequest dynamicTableRequest)
        {
            foreach(var table in dynamicTableRequest.Tables)
            {
                string tableName = table.TableName;
                List<DynamicColumn> columns = table.Columns;

                foreach(var column in columns)
                {
                    if (!column.IsNullable)
                    {
                        string columnName = column.ColumnName;

                        string sql = $"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{columnName}\" SET NOT NULL";

                        Database.ExecuteSqlRaw(sql);
                    }
                }
            }
        }
        public void AddIsUnique(DynamicTableRequest dynamicTableRequest)
        {
            foreach (var table in dynamicTableRequest.Tables)
            {
                string tableName = table.TableName;
                List<DynamicColumn> columns = table.Columns;

                foreach (var column in columns)
                {
                    if (column.IsUnique)
                    {
                        string columnName = column.ColumnName;

                        string sql = $"ALTER TABLE \"{tableName}\" ADD CONSTRAINT UQ_{tableName}_{columnName} UNIQUE (\"{columnName}\")";

                        Database.ExecuteSqlRaw(sql);
                    }
                }
            }
        }
    }

}

