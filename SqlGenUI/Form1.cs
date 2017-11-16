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
        List<Table> _allTables;
        List<Table> _visibleTables;
        ListViewItem[] _tableItems;
        List<Table> _selectedTables;

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
                BeginInvoke((Action<List<Table>>)PopulateTableList, tables);
            }
        }

        void PopulateTableList(List<Table> tables)
        {
            _allTables = tables;
            var filter = CheckedSchema;
            var schemas = tables.Select(t => t.Schema).Distinct();
            schemaToolStripMenuItem.DropDownItems.Clear();
            schemaToolStripMenuItem.DropDownItems.AddRange(schemas.Select(s => new ToolStripMenuItem(s, null, schema_OnClick) { Checked = s == filter }).ToArray());
            SetSchemaFilter();
        }

        private void SetSchemaFilter()
        {
            var filter = CheckedSchema;

            if (filter == null)
                _visibleTables = _allTables;
            else
                _visibleTables = _allTables.Where(t => t.Schema == filter).ToList();

            _tableItems = new ListViewItem[_visibleTables.Count];
            tableList.VirtualListSize = 0;
            tableList.SelectedIndices.Clear();
            tableList.Refresh();
            tableList.VirtualListSize = _visibleTables.Count;
            tableList.EnsureVisible(0);
            sqlTextBox.Text = "";
        }

        void schema_OnClick(object sender, EventArgs args)
        {
            CheckedSchema = ((ToolStripMenuItem)sender).Text;
            SetSchemaFilter();
        }

        private void tableList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs args)
        {
            var lvi = _tableItems[args.ItemIndex];
            if (lvi == null)
            {
                var t = _visibleTables[args.ItemIndex];
                lvi = new ListViewItem(t.ToString()) { Tag = t };
                _tableItems[args.ItemIndex] = lvi;
            }
            args.Item = lvi;
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
                    LoadColumnsAndPrimaryKey(table);

                foreach (var fk in fks)
                {
                    foreach (var gen in SelectedCodeGenerators())
                    {
                        sb.AppendLine(gen.Generate(table, fk));
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

        private void LoadColumnsAndPrimaryKey(Table table)
        {
            ConnectionStringSettings connectionSettings = CheckConnectionString();
            using (var cnn = new SqlConnection(connectionSettings.ConnectionString))
            {
                var da = new TableDataAccess(cnn);
                table.Columns = da.LoadColumns(table.TableName, table.Schema);
                da.PopulatePrimaryKey(table);
            }
        }

        string CheckedSchema
        {
            get { return schemaToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>().FirstOrDefault(mi => mi.Checked)?.Text;  }
            set
            {
                foreach (var mi in schemaToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    mi.Checked = mi.Text == value ? !mi.Checked : false;
                }
            }
        }

        private IEnumerable<Generator> SelectedCodeGenerators()
        {
            return codeList.SelectedItems.Cast<ListViewItem>().Select(lvi => lvi.Tag as Generator);
        }

        private IEnumerable<Table> SelectedTables() => tableList.SelectedIndices.Cast<int>().Select(i => _visibleTables[i]);

        private IEnumerable<TableKey> SelectedForeignKeys() => fkList.SelectedItems.Cast<ListViewItem>().Select(lvi => lvi.Tag as TableKey);

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
                LoadForeignKeysAsync(SelectedTables().FirstOrDefault());
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
                    da.PopulatePrimaryKey(table);
                }
                da.PopulateForeignKeys(table);
            }
            BeginInvoke((Action<Table>)PopulateForeignKeyList, table);
        }

        void PopulateForeignKeyList(Table table)
        {
            fkList.Items.Clear();
            fkList.Items.Add(new ListViewItem { Text = table.PrimaryKey.ConstraintName, Tag=table.PrimaryKey });
            fkList.Items.AddRange(table.ForeignKeys.Select(fk => new ListViewItem { Text = fk.ConstraintName, Tag = fk }).ToArray());
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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menu = databaseToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>().Single(mi => mi.Checked);
            RefreshFromDb((ConnectionStringSettings)menu.Tag);
        }

        private void codeList_MouseDown(object sender, MouseEventArgs e)
        {
            sqlTextBox.DoDragDrop(sqlTextBox.Text, DragDropEffects.Copy);
        }

        private void tableList_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs args)
        {
            for (int i = args.StartIndex; i < _visibleTables.Count; i++)
            {
                var table = _visibleTables[i];
                if (table.TableName.IndexOf(args.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    args.Index = i;
                    break;
                }
            }
        }
    }
}
