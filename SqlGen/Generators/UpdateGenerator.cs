using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}]";
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            AppendSet(table, sb);
            AppendOutput(table, sb);
            AppendWhere(table, sb);
            return sb.ToString();
        }

        private static void AppendSet(Table table, StringBuilder sb)
        {
            sb.AppendLine("SET");
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKey.Contains(col)))
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    [{c.ColumnName}] = [{c.ColumnName}] + 1,");
                else
                    sb.AppendLine($"    [{c.ColumnName}] = {c.ParameterValue()},");
            }

            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendOutput(Table table, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }

            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendWhere(Table table, StringBuilder sb)
        {
            sb.AppendLine("WHERE");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName},");
            }

            sb.Length -= 3;
            sb.AppendLine().AppendLine();
        }

        public override string GrantType() => null;

        public override string ToString() => "SQL Update";
    }
}