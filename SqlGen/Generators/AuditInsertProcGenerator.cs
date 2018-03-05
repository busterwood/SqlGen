using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class AuditInsertProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null)
        {
            if (key == null)
                return $"[{table.Schema}].[{table.TableName}_AUDIT_Insert]";

            var name = string.Join("And", key.Select(c => c.ColumnName.ToPascalCase()));
            return $"[{table.Schema}].[{table.TableName}_AUDIT_InsertBy{name}]";
        }

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, options, sb);
            TableKey key = options.Key ?? table.PrimaryKey;
            foreach (var c in key)
            {
                sb.AppendLine($"    @{c} {c.TypeDeclaration()},");
            }
            sb.AppendLine("    @auditType CHAR(1) = 'U'");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}_AUDIT]");
            sb.AppendLine("(");
            var insertableColumns = table.Columns.Where(c => !c.IsRowVersion());
            foreach (var c in insertableColumns)
            {
                sb.AppendLine($"    [{c}],");
            }
            sb.AppendLine($"    [AUDIT_TYPE],");
            sb.AppendLine($"    [AUDIT_END_DATE]");
            sb.AppendLine(")");
            sb.AppendLine("SELECT");
            foreach (var c in insertableColumns)
            {
                sb.AppendLine($"    [{c}],");
            }

            sb.AppendLine($"    @auditType,");
            sb.AppendLine($"    GETUTCDATE()");
            sb.AppendLine($"FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");
            sb.AppendLine($"WHERE");
            foreach (var c in key)
            {
                sb.AppendLine($"    [{c}] = @{c} AND");
            }
            sb.Length -= 5;
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT OFF");
            return sb.ToString();
        }

        public override string ToString() => "Audit Insert Proc";
    }
}
