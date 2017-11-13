using BusterWood.Mapper;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

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
    }



}
