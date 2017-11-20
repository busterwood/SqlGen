using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableInsertProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_InsertTable]";

        public override string Generate(Table table, TableKey key, bool alter)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, key, alter, sb);
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine("-- using merge so we can capture BULK_SEQ column in output, 1 = 0 forces the insert");
            sb.AppendLine($"MERGE INTO [{table.Schema}].[{table.TableName}] USING @recs AS src ON 1 = 0");
            sb.AppendLine($"WHEN NOT MATCHED THEN INSERT");

            AddFieldNames(table, sb);
            AddValues(sb, table);
            AddOutput(table, sb);
            return sb.ToString();
        }

        private static void AddOutput(Table table, StringBuilder sb)
        {
            sb.AppendLine("OUTPUT");
            sb.AppendLine($"    src.[BULK_SEQ],");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    INSERTED.[{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine(";");
        }

        private static void AddFieldNames(Table table, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.InsertableColumns)
            {
                sb.AppendLine($"    [{c}],");
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
                if (c.IsSequenceNumber())
                    sb.AppendLine($"    1,");
                else
                    sb.AppendLine($"    {c.TableValue("src")},");
            }
            sb.Length -= 3;
            sb.AppendLine().AppendLine(")");
        }

        public override string ToString() => "Table Insert";
    }
}
