namespace AbstractSyntax.Visualizer
{
    partial class SyntaxVisualizerForm
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
            this.syntaxTree = new System.Windows.Forms.TreeView();
            this.valueList = new System.Windows.Forms.ListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // syntaxTree
            // 
            this.syntaxTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syntaxTree.FullRowSelect = true;
            this.syntaxTree.HideSelection = false;
            this.syntaxTree.Location = new System.Drawing.Point(0, 0);
            this.syntaxTree.Name = "syntaxTree";
            this.syntaxTree.Size = new System.Drawing.Size(602, 661);
            this.syntaxTree.TabIndex = 0;
            this.syntaxTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AddSelectHandler);
            // 
            // valueList
            // 
            this.valueList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.valueList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.value});
            this.valueList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueList.FullRowSelect = true;
            this.valueList.GridLines = true;
            this.valueList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.valueList.LabelWrap = false;
            this.valueList.Location = new System.Drawing.Point(0, 0);
            this.valueList.MultiSelect = false;
            this.valueList.Name = "valueList";
            this.valueList.Size = new System.Drawing.Size(603, 661);
            this.valueList.TabIndex = 1;
            this.valueList.UseCompatibleStateImageBehavior = false;
            this.valueList.View = System.Windows.Forms.View.Details;
            this.valueList.ItemActivate += new System.EventHandler(this.ItemActivateHandler);
            // 
            // name
            // 
            this.name.Tag = "";
            this.name.Text = "名前";
            this.name.Width = 169;
            // 
            // value
            // 
            this.value.Tag = "";
            this.value.Text = "値";
            this.value.Width = 423;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.syntaxTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.valueList);
            this.splitContainer1.Size = new System.Drawing.Size(1217, 665);
            this.splitContainer1.SplitterDistance = 606;
            this.splitContainer1.TabIndex = 2;
            // 
            // SyntaxVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 665);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SyntaxVisualizerForm";
            this.Text = "SyntaxVisualizerForm";
            this.Load += new System.EventHandler(this.LoadHandler);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView syntaxTree;
        private System.Windows.Forms.ListView valueList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader value;
    }
}