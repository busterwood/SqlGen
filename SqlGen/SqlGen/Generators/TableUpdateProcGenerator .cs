using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableUpdateProcGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE [{table.Schema}].[{table.TableName}_UpdateTable]");
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_InsertTable] @recs, 'U'");
            sb.AppendLine();

            sb.AppendLine($"UPDATE [{table.Schema}].[{table.TableName}]");
            sb.AppendLine("SET");
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKeyColumns.Contains(col)))
            {
                switch (c.ColumnName.ToUpper())
                {
                    case "AUDIT_START_DATE":
                    case "AUDIT_DATE_TIME":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(src.[{c.ColumnName}], GETUTCDATE()),");
                        break;
                    case "AUDIT_UPDATE_USER":
                    case "AUDIT_USER":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(src.[{c.ColumnName}], dbo.ALL_UserContextGet()),");
                        break;
                    case "AUDIT_APPLICATION_NAME":
                    case "AUDIT_APPLICATION":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(src.[{c.ColumnName}], APP_NAME()),");
                        break;
                    case "AUDIT_MACHINE_NAME":
                    case "AUDIT_MACHINE":
                        sb.AppendLine($"    [{c.ColumnName}] = COALESCE(src.[{c.ColumnName}], HOST_NAME()),");
                        break;
                    case "SEQUENCE_NUMBER":
                        sb.AppendLine($"    [{c.ColumnName}] = [{c.ColumnName}] + 1,");
                        break;
                    default:
                        sb.AppendLine($"    [{c.ColumnName}] = src.[{c.ColumnName}],");
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

            sb.AppendLine("FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}] AS target");
            sb.AppendLine("    JOIN @recs AS src ON");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"        target.[{c.ColumnName}] = src.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Table Update";
    }
}
