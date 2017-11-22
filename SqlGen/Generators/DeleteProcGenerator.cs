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

        public override string Generate(Table table, TableKey key, bool alter)
        {
            IEnumerable<Column> keyCols = key ?? table.PrimaryKey;

            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, key, alter, sb);
            foreach (var c in keyCols)
            {
                sb.AppendLine($"    @{c} {c.TypeDeclaration()},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("AS");
            sb.AppendLine();
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_Insert] ");
            foreach (var c in keyCols)
            {
                sb.Append($"@{c}=@{c}, ");
            }
            sb.AppendLine(" 'D'"); // type = delete
            sb.AppendLine();

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

        public override string ToString() => "Proc Delete";
    }
}
