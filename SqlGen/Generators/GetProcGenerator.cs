using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class GetProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null)
        {
            if (key == null)
                return $"[{table.Schema}].[{table.TableName}_Get]";

            var name = string.Join("And", key.Select(c => ToTitleCase(c.ColumnName)));
            return $"[{table.Schema}].[{table.TableName}_GetBy{name}]";
        }

        public override string Generate(Table table, TableKey key, bool alter)
        {
            if (key == null)
                return GenerateCore(table, ObjectName(table, null), alter, table.PrimaryKey);
            else
                return GenerateCore(table, ObjectName(table, key), alter, key);
        }

        private string GenerateCore(Table table, string procName, bool alter, IEnumerable<Column> keysColumns)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(procName, alter, sb);
            foreach (var c in keysColumns)
            {
                sb.AppendLine($"    @{c} {c.TypeDeclaration()},");
            }
            if (keysColumns.Any())
                sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("AS");
            sb.AppendLine();

            sb.AppendLine($"SELECT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    [{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine($"FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");
            sb.AppendLine($"WHERE");
            foreach (var c in keysColumns)
            {
                sb.AppendLine($"    [{c}] = @{c} AND");
            }
            if (keysColumns.Any())
                sb.Length -= 5;
            sb.AppendLine();

            return sb.ToString();
        }

        static string ToTitleCase(string columnName)
        {
            return string.Join("", columnName.Split('_').Select(word => word.ToLower()).Select(word => char.ToUpper(word[0]) + word.Substring(1)));
        }

        public override string ToString() => "Proc Get";
    }
}
