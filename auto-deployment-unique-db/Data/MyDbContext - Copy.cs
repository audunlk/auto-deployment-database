using Microsoft.EntityFrameworkCore;
using auto_deployment_unique_db.Models;
using System.Reflection.Emit;

////the dbcontext recieve this json object to be able to create the tables and columns dynamically
//{
//    "tables": [
//        {
//        "tableName": "users",
//            "columns": [
//                {
//            "columnName": "user_id",
//                    "dataType": "int",
//                    "isPrimaryKey": true
//                },
//                {
//            "columnName": "email",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "first_name",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "last_name",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "username",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "address_id",
//                    "dataType": "int",
//                    "isForeignKey": true,
//                    "referencedTable": "addresses",
//                    "referencedColumn": "address_id"
//                },
//                {
//            "columnName": "phone",
//                    "dataType": "varchar(25)"
//                }
//            ]
//        },
//        {
//        "tableName": "addresses",
//            "columns": [
//                {
//            "columnName": "address_id",
//                    "dataType": "int",
//                    "isPrimaryKey": true
//                },
//                {
//            "columnName": "street",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "city",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "state",
//                    "dataType": "varchar(25)"
//                },
//                {
//            "columnName": "postal_code",
//                    "dataType": "varchar(25)"
//                }
//            ]
//        }
//    ]
//}

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
    }

}

//public void CreateTablesAndColumns(DynamicTableRequest dynamicTableRequest)
//{
//    foreach (var table in dynamicTableRequest.Tables)
//    {
//        string tableName = table.TableName;
//        List<DynamicColumn> columns = table.Columns;

//        var entityBuilder = modelBuilder.Entity(table.EntityType);

//        foreach (var column in columns)
//        {
//            string columnName = column.ColumnName;
//            string dataType = column.DataType;

//            var propertyBuilder = entityBuilder.Property(columnName).HasColumnType(dataType);

//            if (column.IsPrimaryKey)
//            {
//                propertyBuilder = propertyBuilder.HasColumnName(columnName).IsRequired();
//                entityBuilder.HasKey(columnName);
//            }

//            if (column.IsForeignKey)
//            {
//                string referencedTable = column.ReferencedTable;
//                string referencedColumn = column.ReferencedColumn;

//                propertyBuilder.HasColumnName(columnName);
//                entityBuilder.HasOne(referencedTable, referencedTable).WithMany().HasForeignKey(columnName);
//            }
//        }
//    }
//}
