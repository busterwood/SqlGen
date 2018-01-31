using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            AppendSet(table, options, sb);
            AppendOutput(table, options, sb);
            AppendWhere(table, sb);
            return sb.ToString();
        }

        private static void AppendSet(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("SET");
            foreach (var col in table.InsertableColumns.Where(c => !table.PrimaryKey.Contains(c) && (options.Audit || !c.IsAuditColumn())))
            {
                if (col.IsSequenceNumber())
                    sb.AppendLine($"    [{col}] = [{col}] + 1,");
                else
                    sb.AppendLine($"    [{col}] = {col.ParameterValue()},");
            }

            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendOutput(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            foreach (var col in table.Columns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    INSERTED.[{col}],");
            }

            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendWhere(Table table, StringBuilder sb)
        {
            sb.AppendLine("WHERE");
            foreach (var col in table.PrimaryKey)
            {
                sb.AppendLine($"    [{col}] = @{col},");
            }

            sb.Length -= 3;
            sb.AppendLine().AppendLine();
        }

        public override string GrantType() => null;

        public override string ToString() => "SQL Update";
    }
}