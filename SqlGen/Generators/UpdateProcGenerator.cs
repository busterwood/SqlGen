using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateProcGenerator : UpdateGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_Update]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, options, sb);
            foreach (var c in table.Columns)
            {
                var optional = c.IsAuditColumn() || c.IsSequenceNumber() || c.IsRowVersion() ? " = NULL" : "";
                sb.AppendLine($"    @{c} {c.TypeDeclaration()}{optional},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine("AS");
            AppendExecAuditProc(table, sb);
            sb.AppendLine();
            sb.Append(base.Generate(table, options));
            return sb.ToString();
        }

        private static void AppendExecAuditProc(Table table, StringBuilder sb)
        {
            sb.AppendLine();
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_Insert] ");
            foreach (var c in table.PrimaryKey)
            {
                sb.Append($"@{c}=@{c}, ");
            }
            sb.AppendLine("@auditType='D'"); // type = delete
        }

        public override string GrantType() => "OBJECT";

        public override string ToString() => "Proc Update";
    }
}