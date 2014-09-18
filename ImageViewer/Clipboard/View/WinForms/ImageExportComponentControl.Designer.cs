#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    partial class ImageExportComponentControl
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
			this.DoDispose(disposing);
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageExportComponentControl));
			this._imageExporters = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._buttonConfigure = new System.Windows.Forms.Button();
			this._buttonOk = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._groupFieldOfView = new System.Windows.Forms.GroupBox();
			this._checkOptionCompleteImage = new System.Windows.Forms.RadioButton();
			this._checkOptionWysiwyg = new System.Windows.Forms.RadioButton();
			this._scale = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._pnlDialogButtons = new System.Windows.Forms.FlowLayoutPanel();
			this._groupOutputSize = new System.Windows.Forms.GroupBox();
			this._pnlScale = new System.Windows.Forms.Panel();
			this._lblScalePercent = new System.Windows.Forms.Label();
			this._pnlFixedSize = new System.Windows.Forms.Panel();
			this._pnlBackgroundColor = new System.Windows.Forms.Panel();
			this._backgroundColorSwatch = new System.Windows.Forms.Button();
			this._lblBackgroundColor = new System.Windows.Forms.Label();
			this._imageHeight = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._imageWidth = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._lblHeight = new System.Windows.Forms.Label();
			this._lblWidth = new System.Windows.Forms.Label();
			this._checkOptionFixed = new System.Windows.Forms.RadioButton();
			this._checkOptionScale = new System.Windows.Forms.RadioButton();
			this._groupDpi = new System.Windows.Forms.GroupBox();
			this._lblDpiDefault = new System.Windows.Forms.Label();
			this._outputDpi = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._lblDpi = new System.Windows.Forms.Label();
			this._groupFieldOfView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._scale)).BeginInit();
			this._pnlDialogButtons.SuspendLayout();
			this._groupOutputSize.SuspendLayout();
			this._pnlScale.SuspendLayout();
			this._pnlFixedSize.SuspendLayout();
			this._pnlBackgroundColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageWidth)).BeginInit();
			this._groupDpi.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._outputDpi)).BeginInit();
			this.SuspendLayout();
			// 
			// _imageExporters
			// 
			this._imageExporters.DataSource = null;
			this._imageExporters.DisplayMember = "";
			this._imageExporters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._imageExporters, "_imageExporters");
			this._imageExporters.Name = "_imageExporters";
			this._imageExporters.Value = null;
			// 
			// _buttonConfigure
			// 
			resources.ApplyResources(this._buttonConfigure, "_buttonConfigure");
			this._buttonConfigure.Name = "_buttonConfigure";
			this._buttonConfigure.UseVisualStyleBackColor = true;
			this._buttonConfigure.Click += new System.EventHandler(this.OnConfigureExporter);
			// 
			// _buttonOk
			// 
			resources.ApplyResources(this._buttonOk, "_buttonOk");
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.UseVisualStyleBackColor = true;
			this._buttonOk.Click += new System.EventHandler(this.OnOk);
			// 
			// _buttonCancel
			// 
			resources.ApplyResources(this._buttonCancel, "_buttonCancel");
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.OnCancel);
			// 
			// _groupFieldOfView
			// 
			this._groupFieldOfView.Controls.Add(this._checkOptionCompleteImage);
			this._groupFieldOfView.Controls.Add(this._checkOptionWysiwyg);
			resources.ApplyResources(this._groupFieldOfView, "_groupFieldOfView");
			this._groupFieldOfView.Name = "_groupFieldOfView";
			this._groupFieldOfView.TabStop = false;
			// 
			// _checkOptionCompleteImage
			// 
			resources.ApplyResources(this._checkOptionCompleteImage, "_checkOptionCompleteImage");
			this._checkOptionCompleteImage.Name = "_checkOptionCompleteImage";
			this._checkOptionCompleteImage.UseVisualStyleBackColor = true;
			// 
			// _checkOptionWysiwyg
			// 
			resources.ApplyResources(this._checkOptionWysiwyg, "_checkOptionWysiwyg");
			this._checkOptionWysiwyg.Name = "_checkOptionWysiwyg";
			this._checkOptionWysiwyg.UseVisualStyleBackColor = true;
			// 
			// _scale
			// 
			this._scale.DecimalPlaces = 1;
			this._scale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			resources.ApplyResources(this._scale, "_scale");
			this._scale.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this._scale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._scale.Name = "_scale";
			this._scale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _pnlDialogButtons
			// 
			this._pnlDialogButtons.Controls.Add(this._buttonCancel);
			this._pnlDialogButtons.Controls.Add(this._buttonOk);
			resources.ApplyResources(this._pnlDialogButtons, "_pnlDialogButtons");
			this._pnlDialogButtons.Name = "_pnlDialogButtons";
			// 
			// _groupOutputSize
			// 
			this._groupOutputSize.Controls.Add(this._pnlScale);
			this._groupOutputSize.Controls.Add(this._pnlFixedSize);
			this._groupOutputSize.Controls.Add(this._checkOptionFixed);
			this._groupOutputSize.Controls.Add(this._checkOptionScale);
			resources.ApplyResources(this._groupOutputSize, "_groupOutputSize");
			this._groupOutputSize.Name = "_groupOutputSize";
			this._groupOutputSize.TabStop = false;
			// 
			// _pnlScale
			// 
			this._pnlScale.Controls.Add(this._scale);
			this._pnlScale.Controls.Add(this._lblScalePercent);
			resources.ApplyResources(this._pnlScale, "_pnlScale");
			this._pnlScale.Name = "_pnlScale";
			// 
			// _lblScalePercent
			// 
			resources.ApplyResources(this._lblScalePercent, "_lblScalePercent");
			this._lblScalePercent.Name = "_lblScalePercent";
			// 
			// _pnlFixedSize
			// 
			this._pnlFixedSize.Controls.Add(this._pnlBackgroundColor);
			this._pnlFixedSize.Controls.Add(this._imageHeight);
			this._pnlFixedSize.Controls.Add(this._imageWidth);
			this._pnlFixedSize.Controls.Add(this._lblHeight);
			this._pnlFixedSize.Controls.Add(this._lblWidth);
			resources.ApplyResources(this._pnlFixedSize, "_pnlFixedSize");
			this._pnlFixedSize.Name = "_pnlFixedSize";
			// 
			// _pnlBackgroundColor
			// 
			this._pnlBackgroundColor.Controls.Add(this._backgroundColorSwatch);
			this._pnlBackgroundColor.Controls.Add(this._lblBackgroundColor);
			resources.ApplyResources(this._pnlBackgroundColor, "_pnlBackgroundColor");
			this._pnlBackgroundColor.Name = "_pnlBackgroundColor";
			// 
			// _backgroundColorSwatch
			// 
			this._backgroundColorSwatch.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this._backgroundColorSwatch, "_backgroundColorSwatch");
			this._backgroundColorSwatch.Name = "_backgroundColorSwatch";
			this._backgroundColorSwatch.UseVisualStyleBackColor = false;
			this._backgroundColorSwatch.Click += new System.EventHandler(this.OnBackgroundColorSwatchClick);
			// 
			// _lblBackgroundColor
			// 
			resources.ApplyResources(this._lblBackgroundColor, "_lblBackgroundColor");
			this._lblBackgroundColor.Name = "_lblBackgroundColor";
			// 
			// _imageHeight
			// 
			this._imageHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			resources.ApplyResources(this._imageHeight, "_imageHeight");
			this._imageHeight.Name = "_imageHeight";
			this._imageHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _imageWidth
			// 
			this._imageWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			resources.ApplyResources(this._imageWidth, "_imageWidth");
			this._imageWidth.Name = "_imageWidth";
			this._imageWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _lblHeight
			// 
			resources.ApplyResources(this._lblHeight, "_lblHeight");
			this._lblHeight.Name = "_lblHeight";
			// 
			// _lblWidth
			// 
			resources.ApplyResources(this._lblWidth, "_lblWidth");
			this._lblWidth.Name = "_lblWidth";
			// 
			// _checkOptionFixed
			// 
			resources.ApplyResources(this._checkOptionFixed, "_checkOptionFixed");
			this._checkOptionFixed.Name = "_checkOptionFixed";
			this._checkOptionFixed.TabStop = true;
			this._checkOptionFixed.UseVisualStyleBackColor = true;
			// 
			// _checkOptionScale
			// 
			resources.ApplyResources(this._checkOptionScale, "_checkOptionScale");
			this._checkOptionScale.Name = "_checkOptionScale";
			this._checkOptionScale.TabStop = true;
			this._checkOptionScale.UseVisualStyleBackColor = true;
			// 
			// _groupDpi
			// 
			this._groupDpi.Controls.Add(this._lblDpiDefault);
			this._groupDpi.Controls.Add(this._outputDpi);
			this._groupDpi.Controls.Add(this._lblDpi);
			resources.ApplyResources(this._groupDpi, "_groupDpi");
			this._groupDpi.Name = "_groupDpi";
			this._groupDpi.TabStop = false;
			// 
			// _lblDpiDefault
			// 
			resources.ApplyResources(this._lblDpiDefault, "_lblDpiDefault");
			this._lblDpiDefault.Name = "_lblDpiDefault";
			// 
			// _outputDpi
			// 
			resources.ApplyResources(this._outputDpi, "_outputDpi");
			this._outputDpi.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
			this._outputDpi.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
			this._outputDpi.Name = "_outputDpi";
			this._outputDpi.Value = new decimal(new int[] {
            96,
            0,
            0,
            0});
			// 
			// _lblDpi
			// 
			resources.ApplyResources(this._lblDpi, "_lblDpi");
			this._lblDpi.Name = "_lblDpi";
			// 
			// ImageExportComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupDpi);
			this.Controls.Add(this._groupOutputSize);
			this.Controls.Add(this._pnlDialogButtons);
			this.Controls.Add(this._groupFieldOfView);
			this.Controls.Add(this._buttonConfigure);
			this.Controls.Add(this._imageExporters);
			this.Name = "ImageExportComponentControl";
			this._groupFieldOfView.ResumeLayout(false);
			this._groupFieldOfView.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._scale)).EndInit();
			this._pnlDialogButtons.ResumeLayout(false);
			this._groupOutputSize.ResumeLayout(false);
			this._groupOutputSize.PerformLayout();
			this._pnlScale.ResumeLayout(false);
			this._pnlScale.PerformLayout();
			this._pnlFixedSize.ResumeLayout(false);
			this._pnlFixedSize.PerformLayout();
			this._pnlBackgroundColor.ResumeLayout(false);
			this._pnlBackgroundColor.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageWidth)).EndInit();
			this._groupDpi.ResumeLayout(false);
			this._groupDpi.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._outputDpi)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _imageExporters;
		private System.Windows.Forms.Button _buttonConfigure;
		private System.Windows.Forms.Button _buttonOk;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.RadioButton _checkOptionCompleteImage;
		private System.Windows.Forms.RadioButton _checkOptionWysiwyg;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _scale;
		private System.Windows.Forms.RadioButton _checkOptionFixed;
		private System.Windows.Forms.RadioButton _checkOptionScale;
		private System.Windows.Forms.Label _lblScalePercent;
		private System.Windows.Forms.Label _lblHeight;
		private System.Windows.Forms.Label _lblWidth;
		private System.Windows.Forms.Panel _pnlFixedSize;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageHeight;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageWidth;
		private System.Windows.Forms.Panel _pnlBackgroundColor;
		private System.Windows.Forms.Label _lblBackgroundColor;
		private System.Windows.Forms.Panel _pnlScale;
		private System.Windows.Forms.Button _backgroundColorSwatch;
		private System.Windows.Forms.GroupBox _groupFieldOfView;
		private System.Windows.Forms.FlowLayoutPanel _pnlDialogButtons;
		private System.Windows.Forms.GroupBox _groupOutputSize;
		private System.Windows.Forms.GroupBox _groupDpi;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _outputDpi;
		private System.Windows.Forms.Label _lblDpi;
		private System.Windows.Forms.Label _lblDpiDefault;
    }
}
