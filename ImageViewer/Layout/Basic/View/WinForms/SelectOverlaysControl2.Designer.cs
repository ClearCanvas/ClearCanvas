namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class SelectOverlaysControl2
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._mainTable = new System.Windows.Forms.TableLayoutPanel();
            this._close = new System.Windows.Forms.Button();
            this._applyToAll = new System.Windows.Forms.Button();
            this._listOverlays = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this._mainTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainTable
            // 
            this._mainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._mainTable.ColumnCount = 2;
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._mainTable.Controls.Add(this._close, 0, 1);
            this._mainTable.Controls.Add(this._listOverlays, 0, 1);
            this._mainTable.Controls.Add(this.label1, 0, 0);
            this._mainTable.Controls.Add(this._applyToAll, 1, 2);
            this._mainTable.Location = new System.Drawing.Point(0, 3);
            this._mainTable.Name = "_mainTable";
            this._mainTable.RowCount = 3;
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._mainTable.Size = new System.Drawing.Size(148, 178);
            this._mainTable.TabIndex = 1;
            // 
            // _close
            // 
            this._close.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._close.Location = new System.Drawing.Point(3, 152);
            this._close.Name = "_close";
            this._close.Size = new System.Drawing.Size(43, 23);
            this._close.TabIndex = 0;
            this._close.Text = "Close";
            this._close.UseVisualStyleBackColor = true;
            // 
            // _applyToAll
            // 
            this._applyToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._applyToAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._applyToAll.Location = new System.Drawing.Point(52, 152);
            this._applyToAll.Name = "_applyToAll";
            this._applyToAll.Size = new System.Drawing.Size(93, 23);
            this._applyToAll.TabIndex = 1;
            this._applyToAll.Text = "Apply to all";
            this._applyToAll.UseVisualStyleBackColor = true;
            // 
            // _listOverlays
            // 
            this._listOverlays.CheckOnClick = true;
            this._mainTable.SetColumnSpan(this._listOverlays, 2);
            this._listOverlays.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listOverlays.FormattingEnabled = true;
            this._listOverlays.Location = new System.Drawing.Point(3, 32);
            this._listOverlays.Name = "_listOverlays";
            this._listOverlays.Size = new System.Drawing.Size(142, 114);
            this._listOverlays.TabIndex = 2;
            this._listOverlays.ThreeDCheckBoxes = true;
            // 
            // label1
            // 
            this._mainTable.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Image box overlays";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SelectOverlaysControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._mainTable);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SelectOverlaysControl2";
            this.Size = new System.Drawing.Size(151, 184);
            this._mainTable.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _mainTable;
        private System.Windows.Forms.Button _close;
        private System.Windows.Forms.Button _applyToAll;
        private System.Windows.Forms.CheckedListBox _listOverlays;
        private System.Windows.Forms.Label label1;
    }
}
