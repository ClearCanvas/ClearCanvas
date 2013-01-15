namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TimestampField
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimestampField));
			this._datePicker = new System.Windows.Forms.DateTimePicker();
			this._timePicker = new System.Windows.Forms.DateTimePicker();
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _datePicker
			// 
			this._datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			resources.ApplyResources(this._datePicker, "_datePicker");
			this._datePicker.Name = "_datePicker";
			this._datePicker.ShowCheckBox = true;
			this._datePicker.ValueChanged += new System.EventHandler(this.OnValueChanged);
			this._datePicker.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnDatePickerMouseUp);
			// 
			// _timePicker
			// 
			this._timePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			resources.ApplyResources(this._timePicker, "_timePicker");
			this._timePicker.Name = "_timePicker";
			this._timePicker.ShowUpDown = true;
			// 
			// label
			// 
			resources.ApplyResources(this.label, "label");
			this.label.Name = "label";
			// 
			// TimestampField
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._timePicker);
			this.Controls.Add(this.label);
			this.Controls.Add(this._datePicker);
			this.Name = "TimestampField";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker _datePicker;
        private System.Windows.Forms.DateTimePicker _timePicker;
        private System.Windows.Forms.Label label;
    }
}
