using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
    /// <summary>
	/// Extension point for views onto <see cref="CalibrationComponent"/>.
	/// </summary>
    [ExtensionPoint]
    public sealed class MeasurementListApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {

    }

    /// <summary>
    /// MeasurementListComponent class.
    /// </summary>
    [AssociateView(typeof(MeasurementListApplicationComponentViewExtensionPoint))]
    public class MeasurementListApplicationComponent : ImageViewerToolComponent
    {
        public MeasurementListApplicationComponent(IDesktopWindow window) : base(window)
        {

        }
    }
}
