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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal class VoiLutManagerProxy : IVoiLutManager
	{
		[CloneIgnore]
		private readonly IVoiLutManager _placeholderVoiLutManager;

		[CloneIgnore]
		private IVoiLutManager _realVoiLutManager;

		public VoiLutManagerProxy()
		{
			_placeholderVoiLutManager = new VoiLutManager(new XVoiLutInstaller(), false);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected VoiLutManagerProxy(VoiLutManagerProxy source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_placeholderVoiLutManager = new VoiLutManager(new XVoiLutInstaller(source._realVoiLutManager ?? source._placeholderVoiLutManager), false);
		}

		public void SetRealVoiLutManager(IVoiLutManager realVoiLutManager)
		{
			if (_realVoiLutManager != null)
			{
				Replicate(_realVoiLutManager, _placeholderVoiLutManager);
			}

			_realVoiLutManager = realVoiLutManager;

			if (_realVoiLutManager != null)
			{
				Replicate(_placeholderVoiLutManager, _realVoiLutManager);
			}
		}

		private static void Replicate<T>(T source, T destination) where T : IMemorable
		{
			destination.SetMemento(source.CreateMemento());
		}

		#region IVoiLutManager Members

		[Obsolete("Use the VoiLut property instead.")]
		IVoiLut IVoiLutManager.GetLut()
		{
			if (_realVoiLutManager != null)
				return _realVoiLutManager.GetLut();
			else
				return _placeholderVoiLutManager.GetLut();
		}

		[Obsolete("Use the InstallVoiLut method instead.")]
		void IVoiLutManager.InstallLut(IVoiLut voiLut)
		{
			if (_realVoiLutManager != null)
				_realVoiLutManager.InstallLut(voiLut);
			else
				_placeholderVoiLutManager.InstallLut(voiLut);
		}

		[Obsolete("Use the Invert property instead.")]
		void IVoiLutManager.ToggleInvert() {}

		bool IVoiLutManager.Enabled
		{
			get
			{
				if (_realVoiLutManager != null)
					return _realVoiLutManager.Enabled;
				else
					return _placeholderVoiLutManager.Enabled;
			}
			set
			{
				if (_realVoiLutManager != null)
					_realVoiLutManager.Enabled = value;
				else
					_placeholderVoiLutManager.Enabled = value;
			}
		}

		#endregion

		#region IVoiLutInstaller Members

		IVoiLut IVoiLutInstaller.VoiLut
		{
			get
			{
				if (_realVoiLutManager != null)
					return _realVoiLutManager.VoiLut;
				else
					return _placeholderVoiLutManager.VoiLut;
			}
		}

		void IVoiLutInstaller.InstallVoiLut(IVoiLut voiLut)
		{
			if (_realVoiLutManager != null)
				_realVoiLutManager.InstallVoiLut(voiLut);
			else
				_placeholderVoiLutManager.InstallVoiLut(voiLut);
		}

		bool IVoiLutInstaller.Invert
		{
			get { return false; }
			set { }
		}

        bool IVoiLutInstaller.DefaultInvert
        {
            get { return false; }
        }

		#endregion

		#region IMemorable Members

		object IMemorable.CreateMemento()
		{
			if (_realVoiLutManager != null)
				return _realVoiLutManager.CreateMemento();
			else
				return _placeholderVoiLutManager.CreateMemento();
		}

		void IMemorable.SetMemento(object memento)
		{
			if (_realVoiLutManager != null)
			{
				_realVoiLutManager.SetMemento(memento);
				_realVoiLutManager.Invert = false;
			}
			else
			{
				_placeholderVoiLutManager.SetMemento(memento);
				_placeholderVoiLutManager.Invert = false;
			}
		}

		#endregion

		#region XVoiLutInstaller Class

		private class XVoiLutInstaller : IVoiLutInstaller
		{
			public bool Invert { get; set; }
            public bool DefaultInvert { get; private set; }
            public IVoiLut VoiLut { get; set; }

			public XVoiLutInstaller()
			{
				DefaultInvert = this.Invert = false;
				this.VoiLut = new BasicVoiLutLinear(ushort.MaxValue + 1, 0);
			}

			public XVoiLutInstaller(IVoiLutInstaller source)
			{
                DefaultInvert = this.Invert = source.Invert;
				this.VoiLut = source.VoiLut.Clone();
			}

			public void InstallVoiLut(IVoiLut voiLut)
			{
				this.VoiLut = voiLut;
			}
		}

		#endregion
	}
}