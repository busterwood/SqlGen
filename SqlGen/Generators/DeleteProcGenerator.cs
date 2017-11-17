using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class DeleteProcGenerator : SqlGenerator
    {

        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_Delete]";

        public override string Generate(Table table, TableKey key, bool alter)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, key, alter, sb);
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("AS");
            sb.AppendLine();
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_Insert] ");
            foreach (var c in table.PrimaryKey)
            {
                sb.Append($"@{c.ColumnName}, ");
            }
            sb.AppendLine(" 'D'"); // type = delete
            sb.AppendLine();

            sb.AppendLine($"DELETE FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("WHERE");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Proc Delete";
    }
}
