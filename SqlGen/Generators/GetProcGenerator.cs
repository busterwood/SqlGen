using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class GetProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey fk = null)
        {
            if (fk == null)
                return $"[{table.Schema}].[{table.TableName}_Get]";

            var name = string.Join("And", fk.Select(c => ToTitleCase(c.ColumnName)));
            return $"[{table.Schema}].[{table.TableName}_GetBy{name}]";
        }

        public override string Generate(Table table) => Generate(table, null);

        public override string Generate(Table table, TableKey fk)
        {
            if (fk == null)
                return GenerateCore(table, ObjectName(table, null), table.PrimaryKey);
            else
                return GenerateCore(table, ObjectName(table, fk), fk);
        }

        private string GenerateCore(Table table, string procName, IEnumerable<Column> keysColumns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {procName}");
            foreach (var c in keysColumns)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
            if (keysColumns.Any())
                sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("AS");
            sb.AppendLine();

            sb.AppendLine($"SELECT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine($"FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");
            sb.AppendLine($"WHERE");
            foreach (var c in keysColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName} AND");
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
