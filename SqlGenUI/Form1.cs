using SqlGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlGenUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            databaseToolStripMenuItem.DropDownItems.AddRange(
                ConfigurationManager.ConnectionStrings
                    .Cast<ConnectionStringSettings>()
                    .Select(cs => new ToolStripMenuItem(cs.Name, null, DatabaseChanged) { Tag = cs, Checked = cs.Name=="dev" })
                    .ToArray<ToolStripItem>()
            );

            RefreshFromDb(ConfigurationManager.ConnectionStrings["local"]);
        }

        private void DatabaseChanged(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in databaseToolStripMenuItem.DropDownItems)
                mi.Checked = false;

            var menu = (ToolStripMenuItem)sender;
            menu.Checked = true;
            RefreshFromDb((ConnectionStringSettings)menu.Tag);
        }

        private void RefreshFromDb(ConnectionStringSettings settings)
        {
            tableCombo.Items.Clear();
            RunLoadTablesAsync(settings.ConnectionString);

            toolStripStatusLabel1.Text = new SqlConnectionStringBuilder(settings.ConnectionString) { Password = "xxx" }.ToString();                      

            var generators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => !t.IsAbstract && typeof(Generator).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t))
                .OrderBy(g => g.ToString());

            codeCombo.Items.Clear();
            codeCombo.Items.AddRange(generators.ToArray());
        }

        async Task RunLoadTablesAsync(string connectionString)
        {
            using (var cnn = new SqlConnection(connectionString))
            {
                var da = new TableDataAccess(cnn);
                var tables = await da.LoadNonAuditTable();
                BeginInvoke((Action<List<Table>>)SetTablesCombo, tables);
            }
        }

        void SetTablesCombo(List<Table> tables)
        {
            tableCombo.Items.AddRange(tables.ToArray());
            tableCombo.Focus();
        }

        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var table = tableCombo.SelectedItem as Table;
            var gen = codeCombo.SelectedItem as Generator;
            if (table == null || gen == null)
            {
                sqlTextBox.Text = "";
                return;
            }

            if (table.Columns == null)
            {
                var cs = ConfigurationManager.ConnectionStrings["dev"].ConnectionString;
                using (var cnn = new SqlConnection(cs))
                {
                    var da = new TableDataAccess(cnn);
                    table.Columns = da.LoadColumns(table.TableName, table.Schema);
                    var pks = da.LoadPrimaryKeyColumns(table.TableName, table.Schema);
                    table.PrimaryKeyColumns = pks.Select(pkc => table.Columns.Single(c => c.ColumnName == pkc.ColumnName)).ToList();
                }

            }

            sqlTextBox.Text = gen.Generate(table);

            if (addGrantCheckBox.Checked)
            {
                sqlTextBox.Text += $@"
GO

GRANT EXECUTE ON {gen.GrantType()}::[{table.Schema}].[{table.TableName}] TO [db_execproc] AS [dbo];";
            }
        }

        private void sqlTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            sqlTextBox.DoDragDrop(sqlTextBox.Text, DragDropEffects.Copy);
        }
    }
}
