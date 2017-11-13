using System;
using System.Text;

namespace SqlGen.Generators
{
    class AuditInsertProcGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE [{table.Schema}].[{table.TableName}_AUDIT_Insert]");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
            sb.AppendLine("    @auditType CHAR(1) = 'U'");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}_AUDIT]");
            sb.AppendLine("(");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.AppendLine($"    [AUDIT_TYPE],");
            sb.AppendLine($"    [AUDIT_END_DATE]");
            sb.AppendLine(")");
            sb.AppendLine("SELECT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }

            sb.AppendLine($"    @auditType,");
            sb.AppendLine($"    GETUTCDATE()");
            sb.AppendLine($"FROM [{table.Schema}].[{table.TableName}]");
            sb.Append($"WHERE ");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.Append($"[{c.ColumnName}] = @{c.ColumnName} AND ");
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
