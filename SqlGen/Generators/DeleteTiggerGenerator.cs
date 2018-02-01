using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class DeleteTriggerGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_DELETE_TRIGGER]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TRIGGER {ObjectName(table)} ON [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("FOR DELETE AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("    SET NO COUNT ON");
            sb.AppendLine($"    INSERT INTO [{table.Schema}].[{table.TableName}_AUDIT]");
            sb.AppendLine("    (");
            AppendColumnList(table, sb);
            sb.AppendLine("        [AUDIT_TYPE],");
            sb.AppendLine("        [AUDIT_END_DATE]");
            sb.AppendLine("    )");
            sb.AppendLine("    SELECT");
            AppendColumnList(table, sb);
            sb.AppendLine("        'D', -- deleted");
            sb.AppendLine("        GETUTCDATE()");
            sb.AppendLine("    FROM deleted");
            sb.AppendLine("    SET NOCOUNT OFF");
            sb.AppendLine("END");
            return sb.ToString();
        }

        private static void AppendColumnList(Table table, StringBuilder sb)
        {
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"        [{c}],");
            }
        }

        public override string GrantType() => null;

        public override string ToString() => "Trigger Delete";
    }
}
