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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Provides the Additional Info page to the Order Editor.
	/// </summary>
	[ExtensionOf(typeof(OrderEditorPageProviderExtensionPoint))]
	public class OrderEditorAdditionalInfoPageProvider : IOrderEditorPageProvider
	{
		class OrderEditorAdditionalInfoPage : IOrderEditorPage
		{
			private readonly IOrderEditorContext _context;
			private OrderAdditionalInfoComponent _component;

			public OrderEditorAdditionalInfoPage(IOrderEditorContext context)
			{
				_context = context;
			}

			public void Save()
			{
				_component.SaveData();
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("TitleAdditionalInfo", new ResourceResolver(GetType().Assembly)); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent();
					_component.ModifiedChanged += ((sender, args) => _context.SetModified());
					UpdateComponentData();

					_context.OrderLoaded += (sender, args) => UpdateComponentData();
				}
				return _component;
			}

			private void UpdateComponentData()
			{
				_component.OrderExtendedProperties = _context.OrderExtendedProperties ?? new Dictionary<string, string>();
				_component.Context = _context.OrderRef == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.OrderRef);
			}
		}

		IOrderEditorPage[] IExtensionPageProvider<IOrderEditorPage, IOrderEditorContext>.GetPages(IOrderEditorContext context)
		{
			return new IOrderEditorPage[] { new OrderEditorAdditionalInfoPage(context) };
		}
	}


	/// <summary>
	/// Provides the Additional Info page to the Patient Biography.
	/// </summary>
	[ExtensionOf(typeof(BiographyOrderHistoryPageProviderExtensionPoint))]
	public class BiographyAdditionalInfoPageProvider : IBiographyOrderHistoryPageProvider
	{
		class BiographyAdditionalInfoPage : IBiographyOrderHistoryPage
		{
			private readonly IBiographyOrderHistoryContext _context;
			private OrderAdditionalInfoComponent _component;

			public BiographyAdditionalInfoPage(IBiographyOrderHistoryContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("TitleAdditionalInfo", new ResourceResolver(GetType().Assembly)); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent(true);
					_context.OrderListItemChanged += (sender, args) =>
					{
						_component.OrderExtendedProperties = _context.Order == null ? new Dictionary<string, string>() : _context.Order.ExtendedProperties;
						_component.Context = _context.Order == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.Order.OrderRef);
					};
				}
				return _component;
			}
		}

		IBiographyOrderHistoryPage[] IExtensionPageProvider<IBiographyOrderHistoryPage, IBiographyOrderHistoryContext>.GetPages(IBiographyOrderHistoryContext context)
		{
			return new IBiographyOrderHistoryPage[] { new BiographyAdditionalInfoPage(context) };
		}
	}


	/// <summary>
	/// Provides the Additional Info page to the Performing workspace.
	/// </summary>
	[ExtensionOf(typeof(PerformingDocumentationOrderDetailsPageProviderExtensionPoint))]
	public class PerformingDocumentationAdditionalInfoPageProvider : IPerformingDocumentationOrderDetailsPageProvider
	{
		public class PerformingDocumentationAdditionalInfoPage : IPerformingDocumentationOrderDetailsPage
		{
			private readonly IPerformingDocumentationOrderDetailsContext _context;
			private OrderAdditionalInfoComponent _component;

			public PerformingDocumentationAdditionalInfoPage(IPerformingDocumentationOrderDetailsContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("TitleAdditionalInfo", new ResourceResolver(GetType().Assembly)); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent
									{
										OrderExtendedProperties = _context.OrderExtendedProperties,
										Context = new OrderAdditionalInfoComponent.HealthcareContext(_context.WorklistItem.OrderRef)
									};
				}
				return _component;
			}

			public void Save()
			{
				_component.SaveData();
			}
		}

		IPerformingDocumentationOrderDetailsPage[] IExtensionPageProvider<IPerformingDocumentationOrderDetailsPage, IPerformingDocumentationOrderDetailsContext>.GetPages(IPerformingDocumentationOrderDetailsContext context)
		{
			return new IPerformingDocumentationOrderDetailsPage[] { new PerformingDocumentationAdditionalInfoPage(context) };
		}
	}

	/// <summary>
	/// Provides the Additional Info page to the Reporting workspace.
	/// </summary>
	[ExtensionOf(typeof(ReportingPageProviderExtensionPoint))]
	public class ReportingAdditionalInfoPageProvider : IReportingPageProvider
	{
		class ReportingAdditionalInfoPage : IReportingPage
		{
			private readonly IReportingContext _context;
			private OrderAdditionalInfoComponent _component;

			public ReportingAdditionalInfoPage(IReportingContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("TitleAdditionalInfo", new ResourceResolver(GetType().Assembly)); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent(true);
					UpdateComponentData();

					_context.WorklistItemChanged += (sender, args) => UpdateComponentData();
				}
				return _component;
			}

			private void UpdateComponentData()
			{
				_component.OrderExtendedProperties = _context.Order == null ? new Dictionary<string, string>() : _context.Order.ExtendedProperties;
				_component.Context = _context.Order == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.Order.OrderRef);
			}
		}


		public IReportingPage[] GetPages(IReportingContext context)
		{
			return new IReportingPage[] { new ReportingAdditionalInfoPage(context) };
		}
	}

	/// <summary>
	/// Provides the Additional Info page to the Merge Orders component.
	/// </summary>
	[ExtensionOf(typeof(MergeOrdersPageProviderExtensionPoint))]
	public class MergeOrdersAdditionalInfoPageProvider : IMergeOrdersPageProvider
	{
		class MergeOrdersAdditionalInfoPage : IMergeOrdersPage
		{
			private readonly IMergeOrdersContext _context;
			private OrderAdditionalInfoComponent _component;

			public MergeOrdersAdditionalInfoPage(IMergeOrdersContext context)
			{
				_context = context;
			}

			public Desktop.Path Path
			{
				get { return new Desktop.Path("TitleAdditionalInfo", new ResourceResolver(GetType().Assembly)); }
			}

			public Desktop.IApplicationComponent GetComponent()
			{
				if (_component == null)
				{
					_component = new OrderAdditionalInfoComponent(true);
					UpdateComponentData();

					_context.DryRunMergedOrderChanged += (sender, args) => UpdateComponentData();
				}
				return _component;
			}

			private void UpdateComponentData()
			{
				_component.OrderExtendedProperties = _context.DryRunMergedOrder == null ? new Dictionary<string, string>() : _context.DryRunMergedOrder.ExtendedProperties;
				_component.Context = _context.DryRunMergedOrder == null ? null : new OrderAdditionalInfoComponent.HealthcareContext(_context.DryRunMergedOrder.OrderRef);
			}
		}

		public IMergeOrdersPage[] GetPages(IMergeOrdersContext context)
		{
			return new IMergeOrdersPage[] { new MergeOrdersAdditionalInfoPage(context) };
		}
	}
}
