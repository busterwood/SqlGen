using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableAuditInsertProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}_AUDIT_InsertTable]";

        string Generate(Table table, bool alter)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(ObjectName(table, null), alter, sb);
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY,");
            sb.AppendLine($"    @auditType CHAR(1) = 'U'");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT ON");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}_AUDIT]");

            AppendColumnList(table, sb);
            AppendValues(table, sb);
            sb.AppendLine("    @auditType,");
            sb.AppendLine("    GETUTCDATE()");
            sb.AppendLine("FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}] AS src");
            sb.AppendLine("WHERE");
            sb.Append("    EXISTS (SELECT * FROM @recs AS recs WHERE ");
            foreach (var c in table.PrimaryKey)
            {
                sb.Append($"src.[{c.ColumnName}] = recs.[{c.ColumnName}] AND ");
            }
            sb.Length -= 5;
            sb.AppendLine(")");
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT OFF");

            return sb.ToString();
        }

        // generate audit for details replacement, e.g. replacement of all "order lines" from a FK of "order id"
        public override string Generate(Table table, GeneratorOptions options)
        {
            if (options.Key == null)
                return Generate(table, options.Alter);

            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, options, sb);

            foreach (var c in options.Key)
            {
                sb.AppendLine($"    @{c} {c.TypeDeclaration()},");
            }
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT ON");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}_AUDIT]");

            AppendColumnList(table, sb);
            AppendValues(table, sb);
            sb.AppendLine($"    CASE WHEN recs.[{table.PrimaryKey.First()}] IS NULL THEN 'D' ELSE 'U' END,");
            sb.AppendLine("    GETUTCDATE()");
            sb.AppendLine("FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}] AS src");
            sb.Append("    LEFT JOIN @recs AS recs ON ");            
            foreach (var c in table.PrimaryKey)
            {
                sb.Append($"src.[{c.ColumnName}] = recs.[{c.ColumnName}] AND ");
            }
            foreach (var c in options.Key)
            {
                sb.Append($"src.[{c.ColumnName}] = recs.[{c.ColumnName}] AND ");
            }
            sb.Length -= 5;
            sb.AppendLine();
            sb.AppendLine("WHERE");
            foreach (var c in options.Key)
            {
                sb.AppendLine($"    src.[{c.ColumnName}] = @{c.ColumnName} AND");
            }
            sb.Length -= 5;
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT OFF");

            return sb.ToString();
        }

        private static void AppendValues(Table table, StringBuilder sb)
        {
            sb.AppendLine("SELECT");
            foreach (var c in table.Columns.Where(c => !c.IsRowVersion()))
            {
                sb.AppendLine($"    src.[{c.ColumnName}],");
            }
        }

        private static void AppendColumnList(Table table, StringBuilder sb)
        {
            sb.AppendLine("(");
            foreach (var c in table.Columns.Where(c => !c.IsRowVersion()))
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.AppendLine("    [AUDIT_TYPE],");
            sb.AppendLine("    [AUDIT_END_DATE]");
            sb.AppendLine(")");
        }

        public override string ToString() => "Table Audit Insert";
    }
}
