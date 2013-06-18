#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
    public static class TypeHelper
    {
        public static Type[] GetKnownAttributeValueTypes()
        {
            return new []
                       {
                           typeof(DateTime),
                           typeof(DateTime?),
                           typeof(DateTime[]),
                           typeof(TimeSpan),
                           typeof(TimeSpan?),
                           typeof(TimeSpan[]),
                           typeof(float[]),
                           typeof(double[]),
                           typeof(string[]),
                           typeof(Int16[]),
                           typeof(UInt16[]),
                           typeof(Int32[]),
                           typeof(UInt32[]),
                           typeof(Int64[]),
                           typeof(UInt64[]),
                           typeof(byte[]),
                           typeof(sbyte[])
                        };
        }
    }
}
