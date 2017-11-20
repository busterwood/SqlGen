using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}]";

        public override string Generate(Table table, TableKey key, bool alter)
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
                    sb.AppendLine($"    [{c}] = [{c}] + 1,");
                else
                    sb.AppendLine($"    [{c}] = {c.ParameterValue()},");
            }

            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendOutput(Table table, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c}],");
            }

            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendWhere(Table table, StringBuilder sb)
        {
            sb.AppendLine("WHERE");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"    [{c}] = @{c},");
            }

            sb.Length -= 3;
            sb.AppendLine().AppendLine();
        }

        public override string GrantType() => null;

        public override string ToString() => "SQL Update";
    }
}