namespace ClearCanvas.Desktop.View.WinForms
{
	partial class IndicatorLight
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndicatorLight));
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this._link = new System.Windows.Forms.LinkLabel();
			this._iconLabel = new System.Windows.Forms.Label();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// _imageList
			// 
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			this._imageList.Images.SetKeyName(0, "GreenLight.png");
			this._imageList.Images.SetKeyName(1, "YellowLight.png");
			this._imageList.Images.SetKeyName(2, "RedLight.png");
			// 
			// _link
			// 
			resources.ApplyResources(this._link, "_link");
			this._link.Name = "_link";
			this._link.TabStop = true;
			// 
			// _iconLabel
			// 
			resources.ApplyResources(this._iconLabel, "_iconLabel");
			this._iconLabel.ImageList = this._imageList;
			this._iconLabel.Name = "_iconLabel";
			// 
			// _toolTip
			// 
			this._toolTip.AutoPopDelay = 5000;
			this._toolTip.InitialDelay = 500;
			this._toolTip.ReshowDelay = 100;
			// 
			// IndicatorLight
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._iconLabel);
			this.Controls.Add(this._link);
			this.Name = "IndicatorLight";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.LinkLabel _link;
		private System.Windows.Forms.Label _iconLabel;
		private System.Windows.Forms.ToolTip _toolTip;
	}
}
