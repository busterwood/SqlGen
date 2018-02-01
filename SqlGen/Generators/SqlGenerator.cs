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

        protected void AppendCreateOrAlterProc(Table table, GeneratorOptions options, StringBuilder sb)
        {
            AppendCreateOrAlterProc(ObjectName(table, options.Key), options.Alter, sb);
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

        protected void AppendCreateOrAlterTrigger(Table table, bool alter, string action, StringBuilder sb)
        {
            if (alter)
            {
                var objectIdName = ObjectName(table).Replace("[", "").Replace("]", "");
                sb.AppendLine($"IF OBJECT_ID('{objectIdName}', 'TR') IS NULL");
                sb.AppendLine($"    EXEC('CREATE TRIGGER {ObjectName(table)} ON [{table.Schema}].[{table.TableName}] FOR {action} AS BEGIN END')");
                sb.AppendLine("GO");
                sb.AppendLine();
                sb.Append("ALTER");
            }
            else
                sb.Append("CREATE");
            sb.AppendLine($" TRIGGER {ObjectName(table)} ON [{table.Schema}].[{table.TableName}]");
        }

    }
}