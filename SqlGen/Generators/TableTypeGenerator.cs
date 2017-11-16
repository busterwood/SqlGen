using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableTypeGenerator : Generator
    {
        public override string ObjectName(Table table, TableKey fk = null)
        {
            if (fk == null)
                return $"[{table.Schema}].[{table.TableName}_TABLE_TYPE]";
            if (fk.Count() == 1)
                return $"[{table.Schema}].[{fk.First().ColumnName.ToUpper()}_TABLE_TYPE]";
            return $"[{table.Schema}].[{fk.ConstraintName.ToUpper()}_TABLE_TYPE]";
        }

        public override string Generate(Table table)
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

        public override string Generate(Table table, TableKey fk)
        {
            if (fk == null)
                return Generate(table);

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TYPE {ObjectName(table, fk)} AS TABLE");
            sb.AppendLine("(");
            foreach (var c in fk)
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
