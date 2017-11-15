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
                    .Select(cs => new ToolStripMenuItem(cs.Name, null, DatabaseChanged) { Tag = cs, Checked = cs.Name == "local" })
                    .ToArray<ToolStripItem>()
            );

            ResizeListHeaders();

            RefreshFromDb(ConfigurationManager.ConnectionStrings["local"]);
        }

        private void ResizeListHeaders()
        {
            tableList.Columns[0].Width = tableList.Width - 4 - SystemInformation.VerticalScrollBarWidth;
            fkList.Columns[0].Width = fkList.Width - 4 - SystemInformation.VerticalScrollBarWidth;
            codeList.Columns[0].Width = codeList.Width - 4 - SystemInformation.VerticalScrollBarWidth;
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
            tableList.Items.Clear();
            RunLoadTablesAsync(settings.ConnectionString);

            toolStripStatusLabel1.Text = new SqlConnectionStringBuilder(settings.ConnectionString) { Password = "xxx" }.ToString();                      

            var generators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => !t.IsAbstract && typeof(Generator).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t))
                .Select(gen => new ListViewItem { Text=gen.ToString(), Tag=gen })
                .OrderBy(lvi => lvi.Text);

            codeList.Items.Clear();
            codeList.Items.AddRange(generators.ToArray());
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
            tableList.Items.AddRange(tables.Select(t => new ListViewItem { Text=t.ToString(), Tag=t }).ToArray());
            tableList.Focus();
        }

        private void GenerateSql()
        {
            sqlTextBox.Text = "";

            var sb = new StringBuilder();

            var fks = SelectedForeignKeys().ToList();
            if (fks.Count == 0)
                fks.Add(null);

            foreach (var table in SelectedTables())
            {
                if (table.Columns == null)
                    LoadFillTableDetails(table);

                foreach (var fk in fks)
                {
                    foreach (var gen in SelectedCodeGenerators())
                    {
                        sb.AppendLine(gen.Generate(table, SelectedForeignKey()));
                        sb.AppendLine("GO");
                        sb.AppendLine();
                        if (addGrantToolStripMenuItem.Checked)
                        {
                            sb.AppendLine(gen.Grant(table, fk));
                            sb.AppendLine("GO");
                            sb.AppendLine();
                        }
                    }
                }
            }

            sqlTextBox.Text = sb.ToString();
        }

        private void LoadFillTableDetails(Table table)
        {
            ConnectionStringSettings connectionSettings = CheckConnectionString();
            using (var cnn = new SqlConnection(connectionSettings.ConnectionString))
            {
                var da = new TableDataAccess(cnn);
                table.Columns = da.LoadColumns(table.TableName, table.Schema);
                var pks = da.LoadPrimaryKeyColumns(table.TableName, table.Schema);
                table.PrimaryKeyColumns = pks.Select(pkc => table.Columns.Single(c => c.ColumnName == pkc.ColumnName)).ToList();
            }
        }

        private Generator SelectedCodeGenerator() => codeList.SelectedItems.Cast<ListViewItem>().FirstOrDefault()?.Tag as Generator;
        private IEnumerable<Generator> SelectedCodeGenerators() => codeList.SelectedItems.Cast<ListViewItem>().Select(lvi => lvi.Tag as Generator);

        private Table SelectedTable() => tableList.SelectedItems.Cast<ListViewItem>().FirstOrDefault()?.Tag as Table;
        private IEnumerable<Table> SelectedTables() => tableList.SelectedItems.Cast<ListViewItem>().Select(lvi => lvi.Tag as Table);

        private ForeignKey SelectedForeignKey() => fkList.SelectedItems.Cast<ListViewItem>().FirstOrDefault()?.Tag as ForeignKey;
        private IEnumerable<ForeignKey> SelectedForeignKeys() => fkList.SelectedItems.Cast<ListViewItem>().Select(lvi => lvi.Tag as ForeignKey);

        private ConnectionStringSettings CheckConnectionString()
        {
            var dbMenu = databaseToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().FirstOrDefault(mi => mi.Checked);
            return (ConnectionStringSettings)dbMenu.Tag;
        }

        private void sqlTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            sqlTextBox.DoDragDrop(sqlTextBox.Text, DragDropEffects.Copy);
        }

        private void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == tableList)
                LoadForeignKeysAsync(SelectedTable());
            GenerateSql();
        }

        private async Task LoadForeignKeysAsync(Table table)
        {
            fkList.Items.Clear();
            if (table == null)
                return;

            await Task.Yield();
            ConnectionStringSettings connectionSettings = CheckConnectionString();
            using (var cnn = new SqlConnection(connectionSettings.ConnectionString))
            {
                var da = new TableDataAccess(cnn);
                if (table.Columns == null)
                {
                    table.Columns = da.LoadColumns(table.TableName, table.Schema);
                    var pks = da.LoadPrimaryKeyColumns(table.TableName, table.Schema);
                    table.PrimaryKeyColumns = pks.Select(pkc => table.Columns.Single(c => c.ColumnName == pkc.ColumnName)).ToList();
                }
                table.ForeignKeys = await da.LoadForeignKeys(table.TableName, table.Schema);
                ReplaceFKColumnsWithTableColumns(table);
            }
            BeginInvoke((Action<List<ForeignKey>>)PopulateForeignKeyList, table.ForeignKeys);
        }

        private void ReplaceFKColumnsWithTableColumns(Table table)
        {
            foreach (var fk in table.ForeignKeys)
            {
                fk.TableColumns = fk.TableColumns.Select(c => table.Columns.Single(col => string.Equals(col.ColumnName, c.ColumnName, StringComparison.Ordinal))).ToList();
            }
        }

        void PopulateForeignKeyList(List<ForeignKey> fks)
        {
            fkList.Items.Clear();
            fkList.Items.AddRange(fks.Select(fk => new ListViewItem { Text = fk.ConstraintName, Tag = fk }).ToArray());
        }

        private void addGrantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addGrantToolStripMenuItem.Checked = !addGrantToolStripMenuItem.Checked;
            GenerateSql();
        }

        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            ResizeListHeaders();
        }
    }
}
