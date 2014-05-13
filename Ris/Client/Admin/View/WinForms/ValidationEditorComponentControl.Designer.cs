namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ValidationEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationEditorComponentControl));
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._propertiesTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._testButton = new System.Windows.Forms.Button();
			this._macroButton = new System.Windows.Forms.Button();
			this._propertiesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._editorPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _propertiesTableView
			// 
			resources.ApplyResources(this._propertiesTableView, "_propertiesTableView");
			this._propertiesTableView.MultiSelect = false;
			this._propertiesTableView.Name = "_propertiesTableView";
			this._propertiesTableView.ReadOnly = false;
			// 
			// _testButton
			// 
			resources.ApplyResources(this._testButton, "_testButton");
			this._testButton.Name = "_testButton";
			this._testButton.UseVisualStyleBackColor = true;
			this._testButton.Click += new System.EventHandler(this._testButton_Click);
			// 
			// _macroButton
			// 
			resources.ApplyResources(this._macroButton, "_macroButton");
			this._macroButton.Name = "_macroButton";
			this._macroButton.UseVisualStyleBackColor = true;
			this._macroButton.Click += new System.EventHandler(this._macroButton_Click);
			// 
			// _propertiesMenu
			// 
			this._propertiesMenu.Name = "_propertiesMenu";
			resources.ApplyResources(this._propertiesMenu, "_propertiesMenu");
			this._propertiesMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._propertiesMenu_ItemClicked);
			// 
			// _editorPanel
			// 
			resources.ApplyResources(this._editorPanel, "_editorPanel");
			this._editorPanel.Name = "_editorPanel";
			// 
			// ValidationEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._editorPanel);
			this.Controls.Add(this._macroButton);
			this.Controls.Add(this._testButton);
			this.Controls.Add(this._propertiesTableView);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Name = "ValidationEditorComponentControl";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _propertiesTableView;
        private System.Windows.Forms.Button _testButton;
        private System.Windows.Forms.Button _macroButton;
        private System.Windows.Forms.ContextMenuStrip _propertiesMenu;
        private System.Windows.Forms.Panel _editorPanel;
    }
}
