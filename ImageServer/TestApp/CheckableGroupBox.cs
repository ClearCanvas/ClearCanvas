using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ClearCanvas.ImageServer.TestApp
{
	public class CheckableGroupBoxEventManager
	{
		public class CheckableGroupBoxEventArgs : EventArgs
		{
			public CheckableGroupBox Control;
			public bool IsChecked { get; set; }
		}

		private static CheckableGroupBoxEventManager _instance = new CheckableGroupBoxEventManager();

		public static CheckableGroupBoxEventManager Instance
		{
			get { return _instance; }
		}

		public void FireCheckStateChanged(CheckableGroupBox source, bool isChecked)
		{
			SelectionChange(source, new CheckableGroupBoxEventArgs(){ Control = source, IsChecked=isChecked });
		}

		public event EventHandler<CheckableGroupBoxEventArgs> SelectionChange;

	}
	public class CheckableGroupBox : GroupBox
	{
		#region Fields
		private bool checkedStatus = false;
		private Color textColor = Color.Blue;

		#endregion
		#region Constructor
		public CheckableGroupBox()
		{
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			CheckableGroupBoxEventManager.Instance.SelectionChange += SelectionChange;
		}

		void SelectionChange(object sender, CheckableGroupBoxEventManager.CheckableGroupBoxEventArgs e)
		{
			if (e.Control != this)
			{
				if (e.Control.GroupName == this.GroupName)
				{
					this.Checked = !e.Control.Checked;
				}
			}
		}

		#endregion
		#region Properties
		public bool Checked
		{
			get { return this.checkedStatus; }
			set
			{
				this.checkedStatus = value;
				foreach (Control ctrl in this.Controls)
				{
					ctrl.Enabled = value;
				}

				Invalidate();
			}
		}

		public string GroupName { get; set; }

		public Color TextColor
		{
			get { return this.textColor; }
			set { this.textColor = value; }
		}
		#endregion
		#region Events
		public event EventHandler CheckStateChanged;
		#endregion
		#region Protected Methods
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			float totalLength = e.ClipRectangle.Right - e.ClipRectangle.Left;
			float quaterLength = ((totalLength / 2) / 4);
			int startPoint = (int)(e.ClipRectangle.Left + 10);
			Rectangle rectAngle = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y - 1, e.ClipRectangle.Width, e.ClipRectangle.Height);
			rectAngle.X = startPoint + CheckBoxRenderer.GetGlyphSize(e.Graphics,CheckBoxState.UncheckedNormal).Width;
			SizeF sizeF = e.Graphics.MeasureString(Text, this.Font);
			rectAngle.Width = (int)sizeF.Width + 2;
			rectAngle.Height = this.Font.Height;
			CheckBoxRenderer.RenderMatchingApplicationState = true;
			Rectangle rectAngleBack = new Rectangle(startPoint - 3, rectAngle.Y,
			(rectAngle.X - (startPoint - 3)) + rectAngle.Width, rectAngle.Height);


			RadioButtonRenderer.DrawParentBackground(e.Graphics, rectAngleBack, this);
			Font font = Font;
			RadioButtonRenderer.DrawRadioButton(e.Graphics, new System.Drawing.Point(startPoint, e.ClipRectangle.Y),
			rectAngle, Text, this.Font, TextFormatFlags.VerticalCenter, false,
			checkedStatus ? RadioButtonState.CheckedNormal: RadioButtonState.UncheckedNormal);
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			e.Control.Enabled = this.Checked;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
			{
				using (Graphics g = this.CreateGraphics())
				{
					Rectangle rectAngle = ClientRectangle;
					rectAngle.X = ClientRectangle.Left + 10;
					rectAngle.Y -= 1;
					rectAngle.Width = CheckBoxRenderer.GetGlyphSize(g, checkedStatus ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal).Width;
					rectAngle.Height = CheckBoxRenderer.GetGlyphSize(g, checkedStatus ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal).Height;
					if (rectAngle.Contains(e.Location))
					{
						Checked = !Checked;
						if (CheckStateChanged != null)
						{
							CheckStateChanged(this, new EventArgs());
							CheckableGroupBoxEventManager.Instance.FireCheckStateChanged(this, this.Checked);
						}
					}
				}
				Invalidate();
			}
		}
		#endregion
	}
}
