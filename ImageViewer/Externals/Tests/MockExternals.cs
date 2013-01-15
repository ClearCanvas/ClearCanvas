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

#if UNIT_TESTS

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.ImageViewer.Externals.General;

namespace ClearCanvas.ImageViewer.Externals.Tests
{
	internal interface IMockExternal : IExternal
	{
		string Data { get; set; }
	}

	// This class must be public because of XmlSerializer
	public sealed class MockExternal : ExternalBase, IMockExternal
	{
		public string Data { get; set; }

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		internal sealed class ExternalFactory : ExternalFactoryBase<MockExternal>
		{
			public ExternalFactory() : base("Mock External Type") {}

			public override IExternalPropertiesComponent CreatePropertiesComponent()
			{
				throw new NotImplementedException();
			}
		}
	}

	// This class must be public because of XmlSerializer
	public sealed class MockXmlSerializableExternal : ExternalBase, IMockExternal, IXmlSerializable
	{
		public string Data { get; set; }

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			Name = reader.GetAttribute("Name");
			Label = reader.GetAttribute("Label");
			Enabled = bool.Parse(reader.GetAttribute("Enabled"));
			WindowStyle = (WindowStyle) Enum.Parse(typeof (WindowStyle), reader.GetAttribute("WindowStyle"));

			var isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!isEmptyElement)
			{
				Data = reader.ReadElementString("Data");
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Label", Label);
			writer.WriteAttributeString("Enabled", Enabled.ToString());
			writer.WriteAttributeString("WindowStyle", WindowStyle.ToString());
			writer.WriteElementString("Data", Data);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		internal sealed class ExternalFactory : ExternalFactoryBase<MockXmlSerializableExternal>
		{
			public ExternalFactory() : base("Mock External Type With Custom XML Serialization") {}

			public override IExternalPropertiesComponent CreatePropertiesComponent()
			{
				throw new NotImplementedException();
			}
		}
	}

	// This class must be public because of XmlSerializer
	public sealed class MockBrokenExternal : ExternalBase, IMockExternal, IXmlSerializable
	{
		public string Data { get; set; }

		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.Read(); // simulate bad XML: put the reader in a broken state by advancing asymmetrically
				throw new Exception();
			}
			catch (Exception ex)
			{
				throw new XmlException("Generic XML parse exception", ex);
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			// we don't have any provision in place to mitigate broken serialization code
			// it's easy to break XML accidentally, but IExternal implementations should be safer
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			throw new NotSupportedException();
		}

		internal sealed class ExternalFactory : ExternalFactoryBase<MockBrokenExternal>
		{
			public ExternalFactory() : base("Mock External Type That Simulates XML Serialization Errors") {}

			public override IExternalPropertiesComponent CreatePropertiesComponent()
			{
				throw new NotImplementedException();
			}
		}
	}
}

#endif