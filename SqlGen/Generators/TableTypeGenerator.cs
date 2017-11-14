﻿using System;
using System.Linq;
using System.Text;

namespace SqlGen.Generators
{
    class TableTypeGenerator : Generator
    {
        public override string Generate(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TYPE [{table.Schema}].[{table.TableName}_TABLE_TYPE] AS TABLE");
            sb.AppendLine("(");
            sb.AppendLine("    [BULK_SEQ] INT NULL, -- must be set for inserts");
            foreach (var c in table.Columns.Where(c => !c.IsAuditColumn() && c.DataType != "timestamp"))
            {
                var nullDecl = c.IsNullable() || c.IsSequenceNumber() || table.PrimaryKeyColumns.Any(col => col == c) ? "NULL" : "NOT NULL";
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