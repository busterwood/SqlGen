using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SqlGen
{
    public class Table
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public List<Column> Columns { get; set; }
        public PrimaryKey PrimaryKey { get; internal set; }
        public List<ForeignKey> ForeignKeys { get; set; }
        public IEnumerable<Column> InsertableColumns => Columns.Where(c => !c.IsIdentity && !c.IsRowVersion());
        public override string ToString() => $"{Schema}.{TableName}";

        public void EnsureFullyPopulated(string connectionString)
        {
            if (Columns != null && ForeignKeys != null)
                return;

            using (var cnn = new SqlConnection(connectionString))
            {
                var da = new TableDataAccess(cnn);
                if (Columns == null)
                {
                    Columns = da.LoadColumns(TableName, Schema);
                    da.PopulatePrimaryKey(this);
                }
                if (ForeignKeys == null)
                    da.PopulateForeignKeys(this);
            }

        }
    }

    public static class Extensions
    {
        public static string ToPascalCase(this string sqlName)
        {
            return string.Join("", sqlName.Split('_').Select(w => w.ToLower()).Select(w => char.ToUpper(w[0]) + w.Substring(1)));
        }
    }
}