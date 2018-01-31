using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableUpdateProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_UpdateTable]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, options, sb);
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();

            if (options.Audit)
            {
                sb.AppendLine($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_InsertTable] @recs, 'U'");
                sb.AppendLine();
            }

            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("SET");
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKey.Contains(col) && (options.Audit || !col.IsAuditColumn())))
            {
                sb.AppendLine($"    [{c}] = {c.TableValue("src")},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    INSERTED.[{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}] AS target");
            sb.AppendLine("    JOIN @recs AS src ON");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"        target.[{c}] = src.[{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Table Update";
    }
}
