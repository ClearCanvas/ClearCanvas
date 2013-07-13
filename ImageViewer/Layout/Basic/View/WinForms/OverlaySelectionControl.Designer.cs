namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class OverlaySelectionControl
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
            this._table = new System.Windows.Forms.TableLayoutPanel();
            this._check = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _table
            // 
            this._table.AutoSize = true;
            this._table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._table.ColumnCount = 1;
            this._table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._table.Location = new System.Drawing.Point(3, 3);
            this._table.Name = "_table";
            this._table.RowCount = 1;
            this._table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._table.Size = new System.Drawing.Size(0, 0);
            this._table.TabIndex = 0;
            // 
            // _check
            // 
            this._check.AutoSize = true;
            this._check.Location = new System.Drawing.Point(3, 3);
            this._check.Name = "_check";
            this._check.Size = new System.Drawing.Size(93, 17);
            this._check.TabIndex = 1;
            this._check.Text = "Overlay Name";
            this._check.UseVisualStyleBackColor = true;
            // 
            // OverlaySelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._check);
            this.Controls.Add(this._table);
            this.Name = "OverlaySelectionControl";
            this.Size = new System.Drawing.Size(99, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _table;
        private System.Windows.Forms.CheckBox _check;
    }
}
