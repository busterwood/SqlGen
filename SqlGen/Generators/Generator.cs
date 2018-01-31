namespace SqlGen
{
    public abstract class Generator
    {
        public abstract string Generate(Table table, GeneratorOptions options);

        public abstract override string ToString();
    }


    public class GeneratorOptions
    {
        /// <summary>Primary or foreign key to generate for (optional)</summary>
        public TableKey Key { get; set; }

        /// <summary>Generate SQL grant statements?</summary>
        public bool Grant { get; set; }

        /// <summary>Generator alter statements, not create?</summary>
        public bool Alter { get; set; }

        /// <summary>Include or exclude audit column?</summary>
        public bool Audit { get; set; }
    }

    public static partial class Extensions
    {
        public static GeneratorOptions WithKey(this GeneratorOptions options, TableKey key)
        {
            return new GeneratorOptions { Alter = options.Alter, Audit = options.Audit, Grant = options.Grant, Key = key };
        }
    }
}
