using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableMergeProcGenerator : Generator
    {
        public override string ObjectName(Table table, ForeignKey fk = null) => $"[{table.Schema}].[{table.TableName}_MergeTable]";

        public override string Generate(Table table) => Generate(table, null);

        public override string Generate(Table table, ForeignKey fk)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            AddFKParameters(fk, sb);
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine($"MERGE INTO [{table.Schema}].[{table.TableName}] as target");
            sb.AppendLine($"USING @recs AS src");
            sb.AppendLine($"ON");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    target.{c.ColumnName} = src.{c.ColumnName} AND");
            }
            sb.Length -= 5;
            sb.AppendLine();

            sb.AppendLine($"WHEN NOT MATCHED BY target THEN");
            sb.AppendLine($"INSERT");
            AddFieldNames(table, sb);
            AddValues(sb, table);

            sb.AppendLine($"WHEN MATCHED THEN");
            sb.AppendLine($"UPDATE SET");
            AddUpdateAssignments(table, sb);

            if (fk != null)
            {
                sb.Append($"WHEN MATCHED BY src");
                foreach (var c in fk.TableColumns)
                {
                    sb.Append($" AND target.[{c.ColumnName}] = src.[{c.ColumnName}]");
                }
                sb.AppendLine(" THEN");
                sb.AppendLine("    DELETE");
            }

            AddOutput(table, sb);
            return sb.ToString();
        }

        private static void AddFKParameters(ForeignKey fk, StringBuilder sb)
        {
            if (fk == null)
                return;
            foreach (var c in fk.TableColumns)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
        }

        private static void AddFieldNames(Table table, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private static void AddValues(StringBuilder sb, Table table)
        {
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
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
            sb.AppendLine().AppendLine(")");
        }

        private void AddUpdateAssignments(Table table, StringBuilder sb)
        {
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
        }

        private static void AddOutput(Table table, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            sb.AppendLine($"    src.[BULK_SEQ],");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine(";");
        }

        public override string ToString() => "Table Merge";
    }
}
