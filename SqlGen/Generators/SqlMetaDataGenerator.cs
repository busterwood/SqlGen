using System.Linq;
using System.Text;

namespace SqlGen
{
    class SqlMetaDataGenerator : Generator
    {
        public override string Generate(Table table, TableKey key, bool alter)
        {
            var sb = new StringBuilder();
            var varName = (table.TableName + "_TABLE_TYPE").ToPascalCase() + "Schema =";
            sb.AppendLine($"\t\tstatic readonly SqlMetaData[] {varName}");
            sb.AppendLine("\t\t{");
            foreach (var c in table.Columns.Where(c => !c.IsRowVersion()))
            {                
                sb.AppendLine($"\t\t\tnew SqlMetaData(\"{c}\", SqlDbType.{TypeAndLength(c)}),");
            }
            sb.AppendLine("\t\t};");
            return sb.ToString();
        }

        private object TypeAndLength(Column c)
        {
            switch (c.DataType)
            {
                case "bigint":
                    return $"BigInt";
                case "smallint":
                    return $"SmallInt";
                case "tinyint":
                    return $"TinyInt";
                case "binary":
                    return $"Binary, {c.CharacterMaximumLength}";
                case "varbinary":
                    return $"VarBinary, {c.CharacterMaximumLength}";
                case "char":
                    return $"Char, {c.CharacterMaximumLength}";
                case "nchar":
                    return $"NChar, {c.CharacterMaximumLength}";
                case "varchar":
                    return $"VarChar, {c.CharacterMaximumLength}";
                case "nvarchar":
                    return $"NVarChar, {c.CharacterMaximumLength}";
                case "decimal":
                    return $"Decimal, {c.NumericPrecision}, {c.NumericScale}";
                case "numeric":
                    return $"Numeric, {c.NumericPrecision}, {c.NumericScale}";
                case "datetime":
                    return $"DateTime";
                case "datetimeoffset":
                    return $"DateTimeOffset";
                case "smalldatetime":
                    return $"SmallDateTime";
                case "datetime2":
                    return $"DateTime2";
                default:
                    return c.DataType.ToPascalCase();
            }            
        }

        public override string ToString() => "C# SqlMetaData";
    }
}