namespace ClearCanvas.Desktop.View.WinForms
{
    partial class AlertNotificationForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertNotificationForm));
			this._hostPanel = new System.Windows.Forms.Panel();
			this._openLogLink = new System.Windows.Forms.LinkLabel();
			this._closeButton = new System.Windows.Forms.PictureBox();
			this._icon = new System.Windows.Forms.PictureBox();
			this._contextualLink = new System.Windows.Forms.LinkLabel();
			this._message = new System.Windows.Forms.Label();
			this._timer = new System.Windows.Forms.Timer(this.components);
			this._contentPanel = new System.Windows.Forms.Panel();
			this._hostPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._closeButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._icon)).BeginInit();
			this._contentPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _hostPanel
			// 
			resources.ApplyResources(this._hostPanel, "_hostPanel");
			this._hostPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._hostPanel.Controls.Add(this._contentPanel);
			this._hostPanel.Controls.Add(this._openLogLink);
			this._hostPanel.Controls.Add(this._closeButton);
			this._hostPanel.Name = "_hostPanel";
			// 
			// _openLogLink
			// 
			resources.ApplyResources(this._openLogLink, "_openLogLink");
			this._openLogLink.AutoEllipsis = true;
			this._openLogLink.Name = "_openLogLink";
			this._openLogLink.TabStop = true;
			this._openLogLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._openLogLink_LinkClicked);
			// 
			// _closeButton
			// 
			resources.ApplyResources(this._closeButton, "_closeButton");
			this._closeButton.Name = "_closeButton";
			this._closeButton.TabStop = false;
			this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
			this._closeButton.Paint += new System.Windows.Forms.PaintEventHandler(this._closeButton_Paint);
			this._closeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this._closeButton_MouseDown);
			this._closeButton.MouseEnter += new System.EventHandler(this._closeButton_MouseEnter);
			this._closeButton.MouseLeave += new System.EventHandler(this._closeButton_MouseLeave);
			this._closeButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this._closeButton_MouseUp);
			// 
			// _icon
			// 
			resources.ApplyResources(this._icon, "_icon");
			this._icon.Name = "_icon";
			this._icon.TabStop = false;
			// 
			// _contextualLink
			// 
			resources.ApplyResources(this._contextualLink, "_contextualLink");
			this._contextualLink.AutoEllipsis = true;
			this._contextualLink.Name = "_contextualLink";
			this._contextualLink.TabStop = true;
			this._contextualLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._contextualLink_LinkClicked);
			// 
			// _message
			// 
			resources.ApplyResources(this._message, "_message");
			this._message.AutoEllipsis = true;
			this._message.Name = "_message";
			// 
			// _timer
			// 
			this._timer.Tick += new System.EventHandler(this.OnTimerTick);
			// 
			// _contentPanel
			// 
			this._contentPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this._contentPanel.Controls.Add(this._icon);
			this._contentPanel.Controls.Add(this._message);
			this._contentPanel.Controls.Add(this._contextualLink);
			resources.ApplyResources(this._contentPanel, "_contentPanel");
			this._contentPanel.Name = "_contentPanel";
			// 
			// AlertNotificationForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._hostPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AlertNotificationForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this._hostPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._closeButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._icon)).EndInit();
			this._contentPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _hostPanel;
		private System.Windows.Forms.LinkLabel _contextualLink;
		private System.Windows.Forms.Label _message;
		private System.Windows.Forms.Timer _timer;
		private System.Windows.Forms.PictureBox _icon;
		private System.Windows.Forms.PictureBox _closeButton;
		private System.Windows.Forms.LinkLabel _openLogLink;
		private System.Windows.Forms.Panel _contentPanel;
    }
}