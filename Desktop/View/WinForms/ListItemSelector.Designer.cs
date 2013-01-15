#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class ListItemSelector
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListItemSelector));
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._addItemButton = new System.Windows.Forms.Button();
			this._removeItemButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._availableItems = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._selectedItems = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel3
			// 
			resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
			this.tableLayoutPanel3.Controls.Add(this._addItemButton, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._removeItemButton, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.groupBox2, 2, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// _addItemButton
			// 
			resources.ApplyResources(this._addItemButton, "_addItemButton");
			this._addItemButton.Name = "_addItemButton";
			this._addItemButton.UseVisualStyleBackColor = true;
			this._addItemButton.Click += new System.EventHandler(this.AddSelection);
			// 
			// _removeItemButton
			// 
			resources.ApplyResources(this._removeItemButton, "_removeItemButton");
			this._removeItemButton.Name = "_removeItemButton";
			this._removeItemButton.UseVisualStyleBackColor = true;
			this._removeItemButton.Click += new System.EventHandler(this.RemoveSelection);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this._availableItems);
			this.groupBox1.Name = "groupBox1";
			this.tableLayoutPanel3.SetRowSpan(this.groupBox1, 2);
			this.groupBox1.TabStop = false;
			// 
			// _availableItems
			// 
			resources.ApplyResources(this._availableItems, "_availableItems");
			this._availableItems.ColumnHeaderTooltip = null;
			this._availableItems.FilterTextBoxVisible = true;
			this._availableItems.Name = "_availableItems";
			this._availableItems.ReadOnly = false;
			this._availableItems.SortButtonTooltip = null;
			this._availableItems.ItemDoubleClicked += new System.EventHandler(this.AddSelection);
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this._selectedItems);
			this.groupBox2.Name = "groupBox2";
			this.tableLayoutPanel3.SetRowSpan(this.groupBox2, 2);
			this.groupBox2.TabStop = false;
			// 
			// _selectedItems
			// 
			resources.ApplyResources(this._selectedItems, "_selectedItems");
			this._selectedItems.ColumnHeaderTooltip = null;
			this._selectedItems.FilterTextBoxVisible = true;
			this._selectedItems.Name = "_selectedItems";
			this._selectedItems.ReadOnly = false;
			this._selectedItems.SortButtonTooltip = null;
			this._selectedItems.ItemDoubleClicked += new System.EventHandler(this.RemoveSelection);
			// 
			// ListItemSelector
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel3);
			this.Name = "ListItemSelector";
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button _addItemButton;
        private System.Windows.Forms.Button _removeItemButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableItems;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedItems;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}