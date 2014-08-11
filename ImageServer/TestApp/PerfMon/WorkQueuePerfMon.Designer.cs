namespace ClearCanvas.ImageServer.TestApp.PerfMon
{
	partial class WorkQueuePerfMon
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this._processingSpeedChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._processingSpeedChart)).BeginInit();
			this.SuspendLayout();
			// 
			// _processingSpeedChart
			// 
			chartArea1.AxisX.LabelStyle.Format = "HH:mm:ss";
			chartArea1.AxisX.LineColor = System.Drawing.Color.DarkGray;
			chartArea1.AxisY.Minimum = 0D;
			chartArea1.Name = "ChartArea1";
			this._processingSpeedChart.ChartAreas.Add(chartArea1);
			legend1.Name = "Legend1";
			this._processingSpeedChart.Legends.Add(legend1);
			this._processingSpeedChart.Location = new System.Drawing.Point(47, 96);
			this._processingSpeedChart.Name = "_processingSpeedChart";
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.Legend = "Legend1";
			series1.Name = "SOPs / second";
			series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
			this._processingSpeedChart.Series.Add(series1);
			this._processingSpeedChart.Size = new System.Drawing.Size(776, 354);
			this._processingSpeedChart.TabIndex = 0;
			this._processingSpeedChart.Text = "chart1";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(42, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(470, 25);
			this.label1.TabIndex = 1;
			this.label1.Text = "Combined Processing Speed on All Servers";
			// 
			// WorkQueuePerfMon
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(851, 482);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._processingSpeedChart);
			this.Name = "WorkQueuePerfMon";
			this.Text = "WorkQueuePerfMon";
			((System.ComponentModel.ISupportInitialize)(this._processingSpeedChart)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart _processingSpeedChart;
		private System.Windows.Forms.Label label1;
	}
}