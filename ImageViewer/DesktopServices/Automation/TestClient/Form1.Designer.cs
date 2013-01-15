namespace ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient
{
	partial class Form1
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
			this._openStudy = new System.Windows.Forms.Button();
			this._activateViewer = new System.Windows.Forms.Button();
			this._closeViewer = new System.Windows.Forms.Button();
			this._startViewer = new System.Windows.Forms.Button();
			this._studyGrid = new System.Windows.Forms.DataGridView();
			this.RetrieveAETitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.HasViewers = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.AccessionNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.patientIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.patientsNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.studyDescriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.studyInstanceUidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._studyItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this._activateIfOpen = new System.Windows.Forms.CheckBox();
			this._openViewers = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this._refreshAllViewers = new System.Windows.Forms.Button();
			this._getSelectedInfo = new System.Windows.Forms.Button();
			this._requery = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this._patientId = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._accession = new System.Windows.Forms.TextBox();
			this.studyBrowserGroup = new System.Windows.Forms.GroupBox();
			this._dicomExplorerExecuteQuery = new System.Windows.Forms.Button();
			this._dicomExplorerQueryRemote = new System.Windows.Forms.RadioButton();
			this._dicomExplorerQueryLocal = new System.Windows.Forms.RadioButton();
			this._dicomExplorerRemoteAE = new System.Windows.Forms.TextBox();
			this._clear = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._studyGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._studyItemBindingSource)).BeginInit();
			this.studyBrowserGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// _openStudy
			// 
			this._openStudy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._openStudy.Location = new System.Drawing.Point(786, 237);
			this._openStudy.Name = "_openStudy";
			this._openStudy.Size = new System.Drawing.Size(194, 23);
			this._openStudy.TabIndex = 9;
			this._openStudy.Text = "Open Study";
			this._openStudy.UseVisualStyleBackColor = true;
			this._openStudy.Click += new System.EventHandler(this.OnOpenStudy);
			// 
			// _activateViewer
			// 
			this._activateViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._activateViewer.Location = new System.Drawing.Point(786, 266);
			this._activateViewer.Name = "_activateViewer";
			this._activateViewer.Size = new System.Drawing.Size(194, 23);
			this._activateViewer.TabIndex = 10;
			this._activateViewer.Text = "Activate Viewer";
			this._activateViewer.UseVisualStyleBackColor = true;
			this._activateViewer.Click += new System.EventHandler(this.OnActivateViewer);
			// 
			// _closeViewer
			// 
			this._closeViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._closeViewer.Location = new System.Drawing.Point(786, 295);
			this._closeViewer.Name = "_closeViewer";
			this._closeViewer.Size = new System.Drawing.Size(194, 23);
			this._closeViewer.TabIndex = 11;
			this._closeViewer.Text = "Close Viewer";
			this._closeViewer.UseVisualStyleBackColor = true;
			this._closeViewer.Click += new System.EventHandler(this.OnCloseViewer);
			// 
			// _startViewer
			// 
			this._startViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._startViewer.Location = new System.Drawing.Point(786, 208);
			this._startViewer.Name = "_startViewer";
			this._startViewer.Size = new System.Drawing.Size(194, 23);
			this._startViewer.TabIndex = 8;
			this._startViewer.Text = "Start Viewer";
			this._startViewer.UseVisualStyleBackColor = true;
			this._startViewer.Click += new System.EventHandler(this.OnStartViewer);
			// 
			// _studyGrid
			// 
			this._studyGrid.AllowUserToAddRows = false;
			this._studyGrid.AllowUserToDeleteRows = false;
			this._studyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._studyGrid.AutoGenerateColumns = false;
			this._studyGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this._studyGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this._studyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._studyGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RetrieveAETitle,
            this.HasViewers,
            this.AccessionNumber,
            this.patientIdDataGridViewTextBoxColumn,
            this.patientsNameDataGridViewTextBoxColumn,
            this.studyDescriptionDataGridViewTextBoxColumn,
            this.studyInstanceUidDataGridViewTextBoxColumn});
			this._studyGrid.DataSource = this._studyItemBindingSource;
			this._studyGrid.Location = new System.Drawing.Point(12, 95);
			this._studyGrid.Name = "_studyGrid";
			this._studyGrid.ReadOnly = true;
			this._studyGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._studyGrid.Size = new System.Drawing.Size(768, 341);
			this._studyGrid.TabIndex = 3;
			// 
			// RetrieveAETitle
			// 
			this.RetrieveAETitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.RetrieveAETitle.DataPropertyName = "RetrieveAETitle";
			this.RetrieveAETitle.HeaderText = "Retrieve AE";
			this.RetrieveAETitle.Name = "RetrieveAETitle";
			this.RetrieveAETitle.ReadOnly = true;
			this.RetrieveAETitle.Width = 82;
			// 
			// HasViewers
			// 
			this.HasViewers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.HasViewers.DataPropertyName = "HasViewers";
			this.HasViewers.HeaderText = "Has Viewer(s)";
			this.HasViewers.Name = "HasViewers";
			this.HasViewers.ReadOnly = true;
			this.HasViewers.Width = 70;
			// 
			// AccessionNumber
			// 
			this.AccessionNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.AccessionNumber.DataPropertyName = "AccessionNumber";
			this.AccessionNumber.HeaderText = "Accession #";
			this.AccessionNumber.Name = "AccessionNumber";
			this.AccessionNumber.ReadOnly = true;
			this.AccessionNumber.Width = 84;
			// 
			// patientIdDataGridViewTextBoxColumn
			// 
			this.patientIdDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.patientIdDataGridViewTextBoxColumn.DataPropertyName = "PatientId";
			this.patientIdDataGridViewTextBoxColumn.HeaderText = "Patient Id";
			this.patientIdDataGridViewTextBoxColumn.Name = "patientIdDataGridViewTextBoxColumn";
			this.patientIdDataGridViewTextBoxColumn.ReadOnly = true;
			this.patientIdDataGridViewTextBoxColumn.Width = 71;
			// 
			// patientsNameDataGridViewTextBoxColumn
			// 
			this.patientsNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.patientsNameDataGridViewTextBoxColumn.DataPropertyName = "PatientsName";
			this.patientsNameDataGridViewTextBoxColumn.HeaderText = "Patient\'s Name";
			this.patientsNameDataGridViewTextBoxColumn.Name = "patientsNameDataGridViewTextBoxColumn";
			this.patientsNameDataGridViewTextBoxColumn.ReadOnly = true;
			this.patientsNameDataGridViewTextBoxColumn.Width = 95;
			// 
			// studyDescriptionDataGridViewTextBoxColumn
			// 
			this.studyDescriptionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.studyDescriptionDataGridViewTextBoxColumn.DataPropertyName = "StudyDescription";
			this.studyDescriptionDataGridViewTextBoxColumn.HeaderText = "Study Description";
			this.studyDescriptionDataGridViewTextBoxColumn.Name = "studyDescriptionDataGridViewTextBoxColumn";
			this.studyDescriptionDataGridViewTextBoxColumn.ReadOnly = true;
			this.studyDescriptionDataGridViewTextBoxColumn.Width = 106;
			// 
			// studyInstanceUidDataGridViewTextBoxColumn
			// 
			this.studyInstanceUidDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.studyInstanceUidDataGridViewTextBoxColumn.DataPropertyName = "StudyInstanceUid";
			this.studyInstanceUidDataGridViewTextBoxColumn.HeaderText = "Study Instance Uid";
			this.studyInstanceUidDataGridViewTextBoxColumn.Name = "studyInstanceUidDataGridViewTextBoxColumn";
			this.studyInstanceUidDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// _studyItemBindingSource
			// 
			this._studyItemBindingSource.DataSource = typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyItem);
			// 
			// _activateIfOpen
			// 
			this._activateIfOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._activateIfOpen.AutoSize = true;
			this._activateIfOpen.Checked = true;
			this._activateIfOpen.CheckState = System.Windows.Forms.CheckState.Checked;
			this._activateIfOpen.Location = new System.Drawing.Point(786, 324);
			this._activateIfOpen.Name = "_activateIfOpen";
			this._activateIfOpen.Size = new System.Drawing.Size(137, 17);
			this._activateIfOpen.TabIndex = 12;
			this._activateIfOpen.Text = "Activate if already open";
			this._activateIfOpen.UseVisualStyleBackColor = true;
			// 
			// _openViewers
			// 
			this._openViewers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._openViewers.FormattingEnabled = true;
			this._openViewers.Location = new System.Drawing.Point(787, 107);
			this._openViewers.Name = "_openViewers";
			this._openViewers.Size = new System.Drawing.Size(193, 95);
			this._openViewers.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(787, 93);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Open Viewers";
			// 
			// _refreshAllViewers
			// 
			this._refreshAllViewers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._refreshAllViewers.Location = new System.Drawing.Point(787, 384);
			this._refreshAllViewers.Name = "_refreshAllViewers";
			this._refreshAllViewers.Size = new System.Drawing.Size(194, 23);
			this._refreshAllViewers.TabIndex = 14;
			this._refreshAllViewers.Text = "Refresh Viewers";
			this._refreshAllViewers.UseVisualStyleBackColor = true;
			this._refreshAllViewers.Click += new System.EventHandler(this.OnRefreshAllViewers);
			// 
			// _getSelectedInfo
			// 
			this._getSelectedInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._getSelectedInfo.Location = new System.Drawing.Point(787, 355);
			this._getSelectedInfo.Name = "_getSelectedInfo";
			this._getSelectedInfo.Size = new System.Drawing.Size(194, 23);
			this._getSelectedInfo.TabIndex = 13;
			this._getSelectedInfo.Text = "Get Additional Info";
			this._getSelectedInfo.UseVisualStyleBackColor = true;
			this._getSelectedInfo.Click += new System.EventHandler(this.OnGetSelectedInfo);
			// 
			// _requery
			// 
			this._requery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._requery.Location = new System.Drawing.Point(786, 413);
			this._requery.Name = "_requery";
			this._requery.Size = new System.Drawing.Size(194, 23);
			this._requery.TabIndex = 15;
			this._requery.Text = "Requery";
			this._requery.UseVisualStyleBackColor = true;
			this._requery.Click += new System.EventHandler(this.OnRequery);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "P.Id";
			// 
			// _patientId
			// 
			this._patientId.Location = new System.Drawing.Point(40, 30);
			this._patientId.Name = "_patientId";
			this._patientId.Size = new System.Drawing.Size(155, 20);
			this._patientId.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(208, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(21, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "A#";
			// 
			// _accession
			// 
			this._accession.Location = new System.Drawing.Point(235, 30);
			this._accession.Name = "_accession";
			this._accession.Size = new System.Drawing.Size(155, 20);
			this._accession.TabIndex = 3;
			// 
			// studyBrowserGroup
			// 
			this.studyBrowserGroup.Controls.Add(this._dicomExplorerExecuteQuery);
			this.studyBrowserGroup.Controls.Add(this._dicomExplorerQueryRemote);
			this.studyBrowserGroup.Controls.Add(this._dicomExplorerQueryLocal);
			this.studyBrowserGroup.Controls.Add(this._dicomExplorerRemoteAE);
			this.studyBrowserGroup.Location = new System.Drawing.Point(420, 8);
			this.studyBrowserGroup.Name = "studyBrowserGroup";
			this.studyBrowserGroup.Size = new System.Drawing.Size(360, 81);
			this.studyBrowserGroup.TabIndex = 5;
			this.studyBrowserGroup.TabStop = false;
			this.studyBrowserGroup.Text = "Study Browser";
			// 
			// _dicomExplorerExecuteQuery
			// 
			this._dicomExplorerExecuteQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._dicomExplorerExecuteQuery.Location = new System.Drawing.Point(247, 48);
			this._dicomExplorerExecuteQuery.Name = "_dicomExplorerExecuteQuery";
			this._dicomExplorerExecuteQuery.Size = new System.Drawing.Size(101, 23);
			this._dicomExplorerExecuteQuery.TabIndex = 3;
			this._dicomExplorerExecuteQuery.Text = "Execute Query";
			this._dicomExplorerExecuteQuery.UseVisualStyleBackColor = true;
			this._dicomExplorerExecuteQuery.Click += new System.EventHandler(this.OnDicomExplorerExecuteQuery);
			// 
			// _dicomExplorerQueryRemote
			// 
			this._dicomExplorerQueryRemote.AutoSize = true;
			this._dicomExplorerQueryRemote.Location = new System.Drawing.Point(94, 24);
			this._dicomExplorerQueryRemote.Name = "_dicomExplorerQueryRemote";
			this._dicomExplorerQueryRemote.Size = new System.Drawing.Size(93, 17);
			this._dicomExplorerQueryRemote.TabIndex = 1;
			this._dicomExplorerQueryRemote.Text = "Query Remote";
			this._dicomExplorerQueryRemote.UseVisualStyleBackColor = true;
			// 
			// _dicomExplorerQueryLocal
			// 
			this._dicomExplorerQueryLocal.AutoSize = true;
			this._dicomExplorerQueryLocal.Checked = true;
			this._dicomExplorerQueryLocal.Location = new System.Drawing.Point(6, 24);
			this._dicomExplorerQueryLocal.Name = "_dicomExplorerQueryLocal";
			this._dicomExplorerQueryLocal.Size = new System.Drawing.Size(82, 17);
			this._dicomExplorerQueryLocal.TabIndex = 0;
			this._dicomExplorerQueryLocal.TabStop = true;
			this._dicomExplorerQueryLocal.Text = "Query Local";
			this._dicomExplorerQueryLocal.UseVisualStyleBackColor = true;
			// 
			// _dicomExplorerRemoteAE
			// 
			this._dicomExplorerRemoteAE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._dicomExplorerRemoteAE.Location = new System.Drawing.Point(193, 22);
			this._dicomExplorerRemoteAE.Name = "_dicomExplorerRemoteAE";
			this._dicomExplorerRemoteAE.Size = new System.Drawing.Size(155, 20);
			this._dicomExplorerRemoteAE.TabIndex = 2;
			// 
			// _clear
			// 
			this._clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._clear.Location = new System.Drawing.Point(337, 56);
			this._clear.Name = "_clear";
			this._clear.Size = new System.Drawing.Size(53, 23);
			this._clear.TabIndex = 4;
			this._clear.Text = "Clear";
			this._clear.UseVisualStyleBackColor = true;
			this._clear.Click += new System.EventHandler(this.OnClear);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(992, 448);
			this.Controls.Add(this._clear);
			this.Controls.Add(this.studyBrowserGroup);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._patientId);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._accession);
			this.Controls.Add(this._requery);
			this.Controls.Add(this._getSelectedInfo);
			this.Controls.Add(this._refreshAllViewers);
			this.Controls.Add(this._openViewers);
			this.Controls.Add(this._activateIfOpen);
			this.Controls.Add(this._studyGrid);
			this.Controls.Add(this._startViewer);
			this.Controls.Add(this._closeViewer);
			this.Controls.Add(this._activateViewer);
			this.Controls.Add(this._openStudy);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(1000, 482);
			this.Name = "Form1";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Automation Test Client";
			this.Load += new System.EventHandler(this.OnFormLoad);
			((System.ComponentModel.ISupportInitialize)(this._studyGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._studyItemBindingSource)).EndInit();
			this.studyBrowserGroup.ResumeLayout(false);
			this.studyBrowserGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _openStudy;
		private System.Windows.Forms.Button _activateViewer;
		private System.Windows.Forms.Button _closeViewer;
		private System.Windows.Forms.Button _startViewer;
		private System.Windows.Forms.DataGridView _studyGrid;
		private System.Windows.Forms.CheckBox _activateIfOpen;
		private System.Windows.Forms.ListBox _openViewers;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _refreshAllViewers;
		private System.Windows.Forms.Button _getSelectedInfo;
		private System.Windows.Forms.BindingSource _studyItemBindingSource;
		private System.Windows.Forms.Button _requery;
		private System.Windows.Forms.DataGridViewTextBoxColumn RetrieveAETitle;
		private System.Windows.Forms.DataGridViewCheckBoxColumn HasViewers;
		private System.Windows.Forms.DataGridViewTextBoxColumn patientIdDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn AccessionNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn patientsNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn studyDescriptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn studyInstanceUidDataGridViewTextBoxColumn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _patientId;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _accession;
		private System.Windows.Forms.GroupBox studyBrowserGroup;
		private System.Windows.Forms.Button _dicomExplorerExecuteQuery;
		private System.Windows.Forms.RadioButton _dicomExplorerQueryRemote;
		private System.Windows.Forms.RadioButton _dicomExplorerQueryLocal;
		private System.Windows.Forms.TextBox _dicomExplorerRemoteAE;
		private System.Windows.Forms.Button _clear;
	}
}

