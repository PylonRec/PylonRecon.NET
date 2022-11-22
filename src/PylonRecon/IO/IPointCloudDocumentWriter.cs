namespace PylonRecon.IO;

public interface IPointCloudDocumentWriter
{
    void WriteTo(PointCloud pointCloud, string filePath);
}