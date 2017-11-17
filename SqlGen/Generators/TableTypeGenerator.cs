using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableTypeGenerator : SqlGenerator
    {
        public override string ObjectName(Table table, TableKey key = null)
        {
            if (key == null)
                return $"[{table.Schema}].[{table.TableName}_TABLE_TYPE]";
            if (key.Count() == 1)
                return $"[{table.Schema}].[{key.First().ColumnName.ToUpper()}_TABLE_TYPE]";
            return $"[{table.Schema}].[{key.ConstraintName.ToUpper()}_TABLE_TYPE]";
        }

        string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TYPE {ObjectName(table)} AS TABLE");
            sb.AppendLine("(");
            sb.AppendLine("    [BULK_SEQ] INT NULL, -- must be set for inserts");
            foreach (var c in table.Columns.Where(c => c.DataType != "timestamp"))
            {
                var nullDecl = c.IsNullable() || c.IsSequenceNumber() || table.PrimaryKey.Any(col => col == c) ? "NULL" : "NOT NULL";
                sb.AppendLine($"    [{c.ColumnName}] {c.TypeDeclaration()} {nullDecl},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(")");

            return sb.ToString();
        }

        public override string Generate(Table table, TableKey key, bool alter)
        {
            if (key == null)
                return Generate(table);

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TYPE {ObjectName(table, key)} AS TABLE");
            sb.AppendLine("(");
            foreach (var c in key)
            {
                var nullDecl = c.IsNullable() || c.IsSequenceNumber() || table.PrimaryKey.Any(col => col == c) ? "NULL" : "NOT NULL";
                sb.AppendLine($"    [{c.ColumnName}] {c.TypeDeclaration()} {nullDecl},");
            }
            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(")");

            return sb.ToString();
        }

        public override string GrantType() => "TYPE";
        public override string ToString() => "Table Type";
    }
}
