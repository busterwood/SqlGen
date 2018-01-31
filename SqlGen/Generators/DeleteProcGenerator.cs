using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class DeleteProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null)
        {
            if (key == null)
                return $"[{table.Schema}].[{table.TableName}_Delete]";

            var name = string.Join("And", key.Select(c => c.ColumnName.ToPascalCase()));
            return $"[{table.Schema}].[{table.TableName}_DeleteBy{name}]";
        }

        public override string Generate(Table table, GeneratorOptions options)
        {
            IEnumerable<Column> keyCols = options.Key ?? table.PrimaryKey;

            var sb = new StringBuilder();

            AppendCreateOrAlterProc(ObjectName(table, options.Key), options.Alter, sb);
            foreach (var c in keyCols)
            {
                sb.AppendLine($"    @{c} {c.TypeDeclaration()},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("AS");
            sb.AppendLine();

            if (options.Audit)
            {
                CallAuditProc(table, keyCols, sb);
            }

            sb.AppendLine($"DELETE FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("WHERE");
            foreach (var c in keyCols)
            {
                sb.AppendLine($"    [{c}] = @{c},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        private static void CallAuditProc(Table table, IEnumerable<Column> keyCols, StringBuilder sb)
        {
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_Insert] ");

            foreach (var c in keyCols)
            {
                sb.Append($"@{c}=@{c}, ");
            }
            sb.AppendLine("@auditType='D'"); // type = delete
            sb.AppendLine();
        }

        public override string ToString() => "Proc Delete";
    }
}
