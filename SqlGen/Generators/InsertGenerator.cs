using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class InsertGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_Insert]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}]");
            AppendColumnList(table, options, sb);
            AppendOutput(table, options, sb);
            AppendValues(table, options, sb);
            return sb.ToString();
        }

        private static void AppendValues(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    ISNULL(@{c}, 1),");
                else
                    sb.AppendLine($"    {c.ParameterValue()},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private static void AppendColumnList(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    [{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private static void AppendOutput(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    INSERTED.[{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine();
        }

        public override string GrantType() => null;

        public override string ToString() => "SQL Insert";
    }
}
