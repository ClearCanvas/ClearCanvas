namespace ClearCanvas.Desktop.View.WinForms.Configuration {
	partial class ActionModelConfigurationComponentControl {
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
			this.PerformDispose(disposing);
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionModelConfigurationComponentControl));
			this._pnlNodeProperties = new System.Windows.Forms.Panel();
			this._lyoNodePropertiesExtensions = new System.Windows.Forms.FlowLayoutPanel();
			this._lblDescription = new System.Windows.Forms.Label();
			this._pnlTitleBar = new System.Windows.Forms.Panel();
			this._lblLabel = new System.Windows.Forms.Label();
			this._pnlIcon = new System.Windows.Forms.Panel();
			this._pnlSplit = new System.Windows.Forms.SplitContainer();
			this._actionModelTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._pnlNodeProperties.SuspendLayout();
			this._pnlTitleBar.SuspendLayout();
			this._pnlSplit.Panel1.SuspendLayout();
			this._pnlSplit.Panel2.SuspendLayout();
			this._pnlSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlNodeProperties
			// 
			resources.ApplyResources(this._pnlNodeProperties, "_pnlNodeProperties");
			this._pnlNodeProperties.Controls.Add(this._lyoNodePropertiesExtensions);
			this._pnlNodeProperties.Controls.Add(this._lblDescription);
			this._pnlNodeProperties.Controls.Add(this._pnlTitleBar);
			this._pnlNodeProperties.Name = "_pnlNodeProperties";
			// 
			// _lyoNodePropertiesExtensions
			// 
			resources.ApplyResources(this._lyoNodePropertiesExtensions, "_lyoNodePropertiesExtensions");
			this._lyoNodePropertiesExtensions.Name = "_lyoNodePropertiesExtensions";
			this._lyoNodePropertiesExtensions.SizeChanged += new System.EventHandler(this.OnLyoNodePropertiesExtensionsSizeChanged);
			// 
			// _lblDescription
			// 
			resources.ApplyResources(this._lblDescription, "_lblDescription");
			this._lblDescription.Name = "_lblDescription";
			// 
			// _pnlTitleBar
			// 
			this._pnlTitleBar.Controls.Add(this._lblLabel);
			this._pnlTitleBar.Controls.Add(this._pnlIcon);
			resources.ApplyResources(this._pnlTitleBar, "_pnlTitleBar");
			this._pnlTitleBar.Name = "_pnlTitleBar";
			// 
			// _lblLabel
			// 
			this._lblLabel.AutoEllipsis = true;
			resources.ApplyResources(this._lblLabel, "_lblLabel");
			this._lblLabel.Name = "_lblLabel";
			// 
			// _pnlIcon
			// 
			resources.ApplyResources(this._pnlIcon, "_pnlIcon");
			this._pnlIcon.Name = "_pnlIcon";
			// 
			// _pnlSplit
			// 
			resources.ApplyResources(this._pnlSplit, "_pnlSplit");
			this._pnlSplit.Name = "_pnlSplit";
			// 
			// _pnlSplit.Panel1
			// 
			this._pnlSplit.Panel1.Controls.Add(this._actionModelTree);
			// 
			// _pnlSplit.Panel2
			// 
			this._pnlSplit.Panel2.Controls.Add(this._pnlNodeProperties);
			// 
			// _actionModelTree
			// 
			this._actionModelTree.AllowDrop = true;
			this._actionModelTree.AllowDropToIndex = true;
			this._actionModelTree.CheckBoxes = true;
			resources.ApplyResources(this._actionModelTree, "_actionModelTree");
			this._actionModelTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._actionModelTree.IconResourceSize = ClearCanvas.Desktop.IconSize.Small;
			this._actionModelTree.IconSize = new System.Drawing.Size(24, 24);
			this._actionModelTree.Name = "_actionModelTree";
			this._actionModelTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._actionModelTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._actionModelTree.TreeLineColor = System.Drawing.Color.Black;
			this._actionModelTree.SelectionChanged += new System.EventHandler(this.OnActionModelTreeSelectionChanged);
			this._actionModelTree.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this.OnBindingTreeViewItemDrag);
			// 
			// _toolTip
			// 
			this._toolTip.StripAmpersands = true;
			// 
			// ActionModelConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._pnlSplit);
			this.Name = "ActionModelConfigurationComponentControl";
			this._pnlNodeProperties.ResumeLayout(false);
			this._pnlNodeProperties.PerformLayout();
			this._pnlTitleBar.ResumeLayout(false);
			this._pnlSplit.Panel1.ResumeLayout(false);
			this._pnlSplit.Panel2.ResumeLayout(false);
			this._pnlSplit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _actionModelTree;
		private System.Windows.Forms.Panel _pnlNodeProperties;
		private System.Windows.Forms.Panel _pnlTitleBar;
		private System.Windows.Forms.Label _lblLabel;
		private System.Windows.Forms.Panel _pnlIcon;
		private System.Windows.Forms.SplitContainer _pnlSplit;
		private System.Windows.Forms.FlowLayoutPanel _lyoNodePropertiesExtensions;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.Label _lblDescription;
	}
}
