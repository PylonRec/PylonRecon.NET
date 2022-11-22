using PylonRecon.Geometry;

namespace PylonRecon.IO;

public class XyzDocumentReader : IPointCloudDocumentReader
{
    public PointCloud ReadFrom(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Specified file not found.");
        var reader = File.OpenText(filePath);
        return new(ReadByLine(reader).Distinct());
    }

    private IEnumerable<Point3D> ReadByLine(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            if (reader.ReadLine() is not { } line) continue;
            var segments = line.Split(' ', '\t');
            if (double.TryParse(segments[0], out var x) && double.TryParse(segments[1], out var y) &&
                double.TryParse(segments[2], out var z))
            {
                yield return (x, y, z);
            }
        }
    }
}