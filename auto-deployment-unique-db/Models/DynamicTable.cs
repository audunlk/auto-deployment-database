namespace auto_deployment_unique_db.Models
{
    public class DynamicTable
    {
        public string TableName { get; set; } = string.Empty;
        public List<DynamicColumn> Columns { get; set; } = new List<DynamicColumn>();
    }

}
