namespace SqlGenUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableCombo = new System.Windows.Forms.ComboBox();
            this.sqlTextBox = new System.Windows.Forms.RichTextBox();
            this.codeCombo = new System.Windows.Forms.ComboBox();
            this.addGrantCheckBox = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableCombo
            // 
            this.tableCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tableCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.tableCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tableCombo.FormattingEnabled = true;
            this.tableCombo.Location = new System.Drawing.Point(12, 27);
            this.tableCombo.Name = "tableCombo";
            this.tableCombo.Size = new System.Drawing.Size(250, 21);
            this.tableCombo.TabIndex = 0;
            this.tableCombo.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.sqlTextBox.Location = new System.Drawing.Point(0, 54);
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.ReadOnly = true;
            this.sqlTextBox.Size = new System.Drawing.Size(724, 317);
            this.sqlTextBox.TabIndex = 1;
            this.sqlTextBox.Text = "";
            this.sqlTextBox.WordWrap = false;
            this.sqlTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sqlTextBox_MouseDown);
            // 
            // codeCombo
            // 
            this.codeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.codeCombo.FormattingEnabled = true;
            this.codeCombo.Location = new System.Drawing.Point(268, 27);
            this.codeCombo.Name = "codeCombo";
            this.codeCombo.Size = new System.Drawing.Size(250, 21);
            this.codeCombo.TabIndex = 2;
            this.codeCombo.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            // 
            // addGrantCheckBox
            // 
            this.addGrantCheckBox.AutoSize = true;
            this.addGrantCheckBox.Checked = true;
            this.addGrantCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.addGrantCheckBox.Location = new System.Drawing.Point(537, 29);
            this.addGrantCheckBox.Name = "addGrantCheckBox";
            this.addGrantCheckBox.Size = new System.Drawing.Size(74, 17);
            this.addGrantCheckBox.TabIndex = 3;
            this.addGrantCheckBox.Text = "Add Grant";
            this.addGrantCheckBox.UseVisualStyleBackColor = true;
            this.addGrantCheckBox.CheckedChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 374);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(724, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(724, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.databaseToolStripMenuItem.Text = "Database";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(678, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 396);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.addGrantCheckBox);
            this.Controls.Add(this.codeCombo);
            this.Controls.Add(this.sqlTextBox);
            this.Controls.Add(this.tableCombo);
            this.Name = "Form1";
            this.Text = "Sql Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox tableCombo;
        private System.Windows.Forms.RichTextBox sqlTextBox;
        private System.Windows.Forms.ComboBox codeCombo;
        private System.Windows.Forms.CheckBox addGrantCheckBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

