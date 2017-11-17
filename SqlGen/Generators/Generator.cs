using System;

namespace SqlGen
{
    public abstract class Generator
    {
        public virtual string Generate(Table table, TableKey fk) => Generate(table);

        public abstract string Generate(Table table);

        public abstract override string ToString();
    }

    public abstract class SqlGenerator : Generator
    {
        public abstract string ObjectName(Table table, TableKey fk = null);

        public virtual string GrantType() => "OBJECT";

        public virtual string BatchSeparator() => "GO" + Environment.NewLine + Environment.NewLine;

        public virtual string Grant(Table table, TableKey fk = null)
        {
            var gt = GrantType();
            if (gt == null)
                return null;
            return $@"GRANT EXECUTE ON {GrantType()}::{ObjectName(table, fk)} TO [db_execproc];";
        }
    }

}
