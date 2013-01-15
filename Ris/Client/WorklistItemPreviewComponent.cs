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

using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class PreviewToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IPreviewToolContext : IToolContext
	{
		WorklistItemSummaryBase WorklistItem { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public class WorklistItemPreviewComponent : DHtmlComponent, IPreviewComponent
	{
		class PreviewToolContext : ToolContext, IPreviewToolContext
		{
			private readonly WorklistItemPreviewComponent _component;

			public PreviewToolContext(WorklistItemPreviewComponent component)
			{
				_component = component;
			}

			#region IPreviewToolContext Members

			public WorklistItemSummaryBase WorklistItem
			{
				get { return _component._folderSystemItem as WorklistItemSummaryBase; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			#endregion
		}

		private DataContractBase _folderSystemItem;
		private ToolSet _toolSet;

		public override void Start()
		{
			_toolSet = new ToolSet(new PreviewToolExtensionPoint(), new PreviewToolContext(this));
			base.Start();
		}

		public override void Stop()
		{
			_toolSet.Dispose();
			base.Stop();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _folderSystemItem;
		}

		#region IPreviewComponent Members

		void IPreviewComponent.SetPreviewItems(string url, object[] items)
		{
			_folderSystemItem = CollectionUtils.FirstElement<DataContractBase>(items);
			SetUrl(url);
		}

		#endregion
	}
}
