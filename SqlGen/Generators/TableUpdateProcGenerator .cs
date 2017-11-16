using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableUpdateProcGenerator : Generator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}_UpdateTable]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_InsertTable] @recs, 'U'");
            sb.AppendLine();

            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("SET");
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKey.Contains(col)))
            {
                sb.AppendLine($"    [{c.ColumnName}] = {c.TableValue("src")},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}] AS target");
            sb.AppendLine("    JOIN @recs AS src ON");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"        target.[{c.ColumnName}] = src.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Table Update";
    }
}
