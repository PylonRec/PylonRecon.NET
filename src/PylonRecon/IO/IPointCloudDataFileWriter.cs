namespace PylonRecon.IO;

public interface IPointCloudDataFileWriter
{
    void WriteTo(PointCloud pointCloud, string filePath);
}