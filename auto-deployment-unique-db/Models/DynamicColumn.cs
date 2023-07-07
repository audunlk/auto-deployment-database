namespace auto_deployment_unique_db.Models
{
    public class DynamicColumn
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }

        public bool IsUnique { get; set; }

        public bool IsNullable { get; set; }

        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string ReferencedTable { get; set; } = string.Empty;
        public string ReferencedColumn { get; set; } = string.Empty;
    }
}
