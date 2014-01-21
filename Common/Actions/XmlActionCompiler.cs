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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
    /// Compiler for compiling a set of actions to execute from an XML file.
    /// </summary>
    /// <remarks>
    /// <para>
	/// The <see cref="XmlActionCompiler{TActionContext}"/> can be used to compile a set of actions to perform
	/// from XML.  The <see cref="XmlActionCompiler{TActionContext}.Compile"/> method can be called to create the
    /// set of actions to be performed.  These actions can then be executed based on input data.
    /// </para>
    /// <para>
	/// Action operators are defined by an extension point supplied to the constructor, or can be manually supplied.
    /// The compiler does not contain any predefined operators.  The compiler makes no assumptions
    /// about the attributes of the <see cref="XmlElement"/> for the operator.  Any attributes can be defined
    /// for the operator and are interpreted by the operation defined for the action type.
    /// </para>
    /// </remarks>
    public class XmlActionCompiler<TActionContext>
    {
		private readonly Dictionary<string, IXmlActionCompilerOperator<TActionContext>> _operatorMap = new Dictionary<string, IXmlActionCompilerOperator<TActionContext>>();
		private XmlSchema _schema;

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlActionCompiler(IExtensionPoint operators)
			: this(operators.CreateExtensions().Cast<IXmlActionCompilerOperator<TActionContext>>())
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		public XmlActionCompiler(IEnumerable<IXmlActionCompilerOperator<TActionContext>> operators)
		{
			// add extension operators
			foreach (var compilerOperator in operators)
			{
				AddOperator(compilerOperator);
			}
		}

		/// <summary>
		/// A compiled XML schema used by the compiler to verify specifications.
		/// </summary>
		public XmlSchema Schema
		{
			get { return _schema ?? (_schema = CreateSchema()); }
		}

		/// <summary>
        /// Compile a set of actions to perform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will parse the child <see cref="XmlElement"/>s of <paramref name="containingNode"/>.
		/// Based on the name of the element, the the compiler will look for an action that handles the element type.
		/// A list is constructed of all actions to perform, and a class implementing the 
        /// <see cref="IActionList{TActionContext}"/> interface is returned which can be called to exectute the actions based on input data.
        /// </para>
        /// </remarks>
        /// <param name="containingNode">The input XML containg actions to perform.</param>
        /// <param name="checkSchema">Check the schema when compiling.</param>
        /// <returns>A class instance that implements the <see cref="IActionList{TActionContext}"/> interface.</returns>
        public IActionList<TActionContext> Compile(XmlElement containingNode, bool checkSchema)
        {
            // Note, recursive calls are made to this method to compile.  The schema is not
            // checked on recursive calls, but should be checked once on an initial compile.
            if (checkSchema)
            {
                CheckSchema(containingNode);
            }

            var actions = new List<IActionItem<TActionContext>>();
            var nodes = GetChildElements(containingNode);
            
			foreach(var node in nodes)
            {
                if (_operatorMap.ContainsKey(node.Name))
                {
                    var op = _operatorMap[node.Name];
                    actions.Add(op.Compile(node as XmlElement));
                }
                else
                {
					throw new XmlActionCompilerException(string.Format(SR.FormatUnableToFindMatchingAction, node.Name));
                }
            }

			return new ActionList<TActionContext>(actions);
        }

    	private void CheckSchema(XmlElement containingNode)
    	{
			// We must parse the XML to get the schema validation to work.  So, we write
    		// the xml out to a string, and read it back in with Schema Validation enabled
    		var sw = new StringWriter();

    		var xmlWriterSettings = new XmlWriterSettings
    		                        	{
    		                        		Encoding = Encoding.UTF8,
    		                        		ConformanceLevel = ConformanceLevel.Fragment,
    		                        		Indent = false,
    		                        		NewLineOnAttributes = false,
    		                        		IndentChars = ""
    		                        	};

    		var xmlWriter = XmlWriter.Create(sw, xmlWriterSettings);
    		foreach (XmlNode node in containingNode.ChildNodes)
    			node.WriteTo(xmlWriter);
    		xmlWriter.Close();

    		var xmlReaderSettings = new XmlReaderSettings
    		                        	{
    		                        		Schemas = new XmlSchemaSet(),
    		                        		ValidationType = ValidationType.Schema,
    		                        		ConformanceLevel = ConformanceLevel.Fragment
    		                        	};
    		xmlReaderSettings.Schemas.Add(this.Schema);

    		var xmlReader = XmlTextReader.Create(new StringReader(sw.ToString()), xmlReaderSettings);
    		while (xmlReader.Read()) ;
    		xmlReader.Close();
    	}

		private XmlSchema CreateSchema()
		{
			var baseSchema = new XmlSchema();

			foreach (var op in _operatorMap.Values)
			{
				var element = op.GetSchema();
				if (element != null)
					baseSchema.Items.Add(element);
			}

			var set = new XmlSchemaSet();
			set.Add(baseSchema);
			set.Compile();

			XmlSchema compiledSchema = null;
			foreach (XmlSchema schema in set.Schemas())
			{
				compiledSchema = schema;
			}

			return compiledSchema;
		}

		private void AddOperator(IXmlActionCompilerOperator<TActionContext> op)
        {
            _operatorMap.Add(op.OperatorTag, op);
        }

        private static IEnumerable<XmlNode> GetChildElements(XmlElement node)
        {
            return CollectionUtils.Select<XmlNode>(node.ChildNodes, child => child is XmlElement);
        }
    }
}