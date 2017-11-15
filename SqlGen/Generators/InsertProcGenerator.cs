using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class InsertProcGenerator : Generator
    {
        public override string ObjectName(Table table, ForeignKey fk = null) => $"[{table.Schema}].[{table.TableName}_Insert]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            foreach (var c in table.InsertableColumns.Where(c => !c.IsSequenceNumber()))
            {
                var optional = c.IsAuditColumn() || c.IsSequenceNumber() ? " = NULL" : "";
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()}{optional},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("OUTPUT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    1,");
                else
                    sb.AppendLine($"    {c.ParameterValue()},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");

            return sb.ToString();
        }

        public override string ToString() => "Proc Insert";
    }
}
