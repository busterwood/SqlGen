using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class GetProcGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE [{table.Schema}].[{table.TableName}_Get]");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    @{c.ColumnName} {c.TypeDeclaration()},");
            }
            sb.Length -= 3;
            sb.AppendLine();

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
            sb.AppendLine($"WHERE");
            foreach (var c in table.PrimaryKeyColumns)
            {
                sb.AppendLine($"    [{c.ColumnName}] = @{c.ColumnName} AND");
            }
            sb.Length -= 5;
            sb.AppendLine();

            return sb.ToString();
        }

        public override string ToString() => "Proc Get";
    }
}
