using System.Linq;
using System.Text;

namespace SqlGen
{
    class CsAssertGenerator : Generator
    {
        public override string Generate(Table table, GeneratorOptions options)
        {
            var csClassName = table.TableName.ToPascalCase();
            var csArgsName = char.ToLower(csClassName[0]) + csClassName.Substring(1);
            var sb = new StringBuilder();
            sb.AppendLine($"\t\tvoid AssertAreEqual({csClassName} expected, {csClassName} actual)");
            sb.AppendLine("\t\t{");
            foreach (var col in table.Columns.Where(c => !c.IsRowVersion() && (options.Audit || !c.IsAuditColumn())))
            {
                var propName = col.ColumnName.ToPascalCase();
                sb.AppendLine($"\t\t\tAssert.AreEqual(expected.{propName}, actual.{propName}, \"{propName}\");");
            }

            sb.AppendLine("\t\t}");
            return sb.ToString();
        }

        public override string ToString() => "C# AssertAreEqual";
    }
}