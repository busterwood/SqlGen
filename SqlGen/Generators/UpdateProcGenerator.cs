using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class UpdateProcGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE [{table.Schema}].[{table.TableName}_Update]");
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
                switch (c.ColumnName.ToUpper())
                {
                    case "AUDIT_START_DATE":
                    case "AUDIT_DATE_TIME":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(@{c.ColumnName}, GETUTCDATE()),");
                        break;
                    case "AUDIT_UPDATE_USER":
                    case "AUDIT_USER":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(@{c.ColumnName}, dbo.ALL_UserContextGet()),");
                        break;
                    case "AUDIT_APPLICATION_NAME":
                    case "AUDIT_APPLICATION":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(@{c.ColumnName}, APP_NAME()),");
                        break;
                    case "AUDIT_MACHINE_NAME":
                    case "AUDIT_MACHINE":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(@{c.ColumnName}, HOST_NAME()),");
                        break;
                    case "SEQUENCE_NUMBER":
                        sb.AppendLine($"    [{c.ColumnName}] = [{c.ColumnName}] + 1,");
                        break;
                    default:
                        sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName},");
                        break;
                }
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
                sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Proc Update";
    }
}
