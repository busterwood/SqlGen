using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class DeleteProcGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE [{table.Schema}].[{table.TableName}_Delete]");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("AS");
            sb.AppendLine();
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_Insert] ");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.Append($"@{c.ColumnName}, ");
            }
            sb.AppendLine(" 'D'"); // type = delete
            sb.AppendLine();

            sb.AppendLine($"DELETE FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("WHERE");
            foreach (var c in table.PrimaryKeyColumns)
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
