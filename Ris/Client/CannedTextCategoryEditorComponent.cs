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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CannedTextCategoryEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class CannedTextCategoryEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextCategoryEditorComponent class.
	/// </summary>
	[AssociateView(typeof(CannedTextCategoryEditorComponentViewExtensionPoint))]
	public class CannedTextCategoryEditorComponent : ApplicationComponent
	{
		private readonly List<string> _categoryChoices;
		private string _category;

		public CannedTextCategoryEditorComponent(List<string> categoryChoices, string initialCategory)
		{
			_categoryChoices = categoryChoices;
			_category = initialCategory;
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			// Insert a blank choice as the first element
			_categoryChoices.Insert(0, "");

			base.Start();
		}

		[ValidateNotNull]
		public string Category
		{
			get { return _category; }
			set
			{
				_category = value;
				this.Modified = true;
			}
		}

		public IList CategoryChoices
		{
			get { return _categoryChoices; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}
	}
}
