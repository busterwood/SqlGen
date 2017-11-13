namespace SqlGen
{
    public abstract class Generator
    {
        public abstract string Generate(Table table);
        public virtual string GrantType() => "OBJECT";
        public abstract override string ToString();
    }
}
