
namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    partial class CountRateComponentControl
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
            this.TitleLabel = new System.Windows.Forms.Label();
            this.TotalCountsLabel = new System.Windows.Forms.Label();
            this.TotalCountsBox = new System.Windows.Forms.TextBox();
            this.DurationLabel = new System.Windows.Forms.Label();
            this.DurationBox = new System.Windows.Forms.TextBox();
            this.CountRateLabel = new System.Windows.Forms.Label();
            this.CountRateBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._referenceActivityLabel = new System.Windows.Forms.Label();
            this._referenceActivityBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._currentActivityBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._referenceDateBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._sensitivityBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._acquisitionDateBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(15, 18);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(103, 24);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Count Rate";
            this.TitleLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // TotalCountsLabel
            // 
            this.TotalCountsLabel.AutoSize = true;
            this.TotalCountsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalCountsLabel.Location = new System.Drawing.Point(16, 67);
            this.TotalCountsLabel.Name = "TotalCountsLabel";
            this.TotalCountsLabel.Size = new System.Drawing.Size(88, 17);
            this.TotalCountsLabel.TabIndex = 1;
            this.TotalCountsLabel.Text = "Total Counts";
            // 
            // TotalCountsBox
            // 
            this.TotalCountsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalCountsBox.Location = new System.Drawing.Point(190, 61);
            this.TotalCountsBox.Name = "TotalCountsBox";
            this.TotalCountsBox.Size = new System.Drawing.Size(131, 23);
            this.TotalCountsBox.TabIndex = 2;
            // 
            // DurationLabel
            // 
            this.DurationLabel.AutoSize = true;
            this.DurationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationLabel.Location = new System.Drawing.Point(16, 116);
            this.DurationLabel.Name = "DurationLabel";
            this.DurationLabel.Size = new System.Drawing.Size(134, 17);
            this.DurationLabel.TabIndex = 3;
            this.DurationLabel.Text = "Acquisition Duration";
            // 
            // DurationBox
            // 
            this.DurationBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationBox.Location = new System.Drawing.Point(190, 110);
            this.DurationBox.Name = "DurationBox";
            this.DurationBox.Size = new System.Drawing.Size(131, 23);
            this.DurationBox.TabIndex = 4;
            // 
            // CountRateLabel
            // 
            this.CountRateLabel.AutoSize = true;
            this.CountRateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CountRateLabel.Location = new System.Drawing.Point(16, 166);
            this.CountRateLabel.Name = "CountRateLabel";
            this.CountRateLabel.Size = new System.Drawing.Size(79, 17);
            this.CountRateLabel.TabIndex = 5;
            this.CountRateLabel.Text = "Count Rate";
            // 
            // CountRateBox
            // 
            this.CountRateBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CountRateBox.Location = new System.Drawing.Point(190, 160);
            this.CountRateBox.Name = "CountRateBox";
            this.CountRateBox.Size = new System.Drawing.Size(131, 23);
            this.CountRateBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "Sensitivity";
            // 
            // _referenceActivityLabel
            // 
            this._referenceActivityLabel.AutoSize = true;
            this._referenceActivityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._referenceActivityLabel.Location = new System.Drawing.Point(16, 268);
            this._referenceActivityLabel.Name = "_referenceActivityLabel";
            this._referenceActivityLabel.Size = new System.Drawing.Size(122, 17);
            this._referenceActivityLabel.TabIndex = 8;
            this._referenceActivityLabel.Text = "Reference Activity";
            // 
            // _referenceActivityBox
            // 
            this._referenceActivityBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._referenceActivityBox.Location = new System.Drawing.Point(190, 268);
            this._referenceActivityBox.Name = "_referenceActivityBox";
            this._referenceActivityBox.Size = new System.Drawing.Size(131, 23);
            this._referenceActivityBox.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Activity at Acquisition";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // _currentActivityBox
            // 
            this._currentActivityBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._currentActivityBox.Location = new System.Drawing.Point(190, 391);
            this._currentActivityBox.Name = "_currentActivityBox";
            this._currentActivityBox.Size = new System.Drawing.Size(131, 22);
            this._currentActivityBox.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 316);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "Reference Date";
            // 
            // _referenceDateBox
            // 
            this._referenceDateBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._referenceDateBox.Location = new System.Drawing.Point(190, 316);
            this._referenceDateBox.Name = "_referenceDateBox";
            this._referenceDateBox.Size = new System.Drawing.Size(131, 22);
            this._referenceDateBox.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 434);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Sensitivity";
            // 
            // _sensitivityBox
            // 
            this._sensitivityBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._sensitivityBox.Location = new System.Drawing.Point(190, 434);
            this._sensitivityBox.Name = "_sensitivityBox";
            this._sensitivityBox.Size = new System.Drawing.Size(131, 22);
            this._sensitivityBox.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 359);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 16);
            this.label5.TabIndex = 17;
            this.label5.Text = "Acquisition Date";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // _acquisitionDateBox
            // 
            this._acquisitionDateBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._acquisitionDateBox.Location = new System.Drawing.Point(190, 356);
            this._acquisitionDateBox.Name = "_acquisitionDateBox";
            this._acquisitionDateBox.Size = new System.Drawing.Size(131, 22);
            this._acquisitionDateBox.TabIndex = 18;
            // 
            // CountRateComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._acquisitionDateBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._sensitivityBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._referenceDateBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._currentActivityBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._referenceActivityBox);
            this.Controls.Add(this._referenceActivityLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CountRateBox);
            this.Controls.Add(this.CountRateLabel);
            this.Controls.Add(this.DurationBox);
            this.Controls.Add(this.DurationLabel);
            this.Controls.Add(this.TotalCountsBox);
            this.Controls.Add(this.TotalCountsLabel);
            this.Controls.Add(this.TitleLabel);
            this.Name = "CountRateComponentControl";
            this.Size = new System.Drawing.Size(339, 571);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label TotalCountsLabel;
        private System.Windows.Forms.TextBox TotalCountsBox;
        private System.Windows.Forms.Label DurationLabel;
        private System.Windows.Forms.TextBox DurationBox;
        private System.Windows.Forms.Label CountRateLabel;
        private System.Windows.Forms.TextBox CountRateBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _referenceActivityLabel;
        private System.Windows.Forms.TextBox _referenceActivityBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _currentActivityBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _referenceDateBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _sensitivityBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _acquisitionDateBox;
    }
}
