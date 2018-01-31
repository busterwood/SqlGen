using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class GetAllProcGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null) => $"[{table.Schema}].[{table.TableName}_GetAll]";

        public override string Generate(Table table, GeneratorOptions options)
        {
            var sb = new StringBuilder();
            AppendCreateOrAlterProc(table, options, sb);
            sb.AppendLine("AS");
            sb.AppendLine();

            sb.AppendLine($"SELECT");
            foreach (var c in table.Columns.Where(c => options.Audit || !c.IsAuditColumn()))
            {
                sb.AppendLine($"    [{c}],");
            }
            sb.Length -= 3;
            sb.AppendLine();

            sb.AppendLine($"FROM");
            sb.AppendLine($"    [{table.Schema}].[{table.TableName}]");

            return sb.ToString();
        }

        public override string ToString() => "Proc GetAll";
    }
}
