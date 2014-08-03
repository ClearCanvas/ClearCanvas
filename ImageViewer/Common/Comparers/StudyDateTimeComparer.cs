using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.Comparers
{
	internal class StudyDateTimeComparer : IComparer<StudyIdentifier>, IComparer<StudyRootStudyIdentifier>
	{
		private readonly Dicom.ServiceModel.Query.StudyDateTimeComparer _real = new Dicom.ServiceModel.Query.StudyDateTimeComparer();
		
		#region Implementation of IComparer<in StudyIdentifier>

		public int Compare(StudyIdentifier x, StudyIdentifier y)
		{
			if (x.StudyInstanceUid == y.StudyInstanceUid)
			{
				//When it's the exact same study, we want to order them
				//in the same way that ServerDirectory.GetPriorsServers
				//would have ordered them, which is basically local, streaming, non-streaming.
				
				//Note we ignore the unlikely possibility that the date/time could
				//actually be different for the same study on different servers
				//because it's not really a valid use case, and it is possible that different servers
				//could return slightly different date/time values. Especially time, if it were
				//stored as a DateTime value in a database as opposed to the exact string value
				//from the image header.
				var sortKeyX = ServerDirectory.ServerDirectory.GetSortKey(x.RetrieveAE as IDicomServiceNode);
				var sortKeyY = ServerDirectory.ServerDirectory.GetSortKey(y.RetrieveAE as IDicomServiceNode);
				var compare = sortKeyX.CompareTo(sortKeyY);
				if (compare != 0)
					return compare;
			}

			return _real.Compare(x, y);
		}

		#endregion

		#region Implementation of IComparer<in StudyRootStudyIdentifier>

		public int Compare(StudyRootStudyIdentifier x, StudyRootStudyIdentifier y)
		{
			return Compare((StudyIdentifier)x, y);
		}

		#endregion
	}
}
