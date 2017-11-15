﻿using System;
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
            foreach (var c in table.Columns.Where(c => !c.IsSequenceNumber()))
            {
                var optional = c.IsAuditColumn() ? " = NULL" : "";
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()}{optional},");
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
            sb.AppendLine(" 'U'"); // type = update
            sb.AppendLine();

            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("SET");
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKeyColumns.Contains(col)))
            {
                sb.AppendLine($"    [{c.ColumnName}] = {c.ParameterValue()},");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("WHERE");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}] = {c.ColumnName},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Proc Update";
    }
}
