namespace SqlGen
{
    public abstract class Generator
    {
        public abstract string ObjectName(Table table, ForeignKey fk = null);
        public virtual string Generate(Table table, ForeignKey fk = null) => Generate(table);
        public abstract string Generate(Table table);
        public virtual string GrantType() => "OBJECT";
        public virtual string Grant(Table table, ForeignKey fk = null) => $@"GRANT EXECUTE ON {GrantType()}::{ObjectName(table, fk)} TO [db_execproc];";
        public abstract override string ToString();
    }
    
}
