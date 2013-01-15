namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms
{
	partial class SimpleComposerAdapterComponentPanel {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleComposerAdapterComponentPanel));
			this.iglImages = new System.Windows.Forms.ImageList(this.components);
			this.pnlBottomContent = new System.Windows.Forms.Panel();
			this.lyoButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.xbtnPublish = new System.Windows.Forms.Panel();
			this.btnPublish = new System.Windows.Forms.Button();
			this.btnPublishDropDown = new System.Windows.Forms.Button();
			this.lblTip = new System.Windows.Forms.Label();
			this.lyoMainContent = new System.Windows.Forms.TableLayoutPanel();
			this.pnlImages = new System.Windows.Forms.Panel();
			this.pnlSeries = new System.Windows.Forms.Panel();
			this.pnlStudies = new System.Windows.Forms.Panel();
			this.lblPatients = new System.Windows.Forms.Label();
			this.lblImages = new System.Windows.Forms.Label();
			this.lblSeries = new System.Windows.Forms.Label();
			this.lblStudies = new System.Windows.Forms.Label();
			this.pnlPatients = new System.Windows.Forms.Panel();
			this.dlgExport = new System.Windows.Forms.FolderBrowserDialog();
			this.pnlTopContent = new System.Windows.Forms.Panel();
			this.mnuPublish = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuPublishLocal = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuPublishRemote = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuPublishFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlBottomContent.SuspendLayout();
			this.lyoButtons.SuspendLayout();
			this.xbtnPublish.SuspendLayout();
			this.lyoMainContent.SuspendLayout();
			this.mnuPublish.SuspendLayout();
			this.SuspendLayout();
			// 
			// iglImages
			// 
			this.iglImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iglImages.ImageStream")));
			this.iglImages.TransparentColor = System.Drawing.Color.Transparent;
			this.iglImages.Images.SetKeyName(0, "placeholder");
			// 
			// pnlBottomContent
			// 
			this.pnlBottomContent.Controls.Add(this.lyoButtons);
			this.pnlBottomContent.Controls.Add(this.lblTip);
			this.pnlBottomContent.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottomContent.Location = new System.Drawing.Point(0, 580);
			this.pnlBottomContent.Name = "pnlBottomContent";
			this.pnlBottomContent.Padding = new System.Windows.Forms.Padding(5);
			this.pnlBottomContent.Size = new System.Drawing.Size(1128, 41);
			this.pnlBottomContent.TabIndex = 2;
			// 
			// lyoButtons
			// 
			this.lyoButtons.Controls.Add(this.btnClose);
			this.lyoButtons.Controls.Add(this.btnRefresh);
			this.lyoButtons.Controls.Add(this.xbtnPublish);
			this.lyoButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lyoButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.lyoButtons.Location = new System.Drawing.Point(502, 5);
			this.lyoButtons.Name = "lyoButtons";
			this.lyoButtons.Size = new System.Drawing.Size(621, 31);
			this.lyoButtons.TabIndex = 5;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(523, 3);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(95, 25);
			this.btnClose.TabIndex = 0;
			this.btnClose.Text = "&Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Location = new System.Drawing.Point(422, 3);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(95, 25);
			this.btnRefresh.TabIndex = 1;
			this.btnRefresh.Text = "&Refresh";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// xbtnPublish
			// 
			this.xbtnPublish.Controls.Add(this.btnPublish);
			this.xbtnPublish.Controls.Add(this.btnPublishDropDown);
			this.xbtnPublish.Location = new System.Drawing.Point(296, 3);
			this.xbtnPublish.Name = "xbtnPublish";
			this.xbtnPublish.Size = new System.Drawing.Size(120, 25);
			this.xbtnPublish.TabIndex = 3;
			// 
			// btnPublish
			// 
			this.btnPublish.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnPublish.Location = new System.Drawing.Point(0, 0);
			this.btnPublish.Name = "btnPublish";
			this.btnPublish.Size = new System.Drawing.Size(95, 25);
			this.btnPublish.TabIndex = 2;
			this.btnPublish.Text = "&Publish...";
			this.btnPublish.UseVisualStyleBackColor = true;
			this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
			// 
			// btnPublishDropDown
			// 
			this.btnPublishDropDown.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnPublishDropDown.Image = ((System.Drawing.Image)(resources.GetObject("btnPublishDropDown.Image")));
			this.btnPublishDropDown.Location = new System.Drawing.Point(95, 0);
			this.btnPublishDropDown.Name = "btnPublishDropDown";
			this.btnPublishDropDown.Size = new System.Drawing.Size(25, 25);
			this.btnPublishDropDown.TabIndex = 3;
			this.btnPublishDropDown.UseVisualStyleBackColor = true;
			this.btnPublishDropDown.Click += new System.EventHandler(this.btnPublishDropDown_Click);
			// 
			// lblTip
			// 
			this.lblTip.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblTip.Location = new System.Drawing.Point(5, 5);
			this.lblTip.Name = "lblTip";
			this.lblTip.Size = new System.Drawing.Size(497, 31);
			this.lblTip.TabIndex = 6;
			this.lblTip.Text = "Tip: Hold SHIFT while dragging to create a copy of the item instead of moving it." +
				"";
			this.lblTip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lyoMainContent
			// 
			this.lyoMainContent.ColumnCount = 4;
			this.lyoMainContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.lyoMainContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.lyoMainContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.lyoMainContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.lyoMainContent.Controls.Add(this.pnlImages, 3, 1);
			this.lyoMainContent.Controls.Add(this.pnlSeries, 2, 1);
			this.lyoMainContent.Controls.Add(this.pnlStudies, 1, 1);
			this.lyoMainContent.Controls.Add(this.lblPatients, 0, 0);
			this.lyoMainContent.Controls.Add(this.lblImages, 3, 0);
			this.lyoMainContent.Controls.Add(this.lblSeries, 2, 0);
			this.lyoMainContent.Controls.Add(this.lblStudies, 1, 0);
			this.lyoMainContent.Controls.Add(this.pnlPatients, 0, 1);
			this.lyoMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lyoMainContent.Location = new System.Drawing.Point(0, 0);
			this.lyoMainContent.Name = "lyoMainContent";
			this.lyoMainContent.RowCount = 2;
			this.lyoMainContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.lyoMainContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.lyoMainContent.Size = new System.Drawing.Size(1128, 580);
			this.lyoMainContent.TabIndex = 3;
			// 
			// pnlImages
			// 
			this.pnlImages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlImages.Location = new System.Drawing.Point(849, 23);
			this.pnlImages.Name = "pnlImages";
			this.pnlImages.Size = new System.Drawing.Size(276, 554);
			this.pnlImages.TabIndex = 11;
			// 
			// pnlSeries
			// 
			this.pnlSeries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSeries.Location = new System.Drawing.Point(567, 23);
			this.pnlSeries.Name = "pnlSeries";
			this.pnlSeries.Size = new System.Drawing.Size(276, 554);
			this.pnlSeries.TabIndex = 10;
			// 
			// pnlStudies
			// 
			this.pnlStudies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlStudies.Location = new System.Drawing.Point(285, 23);
			this.pnlStudies.Name = "pnlStudies";
			this.pnlStudies.Size = new System.Drawing.Size(276, 554);
			this.pnlStudies.TabIndex = 9;
			// 
			// lblPatients
			// 
			this.lblPatients.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblPatients.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPatients.Location = new System.Drawing.Point(3, 0);
			this.lblPatients.Name = "lblPatients";
			this.lblPatients.Size = new System.Drawing.Size(276, 20);
			this.lblPatients.TabIndex = 4;
			this.lblPatients.Text = "Patients";
			this.lblPatients.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblPatients.UseMnemonic = false;
			// 
			// lblImages
			// 
			this.lblImages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblImages.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblImages.Location = new System.Drawing.Point(849, 0);
			this.lblImages.Name = "lblImages";
			this.lblImages.Size = new System.Drawing.Size(276, 20);
			this.lblImages.TabIndex = 7;
			this.lblImages.Text = "Images";
			this.lblImages.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblImages.UseMnemonic = false;
			// 
			// lblSeries
			// 
			this.lblSeries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblSeries.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSeries.Location = new System.Drawing.Point(567, 0);
			this.lblSeries.Name = "lblSeries";
			this.lblSeries.Size = new System.Drawing.Size(276, 20);
			this.lblSeries.TabIndex = 6;
			this.lblSeries.Text = "Series";
			this.lblSeries.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblSeries.UseMnemonic = false;
			// 
			// lblStudies
			// 
			this.lblStudies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblStudies.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStudies.Location = new System.Drawing.Point(285, 0);
			this.lblStudies.Name = "lblStudies";
			this.lblStudies.Size = new System.Drawing.Size(276, 20);
			this.lblStudies.TabIndex = 5;
			this.lblStudies.Text = "Studies";
			this.lblStudies.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblStudies.UseMnemonic = false;
			// 
			// pnlPatients
			// 
			this.pnlPatients.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlPatients.Location = new System.Drawing.Point(3, 23);
			this.pnlPatients.Name = "pnlPatients";
			this.pnlPatients.Size = new System.Drawing.Size(276, 554);
			this.pnlPatients.TabIndex = 8;
			// 
			// pnlTopContent
			// 
			this.pnlTopContent.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTopContent.Location = new System.Drawing.Point(0, 0);
			this.pnlTopContent.Name = "pnlTopContent";
			this.pnlTopContent.Size = new System.Drawing.Size(1128, 0);
			this.pnlTopContent.TabIndex = 4;
			this.pnlTopContent.Visible = false;
			// 
			// mnuPublish
			// 
			this.mnuPublish.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPublishLocal,
            this.mnuPublishRemote,
            this.mnuPublishFolder});
			this.mnuPublish.Name = "mnuPublish";
			this.mnuPublish.Size = new System.Drawing.Size(208, 92);
			// 
			// mnuPublishLocal
			// 
			this.mnuPublishLocal.Name = "mnuPublishLocal";
			this.mnuPublishLocal.Size = new System.Drawing.Size(207, 22);
			this.mnuPublishLocal.Text = "Publish to &Local Server";
			this.mnuPublishLocal.Click += new System.EventHandler(this.mnuPublishLocal_Click);
			// 
			// mnuPublishRemote
			// 
			this.mnuPublishRemote.Name = "mnuPublishRemote";
			this.mnuPublishRemote.Size = new System.Drawing.Size(207, 22);
			this.mnuPublishRemote.Text = "Publish to &Remote Server...";
			this.mnuPublishRemote.Click += new System.EventHandler(this.mnuPublishRemote_Click);
			// 
			// mnuPublishFolder
			// 
			this.mnuPublishFolder.Name = "mnuPublishFolder";
			this.mnuPublishFolder.Size = new System.Drawing.Size(207, 22);
			this.mnuPublishFolder.Text = "Publish to &Folder...";
			this.mnuPublishFolder.Click += new System.EventHandler(this.mnuPublishFolder_Click);
			// 
			// SimpleComposerAdapterComponentPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lyoMainContent);
			this.Controls.Add(this.pnlBottomContent);
			this.Controls.Add(this.pnlTopContent);
			this.Name = "SimpleComposerAdapterComponentPanel";
			this.Size = new System.Drawing.Size(1128, 621);
			this.pnlBottomContent.ResumeLayout(false);
			this.lyoButtons.ResumeLayout(false);
			this.xbtnPublish.ResumeLayout(false);
			this.lyoMainContent.ResumeLayout(false);
			this.mnuPublish.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlBottomContent;
		private System.Windows.Forms.TableLayoutPanel lyoMainContent;
		private System.Windows.Forms.FlowLayoutPanel lyoButtons;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.ImageList iglImages;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.Button btnPublish;
		private System.Windows.Forms.FolderBrowserDialog dlgExport;
		private System.Windows.Forms.Panel pnlTopContent;
		private System.Windows.Forms.Label lblImages;
		private System.Windows.Forms.Label lblSeries;
		private System.Windows.Forms.Label lblStudies;
		private System.Windows.Forms.Label lblPatients;
		private System.Windows.Forms.Panel pnlImages;
		private System.Windows.Forms.Panel pnlSeries;
		private System.Windows.Forms.Panel pnlStudies;
		private System.Windows.Forms.Panel pnlPatients;
		private System.Windows.Forms.Label lblTip;
		private System.Windows.Forms.Panel xbtnPublish;
		private System.Windows.Forms.Button btnPublishDropDown;
		private System.Windows.Forms.ContextMenuStrip mnuPublish;
		private System.Windows.Forms.ToolStripMenuItem mnuPublishLocal;
		private System.Windows.Forms.ToolStripMenuItem mnuPublishRemote;
		private System.Windows.Forms.ToolStripMenuItem mnuPublishFolder;
	}
}
