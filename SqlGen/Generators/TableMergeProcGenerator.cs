using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableMergeProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}_MergeTable]";

        public override string Generate(Table table) => Generate(table, null);

        public override string Generate(Table table, TableKey fk)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            AddFKParameters(fk, sb);
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            ExecAuditProc(table, fk, sb);

            sb.AppendLine($"MERGE INTO [{table.Schema}].[{table.TableName}] as target");
            sb.AppendLine($"USING @recs AS src");
            sb.AppendLine($"ON");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"    target.{c.ColumnName} = src.{c.ColumnName} AND");
            }
            sb.Length -= 5;
            sb.AppendLine();

            sb.AppendLine($"WHEN NOT MATCHED BY target THEN");
            sb.AppendLine($"INSERT");
            AddInsertFieldNames(table, sb);
            AddInsertValues(sb, table);

            sb.AppendLine($"WHEN MATCHED THEN");
            sb.AppendLine($"UPDATE SET");
            AddUpdateAssignments(table, sb);

            if (fk != null)
            {
                sb.Append($"WHEN MATCHED BY src");
                foreach (var c in fk)
                {
                    sb.Append($" AND target.[{c.ColumnName}] = src.[{c.ColumnName}]");
                }
                sb.AppendLine(" THEN");
                sb.AppendLine("    DELETE");
            }

            AddOutput(table, sb);
            return sb.ToString();
        }

        private static void ExecAuditProc(Table table, TableKey fk, StringBuilder sb)
        {
            if (fk == null)
                sb.AppendLine($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_InsertTable] @recs=@recs");
            else
            {
                sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_InsertTable] @recs=@recs");
                foreach (var c in fk)
                {
                    sb.Append($", @{c.ColumnName}=@{c.ColumnName}");
                }
                sb.AppendLine();
            }
            sb.AppendLine();
        }

        private static void AddFKParameters(TableKey fk, StringBuilder sb)
        {
            if (fk == null)
                return;
            foreach (var c in fk)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
        }

        private static void AddInsertFieldNames(Table table, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private static void AddInsertValues(StringBuilder sb, Table table)
        {
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    1,");
                else
                    sb.AppendLine($"    {c.TableValue("src")},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private void AddUpdateAssignments(Table table, StringBuilder sb)
        {
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKey.Contains(col)))
            {
                sb.AppendLine($"    [{c.ColumnName}] = {c.TableValue("src")},");
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
