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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="WorklistTimeWindowEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class WorklistTimeWindowEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistTimeWindowEditorComponent class
	/// </summary>
	[AssociateView(typeof(WorklistTimeWindowEditorComponentViewExtensionPoint))]
	public class WorklistTimeWindowEditorComponent : ApplicationComponent
	{
		class DummyItem
		{
			private readonly string _displayString;

			public DummyItem(string displayString)
			{
				_displayString = displayString;
			}

			public override string ToString()
			{
				return _displayString;
			}
		}

		private static readonly object Hours = new DummyItem(SR.DummyItemHours);
		private static readonly object Days = new DummyItem(SR.DummyItemDays);

		private static readonly object[] _slidingScaleChoices = { Days, Hours };

		private readonly WorklistAdminDetail _worklistDetail;

		private bool _isFixedTimeWindow;
		private bool _startTimeChecked;
		private bool _endTimeChecked;
		private int _slidingStartTime;  // Represents the start time in hour or day depend on the sliding scale.
		private int _slidingEndTime;  // Represents the end time in hour or day depend on the sliding scale.
		private DateTime _fixedStartTime;
		private DateTime _fixedEndTime;
		private object _slidingScale;
		private readonly bool _setDefaultTimeWindows;
		private readonly int _maxSpanDays;

		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistTimeWindowEditorComponent(WorklistAdminDetail detail, bool setDefaultTimeWindows, int maxSpanDays)
		{
			_worklistDetail = detail;
			_setDefaultTimeWindows = setDefaultTimeWindows;
			_maxSpanDays = maxSpanDays;
		}

		public override void Start()
		{
			// init both fixed and sliding times to "now"
			_fixedStartTime = _fixedEndTime = Platform.Time;
			_slidingStartTime = _slidingEndTime = 0;
			_slidingScale = Hours;

			if (_worklistDetail.StartTime != null)
			{
				_startTimeChecked = true;
				if (_worklistDetail.StartTime.FixedTime != null)
				{
					_fixedStartTime = _worklistDetail.StartTime.FixedTime.Value;
					_isFixedTimeWindow = true;
				}

				if (_worklistDetail.StartTime.RelativeTime != null)
				{
					_slidingScale = GetSlidingScale(_worklistDetail.StartTime);
					_slidingStartTime = ConvertTimePointToRelativeTime(_worklistDetail.StartTime);
				}
			}
			else if (_setDefaultTimeWindows)
			{
				_startTimeChecked = true;
			}

			if (_worklistDetail.EndTime != null)
			{
				_endTimeChecked = true;
				if (_worklistDetail.EndTime.FixedTime != null)
				{
					_fixedEndTime = _worklistDetail.EndTime.FixedTime.Value;
					_isFixedTimeWindow = true;
				}

				if (_worklistDetail.EndTime.RelativeTime != null)
				{
					_slidingScale = GetSlidingScale(_worklistDetail.EndTime);
					_slidingEndTime = ConvertTimePointToRelativeTime(_worklistDetail.EndTime);
				}
			}
			else if (_setDefaultTimeWindows)
			{
				_endTimeChecked = true;
			}

			this.Validation.Add(new ValidationRule("SlidingEndTime",
				delegate
				{
					// this rule only applies if user has selected a sliding time window
					// only need to validate the time difference if both Start and End are specified
					if(!_isFixedTimeWindow && _startTimeChecked && _endTimeChecked)
					{
						var timeDiff = _slidingEndTime - _slidingStartTime;

						// if the scale is Hours, then the end-time must be greater than start-time
						// if the scale is Days, then the end-time must be greater than or equal to start-time
						var ok = _slidingScale == Hours ? (timeDiff > 0) : (timeDiff >= 0);
						var message = _slidingScale == Hours ? SR.MessageEndTimeMustBeGreaterThanStartTime : SR.MessageEndTimeMustBeGreaterOrEqualStartTime;

						// If first validation pass, validate maximum span days
						if (ok && _maxSpanDays > 0)
						{
							// Add 1 days to timeDiff because "X days from now" means the end of the day "X days from now"
							ok = _slidingScale == Days && (timeDiff+1) <= _maxSpanDays
								|| _slidingScale == Hours && timeDiff <= _maxSpanDays * 24;
							message = string.Format(SR.MessageValidateWorklistTimeSpan, _maxSpanDays);
						}

						return new ValidationResult(ok, message);
					}

					// rule not applicable
					return new ValidationResult(true, "");
				}));

			this.Validation.Add(new ValidationRule("FixedEndTime",
				delegate
				{
					// this rule only applies if user has selected a fixed time window
					// only need to validate the time difference if both Start and End are specified
					if (_isFixedTimeWindow && _startTimeChecked && _endTimeChecked)
					{
						var timeDiff = _fixedEndTime.Subtract(_fixedStartTime);
						var ok = timeDiff.Ticks >= 0;
						var message = SR.MessageEndTimeMustBeGreaterOrEqualStartTime;

						// If first validation pass, validate maximum span days
						if (ok && _maxSpanDays > 0)
						{
							ok = timeDiff.TotalDays <= _maxSpanDays;
							message = string.Format(SR.MessageValidateWorklistTimeSpan, _maxSpanDays);
						}

						return new ValidationResult(ok, message);
					}

					// rule not applicable
					return new ValidationResult(true, "");
				}));

			base.Start();
		}

		#region Presentation Model

		public bool IsFixedTimeWindow
		{
			get { return _isFixedTimeWindow; }
			set
			{
				if (value == _isFixedTimeWindow)
					return;

				_isFixedTimeWindow = value;
				this.Modified = true;

				NotifyPropertyChanged("SlidingStartTimeEnabled");
				NotifyPropertyChanged("FixedStartTimeEnabled");
				NotifyPropertyChanged("SlidingEndTimeEnabled");
				NotifyPropertyChanged("FixedEndTimeEnabled");
				NotifyPropertyChanged("SlidingScaleEnabled");
			}
		}

		public bool IsSlidingTimeWindow
		{
			get { return !_isFixedTimeWindow; }
			set
			{
				// do nothing - the reciprocal IsFixedTimeWindow takes care of it
			}
		}

		public bool FixedSlidingChoiceEnabled
		{
			get { return _startTimeChecked || _endTimeChecked; }
		}

		public bool StartTimeChecked
		{
			get { return _startTimeChecked; }
			set
			{
				if (value == _startTimeChecked)
					return;

				_startTimeChecked = value;
				this.Modified = true;

				NotifyPropertyChanged("SlidingStartTimeEnabled");
				NotifyPropertyChanged("FixedStartTimeEnabled");
				NotifyPropertyChanged("FixedSlidingChoiceEnabled");
				NotifyPropertyChanged("SlidingScaleEnabled");
			}
		}

		public bool SlidingStartTimeEnabled
		{
			get { return _startTimeChecked && !_isFixedTimeWindow; }
		}

		public bool FixedStartTimeEnabled
		{
			get { return _startTimeChecked && _isFixedTimeWindow; }
		}

		public bool EndTimeChecked
		{
			get { return _endTimeChecked; }
			set
			{
				if (value == _endTimeChecked)
					return;

				_endTimeChecked = value;
				this.Modified = true;

				NotifyPropertyChanged("SlidingEndTimeEnabled");
				NotifyPropertyChanged("FixedEndTimeEnabled");
				NotifyPropertyChanged("FixedSlidingChoiceEnabled");
				NotifyPropertyChanged("SlidingScaleEnabled");
			}
		}

		public bool SlidingEndTimeEnabled
		{
			get { return _endTimeChecked && !_isFixedTimeWindow; }
		}

		public bool FixedEndTimeEnabled
		{
			get { return _endTimeChecked && _isFixedTimeWindow; }
		}

		public DateTime FixedStartTime
		{
			get { return _fixedStartTime; }
			set
			{
				if (_fixedStartTime == value)return;

				_fixedStartTime = value;
				this.Modified = true;
			}
		}

		public DateTime FixedEndTime
		{
			get { return _fixedEndTime; }
			set
			{
				if (_fixedEndTime == value)
					return;

				_fixedEndTime = value;
				this.Modified = true;
			}
		}

		public bool SlidingScaleEnabled
		{
			get { return !_isFixedTimeWindow && (_startTimeChecked || _endTimeChecked); }
		}

		public IList SlidingScaleChoices
		{
			get { return _slidingScaleChoices; }
		}

		public object SlidingScale
		{
			get { return _slidingScale; }
			set
			{
				if (value == _slidingScale)
					return;

				_slidingScale = value;

				// if validation is enabled, temporarily disable it, otherwise we get exceptions from transient comparisons
				var validationVisible = this.ValidationVisible;
				ShowValidation(false);

				_slidingStartTime = _slidingEndTime = 0;

				NotifyPropertyChanged("SlidingStartTime");
				NotifyPropertyChanged("SlidingEndTime");
				NotifyPropertyChanged("SlidingTimeMaximum");
				NotifyPropertyChanged("SlidingTimeMinimum");

				// re-enable validation if enabled
				ShowValidation(validationVisible);

				this.Modified = true;
			}
		}

		public int SlidingStartTime
		{
			get { return _slidingStartTime; }
			set
			{
				if (Equals(value, _slidingStartTime))
					return;

				_slidingStartTime = value;
				this.Modified = true;
				NotifyPropertyChanged("SlidingStartTime");
			}
		}

		public int SlidingEndTime
		{
			get { return _slidingEndTime; }
			set
			{
				if (Equals(value, _slidingEndTime))
					return;

				_slidingEndTime = value;
				this.Modified = true;
			}
		}

		public string FormatSlidingTime(decimal value)
		{
			return ConvertSlidingValueToRelativeTime((int)value).ToString();
		}

		#endregion

		internal void SaveData()
		{
			if (_startTimeChecked)
			{
				_worklistDetail.StartTime = _isFixedTimeWindow 
					? new WorklistAdminDetail.TimePoint(_fixedStartTime, 1440)
					: ConvertRelativeTimeToTimePoint(ConvertSlidingValueToRelativeTime(_slidingStartTime));
			}
			else
				_worklistDetail.StartTime = null;

			if (_endTimeChecked)
			{
				_worklistDetail.EndTime = _isFixedTimeWindow 
					? new WorklistAdminDetail.TimePoint(_fixedEndTime, 1440)
					: ConvertRelativeTimeToTimePoint(ConvertSlidingValueToRelativeTime(_slidingEndTime));
			}
			else
				_worklistDetail.EndTime = null;
		}

		private static object GetSlidingScale(WorklistAdminDetail.TimePoint ts)
		{
			// resolution 1440 minutes per day
			return ts.Resolution == 1440 ? Days : Hours;
		}

		private static int ConvertTimePointToRelativeTime(WorklistAdminDetail.TimePoint ts)
		{
			// resolution 1440 minutes per day
			return ts.Resolution == 1440 ? ts.RelativeTime.Value.Days : ts.RelativeTime.Value.Hours;
		}

		private static WorklistAdminDetail.TimePoint ConvertRelativeTimeToTimePoint(RelativeTime rt)
		{
			// in days, use a resolution of days (1440 mintues per day)
			if (rt is RelativeTimeInDays)
				return new WorklistAdminDetail.TimePoint(TimeSpan.FromDays(rt.Value), 1440);

			// in hours, use real-time resolution
			// (this was a User Experience decision - that it would be more intuitive to have a real-time window, rather than nearest-hour)
			if (rt is RelativeTimeInHours)
				return new WorklistAdminDetail.TimePoint(TimeSpan.FromHours(rt.Value), 0);

			// no other types are currently implemented
			throw new NotImplementedException();
		}

		private RelativeTime ConvertSlidingValueToRelativeTime(int value)
		{
			if (_slidingScale == Hours)
				return new RelativeTimeInHours(value);
			
			return new RelativeTimeInDays(value);
		}
	}
}
