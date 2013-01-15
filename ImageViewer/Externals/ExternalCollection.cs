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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Externals.Config;

namespace ClearCanvas.ImageViewer.Externals
{
	public class ExternalCollection : ICollection<IExternal>, IXmlSerializable
	{
		private readonly List<IExternal> _definitions;

		public ExternalCollection()
		{
			_definitions = new List<IExternal>();
		}

		public ExternalCollection(IEnumerable<IExternal> defintions)
		{
			_definitions = new List<IExternal>(defintions);
		}

		#region ICollection<IExternalLauncher> Members

		public void Add(IExternal item)
		{
			_definitions.Add(item);
		}

		public void Clear()
		{
			_definitions.Clear();
		}

		public bool Contains(IExternal item)
		{
			return _definitions.Contains(item);
		}

		public void CopyTo(IExternal[] array, int arrayIndex)
		{
			_definitions.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _definitions.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IExternal item)
		{
			return _definitions.Remove(item);
		}

		#endregion

		#region IEnumerable<IExternalLauncher> Members

		public IEnumerator<IExternal> GetEnumerator()
		{
			return _definitions.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		public void Sort()
		{
			_definitions.Sort((x, y) => string.Compare(x.Label, y.Label, StringComparison.CurrentCultureIgnoreCase));
		}

		#region SavedExternals Static Instance

		private static ExternalCollection _savedExternals = null;

		public static ExternalCollection SavedExternals
		{
			get
			{
				if (_savedExternals == null)
				{
					try
					{
						ReloadSavedExternals();
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Error encountered while deserializing External Application definitions. The XML may be corrupt.");
					}
				}
				return _savedExternals;
			}
		}

		public static void ReloadSavedExternals()
		{
			try
			{
				ExternalsConfigurationSettings settings = ExternalsConfigurationSettings.Default;
				_savedExternals = settings.Externals;
				if (_savedExternals == null)
					_savedExternals = new ExternalCollection();
			}
			catch (Exception)
			{
				// make sure that the SavedExternals property is never simply null
				_savedExternals = new ExternalCollection();
				throw;
			}
		}

		#endregion

		#region IXmlSerializable Members

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element)
			{
				var isEmptyElement = reader.IsEmptyElement; // yes, this must be checked before consuming the element tag
				var list = new List<IExternal>();

				reader.ReadStartElement(); // consume the collection container (start) element tag
				if (!isEmptyElement)
				{
					// descendents of collection container should all be elements representing individual externals
					//  at the end of this loop, we must be positioned *past* each </External> tag
					var count = 0;
					while (reader.MoveToContent() == XmlNodeType.Element)
					{
						var external = (IExternal) null;
						var elementName = reader.LocalName;
						if (reader.IsEmptyElement) // An empty element
						{
							reader.ReadStartElement(); // consume the empty element
						}
						else if (elementName == "External") // The "External" element denotes start of an external definition
						{
							var type = reader.GetAttribute("Type"); // "Type" is an attribute of External
							var childReader = reader.ReadSubtree(); // use the External element as the root of a new reader
							childReader.MoveToContent();
							try
							{
								// perform deserialization of external
								var factory = new ExternalFactoryExtensionPoint().CreateExtension(type);
								if (factory != null)
								{
									childReader.ReadStartElement(); // consume the "External" element in the child reader
									external = (IExternal) new XmlSerializer(factory.ExternalType).Deserialize(childReader);
								}
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Warn, ex, "Error deserializing External Application definition at #{1}: <{0}> may contain corrupted XML.", elementName, count);
							}
							finally
							{
								// when we close the child reader, we are at the end element External, so consume it now to advance past it
								childReader.Close();
								reader.ReadEndElement();
							}
						}
						else if (elementName == "IExternal") // The "IExternal" element denotes legacy definition format
						{
							try
							{
								var typeName = reader.GetAttribute("Concrete-Type"); // "Concrete-Type" is an attribute of IExternal
								var data = reader.ReadElementContentAsString(); // read contents of IExternal as a string; advances *past* the IExternal end element

								// perform deserialization of external
								var type = Type.GetType(typeName, false);
								if (type != null)
								{
									using (var szReader = new StringReader(data))
									{
										external = (IExternal) new XmlSerializer(type).Deserialize(szReader);
									}
								}
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Warn, ex, "Error deserializing External Application definition at #{1}: <{0}> may contain corrupted XML.", elementName, count);
							}
						}
						else
						{
							Platform.Log(LogLevel.Debug, "Unrecognized External Application definition at #{1}: <{0}> is not a valid child element.", elementName, count);

							// if this child node of ExternalCollection is unrecognized, we have to consume it!
							//  otherwise MoveToContent in the next iteration doesn't do anything and we end up dividing by zero
							reader.ReadSubtree().Close();
							reader.ReadEndElement();
						}

						if (external != null)
							list.Add(external);

						++count;
					}
					reader.ReadEndElement(); // consume the collection container end element tag
				}

				_definitions.Clear();
				_definitions.AddRange(list);
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (var external in this)
			{
				var xmlSerializer = new XmlSerializer(external.GetType());
				writer.WriteStartElement("External");
				writer.WriteAttributeString("Type", external.GetType().FullName);
				xmlSerializer.Serialize(writer, external);
				writer.WriteEndElement();
			}
		}

		#endregion

		#region Unit Test Support

#if UNIT_TESTS

		/// <summary>
		/// Unit test entry point for <see cref="ExternalCollection"/> serialization.
		/// </summary>
		internal static string Serialize(ExternalCollection obj)
		{
			if (obj == null)
				return string.Empty;

			using (StringWriter writer = new StringWriter())
			{
				XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
				xmlSerializer.Serialize(writer, obj);
				return writer.ToString();
			}
		}

		/// <summary>
		/// Unit test entry point for <see cref="ExternalCollection"/> deserialization.
		/// </summary>
		internal static ExternalCollection Deserialize(string xml)
		{
			if (string.IsNullOrEmpty(xml))
				return null;

			using (StringReader reader = new StringReader(xml))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof (ExternalCollection));
				return (ExternalCollection) xmlSerializer.Deserialize(reader);
			}
		}

#endif

		#endregion
	}
}