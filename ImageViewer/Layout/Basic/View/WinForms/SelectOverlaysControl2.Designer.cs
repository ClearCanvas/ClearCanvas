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
            this._mainTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainTable
            // 
            this._mainTable.AutoSize = true;
            this._mainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._mainTable.ColumnCount = 2;
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._mainTable.Controls.Add(this._close, 0, 1);
            this._mainTable.Controls.Add(this._applyToAll, 1, 1);
            this._mainTable.Controls.Add(this._listOverlays, 0, 0);
            this._mainTable.Location = new System.Drawing.Point(3, 3);
            this._mainTable.Name = "_mainTable";
            this._mainTable.RowCount = 2;
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._mainTable.Size = new System.Drawing.Size(157, 159);
            this._mainTable.TabIndex = 1;
            // 
            // _close
            // 
            this._close.AutoSize = true;
            this._close.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._close.Location = new System.Drawing.Point(3, 133);
            this._close.Name = "_close";
            this._close.Size = new System.Drawing.Size(43, 23);
            this._close.TabIndex = 0;
            this._close.Text = "Close";
            this._close.UseVisualStyleBackColor = true;
            // 
            // _applyToAll
            // 
            this._applyToAll.AutoSize = true;
            this._applyToAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._applyToAll.Location = new System.Drawing.Point(52, 133);
            this._applyToAll.Name = "_applyToAll";
            this._applyToAll.Size = new System.Drawing.Size(102, 23);
            this._applyToAll.TabIndex = 1;
            this._applyToAll.Text = "Apply Everywhere";
            this._applyToAll.UseVisualStyleBackColor = true;
            // 
            // _listOverlays
            // 
            this._listOverlays.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._listOverlays.CheckOnClick = true;
            this._mainTable.SetColumnSpan(this._listOverlays, 2);
            this._listOverlays.FormattingEnabled = true;
            this._listOverlays.Location = new System.Drawing.Point(3, 3);
            this._listOverlays.Name = "_listOverlays";
            this._listOverlays.Size = new System.Drawing.Size(151, 124);
            this._listOverlays.TabIndex = 2;
            this._listOverlays.ThreeDCheckBoxes = true;
            // 
            // SelectOverlaysControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._mainTable);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SelectOverlaysControl2";
            this.Size = new System.Drawing.Size(163, 165);
            this._mainTable.ResumeLayout(false);
            this._mainTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _mainTable;
        private System.Windows.Forms.Button _close;
        private System.Windows.Forms.Button _applyToAll;
        private System.Windows.Forms.CheckedListBox _listOverlays;
    }
}
