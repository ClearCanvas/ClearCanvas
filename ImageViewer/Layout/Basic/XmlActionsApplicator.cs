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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	//TODO: later, move this into ImageViewer.Common or Common.Actions
	#region Xml Actions

	internal class XmlActionPropertyAttribute : Attribute
	{
		public XmlActionPropertyAttribute(string xmlAttributeName)
		{
			XmlAttributeName = xmlAttributeName;
		}

		public readonly string XmlAttributeName;
	}

	internal interface IXmlAction
	{
		string Name { get; }
		void Apply(XmlElement actionNode, object item);
	}

	internal abstract class XmlActionContext
	{
		private string _propertyName;

		protected XmlActionContext()
		{
		}

		[XmlActionProperty("property")]
		public string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

		public object GetPropertyValue(object item)
		{
			if (item != null)
			{
				if (!String.IsNullOrEmpty(_propertyName))
				{
					PropertyInfo property = item.GetType().GetProperty(_propertyName);
					if (property != null)
						return property.GetValue(item, null);
				}
			}

			return null;
		}

		public void SetPropertyValue(object item, object value)
		{
			if (item == null)
				return;

			if (!String.IsNullOrEmpty(_propertyName))
			{
				PropertyInfo property = item.GetType().GetProperty(_propertyName);
				if (property != null)
					property.SetValue(item, value, null);
			}
		}
	}

	internal abstract class XmlAction : IXmlAction
	{
		public abstract string Name { get; }

		public abstract void Apply(XmlElement actionNode, object item);
	}

	internal abstract class XmlAction<TActionContext> : XmlAction where TActionContext : class, new()
	{
		[ThreadStatic]
		private static Dictionary<string, string> _xmlAttributeMap;

		protected XmlAction()
		{
		}

		private static Dictionary<string, string> XmlAttributeMap
		{
			get
			{
				if (_xmlAttributeMap == null)
				{
					_xmlAttributeMap = new Dictionary<string, string>();

					foreach (PropertyInfo propertyInfo in typeof(TActionContext).GetProperties())
					{
						if (propertyInfo.IsDefined(typeof(XmlActionPropertyAttribute), true))
						{
							XmlActionPropertyAttribute attribute =
								(XmlActionPropertyAttribute)propertyInfo.GetCustomAttributes(typeof(XmlActionPropertyAttribute), true)[0];
							_xmlAttributeMap[attribute.XmlAttributeName] = propertyInfo.Name;
						}
					}
				}

				return _xmlAttributeMap;
			}
		}

		protected static IDictionary<string, string> RemapDeserializedProperties(IDictionary<string, string> deserialized)
		{
			Dictionary<string, string> remapped = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> pair in deserialized)
			{
				if (XmlAttributeMap.ContainsKey(pair.Key))
					remapped[XmlAttributeMap[pair.Key]] = pair.Value;
			}

			return remapped;
		}

		public sealed override void Apply(XmlElement actionNode, object item)
		{
			Dictionary<string, string> contextValues = new Dictionary<string, string>();
			foreach (XmlAttribute attribute in actionNode.Attributes)
				contextValues[attribute.Name] = attribute.InnerText;

			TActionContext context = new TActionContext();
			SimpleSerializer.Serialize<XmlActionPropertyAttribute>(context, RemapDeserializedProperties(contextValues));
			Apply(context, item);
		}

		public abstract void Apply(TActionContext actionContext, object item);
	}

	internal abstract class XmlAction<TItem, TActionContext> : XmlAction<TActionContext>
		where TActionContext : class, new()
		where TItem : class
	{
		public override void Apply(TActionContext actionContext, object item)
		{
			Apply(actionContext, item as TItem);
		}

		public abstract void Apply(TActionContext actionContext, TItem item);
	}

	#endregion

	#region Default Actions

	internal class TrimEndXmlAction : XmlAction<TrimEndXmlAction.Context>
	{
		public class Context : XmlActionContext
		{
			private int _characters;

			public Context()
			{
			}

			[XmlActionProperty("characters")]
			public int Characters
			{
				get { return _characters; }
				set { _characters = value; }
			}
		}

		public TrimEndXmlAction()
		{ }

		#region IXmlAction Members

		public override string Name
		{
			get { return "trim-end"; }
		}

		public override void Apply(Context actionContext, object item)
		{
			if (actionContext.Characters <= 0)
				return;

			string propertyValue = actionContext.GetPropertyValue(item) as string;
			if (propertyValue == null)
				return;

			int length = propertyValue.Length;
			int trim = Math.Min(actionContext.Characters, length);
			propertyValue = propertyValue.Remove(propertyValue.Length - trim);
			actionContext.SetPropertyValue(item, propertyValue);
		}

		#endregion
	}

	internal class TrimStartXmlAction : XmlAction<TrimStartXmlAction.Context>
	{
		public class Context : XmlActionContext
		{
			private int _characters;

			public Context()
			{
			}

			[XmlActionProperty("characters")]
			public int Characters
			{
				get { return _characters; }
				set { _characters = value; }
			}
		}

		public TrimStartXmlAction()
		{ }

		#region IXmlAction Members

		public override string Name
		{
			get { return "trim-start"; }
		}

		public override void Apply(Context actionContext, object item)
		{
			if (actionContext.Characters <= 0)
				return;

			string propertyValue = actionContext.GetPropertyValue(item) as string;
			if (propertyValue == null)
				return;

			int length = propertyValue.Length;
			int trim = Math.Min(actionContext.Characters, length);
			propertyValue = propertyValue.Remove(0, trim);
			actionContext.SetPropertyValue(item, propertyValue);
		}

		#endregion
	}

	internal class ReplaceRegexXmlAction : XmlAction<ReplaceRegexXmlAction.Context>
	{
		public class Context : XmlActionContext
		{
			private string _pattern;
			private string _replacement;

			public Context()
			{
			}

			[XmlActionProperty("replacement")]
			public string Replacement
			{
				get { return _replacement; }
				set { _replacement = value; }
			}

			[XmlActionProperty("pattern")]
			public string Pattern
			{
				get { return _pattern; }
				set { _pattern = value; }
			}
		}

		public ReplaceRegexXmlAction()
		{ }

		#region IXmlAction Members

		public override string Name
		{
			get { return "replace-regex"; }
		}

		public override void Apply(Context actionContext, object item)
		{
			if (String.IsNullOrEmpty(actionContext.Pattern))
				return;

			actionContext.Replacement = actionContext.Replacement ?? "";

			string propertyValue = actionContext.GetPropertyValue(item) as string;
			if (propertyValue == null)
				return;

			string replaced = Regex.Replace(propertyValue, actionContext.Pattern, actionContext.Replacement);
			actionContext.SetPropertyValue(item, replaced);
		}

		#endregion
	}

	internal class ReplaceXmlAction : XmlAction<ReplaceXmlAction.Context>
	{
		public class Context : XmlActionContext
		{
			private string _occurrence;
			private string _replacement;

			public Context()
			{
			}

			[XmlActionProperty("occurrence")]
			public string Occurrence
			{
				get { return _occurrence; }
				set { _occurrence = value; }
			}

			[XmlActionProperty("replacement")]
			public string Replacement
			{
				get { return _replacement; }
				set { _replacement = value; }
			}
		}

		public ReplaceXmlAction()
		{ }

		#region IXmlAction Members

		public override string Name
		{
			get { return "replace"; }
		}

		public override void Apply(Context actionContext, object item)
		{
			if (String.IsNullOrEmpty(actionContext.Occurrence))
				return;

			actionContext.Replacement = actionContext.Replacement ?? "";

			string propertyValue = actionContext.GetPropertyValue(item) as string;
			if (propertyValue == null)
				return;

			string replaced = propertyValue.Replace(actionContext.Occurrence, actionContext.Replacement);
			actionContext.SetPropertyValue(item, replaced);
		}

		#endregion
	}
	
	internal class AppendXmlAction : XmlAction<AppendXmlAction.Context>
	{
		public class Context : XmlActionContext
		{
			private string _value;

			public Context()
			{
			}

			[XmlActionProperty("value")]
			public string Value
			{
				get { return _value; }
				set { _value = value; }
			}
		}

		public AppendXmlAction()
		{ }

		#region IXmlAction Members

		public override string Name
		{
			get { return "append"; }
		}

		public override void Apply(Context actionContext, object item)
		{
			if (string.IsNullOrEmpty(actionContext.Value))
				return;

			object propertyValue = actionContext.GetPropertyValue(item);
			if (propertyValue is string)
				actionContext.SetPropertyValue(item, String.Concat(propertyValue ?? "", actionContext.Value));
		}

		#endregion
	}

	internal class PrependXmlAction : XmlAction<PrependXmlAction.Context>
	{
		public class Context : XmlActionContext
		{
			private string _value;

			public Context()
			{
			}

			[XmlActionProperty("value")]
			public string Value
			{
				get { return _value; }
				set { _value = value; }
			}
		}

		public PrependXmlAction()
		{ }

		#region IXmlAction Members

		public override string Name
		{
			get { return "prepend"; }
		}

		public override void Apply(Context actionContext, object item)
		{
			if (string.IsNullOrEmpty(actionContext.Value))
				return;

			object propertyValue = actionContext.GetPropertyValue(item);
			if (propertyValue is string)
				actionContext.SetPropertyValue(item, String.Concat(actionContext.Value, propertyValue ?? ""));
		}

		#endregion
	}
	#endregion

	internal static class DefaultActions
	{
		static DefaultActions()
		{
		}

		public static List<IXmlAction> GetStandardActions()
		{
			List<IXmlAction> actions = new List<IXmlAction>();

			//TODO: when this becomes non-internal, make these extensions.
			actions.Add(new TrimEndXmlAction());
			actions.Add(new TrimStartXmlAction());
			actions.Add(new ReplaceRegexXmlAction());
			actions.Add(new ReplaceXmlAction());
			actions.Add(new AppendXmlAction());
			actions.Add(new PrependXmlAction());

			return actions;
		}
	}

	internal class XmlActionsApplicator
	{
		private readonly XmlSpecificationCompiler _compiler = new XmlSpecificationCompiler("jscript");
		private readonly Dictionary<string, IXmlAction> _actions;

		public XmlActionsApplicator(IEnumerable<IXmlAction> actions)
		{
			_actions = new Dictionary<string, IXmlAction>();

			foreach (IXmlAction action in actions)
				_actions[action.Name] = action;
		}

		public bool Apply(XmlElement ruleNode, object item)
		{
			XmlElement conditionNode = ruleNode.SelectSingleNode("condition") as XmlElement;
			if (conditionNode != null)
			{
				ISpecification specification = _compiler.Compile(conditionNode, true);
				if (specification != null)
				{
					if (specification.Test(item).Success)
					{
						XmlElement actionsNode = ruleNode.SelectSingleNode("actions") as XmlElement;
						if (actionsNode != null)
						{
							foreach (XmlNode actionNode in actionsNode.ChildNodes)
							{
								if (actionNode is XmlElement)
								{
									if (_actions.ContainsKey(actionNode.Name))
									{
										IXmlAction action = _actions[actionNode.Name];
										if (action != null)
											action.Apply((XmlElement) actionNode, item);
									}
								}
							}
						}

						return true;
					}
				}
			}

			return false;
		}
	}
}
