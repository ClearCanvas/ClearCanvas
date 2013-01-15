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

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Adapted from C:\Program Files\Microsoft SDKs\Windows\v6.0A\Include\vsstyle.h
	/// </summary>
	public class VsStyles
	{
		public class ProgressBar
		{
			public const string ProgressStyle = "PROGRESSSTYLE";
			public const string Progress = "PROGRESS";

			public class ProgressParts
			{
				public const int PP_BAR = 1;
				public const int PP_BARVERT = 2;
				public const int PP_CHUNK = 3;
				public const int PP_CHUNKVERT = 4;
				public const int PP_FILL = 5;
				public const int PP_FILLVERT = 6;
				public const int PP_PULSEOVERLAY = 7;
				public const int PP_MOVEOVERLAY = 8;
				public const int PP_PULSEOVERLAYVERT = 9;
				public const int PP_MOVEOVERLAYVERT = 10;
				public const int PP_TRANSPARENTBAR = 11;
				public const int PP_TRANSPARENTBARVERT = 12;
			};



			public class TransparentBarStates
			{
				public const int PBBS_NORMAL = 1;
				public const int PBBS_PARTIAL = 2;
			};

			public class TransparentBarVertStates
			{
				public const int PBBVS_NORMAL = 1;
				public const int PBBVS_PARTIAL = 2;
			};

			public class FillStates
			{
				public const int PBFS_NORMAL = 1;
				public const int PBFS_ERROR = 2;
				public const int PBFS_PAUSED = 3;
				public const int PBFS_PARTIAL = 4;
			};

			public class FillVertStates
			{
				public const int PBFVS_NORMAL = 1;
				public const int PBFVS_ERROR = 2;
				public const int PBFVS_PAUSED = 3;
				public const int PBFVS_PARTIAL = 4;
			};
		}
	}
}
