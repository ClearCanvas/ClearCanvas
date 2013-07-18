using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class IconCheckBox : UserControl
    {
        public IconCheckBox()
        {
            InitializeComponent();
        }

        public bool Checked
        {
            get { return _check.Checked; }
            set { _check.Checked = value; }
        }

        public bool Enabled
        {
            get { return _check.Enabled; }
            set { _check.Enabled = value; }
        }

        public event EventHandler CheckedChanged
        {
            add { _check.CheckedChanged += value; }
            remove { _check.CheckedChanged -= value; }
        }

        public string Text
        {
            get { return _checkLabel.Text; }
            set { _checkLabel.Text = value; }
        }

        public Image Image
        {
            get { return _icon.Image; }
            set { _icon.Image = value; }
        }
    }
}
