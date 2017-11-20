using System.Linq;
using System.Text;

namespace SqlGen
{
    class CsCallInsertProcGenerator : Generator
    {
        public override string Generate(Table table, TableKey key, bool alter)
        {
            var csClassName = table.TableName.ToPascalCase();
            var csArgsName = char.ToLower(csClassName[0]) + csClassName.Substring(1);

            var sb = new StringBuilder();
            sb.AppendLine($"\t\tTask<{csClassName}> InsertAsync(DbConnection connection, {csClassName} {csArgsName})");
            sb.AppendLine("\t\t{");
            sb.AppendLine($"\t\t\tconst string sql = @\"EXEC {table.Schema}.{table.TableName}_Insert");
            foreach (var c in table.InsertableColumns)
            {                
                sb.AppendLine($"{c}=@{c.ColumnName.ToPascalCase()}, ");
            }
            sb.Length -= 4;
            sb.AppendLine("\";");
            sb.AppendLine($"\t\t\treturn connection.QueryProcAsync(sql, {csArgsName}).SingleAsync<{csClassName}>();");
            sb.AppendLine("\t\t}");
            return sb.ToString();
        }

        public override string ToString() => "C# Call Insert Proc";
    }
}