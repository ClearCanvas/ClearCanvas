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
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Defines an extension point to provide implementations of <see cref="IProcedureStepBuilder"/>.
	/// </summary>
	[ExtensionPoint]
	public class ProcedureStepBuilderExtensionPoint : ExtensionPoint<IProcedureStepBuilder>
	{
	}

	/// <summary>
	/// Defines an interface to a procedure-step builder.  A procedure-step builder is an object
	/// that is responsible for instantiating a given class of procedure step from an XML plan.
	/// </summary>
	/// <remarks>
	/// Do not implement this interface directly. Instead use the abstract base class
	/// <see cref="ProcedureStepBuilderBase"/>.
	/// </remarks>
	public interface IProcedureStepBuilder
	{
		/// <summary>
		/// Gets the class of procedure step that this builder is responsible for.
		/// </summary>
		Type ProcedureStepClass { get; }

		/// <summary>
		/// Creates an instance of a procedure-step class from XML.
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="procedure"></param>
		/// <returns></returns>
		/// <remarks>
		/// The procedure is provided for reference only.  For example, the builder
		/// may need to create another object that refers to the procedure.  The
		/// builder is NOT responsible for adding the created <see cref="ProcedureStep"/>
		/// to the procedure, and must NOT do so.
		/// </remarks>
		ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure);

		/// <summary>
		/// Creates an XML representation of the specified procedure-step prototype.
		/// </summary>
		/// <param name="prototype"></param>
		/// <param name="xmlNode"></param>
		void SaveInstance(ProcedureStep prototype, XmlElement xmlNode);
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IProcedureStepBuilder"/>.
	/// </summary>
	public abstract class ProcedureStepBuilderBase : IProcedureStepBuilder
	{
		#region IProcedureStepBuilder Members

		/// <summary>
		/// Gets the class of procedure step that this builder is responsible for.
		/// </summary>
		public abstract Type ProcedureStepClass { get; }

		/// <summary>
		/// Creates an instance of a procedure-step class from XML.
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="procedure"></param>
		/// <returns></returns>
		/// <remarks>
		/// The procedure is provided for reference only.  For example, the builder
		/// may need to create another object that refers to the procedure.  The
		/// builder is NOT responsible for adding the created <see cref="ProcedureStep"/>
		/// to the procedure, and must NOT do so.
		/// </remarks>
		public abstract ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure);

		/// <summary>
		/// Creates an XML representation of the specified procedure-step prototype.
		/// </summary>
		/// <param name="prototype"></param>
		/// <param name="xmlNode"></param>
		public abstract void SaveInstance(ProcedureStep prototype, XmlElement xmlNode);

		#endregion

		/// <summary>
		/// Utility method to get the value of an attribute from an XML node, optionally 
		/// throwing an exception if the attribute does not exist.
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="attribute"></param>
		/// <param name="required"></param>
		/// <returns></returns>
		protected static string GetAttribute(XmlElement xmlNode, string attribute, bool required)
		{
			string value = xmlNode.GetAttribute(attribute);
			if (required && string.IsNullOrEmpty(value))
				throw new ProcedureBuilderException(string.Format("Required attribute '{0}' is missing.", attribute));
			return value;
		}
	}

	#region ProcedureBuilderException class

	/// <summary>
	/// Defines an exception class for errors that occur in the <see cref="ProcedureBuilder"/>.
	/// </summary>
	public class ProcedureBuilderException : Exception
	{
		public ProcedureBuilderException(string message)
			: base(message)
		{
		}

		public ProcedureBuilderException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	#endregion

	/// <summary>
	/// Internal class that assembles <see cref="Procedure"/> objects according to a plan
	/// specified by a <see cref="ProcedureType"/>.
	/// </summary>
	internal class ProcedureBuilder
	{

		#region Static Cache

		/// <summary>
		/// Cache of <see cref="IProcedureStepBuilder"/> for each class of procedure step.
		/// </summary>
		private static readonly Dictionary<Type, IProcedureStepBuilder> _mapClassToBuilder;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ProcedureBuilder()
		{
			_mapClassToBuilder = new Dictionary<Type, IProcedureStepBuilder>();
			foreach (IProcedureStepBuilder builder in (new ProcedureStepBuilderExtensionPoint().CreateExtensions()))
			{
				_mapClassToBuilder.Add(builder.ProcedureStepClass, builder);
			}
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Adds procedure steps to the specified <see cref="Procedure"/>,
		/// according to the plan specified by its <see cref="Procedure.Type"/> property.
		/// </summary>
		/// <remarks>
		/// This builds the specified procedure according to its <see cref="Procedure.Type"/>.
		/// It also takes into account any procedure-type inheritance, adding inherited procedure
		/// steps as well.
		/// </remarks>
		/// <param name="procedure"></param>
		internal void BuildProcedureFromPlan(Procedure procedure)
		{
			BuildProcedureFromPlan(procedure, procedure.Type);
		}

		/// <summary>
		/// Uses the specified <see cref="Procedure"/> as a prototype
		/// to create and save a plan in the <see cref="Procedure.Type"/> property.
		/// </summary>
		/// <remarks>
		/// This method generates the procedure plan XML by simply iterating over
		/// all procedure steps in the <see cref="Procedure.ProcedureSteps"/>
		/// property of the specified procedure.  It does not take procedure-plan
		/// inheritance into account. Therefore, it should not be considered an
		/// inverse of <see cref="BuildProcedureFromPlan(Procedure)"/>.
		/// </remarks>
		/// <param name="procedure"></param>
		internal XmlDocument CreatePlanFromProcedure(Procedure procedure)
		{
			var xmlDoc = new XmlDocument();
			var stepsNode = xmlDoc.CreateElement("procedure-steps");
			xmlDoc.AppendChild(stepsNode);
			foreach (var step in procedure.ProcedureSteps)
			{
				var builder = GetBuilderForClass(step.GetClass());
				var stepNode = xmlDoc.CreateElement("procedure-step");
				stepNode.SetAttribute("class", step.GetClass().FullName);
				builder.SaveInstance(step, stepNode);

				stepsNode.AppendChild(stepNode);
			}
			return xmlDoc;
		}

		#endregion

		private static void BuildProcedureFromPlan(Procedure procedure, ProcedureType type)
		{
			// if the type specifies a base type, apply it first
			if (type.BaseType != null)
			{
				BuildProcedureFromPlan(procedure, type.BaseType);
			}
			else
			{
				// otherwise use the root plan as the base plan
				BuildProcedureFromPlan(procedure, ProcedurePlan.GetRootPlan());
			}

			BuildProcedureFromPlan(procedure, type.Plan);
		}

		private static void BuildProcedureFromPlan(Procedure procedure, ProcedurePlan plan)
		{
			foreach (var stepNode in plan.ProcedureStepNodes)
			{
				var className = stepNode.GetAttribute("class");
				if (string.IsNullOrEmpty(className))
					throw new ProcedureBuilderException("Required attribute 'class' is missing.");

				var stepClass = Type.GetType(className);
				if (stepClass == null)
					throw new ProcedureBuilderException(string.Format("Unable to resolve class {0}.", className));

				var builder = GetBuilderForClass(stepClass);
				var step = builder.CreateInstance(stepNode, procedure);

				if (procedure.DowntimeRecoveryMode && !step.CreateInDowntimeMode)
					continue;

				procedure.AddProcedureStep(step);
			}
		}

		private static IProcedureStepBuilder GetBuilderForClass(Type stepClass)
		{
			IProcedureStepBuilder builder;
			if (_mapClassToBuilder.TryGetValue(stepClass, out builder))
				return builder;

			throw new ProcedureBuilderException(string.Format("No builder found for class {0}.", stepClass.FullName));
		}
	}
}
