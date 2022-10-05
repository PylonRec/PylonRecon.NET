namespace PylonRecon.IO;

public interface IPointCloudDataFileReader
{
    PointCloud ReadFrom(string filePath);
}