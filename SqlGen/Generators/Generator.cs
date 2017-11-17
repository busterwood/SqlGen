using System;
using System.Text;

namespace SqlGen
{
    public abstract class Generator
    {
        public abstract string Generate(Table table, TableKey fk, bool alter = false);

        public abstract override string ToString();
    }

}
