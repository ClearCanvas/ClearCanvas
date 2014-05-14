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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(PerformedStepEditorPageProviderExtensionPoint))]
	public class MppsStructuredDocumentationComponentProvider : IPerformedStepEditorPageProvider
	{
		public IPerformedStepEditorPage[] GetPages(IPerformedStepEditorContext context)
		{
			var component = new MppsStructuredDocumentationComponent(context);
			return new IPerformedStepEditorPage[] {component};
		}
	}

	class MppsStructuredDocumentationComponent : DHtmlComponent, IPerformedStepEditorPage
	{
		#region HealthcareContext

		[DataContract]
		class HealthcareContext : DataContractBase
		{
			private readonly MppsStructuredDocumentationComponent _owner;

			public HealthcareContext(MppsStructuredDocumentationComponent owner)
			{
				_owner = owner;
			}

			[DataMember]
			public EntityRef OrderRef
			{
				get { return _owner._context.OrderRef; }
			}

			[DataMember]
			public EntityRef PatientRef
			{
				get { return _owner._context.PatientRef; }
			}

			[DataMember]
			public EntityRef PatientProfileRef
			{
				get { return _owner._context.PatientProfileRef; }
			}

			[DataMember]
			public ModalityPerformedProcedureStepDetail ModalityPerformedProcedureStep
			{
				get { return _owner._context.SelectedPerformedStep; }
			}
		}

		#endregion

		private readonly IPerformedStepEditorContext _context;

		public MppsStructuredDocumentationComponent(IPerformedStepEditorContext context)
		{
			_context = context;
		}

		public override void Start()
		{
			// when the selected step changes, refresh the browser
			_context.SelectedPerformedStepChanged += delegate
			{
				SetUrl(WebResourcesSettings.Default.DetailsPageUrl);
			};

			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return new HealthcareContext(this);
		}

		protected override IDictionary<string, string> TagData
		{
			get { return _context.SelectedPerformedStepExtendedProperties; }
		}

		#region IPerformedStepEditorPage Members

		Path IExtensionPage.Path
		{
			get { return new Path("TitleDetails", new ResourceResolver(this.GetType().Assembly)); }
		}

		IApplicationComponent IExtensionPage.GetComponent()
		{
			return this;
		}

		void IPerformedStepEditorPage.Save()
		{
			SaveData();
		}

		#endregion
	}
}
