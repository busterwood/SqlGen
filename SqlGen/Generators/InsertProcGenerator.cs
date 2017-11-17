using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class InsertProcGenerator : InsertGenerator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}_Insert]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            foreach (var c in table.Columns.Where(c => !c.IsIdentity)) // we want a row version parameter, but it is ignored
            {
                var optional = c.IsAuditColumn() || c.IsSequenceNumber() || c.IsRowVersion() ? " = NULL" : "";
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()}{optional},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.Append(base.Generate(table));
            return sb.ToString();
        }

        public override string GrantType() => "OBJECT";

        public override string ToString() => "Proc Insert";

    }
}
