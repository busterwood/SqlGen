using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableInsertProcGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE [{table.Schema}].[{table.TableName}_InsertTable]");
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}]");

            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");

            sb.AppendLine("OUTPUT");
            sb.AppendLine($"    src.[BULK_SEQ],");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine("SELECT");
            foreach (var c in table.InsertableColumns)
            {
                switch (c.ColumnName.ToUpper())
                {
                    case "AUDIT_START_DATE":
                        sb.AppendLine($"    src.COALESCE(src.[{c.ColumnName}], GETUTCDATE()),");
                        break;
                    case "AUDIT_UPDATE_USER":
                        sb.AppendLine($"    src.COALESCE(src.[{c.ColumnName}], dbo.ALL_UserContextGet()),");
                        break;
                    case "AUDIT_APPLICATION_NAME":
                        sb.AppendLine($"    src.COALESCE(src.[{c.ColumnName}], APP_NAME()),");
                        break;
                    case "AUDIT_MACHINE_NAME":
                        sb.AppendLine($"    src.COALESCE(src.[{c.ColumnName}], HOST_NAME()),");
                        break;
                    case "SEQUENCE_NUMBER":
                        sb.AppendLine($"    src.COALESCE(src.[{c.ColumnName}], 1),");
                        break;
                    default:
                        sb.AppendLine($"    src.[{c.ColumnName}],");
                        break;
                }

            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine("FROM");
            sb.AppendLine("    @recs AS src");

            return sb.ToString();
        }

        public override string ToString() => "Table Insert";
    }
}
