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
            foreach (var c in table.InsertableColumns)
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
                switch (c.ColumnName.ToUpper())
                {
                    case "AUDIT_START_DATE":
                        sb.AppendLine($"    COALESCE(@{c.ColumnName}, GETUTCDATE()),");
                        break;
                    case "AUDIT_UPDATE_USER":
                        sb.AppendLine($"    COALESCE(@{c.ColumnName}, dbo.ALL_UserContextGet()),");
                        break;
                    case "AUDIT_APPLICATION_NAME":
                        sb.AppendLine($"    COALESCE(@{c.ColumnName}, APP_NAME()),");
                        break;
                    case "AUDIT_MACHINE_NAME":
                        sb.AppendLine($"    COALESCE(@{c.ColumnName}, HOST_NAME()),");
                        break;
                    case "SEQUENCE_NUMBER":
                        sb.AppendLine($"    COALESCE(@{c.ColumnName}, 1),");
                        break;
                    default:
                        sb.AppendLine($"    @{c.ColumnName},");
                        break;
                }

            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");

            return sb.ToString();
        }

        public override string ToString() => "Proc Insert";
    }
}
