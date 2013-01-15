#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.ImageProperties;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ImagePropertiesApplicationComponent"/>.
	/// </summary>
	public partial class ImagePropertiesApplicationComponentControl : ApplicationComponentUserControl
	{
		private readonly ImagePropertiesApplicationComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ImagePropertiesApplicationComponentControl(ImagePropertiesApplicationComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_component.PropertyChanged += Update;

			Update(null, null);
		}

		private void Update(object sender, PropertyChangedEventArgs e)
		{
			_properties.SelectedObject = new ImageProperties(_component.ImageProperties);
		}
	}

	//hack to get rid of the "Property Pages" toolbar item and make the text edit box read-only
	internal class CustomPropertyGrid : PropertyGrid
	{
		public CustomPropertyGrid()
		{
			base.ToolStripRenderer = new CustomToolStripRenderer();
		}

		protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
		{
			base.OnSelectedGridItemChanged(e);
			MakeAllTextBoxesReadOnly(this);
		}

		protected override void OnSelectedObjectsChanged(EventArgs e)
		{
			base.OnSelectedObjectsChanged(e);
			MakeAllTextBoxesReadOnly(this);
		}

		private static void MakeAllTextBoxesReadOnly(Control parent)
		{
			//force all TextBox controls owned by the parent to be read-only.
			foreach (Control control in parent.Controls)
			{
				if (control is TextBox)
				{
					TextBox box = (TextBox) control;
					box.ReadOnly = true;
				}
				else
				{
					MakeAllTextBoxesReadOnly(control);
				}
			}
		}
	}

	//hack to hide the "Property Pages" toolbar item
	internal class CustomToolStripRenderer : ToolStripRenderer
	{
		private string _propertyPagesTooltip;

		protected override void Initialize(ToolStrip toolStrip)
		{
			if (toolStrip.Items.Count == 5) //reflector shows the toolbar is fixed, so this is ok.
				_propertyPagesTooltip = toolStrip.Items[4].ToolTipText;

			base.Initialize(toolStrip);
		}

		protected override void InitializeItem(ToolStripItem item)
		{
			base.InitializeItem(item);

			if (item.ToolTipText == _propertyPagesTooltip)
			{
				item.Visible = false;
				item.Enabled = false;
			}
		}
	}

	internal class ShowValueEditor : UITypeEditor
	{
		public ShowValueEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			ImagePropertyDescriptor descriptor = (ImagePropertyDescriptor)context.PropertyDescriptor;
			string stringValue = ImagePropertyDescriptor.GetStringValue(descriptor.ImageProperty.Value, false);
			ShowValueDialog.Show(descriptor.DisplayName, descriptor.Description, stringValue);
			return value; //no edits allowed
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}

	internal class ImagePropertyDescriptor : PropertyDescriptor
	{
		public readonly IImageProperty ImageProperty;
		private object _value;

		public ImagePropertyDescriptor(IImageProperty imageProperty)
			: base(imageProperty.Identifier, CreateAttributes(imageProperty))
		{
			ImageProperty = imageProperty;
		}

		private static Attribute[] CreateAttributes(IImageProperty imageProperty)
		{
			CategoryAttribute category = new CategoryAttribute(imageProperty.Category);
			DescriptionAttribute description = new DescriptionAttribute(imageProperty.Description);
			EditorAttribute editor = new EditorAttribute(typeof(ShowValueEditor), typeof(UITypeEditor));

			return new Attribute[] { category, description, editor };
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override string DisplayName
		{
			get
			{
				return ImageProperty.Name;
			}
		}

		public override Type ComponentType
		{
			get { return ImageProperty.GetType(); }
		}

		public override object GetValue(object component)
		{
			if (_value == null)
			{
				// if the value is a nested set of properties, wrap in an ImageProperties container
				if (ImageProperty.Value is IEnumerable<IImageProperty>)
					_value = new ImageProperties(new List<IImageProperty>((IEnumerable<IImageProperty>) ImageProperty.Value));
				else
					_value = GetStringValue(ImageProperty.Value, true);
			}
			return _value;
		}

		public override bool IsReadOnly
		{
			//has to be false, otherwise the text renders too light.
			get { return false; }
		}

		public override Type PropertyType
		{
			get
			{
				// if the value is a nested set of properties, wrap in an ImageProperties container
				if (ImageProperty.Value is IEnumerable<IImageProperty>)
					return typeof (ImageProperties);
				return ImageProperty.ValueType;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		internal static string GetStringValue(object value, bool removeCrLf)
		{
			if (value == null)
				return "";

			string returnValue;

			if (value is string)
			{
				returnValue = (string)value;
			}
			else if (value is IEnumerable<IImageProperty>)
			{
				var sb = new StringBuilder();
				foreach (var property in (IEnumerable<IImageProperty>) value)
				{
					if (sb.Length > 0)
						sb.AppendLine();
					sb.Append(GetStringValue(property.Value, true));
				}
				returnValue = sb.ToString();
			}
			else
			{
				TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
				if (converter != null && converter.CanConvertTo(typeof(string)))
					returnValue = converter.ConvertToString(value);
				else
					returnValue = value.ToString();
			}

			returnValue = returnValue ?? "";
			if (removeCrLf)
			{
				returnValue = returnValue.Replace("\r\n", "/");
				returnValue = returnValue.Replace("\r", "/");
				returnValue = returnValue.Replace("\n", "/");
			}

			return returnValue;
		}
	}

	[TypeConverter(typeof(ImagePropertiesConverter))]
	internal class ImageProperties : ICustomTypeDescriptor
	{
		private readonly PropertyDescriptorCollection _propertyDescriptors;

		public ImageProperties(IList<IImageProperty> properties)
		{
			_propertyDescriptors = new PropertyDescriptorCollection(
				CollectionUtils.Map(properties,
								delegate(IImageProperty property)
									{
										return new ImagePropertyDescriptor(property);
									}).ToArray()
				);
		}

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return null;
		}

		public object GetEditor(Type editorBaseType)
		{
			return new ShowValueEditor();
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return _propertyDescriptors;
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region ImagePropertiesConverter Class

		private class ImagePropertiesConverter : ExpandableObjectConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof (string))
					return true;
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				var imageProperties = value as ImageProperties;
				if (destinationType == typeof (string) && imageProperties != null)
					return ImagePropertyDescriptor.GetStringValue(CollectionUtils.Map<ImagePropertyDescriptor, IImageProperty>(imageProperties.GetProperties(), p => p.ImageProperty), true);
				return base.ConvertTo(context, culture, value, destinationType);
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				var imageProperties = value as ImageProperties;
				if (imageProperties != null)
					return imageProperties.GetProperties(attributes);
				return base.GetProperties(context, value, attributes);
			}
		}

		#endregion
	}
}
