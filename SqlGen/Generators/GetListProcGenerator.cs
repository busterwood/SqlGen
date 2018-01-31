using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class GetListProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey fk = null)
        {
            string name = fk == null
                ? string.Join("And", table.PrimaryKey.Select(c => ToTitleCase(c.ColumnName)))
                : string.Join("And", fk.Select(c => ToTitleCase(c.ColumnName)));
            return $"[{table.Schema}].[{table.TableName}_GetByListOf{name}]";
        }

        public override string Generate(Table table, GeneratorOptions options)
        {
            if (options.Key == null)
                return GenerateCore(table, ObjectName(table, null), options, table.PrimaryKey);
            else
                return GenerateCore(table, ObjectName(table, options.Key), options, options.Key);
        }

        private string GenerateCore(Table table, string procName, GeneratorOptions options, IEnumerable<Column> keysColumns)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(procName, options.Alter, sb);
            if (keysColumns.Count() != 1)
            {
                throw new NotSupportedException("Multiple key column parameters are not supported");
            }

            var col = keysColumns.First();
            var tableTypeParam = col.ColumnName + "S";
            sb.AppendLine($"    @{tableTypeParam} {col.DataType.ToUpper()}_TABLE_TYPE READONLY");

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
                sb.AppendLine($"    [{c}] IN (SELECT * FROM @{tableTypeParam}) AND");
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

        public override string ToString() => "Proc Get by list";
    }
}
