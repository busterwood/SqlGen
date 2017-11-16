using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class GetAllProcGenerator : Generator
    {
        public override string ObjectName(Table table, TableKey fk = null) => $"[{table.Schema}].[{table.TableName}_GetAll]";

        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE {ObjectName(table)}");
            sb.AppendLine("AS");
            sb.AppendLine();

            sb.AppendLine($"SELECT");
            foreach (var c in table.Columns)
            {
                sb.AppendLine($"    [{c.ColumnName}],");
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
