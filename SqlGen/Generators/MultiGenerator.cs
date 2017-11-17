using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlGen
{
    public class MultiGenerator
    {
        readonly string connectionString;

        public bool Grant { get; set; }

        public MultiGenerator(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            this.connectionString = connectionString;
        }

        public string Generate(IEnumerable<Table> tables, IEnumerable<TableKey> keys, IEnumerable<Generator> generators)
        {
            var sb = new StringBuilder();

            foreach (var table in tables)
            {
                if (table.Columns == null)
                    LoadColumnsAndPrimaryKey(table);

                foreach (var key in keys.Any() ? keys : new TableKey[] { null })
                {
                    foreach (var gen in generators)
                    {
                        sb.AppendLine(gen.Generate(table, key));
                        sb.AppendLine("GO");
                        sb.AppendLine();
                        if (Grant)
                        {
                            var grantSql = gen.Grant(table, key);
                            if (grantSql != null)
                            {
                                sb.AppendLine(grantSql);
                                sb.AppendLine("GO");
                                sb.AppendLine();
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private void LoadColumnsAndPrimaryKey(Table table)
        {
            using (var cnn = new SqlConnection(connectionString))
            {
                var da = new TableDataAccess(cnn);
                table.Columns = da.LoadColumns(table.TableName, table.Schema);
                da.PopulatePrimaryKey(table);
            }
        }

    }
}
