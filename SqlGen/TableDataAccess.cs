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

        const string primaryKeySql = @"SELECT ku.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
where tc.table_name = @table 
and tc.TABLE_SCHEMA = @schema
order by ku.ORDINAL_POSITION";

        public List<Column> LoadPrimaryKeyColumns(string table, string schema = "dbo")
        {
            return connection.Query(primaryKeySql, new { table, schema }).ToList<Column>();
        }

        public async Task<List<ForeignKey>> LoadForeignKeys(string table, string schema = "dbo")
        {
            var fks = await LoadForeignKeyContraints(table, schema);
            var cols = await LoadForeignKeyColumns(table, schema);
            foreach (var fk in fks)
            {
                fk.TableColumns = cols[fk.ConstraintName].Cast<Column>().ToList();
            }
            return fks;
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

        Task<List<ForeignKey>> LoadForeignKeyContraints(string table, string schema)
        {
            return connection.QueryAsync(foreignKeySql, new { table, schema }).ToListAsync<ForeignKey>();
        }

        const string foreignKeyColumnSql = @"select *
from INFORMATION_SCHEMA.KEY_COLUMN_USAGE
where TABLE_SCHEMA = @schema and TABLE_NAME = @table";

        Task<HashLookup<string, ForeignKeyColumn>> LoadForeignKeyColumns(string table, string schema)
        {
            return connection.QueryAsync(foreignKeyColumnSql, new { table, schema }).ToLookupAsync<string, ForeignKeyColumn>(c => c.ConstraintName);
        }
    }

    public class ForeignKey
    {
        public string ConstraintName { get; set; }
        public List<Column> TableColumns { get; set; }
        //public List<Column> UniqueColumns { get; set; }
    }

    class ForeignKeyColumn : Column
    {
        public string ConstraintName { get; set; }
    }

}
