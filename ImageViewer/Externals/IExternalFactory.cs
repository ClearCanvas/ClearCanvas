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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Externals
{
	[ExtensionPoint]
	public sealed class ExternalFactoryExtensionPoint : ExtensionPoint<IExternalFactory>
	{
		public IExternalFactory CreateExtension(string externalType)
		{
			var extensions = CreateExtensions();
			var result = (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType.FullName == externalType);
			result = result ?? (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType.Name == externalType);
			result = result ?? (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType.AssemblyQualifiedName == externalType);
			return result;
		}

		public IExternalFactory CreateExtension(Type externalType)
		{
			var extensions = CreateExtensions();
			return (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType == externalType);
		}
	}

	public interface IExternalFactory
	{
		string Description { get; }
		Type ExternalType { get; }
		IExternal CreateNew();
		IExternalPropertiesComponent CreatePropertiesComponent();
	}

	public abstract class ExternalFactoryBase<T> : IExternalFactory where T : IExternal, new()
	{
		private readonly string _description;

		protected ExternalFactoryBase(string description)
		{
			_description = description;
		}

		public string Description
		{
			get { return _description; }
		}

		public Type ExternalType
		{
			get { return typeof (T); }
		}

		public IExternal CreateNew()
		{
			T t = new T();
			t.Enabled = true;
			t.Name = t.Label = SR.StringNewExternal;
			t.WindowStyle = WindowStyle.Normal;
			return t;
		}

		public abstract IExternalPropertiesComponent CreatePropertiesComponent();
	}
}