using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableAuditInsertProcGenerator : Generator
    {
        public override string ObjectName(Table table, ForeignKey fk = null) => $"[{table.Schema}].[{table.TableName}_AUDIT_InsertTable]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            sb.AppendLine($"    @recs [{table.Schema}].[{table.TableName}_TABLE_TYPE] READONLY");
            sb.AppendLine("    @auditType CHAR(1) = 'U'");
            sb.AppendLine("AS");
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT ON");
            sb.AppendLine();
            sb.AppendLine($"INSERT INTO [{table.Schema}].[{table.TableName}_AUDIT]");

            sb.AppendLine("(");
            foreach (var c in table.Columns.Where(c => !c.IsRowVersion()))
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }
            sb.AppendLine("    [AUDIT_TYPE],");
            sb.AppendLine("    [AUDIT_END_DATE]");
            sb.AppendLine(")");
            sb.AppendLine("SELECT");
            foreach (var c in table.Columns.Where(c => !c.IsRowVersion()))
            {
                sb.AppendLine($"    [{c.ColumnName}],");
            }

            sb.AppendLine("    @auditType,");
            sb.AppendLine("    GETUTCDATE()");
            sb.AppendLine("FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}] AS target");
            sb.AppendLine("WHERE");
            sb.Append("    EXISTS (SELECT * FROM @recs AS src WHERE ");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.Append($"src.[{c.ColumnName}] = target.[{c.ColumnName}] AND ");
            }
            sb.Length -= 5;
            sb.AppendLine(")");
            sb.AppendLine();
            sb.AppendLine("SET NOCOUNT OFF");

            return sb.ToString();
        }

        public override string ToString() => "Table Audit Insert";
    }
}
