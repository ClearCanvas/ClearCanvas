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
using System.Reflection;
using System.Text;
using System.Xml;
using Iesi.Collections;
using NHibernate.Util;
using NHibernate.Cfg;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Analyzes the contents of the <c>hbm.xml</c> files embedded in the 
    /// <see cref="Assembly"/> for their dependency order.
    /// </summary>
    /// <remarks>
    /// This class is modified from version 1.2.0 of NHibernate's AssemblyHbmOrderer.  The class will order all .hbm.xml file in all assemblies
    /// as oppose to ordering only the .hbm.xml files within ONE assembly.  There is a bug report in http://jira.nhibernate.org/browse/NH-989
    /// with the proper fixed in version 1.2.1.  So this class may be obsolete after 1.2.1
    /// </remarks>
    public class AssembliesHbmOrderer
	{
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        public AssembliesHbmOrderer(IEnumerable plugins)
        {
            IList<Assembly> assemblies = CollectionUtils.Map<PluginInfo, Assembly, List<Assembly>>(
				plugins, plugin => plugin.Assembly.Resolve());

            _assemblies.AddRange(assemblies);
        }
        
        public AssembliesHbmOrderer(IEnumerable<Assembly> assemblies)
		{
            _assemblies.AddRange(assemblies);
		}

        public void AddToConfiguration(Configuration cfg)
        {
            ArrayList orderedHbms;
            ArrayList extraFiles;
            this.GetHbmFiles(out orderedHbms, out extraFiles);

            foreach (NonClassEntry file in extraFiles)
            {
                cfg.AddResource(file.FileName, file.Assembly);    
            }

            // Add ordered hbms *after* the extra files, so that the extra files are processed first.
            // This may be useful if the extra files define filters, etc. that are being used by
            // the entity mappings.
            foreach (ClassEntry classEntry in orderedHbms)
            {
                cfg.AddResource(classEntry.FileName, classEntry.Assembly);
            }
        }

        private static ClassEntry BuildClassEntry(XmlReader xmlReader, string fileName, string assemblyName, string @namespace, Assembly assembly)
		{
			xmlReader.MoveToAttribute("name");
			string className = xmlReader.Value;

			string extends = null;
			if (xmlReader.MoveToAttribute("extends"))
			{
				extends = xmlReader.Value;
			}

			return new ClassEntry(extends, className, assemblyName, @namespace, fileName, assembly);
		}

        /// <summary>
        /// Gets an ordered array of .hbm.xml files that contain classes and an array that contains no classes
        /// </summary>
        /// <param name="classEntries">an ordered list of class entries</param>
        /// <param name="nonClassFiles">a list of files that do not contain classes.  These extra files may define filters, etc. 
        /// that are being used by the classes</param>
		private void GetHbmFiles(out ArrayList classEntries, out ArrayList nonClassFiles)
		{
			HashedSet classes = new HashedSet();

			// tracks if any hbm.xml files make use of the "extends" attribute
			bool containsExtends = false;

            // tracks any extra files, i.e. those that do not contain a class definition.
            ArrayList extraFiles = new ArrayList();

            foreach (Assembly assembly in _assemblies)
            {
                foreach (string fileName in assembly.GetManifestResourceNames())
                {
                    if (!fileName.EndsWith(".hbm.xml"))
                        continue;

                    bool fileContainsClasses = false;

                    using (Stream xmlInputStream = assembly.GetManifestResourceStream(fileName))
                    {
                        // XmlReader does not implement IDisposable on .NET 1.1 so have to use
                        // try/finally instead of using here.
                        XmlTextReader xmlReader = new XmlTextReader(xmlInputStream);

                        string assemblyName = null;
                        string @namespace = null;

                        try
                        {
                            while (xmlReader.Read())
                            {
                                if (xmlReader.NodeType != XmlNodeType.Element)
                                {
                                    continue;
                                }

                                switch (xmlReader.Name)
                                {
                                    case "hibernate-mapping":
                                        assemblyName = xmlReader.MoveToAttribute("assembly") ? xmlReader.Value : null;
                                        @namespace = xmlReader.MoveToAttribute("namespace") ? xmlReader.Value : null;
                                        break;
                                    case "class":
                                    case "joined-subclass":
                                    case "subclass":
                                    case "union-subclass":
                                        ClassEntry ce = BuildClassEntry(xmlReader, fileName, assemblyName, @namespace, assembly);
                                        classes.Add(ce);
                                        fileContainsClasses = true;
                                        containsExtends = containsExtends || ce.FullExtends != null;
                                        break;
                                }

                                // no need to keep reading, since we already know the file contains classes
								//if (fileContainsClasses)
								//    break;
                            }
                        }
                        finally
                        {
                            xmlReader.Close();
                        }
                    }

                    if (!fileContainsClasses)
                    {
                        extraFiles.Add(new NonClassEntry(fileName, assembly));
                    }
                }
            }

		    // only bother to do the sorting if one of the hbm files uses 'extends' - 
			// the sorting does quite a bit of looping through collections so if we don't
			// need to spend the time doing that then don't bother.
			if (containsExtends)
			{
				// Add ordered hbms *after* the extra files, so that the extra files are processed first.
				// This may be useful if the extra files define filters, etc. that are being used by
				// the entity mappings.
                classEntries = OrderedHbmFiles(classes);
			}
			else
			{
                ArrayList unSortedList = new ArrayList();
			    unSortedList.AddRange(classes);
			    classEntries = unSortedList;
			}

		    nonClassFiles = extraFiles;
		}

		private static string FormatExceptionMessage(ISet classEntries)
		{
			StringBuilder message = new StringBuilder("These classes extend unmapped classes:");

			foreach (ClassEntry entry in classEntries)
			{
				message.Append('\n')
					.Append(entry.FullClassName)
					.Append(" extends ")
					.Append(entry.FullExtends);
			}

			return message.ToString();
		}

		/// <summary>
		/// Returns an <see cref="IList"/> of <c>hbm.xml</c> files in the order that ensures
		/// base classes are loaded before their subclass/joined-subclass.
		/// </summary>
		/// <param name="unorderedClasses">An <see cref="ISet"/> of <see cref="ClassEntry"/> objects.</param>
		/// <returns>
		/// An <see cref="IList"/> of <see cref="String"/> objects that contain the <c>hbm.xml</c> file names.
		/// </returns>
        private static ArrayList OrderedHbmFiles(ISet unorderedClasses)
		{
			// Make sure joined-subclass mappings are loaded after base class
			ArrayList sortedList = new ArrayList();
			ISet processedClassNames = new HashedSet();
			ArrayList processedInThisIteration = new ArrayList();

			while (true)
			{
				foreach (ClassEntry ce in unorderedClasses)
				{
                    if (ce.FullExtends == null || processedClassNames.Contains(ce.FullExtends))
					{
						// This class extends nothing, or is derived from one of the classes that were already processed.
						// Append it to the list since it's safe to process now.
						// but only if the list doesn't already contain an entry for this file (each file should only appear once in the list)
						if (!CollectionUtils.Contains(sortedList,
							delegate(object entry) { return ((ClassEntry)entry).FileName == ce.FileName; }))
						{
							sortedList.Add(ce);
						}
						processedClassNames.Add(ce.FullClassName);
						processedInThisIteration.Add(ce);
					}
				}

				unorderedClasses.RemoveAll(processedInThisIteration);

				if (processedInThisIteration.Count == 0)
				{
					if (!unorderedClasses.IsEmpty)
					{
						throw new NHibernate.MappingException(FormatExceptionMessage(unorderedClasses));
					}
					break;
				}

				processedInThisIteration.Clear();
			}

            return sortedList;
		}

		/// <summary>
		/// Holds information about mapped classes found in the <c>hbm.xml</c> files.
		/// </summary>
		internal class ClassEntry
		{
			private readonly AssemblyQualifiedTypeName _fullExtends;
			private readonly AssemblyQualifiedTypeName _fullClassName;
			private readonly string _fileName;
		    private readonly Assembly _assembly;

			public ClassEntry(string extends, string className, string assemblyName, string @namespace, string fileName, Assembly assembly)
			{
                _fullExtends = extends == null ? null : TypeNameParser.Parse(extends, @namespace, assemblyName);
                _fullClassName = TypeNameParser.Parse(className, @namespace, assemblyName);
				_fileName = fileName;
                _assembly = assembly;
			}

			public AssemblyQualifiedTypeName FullExtends
			{
				get { return _fullExtends; }
			}

			public AssemblyQualifiedTypeName FullClassName
			{
				get { return _fullClassName; }
			}

			/// <summary>
			/// Gets the name of the <c>hbm.xml</c> file this class was found in.
			/// </summary>
			public string FileName
			{
				get { return _fileName; }
			}

		    public Assembly Assembly
		    {
                get { return _assembly; }
		    }
		}

        internal class NonClassEntry
        {
            private readonly string _fileName;
            private readonly Assembly _assembly;

            public NonClassEntry(string fileName, Assembly assembly)
            {
                _fileName = fileName;
                _assembly = assembly;
            }

            public string FileName
            {
                get { return _fileName; }
            }

            public Assembly Assembly
            {
                get { return _assembly; }
            }
        }
    }
}