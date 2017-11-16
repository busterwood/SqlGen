using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BusterWood.Mapper;

namespace SqlGen
{
    public class TableDataAccess
    {
        readonly DbConnection connection;

        public TableDataAccess(DbConnection connection)
        {
            this.connection = connection;
        }

        const string tableSql = @"select TABLE_SCHEMA, TABLE_NAME
from INFORMATION_SCHEMA.TABLES
where TABLE_NAME NOT LIKE '%_AUDIT'
order by TABLE_SCHEMA, TABLE_NAME";

        public Task<List<Table>> LoadNonAuditTable()
        {
            return connection.QueryAsync(tableSql).ToListAsync<Table>();
        }

        const string columnSql = @"select *, cast(COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') as bit) as [IsIdentity]
from INFORMATION_SCHEMA.COLUMNS 
where table_name = @table 
and TABLE_SCHEMA = @schema
order by ORDINAL_POSITION";

        public List<Column> LoadColumns(string table, string schema = "dbo")
        {
            return connection.Query(columnSql, new { table, schema }).ToList<Column>();
        }

        const string primaryKeySql = @"SELECT tc.CONSTRAINT_NAME, ku.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
where tc.table_name = @table 
and tc.TABLE_SCHEMA = @schema
order by ku.ORDINAL_POSITION";

        public void PopulatePrimaryKey(Table table)
        {
            var colMap = table.Columns.ToDictionary(col => col.ColumnName);
            var pkCols = LoadPrimaryKeyColumns(table.TableName, table.Schema);
            table.PrimaryKey = new PrimaryKey {
                ConstraintName = pkCols.FirstOrDefault()?.ConstraintName,
                KeyColumns = pkCols.Select(pkc => colMap[pkc.ColumnName]).ToList()
            };
        }

        List<KeyColumn> LoadPrimaryKeyColumns(string table, string schema)
        {
            return connection.Query(primaryKeySql, new { table, schema }).ToList<KeyColumn>();
        }

        public void PopulateForeignKeys(Table table)
        {
            var colMap = table.Columns.ToDictionary(col => col.ColumnName);
            var fks = LoadForeignKeyContraints(table.TableName, table.Schema);
            var colsByFkName = LoadForeignKeyColumns(table.TableName, table.Schema);
            foreach (var fk in fks)
            {
                fk.TableColumns = colsByFkName[fk.ConstraintName].Select(c => colMap[c.ColumnName]).ToList();
            }
            table.ForeignKeys = fks;
        }

        const string foreignKeySql = @"select CONSTRAINT_NAME 
from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS as RC
where exists (
	select * 
	from INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1
	where KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG
			AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA
			AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
			and TABLE_SCHEMA = @schema
			and TABLE_NAME = @table
)";

        List<ForeignKey> LoadForeignKeyContraints(string table, string schema)
        {
            return connection.Query(foreignKeySql, new { table, schema }).ToList<ForeignKey>();
        }

        const string foreignKeyColumnSql = @"select *
from INFORMATION_SCHEMA.KEY_COLUMN_USAGE
where TABLE_SCHEMA = @schema and TABLE_NAME = @table";

        HashLookup<string, KeyColumn> LoadForeignKeyColumns(string table, string schema)
        {
            return connection.Query(foreignKeyColumnSql, new { table, schema }).ToLookup<string, KeyColumn>(c => c.ConstraintName);
        }
    }

    public abstract class TableKey : IEnumerable<Column>
    {
        public string ConstraintName { get; set; }
        public abstract IEnumerator<Column> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class PrimaryKey : TableKey
    {
        public List<Column> KeyColumns { get; set; }
        public override IEnumerator<Column> GetEnumerator() => KeyColumns.GetEnumerator();
    }

    public class ForeignKey : TableKey
    {
        public List<Column> TableColumns { get; set; }
        public override IEnumerator<Column> GetEnumerator() => TableColumns.GetEnumerator();
    }

    class KeyColumn : Column
    {
        public string ConstraintName { get; set; }
    }

}
