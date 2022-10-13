namespace PylonRecon.IO;

public class XyzPointCloudDataFileWriter : IPointCloudDataFileWriter
{
    public void WriteTo(PointCloud pointCloud, string filePath)
    {
        FileStream stream = new(filePath, FileMode.Create);
        var writer = new StreamWriter(stream);
        foreach (var point in pointCloud.Locations)
        {
            writer.WriteLine($"{point.X}\t{point.Y}\t{point.Z}");
        }
        writer.Flush();
        writer.Close();
        stream.Close();
    }
}