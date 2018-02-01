using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableMergeProcGenerator : SqlGenerator
    {
        static readonly IEnumerable<Column> NoColumns = new Column[0];

        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_MergeTable]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, options, sb);
            AddFKParameters(options.Key, sb);
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            ExecAuditProc(table, options.Key, sb);

            sb.AppendLine($"MERGE INTO [{table.Schema}].[{table.TableName}] as target");
            sb.AppendLine($"USING @recs AS src");
            AddMergeOn(table, sb);
            sb.AppendLine($"WHEN NOT MATCHED BY TARGET THEN");
            sb.AppendLine($"INSERT");
            AddInsertFieldNames(table, options, sb);
            AddInsertValues(table, options, sb);
            sb.AppendLine($"WHEN MATCHED THEN");
            sb.AppendLine($"UPDATE SET");
            AddUpdateAssignments(table, options, sb);
            AddOptionalDelete(options.Key, sb);
            AddOutput(table, options, sb);

            return sb.ToString();
        }

        private static void AddMergeOn(Table table, StringBuilder sb)
        {
            sb.AppendLine($"ON");
            foreach (var c in table.PrimaryKey)
            {
                sb.AppendLine($"    target.{c} = src.{c} AND");
            }
            sb.Length -= 5;
            sb.AppendLine();
        }

        private static void ExecAuditProc(Table table, IEnumerable<Column> keyCols, StringBuilder sb)
        {
            sb.Append($"EXEC [{table.Schema}].[{table.TableName}_AUDIT_InsertTable] @recs=@recs");
            foreach (var c in keyCols ?? NoColumns)
            {
                sb.Append($", @{c}=@{c}");
            }
            sb.AppendLine(";");
            sb.AppendLine();
        }

        private static void AddFKParameters(IEnumerable<Column> keyCols, StringBuilder sb)
        {
            foreach (var c in keyCols ?? NoColumns)
            {
                sb.AppendLine($"    @{c} {c.TypeDeclaration()},");
            }
        }

        private static void AddInsertFieldNames(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    [{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private static void AddInsertValues(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    1,");
                else
                    sb.AppendLine($"    {c.TableValue("src")},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        private void AddUpdateAssignments(Table table, GeneratorOptions options, StringBuilder sb)
        {
            foreach (var c in table.InsertableColumns.Where(col => !table.PrimaryKey.Contains(col)).Where(c => options.Audit || !c.IsAuditColumn()))
            {
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    [{c}] = target.{c} + 1,");
                else
                    sb.AppendLine($"    [{c}] = {c.TableValue("src")},");
            }
            sb.Length -= 3;
            sb.AppendLine();
        }

        private static void AddOptionalDelete(IEnumerable<Column> key, StringBuilder sb)
        {
            if (key == null || !key.Any())
                return;

            sb.Append($"WHEN NOT MATCHED BY SOURCE");
            foreach (var c in key)
            {
                sb.Append($" AND target.[{c}] = @{c}");
            }
            sb.AppendLine(" THEN");
            sb.AppendLine("    DELETE");
        }

        private static void AddOutput(Table table, GeneratorOptions options, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            sb.AppendLine($"    src.[BULK_SEQ],");
            foreach (var c in table.Columns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    INSERTED.[{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine(";");
        }

        public override string ToString() => "Table Merge";
    }
}
