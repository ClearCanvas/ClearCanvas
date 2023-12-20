namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis
{
    public interface IRoiInfoItem
    {
        string Name { get; }
        object Value { get; }
        string ValueAsString { get; }
        void SetValue(object value);
        bool ComputeValue();
    }
}