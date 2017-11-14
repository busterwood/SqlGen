namespace SqlGen
{
    public abstract class Generator
    {
        public abstract string ObjectName(Table table, ForeignKey fk = null);
        public virtual string Generate(Table table, ForeignKey fk = null) => Generate(table);
        public abstract string Generate(Table table);
        public virtual string GrantType() => "OBJECT";
        public abstract override string ToString();
    }
    
}
