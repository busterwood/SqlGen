using System.Collections.Generic;
using System.Linq;

namespace SqlGen
{
    public class Table
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public List<Column> Columns { get; set; }
        public List<Column> PrimaryKeyColumns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }
        public IEnumerable<Column> InsertableColumns => Columns.Where(c => !c.IsIdentity && !c.IsRowVersion());
        public override string ToString() => $"{Schema}.{TableName}";
    }
}