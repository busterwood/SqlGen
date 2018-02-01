using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateTriggerGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_UPDATE_TRIGGER]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterTrigger(table, options.Alter, "DELETE", sb);
            sb.AppendLine("FOR UPDATE AS");
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
            sb.AppendLine("        'U', -- updated");
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

        public override string ToString() => "Trigger Update";
    }
}
