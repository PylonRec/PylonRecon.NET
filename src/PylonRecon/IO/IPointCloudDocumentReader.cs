namespace PylonRecon.IO;

public interface IPointCloudDocumentReader
{
    PointCloud ReadFrom(string filePath);
}