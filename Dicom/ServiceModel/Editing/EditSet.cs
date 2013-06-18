#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
 
namespace ClearCanvas.Dicom.ServiceModel.Editing
{
    [DataContract(Namespace = DicomEditNamespace.Value)]
    [EditType("CA1E69EE-5264-4370-9339-F65ED9C78744")]
    public class EditSet : Edit
    {
        private List<Edit> _edits;

        [DataMember(IsRequired = true)]
        public Condition Condition { get; set; }

        [DataMember(IsRequired = true)]
        public List<Edit> Edits
        {
            get { return _edits ?? (_edits = new List<Edit>()); }
            set { _edits = value; }
        }

        #region Overrides of Edit

        public override void Apply(DicomAttributeCollection collection)
        {
            if (Condition != null && !Condition.IsMatch(collection))
                return;

            foreach (var edit in Edits)
                edit.Apply(collection);
        }

        #endregion

        public override string ToString()
        {
            if (Edits.Count == 0)
                return "No Edits";

            if (Edits.Count == 1)
                return "1 Edit";

            return String.Format("{0} Edits", Edits.Count);
        }
    }
}