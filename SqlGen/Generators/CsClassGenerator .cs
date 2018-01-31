using System.Linq;
using System.Text;

namespace SqlGen
{
    class CsClassGenerator : Generator
    {
        public override string Generate(Table table, GeneratorOptions options)
        {
            var csClassName = table.TableName.ToPascalCase();
            var csArgsName = char.ToLower(csClassName[0]) + csClassName.Substring(1);
            var sb = new StringBuilder();
            sb.AppendLine($"\tpublic class {csClassName}");
            sb.AppendLine("\t{");
            foreach (var c in table.Columns.Where(c => !c.IsRowVersion() && (options.Audit || !c.IsAuditColumn())))
            {
                var propName = c.ColumnName.ToPascalCase();
                var propType = c.ClrTypeName();
                sb.AppendLine($"\t\tpublic {propType} {propName} {{ get; set; }}");
            }

            sb.AppendLine("\t}");
            return sb.ToString();
        }

        public override string ToString() => "C# Class";
    }
}