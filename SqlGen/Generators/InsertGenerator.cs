﻿using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class InsertGenerator : Generator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}_Insert]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}]");
            AppendColumnList(table, sb);
            AppendOutput(table, sb);
            AppendValues(table, sb);
            return sb.ToString();
        }

        private static void AppendValues(Table table, StringBuilder sb)
        {
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    ISNULL(@{c}, 1),");
                else
                    sb.AppendLine($"    {c.ParameterValue()},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private static void AppendColumnList(Table table, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
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

        public override string GrantType() => null;

        public override string ToString() => "SQL Insert";
    }
}