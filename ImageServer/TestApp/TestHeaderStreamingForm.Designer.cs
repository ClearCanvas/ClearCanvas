namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestHeaderStreamingForm
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
            this.Query = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ServerAE = new System.Windows.Forms.TextBox();
            this.LogTextPanel = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.certifcateGroupbox = new System.Windows.Forms.GroupBox();
            this.ValidateServerCert = new System.Windows.Forms.CheckBox();
            this.ClientCertSubjectName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.EndPointDNS = new System.Windows.Forms.TextBox();
            this.ServiceName = new System.Windows.Forms.TextBox();
            this.WcfPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Port = new System.Windows.Forms.TextBox();
            this.AETitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SeriesTree = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.StudyTree = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SeriesLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.StatisticsLog = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.certifcateGroupbox.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Query
            // 
            this.Query.Location = new System.Drawing.Point(282, 88);
            this.Query.Name = "Query";
            this.Query.Size = new System.Drawing.Size(102, 23);
            this.Query.TabIndex = 1;
            this.Query.Text = "Search Studies";
            this.Query.UseVisualStyleBackColor = true;
            this.Query.Click += new System.EventHandler(this.Query_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Server Partition AE";
            // 
            // ServerAE
            // 
            this.ServerAE.Location = new System.Drawing.Point(156, 40);
            this.ServerAE.Name = "ServerAE";
            this.ServerAE.Size = new System.Drawing.Size(100, 20);
            this.ServerAE.TabIndex = 4;
            this.ServerAE.Text = "ImageServer";
            // 
            // LogTextPanel
            // 
            this.LogTextPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextPanel.Location = new System.Drawing.Point(3, 520);
            this.LogTextPanel.Multiline = true;
            this.LogTextPanel.Name = "LogTextPanel";
            this.LogTextPanel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextPanel.Size = new System.Drawing.Size(1066, 228);
            this.LogTextPanel.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.IP);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Port);
            this.groupBox1.Controls.Add(this.AETitle);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ServerAE);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Query);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1066, 144);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.certifcateGroupbox);
            this.groupBox3.Controls.Add(this.EndPointDNS);
            this.groupBox3.Controls.Add(this.ServiceName);
            this.groupBox3.Controls.Add(this.WcfPort);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(412, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(648, 117);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "WCF Settings";
            // 
            // certifcateGroupbox
            // 
            this.certifcateGroupbox.Controls.Add(this.ValidateServerCert);
            this.certifcateGroupbox.Controls.Add(this.ClientCertSubjectName);
            this.certifcateGroupbox.Controls.Add(this.label9);
            this.certifcateGroupbox.Location = new System.Drawing.Point(332, 10);
            this.certifcateGroupbox.Name = "certifcateGroupbox";
            this.certifcateGroupbox.Size = new System.Drawing.Size(310, 101);
            this.certifcateGroupbox.TabIndex = 18;
            this.certifcateGroupbox.TabStop = false;
            this.certifcateGroupbox.Text = "certificate";
            // 
            // ValidateServerCert
            // 
            this.ValidateServerCert.AutoSize = true;
            this.ValidateServerCert.Location = new System.Drawing.Point(146, 41);
            this.ValidateServerCert.Name = "ValidateServerCert";
            this.ValidateServerCert.Size = new System.Drawing.Size(144, 17);
            this.ValidateServerCert.TabIndex = 16;
            this.ValidateServerCert.Text = "validate server certificate";
            this.ValidateServerCert.UseVisualStyleBackColor = true;
            // 
            // ClientCertSubjectName
            // 
            this.ClientCertSubjectName.Enabled = false;
            this.ClientCertSubjectName.Location = new System.Drawing.Point(22, 41);
            this.ClientCertSubjectName.Name = "ClientCertSubjectName";
            this.ClientCertSubjectName.Size = new System.Drawing.Size(100, 20);
            this.ClientCertSubjectName.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "local certificate";
            // 
            // EndPointDNS
            // 
            this.EndPointDNS.Location = new System.Drawing.Point(206, 32);
            this.EndPointDNS.Name = "EndPointDNS";
            this.EndPointDNS.Size = new System.Drawing.Size(100, 20);
            this.EndPointDNS.TabIndex = 17;
            // 
            // ServiceName
            // 
            this.ServiceName.Location = new System.Drawing.Point(19, 32);
            this.ServiceName.Name = "ServiceName";
            this.ServiceName.Size = new System.Drawing.Size(100, 20);
            this.ServiceName.TabIndex = 16;
            this.ServiceName.Text = "HeaderRetrieval";
            // 
            // WcfPort
            // 
            this.WcfPort.Location = new System.Drawing.Point(136, 32);
            this.WcfPort.Name = "WcfPort";
            this.WcfPort.Size = new System.Drawing.Size(55, 20);
            this.WcfPort.TabIndex = 15;
            this.WcfPort.Text = "50221";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Binding";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Service";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(203, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "EndPoint Identify";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(133, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Port";
            // 
            // IP
            // 
            this.IP.Location = new System.Drawing.Point(35, 90);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(100, 20);
            this.IP.TabIndex = 11;
            this.IP.Text = "localhost";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(153, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Server Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Server host";
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(156, 90);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(100, 20);
            this.Port.TabIndex = 9;
            this.Port.Text = "5001";
            // 
            // AETitle
            // 
            this.AETitle.Location = new System.Drawing.Point(35, 40);
            this.AETitle.Name = "AETitle";
            this.AETitle.Size = new System.Drawing.Size(100, 20);
            this.AETitle.TabIndex = 7;
            this.AETitle.Text = "SIM_AE";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "AE Title";
            // 
            // SeriesTree
            // 
            this.SeriesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SeriesTree.Location = new System.Drawing.Point(3, 16);
            this.SeriesTree.Name = "SeriesTree";
            this.SeriesTree.Size = new System.Drawing.Size(533, 204);
            this.SeriesTree.TabIndex = 8;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 153);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1066, 223);
            this.splitContainer1.SplitterDistance = 523;
            this.splitContainer1.TabIndex = 9;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.StudyTree, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(523, 223);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(517, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Studies";
            // 
            // StudyTree
            // 
            this.StudyTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StudyTree.Location = new System.Drawing.Point(3, 16);
            this.StudyTree.Name = "StudyTree";
            this.StudyTree.Size = new System.Drawing.Size(517, 204);
            this.StudyTree.TabIndex = 0;
            this.StudyTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.StudyTree_AfterSelect);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.SeriesTree, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SeriesLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(539, 223);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // SeriesLabel
            // 
            this.SeriesLabel.AutoSize = true;
            this.SeriesLabel.Location = new System.Drawing.Point(3, 0);
            this.SeriesLabel.Name = "SeriesLabel";
            this.SeriesLabel.Size = new System.Drawing.Size(36, 13);
            this.SeriesLabel.TabIndex = 9;
            this.SeriesLabel.Text = "Series";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.StatisticsLog);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 382);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1066, 132);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Statistics";
            // 
            // StatisticsLog
            // 
            this.StatisticsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatisticsLog.Location = new System.Drawing.Point(3, 16);
            this.StatisticsLog.Multiline = true;
            this.StatisticsLog.Name = "StatisticsLog";
            this.StatisticsLog.Size = new System.Drawing.Size(1060, 113);
            this.StatisticsLog.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.LogTextPanel, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1072, 697);
            this.tableLayoutPanel4.TabIndex = 11;
            // 
            // TestHeaderStreamingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 697);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Name = "TestHeaderStreamingForm";
            this.Text = "HeaderStreamingTest";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.certifcateGroupbox.ResumeLayout(false);
            this.certifcateGroupbox.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Query;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ServerAE;
        private System.Windows.Forms.TextBox LogTextPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView SeriesTree;
        private System.Windows.Forms.TextBox AETitle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TreeView StudyTree;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox IP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label SeriesLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox StatisticsLog;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox WcfPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ServiceName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ClientCertSubjectName;
        private System.Windows.Forms.TextBox EndPointDNS;
        private System.Windows.Forms.GroupBox certifcateGroupbox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox ValidateServerCert;
        private System.Windows.Forms.Label label10;
    }
}