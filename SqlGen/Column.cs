using System;

namespace SqlGen
{
    public class Column
    {
        public string TableCatalog { get; set; } //nvarchar
        public string TableSchema { get; set; } //nvarchar
        public string TableName { get; set; } //sysname
        public string ColumnName { get; set; } //sysname
        public string ColumnDefault { get; set; } //nvarchar
        public string IsNullable { get; set; } //Varchar
        public string DataType { get; set; } //Nvarchar
        public int CharacterMaximumLength { get; set; }
        public int CharacterOctetLength { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericPrecisionRadix { get; set; }
        public int NumericScale { get; set; }
        public int DatetimePrecision { get; set; }
        public string CharacterSetCatalog { get; set; } //Sysname
        public string CharacterSetSchema { get; set; } //Sysname
        public string CharacterSetName { get; set; } //Sysname
        public string CollationCatalog { get; set; } //Sysname
        public string CollationSchema { get; set; } //Sysname
        public string CollationName { get; set; } //Sysname
        public string DomainCatalog { get; set; } //Sysname
        public string DomainSchema { get; set; } //Sysname
        public string DomainName { get; set; } //sysname
        public bool IsIdentity { get; set; }

        public override string ToString() => ColumnName;
    }
    public static class ColumnExtensions
    {
        public static bool IsRowVersion(this Column c) => string.Equals("timestamp", c.DataType, StringComparison.OrdinalIgnoreCase) || string.Equals("rowversion", c.DataType, StringComparison.OrdinalIgnoreCase);
        public static bool IsNullable(this Column c) => string.Equals("YES", c.IsNullable, StringComparison.OrdinalIgnoreCase);
        public static bool IsAuditColumn(this Column c) => c.ColumnName.StartsWith("AUDIT_", StringComparison.OrdinalIgnoreCase);
        public static bool IsSequenceNumber(this Column c) => c.ColumnName.Equals("SEQUENCE_NUMBER", StringComparison.OrdinalIgnoreCase);

        public static string TypeDeclaration(this Column c)
        {
            const string max = "max";
            switch (c.DataType.ToLower())
            {
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    var len = c.CharacterMaximumLength == -1 ? max : c.CharacterMaximumLength.ToString();
                    return $"{c.DataType}({len})";
                default:
                    return c.DataType;
            }
        }
    }

}