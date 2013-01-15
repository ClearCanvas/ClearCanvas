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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
	/// Defines an extension point for types of actions that can be parsed by the <see cref="XmlActionCompiler{TActionContext, TSchemaContext}"/>.
    /// </summary>
	/// <seealso cref="IXmlActionCompilerOperator{TActionContext, TSchemaContext}"/>
    [ExtensionPoint]
	public sealed class XmlActionCompilerOperatorExtensionPoint<TActionContext, TSchemaContext> : ExtensionPoint<IXmlActionCompilerOperator<TActionContext, TSchemaContext>>
    {
    }

    /// <summary>
    /// Compiler for compiling a set of actions to execute from an XML file.
    /// </summary>
    /// <remarks>
    /// <para>
	/// The <see cref="XmlActionCompiler{TActionContext, TSchemaContext}"/> can be used to compile a set of actions to perform
	/// from XML.  The <see cref="XmlActionCompiler{TActionContext, TSchemaContext}.Compile"/> method can be called to create the
    /// set of actions to be performed.  These actions can then be executed based on input data.
    /// </para>
    /// <para>
	/// Actions are defined by the <see cref="XmlActionCompilerOperatorExtensionPoint{TActionContext, TSchemaContext}"/> extension
    /// point.  The compiler does not contain any predefined actions.  The compiler makes no assumptions
    /// about the attributes of the <see cref="XmlElement"/> for the action.  Any attributes can be defined
    /// for the action and are interpreted by the operation defined for the action type.
    /// </para>
    /// </remarks>
    public class XmlActionCompiler<TActionContext, TSchemaContext>
    {
		private readonly Dictionary<string, IXmlActionCompilerOperator<TActionContext, TSchemaContext>> _operatorMap = new Dictionary<string, IXmlActionCompilerOperator<TActionContext, TSchemaContext>>();
        private readonly Dictionary<TSchemaContext,XmlSchema> _schemas = new Dictionary<TSchemaContext, XmlSchema>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlActionCompiler()
        {
            // add extension operators
			XmlActionCompilerOperatorExtensionPoint<TActionContext, TSchemaContext> xp = new XmlActionCompilerOperatorExtensionPoint<TActionContext, TSchemaContext>();
			foreach (IXmlActionCompilerOperator<TActionContext, TSchemaContext> compilerOperator in xp.CreateExtensions())
            {
                AddOperator(compilerOperator);
            }
        }

        private XmlSchema CreateSchema(TSchemaContext context)
        {
            XmlSchema baseSchema = new XmlSchema();

			foreach (IXmlActionCompilerOperator<TActionContext, TSchemaContext> op in _operatorMap.Values)
            {
                XmlSchemaElement element = op.GetSchema(context);
				if (element != null)
					baseSchema.Items.Add(element);
            }

            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(baseSchema);
            set.Compile();

            XmlSchema compiledSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                compiledSchema = schema;
            }

            //StringWriter sw = new StringWriter();
            //compiledSchema.Write(sw);
            //Platform.Log(LogLevel.Info, sw);

            return compiledSchema;
        }

        /// <summary>
        /// Compile a set of actions to perform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will parse the child <see cref="XmlElement"/>s of <paramref name="containingNode"/>.
		/// Based on the name of the element, the the compiler will look for an <see cref="XmlActionCompilerOperatorExtensionPoint{TActionContext, TSchemaContext}"/>
        /// extension that handles the element type.  A list is constructed of all actions to perform, and a class implementing the 
        /// <see cref="IActionSet{T}"/> interface is returned which can be called to exectute the actions based on input data.
        /// </para>
        /// </remarks>
        /// <param name="containingNode">The input XML containg actions to perform.</param>
        /// <param name="schemaContext"></param>
        /// <param name="checkSchema">Check the schema when compiling.</param>
        /// <returns>A class instance that implements the <see cref="IActionSet{T}"/> interface.</returns>
        public IActionSet<TActionContext> Compile(XmlElement containingNode, TSchemaContext schemaContext, bool checkSchema)
        {
            // Note, recursive calls are made to this method to compile.  The schema is not
            // checked on recursive calls, but should be checked once on an initial compile.
            if (checkSchema)
            {
                // We must parse the XML to get the schema validation to work.  So, we write
                // the xml out to a string, and read it back in with Schema Validation enabled
                StringWriter sw = new StringWriter();

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = Encoding.UTF8;
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
                xmlWriterSettings.Indent = false;
                xmlWriterSettings.NewLineOnAttributes = false;
                xmlWriterSettings.IndentChars = "";

                XmlWriter xmlWriter = XmlWriter.Create(sw, xmlWriterSettings);
                foreach (XmlNode node in containingNode.ChildNodes)
                    node.WriteTo(xmlWriter);
                xmlWriter.Close();

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.Schemas = new XmlSchemaSet();
				if (!_schemas.ContainsKey(schemaContext))
				{
				    XmlSchema s = CreateSchema(schemaContext);
					_schemas.Add(schemaContext, s);

                    //using (StringWriter sw2 = new StringWriter())
                    //{
                    //    s.Write(sw);
                    //    string t = sw2.ToString();
                    //    Platform.Log(LogLevel.Info, sw);
                    //}
				}
				xmlReaderSettings.Schemas.Add(_schemas[schemaContext]);
                xmlReaderSettings.ValidationType = ValidationType.Schema;
                xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                XmlReader xmlReader = XmlTextReader.Create(new StringReader(sw.ToString()), xmlReaderSettings);
                while (xmlReader.Read()) ;
                xmlReader.Close();
            }

            List<IActionItem<TActionContext>> actions = new List<IActionItem<TActionContext>>();
            ICollection<XmlNode> nodes = GetChildElements(containingNode);
            
			foreach(XmlNode node in nodes)
            {
                if (_operatorMap.ContainsKey(node.Name))
                {
                    IXmlActionCompilerOperator<TActionContext, TSchemaContext> op = _operatorMap[node.Name];
                    actions.Add(op.Compile(node as XmlElement));
                }
                else
                {
					throw new XmlActionCompilerException(string.Format(SR.FormatUnableToFindMatchingAction, node.Name));
                }
            }

			return new ActionSet<TActionContext>(actions);
        }

        private void AddOperator(IXmlActionCompilerOperator<TActionContext,TSchemaContext> op)
        {
            _operatorMap.Add(op.OperatorTag, op);
        }

        private static ICollection<XmlNode> GetChildElements(XmlElement node)
        {
            return CollectionUtils.Select<XmlNode>(node.ChildNodes, delegate(XmlNode child) { return child is XmlElement; });
        }
    }
}