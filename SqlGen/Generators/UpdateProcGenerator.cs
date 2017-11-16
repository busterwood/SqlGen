using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateProcGenerator : Generator
    {
        public override string ObjectName(Table table, ForeignKey fk = null) => $"[{table.Schema}].[{table.TableName}_Update]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            foreach (var c in table.Columns)
            {
                var optional = c.IsAuditColumn() || c.IsSequenceNumber() || c.IsRowVersion()  ? " = NULL" : "";
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()}{optional},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine("AS");
            AppendExecAuditProc(table, sb);
            sb.AppendLine();
            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            AppendSet(table, sb);
            AppendOutput(table, sb);
            AppendWhere(table, sb);
            return sb.ToString();
        }

        private static void AppendExecAuditProc(Table table, StringBuilder sb)
        {
            sb.AppendLine();
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_Insert] ");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.Append($"@{c.ColumnName}, ");
            }
            sb.AppendLine(" 'U'"); // type = update
        }

        private static void AppendSet(Table table, StringBuilder sb)
        {
            sb.AppendLine("SET");
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKeyColumns.Contains(col)))
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    [{c.ColumnName}] = [{c.ColumnName}] + 1,");
                else
                    sb.AppendLine($"    [{c.ColumnName}] = {c.ParameterValue()},");
            }
            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendOutput(Table table, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AppendWhere(Table table, StringBuilder sb)
        {
            sb.AppendLine("WHERE");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();
        }

        public override string ToString() => "Proc Update";
    }
}
