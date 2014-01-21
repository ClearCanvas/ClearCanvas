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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	internal interface IAutoVoiLutApplicator
	{
		//Technically, shouldn't be here, but this stuff needs some serious refactoring ...
		IVoiLut GetInitialLut();

		bool ApplyInitialLut();
		bool ApplyNextLut();

		bool ApplyLinearLut(string lutExplanation);
		bool ApplyLinearLut(int index);
		bool ApplyDataLut(string lutExplanation);
		bool ApplyDataLut(int index);

		IAutoVoiLut GetAppliedLut(); //Should 
	}

	internal abstract class AutoVoiLutApplicator : IAutoVoiLutApplicator
	{
		protected IPresentationImage Image { get; private set; }

		#region IAutoVoiLutApplicator Members

		public abstract IVoiLut GetInitialLut();

		public abstract bool ApplyInitialLut();
		public abstract bool ApplyNextLut();
		public abstract bool ApplyLinearLut(string lutExplanation);
		public abstract bool ApplyLinearLut(int index);
		public abstract bool ApplyDataLut(string lutExplanation);
		public abstract bool ApplyDataLut(int index);
		public abstract IAutoVoiLut GetAppliedLut();

		#endregion

		public static bool CanCreate(IPresentationImage image)
		{
			return Grayscale.CanCreateFrom(image) || Color.CanCreateFrom(image) || Other.CanCreateFrom(image);
		}

		public static IAutoVoiLutApplicator Create(IPresentationImage image)
		{
			if (Grayscale.CanCreateFrom(image))
				return new Grayscale {Image = image};

			if (Color.CanCreateFrom(image))
				return new Color {Image = image};

			if (Other.CanCreateFrom(image))
				return new Other {Image = image};

			return null;
		}

		#region Nested type: Grayscale

		private class Grayscale : AutoVoiLutApplicator
		{
			private static readonly IList<State> _stateProgression = new State[]
			                                                         	{
			                                                         		State.PresentationData,
			                                                         		State.PresentationLinear,
			                                                         		State.ImageData,
			                                                         		State.ImageLinear
			                                                         	};

			private IDicomVoiLutsProvider DicomVoiLutsProvider
			{
				get
				{
					if (Image is IDicomVoiLutsProvider)
						return (IDicomVoiLutsProvider) Image;
					return null;
				}
			}

			private GrayscalePixelData PixelData
			{
				get { return (GrayscalePixelData) ((IImageGraphicProvider) Image).ImageGraphic.PixelData; }
			}

			private IVoiLutManager VoiLutManager
			{
				get { return ((IVoiLutProvider) Image).VoiLutManager; }
			}

			private IVoiLut CurrentLut
			{
				get { return VoiLutManager.VoiLut; }
			}

			private MinMaxPixelCalculatedLinearLut GetDefaultMinMaxLut()
			{
				if (LutHelper.IsModalityLutProvider(Image))
					return new MinMaxPixelCalculatedLinearLut(PixelData, ((IModalityLutProvider) Image).ModalityLut);
				else
					return new MinMaxPixelCalculatedLinearLut(PixelData);
			}

			public override IVoiLut GetInitialLut()
			{
				foreach (State state in _stateProgression)
				{
					IVoiLut lut = state.GetLut(this);
					if (lut != null)
						return lut;
				}
				return GetDefaultMinMaxLut();
			}

			public override bool ApplyInitialLut()
			{
				VoiLutManager.InstallVoiLut(GetInitialLut());
				return true;
			}

			public override bool ApplyNextLut()
			{
				IVoiLut currentLut = CurrentLut;
				State currentState = State.GetState(currentLut);

				if (currentLut is IAutoVoiLut)
				{
					var autoVoiLut = (IAutoVoiLut) currentLut;
					if (autoVoiLut.IsLast)
					{
						int nextState = _stateProgression.IndexOf(currentState) + 1;
						for (int n = nextState; n < nextState + _stateProgression.Count; n++)
						{
							IVoiLut lut = _stateProgression[(n%_stateProgression.Count)].GetLut(this);
							if (lut != null)
							{
								VoiLutManager.InstallVoiLut(lut);
								return true;
							}
						}
					}
					else
					{
						autoVoiLut.ApplyNext();
						return true;
					}
				}
				else
				{
					ApplyInitialLut();
					return true;
				}

				return false;
			}

			public override bool ApplyDataLut(int index)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiDataLut lut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider, index);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(new AdjustableDataLut(lut));

				return true;
			}

			public override bool ApplyDataLut(string lutExplanation)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiDataLut lut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider, lutExplanation);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(new AdjustableDataLut(lut));
				return true;
			}

			public override bool ApplyLinearLut(int index)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiLutLinear lut = AutoImageVoiLutLinear.CreateFrom(voiLutsProvider, index);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(lut);
				return true;
			}

			public override bool ApplyLinearLut(string lutExplanation)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiLutLinear lut = AutoImageVoiLutLinear.CreateFrom(voiLutsProvider, lutExplanation);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(lut);
				return true;
			}

			public override IAutoVoiLut GetAppliedLut()
			{
				if (CurrentLut is AdjustableDataLut)
				{
					var adjustable = (AdjustableDataLut) CurrentLut;
					return adjustable.DataLut as IAutoVoiLut;
				}

				return CurrentLut as IAutoVoiLut;
			}

			public static bool CanCreateFrom(IPresentationImage presentationImage)
			{
				return LutHelper.IsDicomVoiLutsProvider(presentationImage)
				       && LutHelper.IsVoiLutProvider(presentationImage)
				       && LutHelper.IsVoiLutEnabled(presentationImage)
				       && LutHelper.IsGrayScaleImage(presentationImage)
				       && LutHelper.IsImageSopProvider(presentationImage);
			}

			#region State Machine

			private class State
			{
				public static readonly State ImageData = new State(GetImageDataLut);
				public static readonly State ImageLinear = new State(GetImageLinearLut);
				public static readonly State PresentationData = new State(GetPresentationDataLut);
				public static readonly State PresentationLinear = new State(GetPresentationLinearLut);
				private readonly LutGetter _lutGetter;

				private State(LutGetter lutGetter)
				{
					_lutGetter = lutGetter;
				}

				public IVoiLut GetLut(Grayscale applicator)
				{
					return _lutGetter(applicator);
				}

				public static State GetState(IVoiLut currentLut)
				{
					if (currentLut is AdjustableDataLut)
					{
						var adj = (AdjustableDataLut) currentLut;
						if (adj.DataLut is AutoPresentationVoiDataLut)
							return PresentationData;
						else if (adj.DataLut is AutoImageVoiDataLut)
							return ImageData;
					}
					else if (currentLut is AutoPresentationVoiLutLinear)
					{
						return PresentationLinear;
					}
					else if (currentLut is AutoImageVoiLutLinear)
					{
						return ImageLinear;
					}
					return null;
				}

				private static AdjustableDataLut GetImageDataLut(Grayscale applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;

					AutoVoiDataLut dataLut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider);
					if (dataLut == null)
						return null;
					return new AdjustableAutoVoiDataLut(dataLut);
				}

				private static AutoVoiLutLinear GetImageLinearLut(Grayscale applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;
					return AutoImageVoiLutLinear.CreateFrom(voiLutsProvider);
				}

				private static AdjustableDataLut GetPresentationDataLut(Grayscale applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;

					AutoVoiDataLut dataLut = AutoPresentationVoiDataLut.CreateFrom(voiLutsProvider);
					if (dataLut == null)
						return null;
					return new AdjustableAutoVoiDataLut(dataLut);
				}

				private static AutoVoiLutLinear GetPresentationLinearLut(Grayscale applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;
					return AutoPresentationVoiLutLinear.CreateFrom(voiLutsProvider);
				}

				#region Nested type: LutGetter

				private delegate IVoiLut LutGetter(Grayscale applicator);

				#endregion
			}

			#endregion
		}

		#endregion

		#region Nested type: Other

		private class Other : AutoVoiLutApplicator
		{
			private static readonly IList<State> _stateProgression = new State[]
			                                                         	{
			                                                         		State.PresentationData,
			                                                         		State.PresentationLinear,
			                                                         		State.ImageData,
			                                                         		State.ImageLinear
			                                                         	};

			private IDicomVoiLutsProvider DicomVoiLutsProvider
			{
				get
				{
					if (Image is IDicomVoiLutsProvider)
						return (IDicomVoiLutsProvider) Image;
					return null;
				}
			}

			private IVoiLutManager VoiLutManager
			{
				get { return ((IVoiLutProvider) Image).VoiLutManager; }
			}

			private IVoiLut CurrentLut
			{
				get { return VoiLutManager.VoiLut; }
			}

			public override IVoiLut GetInitialLut()
			{
				foreach (State state in _stateProgression)
				{
					IVoiLut lut = state.GetLut(this);
					if (lut != null)
						return lut;
				}
				return null;
			}

			public override bool ApplyInitialLut()
			{
				var lut = GetInitialLut();
				if (lut != null)
					VoiLutManager.InstallVoiLut(lut);
				return true;
			}

			public override bool ApplyNextLut()
			{
				IVoiLut currentLut = CurrentLut;
				State currentState = State.GetState(currentLut);

				if (currentLut is IAutoVoiLut)
				{
					var autoVoiLut = (IAutoVoiLut) currentLut;
					if (autoVoiLut.IsLast)
					{
						int nextState = _stateProgression.IndexOf(currentState) + 1;
						for (int n = nextState; n < nextState + _stateProgression.Count; n++)
						{
							IVoiLut lut = _stateProgression[(n%_stateProgression.Count)].GetLut(this);
							if (lut != null)
							{
								VoiLutManager.InstallVoiLut(lut);
								return true;
							}
						}
					}
					else
					{
						autoVoiLut.ApplyNext();
						return true;
					}
				}
				else
				{
					ApplyInitialLut();
					return true;
				}

				return false;
			}

			public override bool ApplyDataLut(int index)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiDataLut lut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider, index);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(new AdjustableDataLut(lut));

				return true;
			}

			public override bool ApplyDataLut(string lutExplanation)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiDataLut lut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider, lutExplanation);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(new AdjustableDataLut(lut));
				return true;
			}

			public override bool ApplyLinearLut(int index)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiLutLinear lut = AutoImageVoiLutLinear.CreateFrom(voiLutsProvider, index);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(lut);
				return true;
			}

			public override bool ApplyLinearLut(string lutExplanation)
			{
				if (VoiLutManager == null)
					return false;

				var voiLutsProvider = Image as IDicomVoiLutsProvider;
				AutoImageVoiLutLinear lut = AutoImageVoiLutLinear.CreateFrom(voiLutsProvider, lutExplanation);
				if (lut == null)
					return false;

				VoiLutManager.InstallVoiLut(lut);
				return true;
			}

			public override IAutoVoiLut GetAppliedLut()
			{
				if (CurrentLut is AdjustableDataLut)
				{
					var adjustable = (AdjustableDataLut) CurrentLut;
					return adjustable.DataLut as IAutoVoiLut;
				}

				return CurrentLut as IAutoVoiLut;
			}

			public static bool CanCreateFrom(IPresentationImage presentationImage)
			{
				return LutHelper.IsDicomVoiLutsProvider(presentationImage)
				       && LutHelper.IsVoiLutProvider(presentationImage)
				       && LutHelper.IsVoiLutEnabled(presentationImage)
				       && LutHelper.IsImageSopProvider(presentationImage);
			}

			#region State Machine

			private class State
			{
				public static readonly State ImageData = new State(GetImageDataLut);
				public static readonly State ImageLinear = new State(GetImageLinearLut);
				public static readonly State PresentationData = new State(GetPresentationDataLut);
				public static readonly State PresentationLinear = new State(GetPresentationLinearLut);
				private readonly LutGetter _lutGetter;

				private State(LutGetter lutGetter)
				{
					_lutGetter = lutGetter;
				}

				public IVoiLut GetLut(Other applicator)
				{
					return _lutGetter(applicator);
				}

				public static State GetState(IVoiLut currentLut)
				{
					if (currentLut is AdjustableDataLut)
					{
						var adj = (AdjustableDataLut) currentLut;
						if (adj.DataLut is AutoPresentationVoiDataLut)
							return PresentationData;
						else if (adj.DataLut is AutoImageVoiDataLut)
							return ImageData;
					}
					else if (currentLut is AutoPresentationVoiLutLinear)
					{
						return PresentationLinear;
					}
					else if (currentLut is AutoImageVoiLutLinear)
					{
						return ImageLinear;
					}
					return null;
				}

				private static AdjustableDataLut GetImageDataLut(Other applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;

					AutoVoiDataLut dataLut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider);
					if (dataLut == null)
						return null;
					return new AdjustableAutoVoiDataLut(dataLut);
				}

				private static AutoVoiLutLinear GetImageLinearLut(Other applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;
					return AutoImageVoiLutLinear.CreateFrom(voiLutsProvider);
				}

				private static AdjustableDataLut GetPresentationDataLut(Other applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;

					AutoVoiDataLut dataLut = AutoPresentationVoiDataLut.CreateFrom(voiLutsProvider);
					if (dataLut == null)
						return null;
					return new AdjustableAutoVoiDataLut(dataLut);
				}

				private static AutoVoiLutLinear GetPresentationLinearLut(Other applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;
					return AutoPresentationVoiLutLinear.CreateFrom(voiLutsProvider);
				}

				#region Nested type: LutGetter

				private delegate IVoiLut LutGetter(Other applicator);

				#endregion
			}

			#endregion
		}

		#endregion

		#region Lut Application for Color Images

		private class Color : AutoVoiLutApplicator
		{
			public Color() {}

			private IVoiLutManager VoiLutManager
			{
				get { return ((IVoiLutProvider) Image).VoiLutManager; }
			}

			public override IVoiLut GetInitialLut()
			{
				return new IdentityVoiLinearLut();
			}

			public override bool ApplyInitialLut()
			{
				VoiLutManager.InstallVoiLut(GetInitialLut());
				return true;
			}

			public override bool ApplyNextLut()
			{
				ApplyInitialLut();
				return true;
			}

			public override bool ApplyDataLut(int index)
			{
				return false;
			}

			public override bool ApplyDataLut(string lutExplanation)
			{
				return false;
			}

			public override bool ApplyLinearLut(int index)
			{
				return false;
			}

			public override bool ApplyLinearLut(string lutExplanation)
			{
				return false;
			}

			public override IAutoVoiLut GetAppliedLut()
			{
				return null;
			}

			public static bool CanCreateFrom(IPresentationImage presentationImage)
			{
				return LutHelper.IsVoiLutProvider(presentationImage)
				       && LutHelper.IsVoiLutEnabled(presentationImage)
				       && LutHelper.IsColorImage(presentationImage)
				       && LutHelper.IsImageSopProvider(presentationImage);
			}
		}

		#endregion
	}
}