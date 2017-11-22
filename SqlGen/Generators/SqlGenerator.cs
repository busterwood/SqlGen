using System;
using System.Text;

namespace SqlGen
{
    public abstract class SqlGenerator : Generator
    {
        public abstract string ObjectName(Table table, TableKey fk = null);

        public virtual string GrantType() => "OBJECT";

        public virtual string BatchSeparator() => "GO" + Environment.NewLine + Environment.NewLine;

        public virtual string Grant(Table table, TableKey key = null)
        {
            return $@"GRANT EXECUTE ON {GrantType()}::{ObjectName(table, key)} TO [db_execproc] AS [dbo];";
        }

        protected void AppendCreateOrAlterProc(Table table, TableKey key, bool alter, StringBuilder sb)
        {
            AppendCreateOrAlterProc(ObjectName(table, key), alter, sb);
        }

        protected void AppendCreateOrAlterProc(string procName, bool alter, StringBuilder sb)
        {
            if (alter)
            {
                var objectIdName = procName.Replace("[", "").Replace("]", "");
                sb.AppendLine($"IF OBJECT_ID('{objectIdName}', 'P') IS NULL");
                sb.AppendLine($"    EXEC('CREATE PROCEDURE {procName} AS SELECT 1')");
                sb.AppendLine("GO");
                sb.AppendLine();
                sb.AppendLine($"ALTER PROCEDURE {procName}");
            }
            else
                sb.AppendLine($"CREATE PROCEDURE {procName}");
        }
    }
}