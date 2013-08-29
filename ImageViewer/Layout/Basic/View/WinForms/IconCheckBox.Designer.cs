namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class IconCheckBox
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
            this._layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._checkLabel = new System.Windows.Forms.Label();
            this._check = new System.Windows.Forms.CheckBox();
            this._icon = new System.Windows.Forms.PictureBox();
            this._layoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._icon)).BeginInit();
            this.SuspendLayout();
            // 
            // _layoutPanel
            // 
            this._layoutPanel.AutoSize = true;
            this._layoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._layoutPanel.ColumnCount = 3;
            this._layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._layoutPanel.Controls.Add(this._checkLabel, 2, 0);
            this._layoutPanel.Controls.Add(this._check, 0, 0);
            this._layoutPanel.Controls.Add(this._icon, 1, 0);
            this._layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._layoutPanel.Location = new System.Drawing.Point(0, 0);
            this._layoutPanel.Name = "_layoutPanel";
            this._layoutPanel.RowCount = 1;
            this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layoutPanel.Size = new System.Drawing.Size(105, 26);
            this._layoutPanel.TabIndex = 0;
            // 
            // _checkLabel
            // 
            this._checkLabel.AutoSize = true;
            this._checkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._checkLabel.Location = new System.Drawing.Point(47, 0);
            this._checkLabel.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this._checkLabel.Name = "_checkLabel";
            this._checkLabel.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this._checkLabel.Size = new System.Drawing.Size(57, 26);
            this._checkLabel.TabIndex = 2;
            this._checkLabel.Text = "CheckBox";
            this._checkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _check
            // 
            this._check.AutoSize = true;
            this._check.Dock = System.Windows.Forms.DockStyle.Fill;
            this._check.Location = new System.Drawing.Point(5, 1);
            this._check.Margin = new System.Windows.Forms.Padding(5, 1, 1, 1);
            this._check.Name = "_check";
            this._check.Size = new System.Drawing.Size(15, 24);
            this._check.TabIndex = 0;
            this._check.UseVisualStyleBackColor = true;
            // 
            // _icon
            // 
            this._icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._icon.Location = new System.Drawing.Point(22, 1);
            this._icon.Margin = new System.Windows.Forms.Padding(1);
            this._icon.Name = "_icon";
            this._icon.Size = new System.Drawing.Size(24, 24);
            this._icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._icon.TabIndex = 1;
            this._icon.TabStop = false;
            // 
            // IconCheckBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._layoutPanel);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "IconCheckBox";
            this.Size = new System.Drawing.Size(105, 26);
            this._layoutPanel.ResumeLayout(false);
            this._layoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _layoutPanel;
        private System.Windows.Forms.CheckBox _check;
        private System.Windows.Forms.PictureBox _icon;
        private System.Windows.Forms.Label _checkLabel;
    }
}
