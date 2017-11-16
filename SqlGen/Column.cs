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
                case "numeric":
                case "decimal":
                    return $"{c.DataType}({c.NumericPrecision},{c.NumericScale})";
                default:
                    return c.DataType;
            }
        }

        public static string TableValue(this Column col, string alias)
        {
            switch (col.ColumnName.ToUpper())
            {
                case "AUDIT_START_DATE":
                case "AUDIT_DATE_TIME":
                    return $"ISNULL({alias}.[{col}], GETUTCDATE())";
                case "AUDIT_UPDATE_USER":
                case "AUDIT_USER":
                    return $"ISNULL({alias}.[{col}], dbo.ALL_UserContextGet())";
                case "AUDIT_APPLICATION_NAME":
                case "AUDIT_APPLICATION":
                    return $"ISNULL({alias}.[{col}], APP_NAME())";
                case "AUDIT_MACHINE_NAME":
                case "AUDIT_MACHINE":
                    return $"ISNULL({alias}.[{col}], HOST_NAME())";
                case "SEQUENCE_NUMBER":
                    return $"ISNULL({alias}.[{col}], 0) + 1";
                default:
                    return $"{alias}.[{col}]";
            }
        }

        public static string ParameterValue(this Column col)
        {
            switch (col.ColumnName.ToUpper())
            {
                case "AUDIT_START_DATE":
                case "AUDIT_DATE_TIME":
                    return $"ISNULL(@{col}, GETUTCDATE())";
                case "AUDIT_UPDATE_USER":
                case "AUDIT_USER":
                    return $"ISNULL(@{col}, dbo.ALL_UserContextGet())";
                case "AUDIT_APPLICATION_NAME":
                case "AUDIT_APPLICATION":
                    return $"ISNULL(@{col}, APP_NAME())";
                case "AUDIT_MACHINE_NAME":
                case "AUDIT_MACHINE":
                    return $"ISNULL(@{col}, HOST_NAME())";
                case "SEQUENCE_NUMBER":
                    return $"ISNULL(@{col}, 0) + 1";
                default:
                    return $"@{col}";
            }
        }

    }

}