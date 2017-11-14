﻿namespace SqlGenUI
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
            this.sqlTextBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGrantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.codeList = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fkList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.sqlTextBox.Location = new System.Drawing.Point(0, 209);
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.ReadOnly = true;
            this.sqlTextBox.Size = new System.Drawing.Size(663, 289);
            this.sqlTextBox.TabIndex = 1;
            this.sqlTextBox.Text = "";
            this.sqlTextBox.WordWrap = false;
            this.sqlTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sqlTextBox_MouseDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 501);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(663, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(648, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(663, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.databaseToolStripMenuItem.Text = "Database";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGrantToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // addGrantToolStripMenuItem
            // 
            this.addGrantToolStripMenuItem.Checked = true;
            this.addGrantToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.addGrantToolStripMenuItem.Name = "addGrantToolStripMenuItem";
            this.addGrantToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addGrantToolStripMenuItem.Text = "Add Grant";
            this.addGrantToolStripMenuItem.Click += new System.EventHandler(this.addGrantToolStripMenuItem_Click);
            // 
            // tableList
            // 
            this.tableList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.tableList.FullRowSelect = true;
            this.tableList.HideSelection = false;
            this.tableList.Location = new System.Drawing.Point(12, 28);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(209, 175);
            this.tableList.TabIndex = 6;
            this.tableList.UseCompatibleStateImageBehavior = false;
            this.tableList.View = System.Windows.Forms.View.Details;
            this.tableList.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Tables / Views";
            this.columnHeader1.Width = 205;
            // 
            // codeList
            // 
            this.codeList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.codeList.FullRowSelect = true;
            this.codeList.HideSelection = false;
            this.codeList.Location = new System.Drawing.Point(442, 27);
            this.codeList.Name = "codeList";
            this.codeList.Size = new System.Drawing.Size(209, 175);
            this.codeList.TabIndex = 7;
            this.codeList.UseCompatibleStateImageBehavior = false;
            this.codeList.View = System.Windows.Forms.View.Details;
            this.codeList.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Code";
            this.columnHeader2.Width = 205;
            // 
            // fkList
            // 
            this.fkList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.fkList.FullRowSelect = true;
            this.fkList.HideSelection = false;
            this.fkList.Location = new System.Drawing.Point(227, 27);
            this.fkList.Name = "fkList";
            this.fkList.Size = new System.Drawing.Size(209, 175);
            this.fkList.TabIndex = 8;
            this.fkList.UseCompatibleStateImageBehavior = false;
            this.fkList.View = System.Windows.Forms.View.Details;
            this.fkList.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Foreign Keys";
            this.columnHeader3.Width = 205;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 523);
            this.Controls.Add(this.fkList);
            this.Controls.Add(this.codeList);
            this.Controls.Add(this.tableList);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.sqlTextBox);
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
        private System.Windows.Forms.RichTextBox sqlTextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ListView tableList;
        private System.Windows.Forms.ListView codeList;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addGrantToolStripMenuItem;
        private System.Windows.Forms.ListView fkList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
