using System.Drawing;

namespace PylonRecon.IO;

public class PlyDocumentReader : IPointCloudDocumentReader
{
    public PointCloud ReadFrom(string filePath)
    {
        var reader = File.OpenText(filePath);
        List<(string ElementType, int Count)> elements = new();
        List<string> properties = new();
        bool recordFlag = false;
        foreach (var line in ReadHeader(reader))
        {
            if (line.StartsWith("element"))
            {
                var segments = line.Split(' ');
                elements.Add((segments[1], int.Parse(segments[2])));
                recordFlag = segments[1].ToLower() == "vertex";
            }
            else if (line.StartsWith("property") && recordFlag)
            {
                properties.Add(line.Split(' ')[2]);
            }
        }

        List<CloudPoint> points = new();
        foreach (var element in elements)
        {
            if (element.ElementType == "vertex")
            {
                for (int i = 0; i < element.Count; i++)
                {
                    CloudPoint currentPoint = new();
                    var segments = reader.ReadLine()?.Split(' ');
                    if (segments is null) continue;
                    for (int j = 0; j < segments.Length && j < properties.Count; j++)
                    {
                        PropertyHelper.PropertyWriterMapping[properties[j]](segments[j], currentPoint);
                    }
                    points.Add(currentPoint);
                }
            }
        }
        return new(points);
    }
    
    private IEnumerable<string> ReadHeader(StreamReader reader)
    {
        if (reader.ReadLine() is not "ply") throw new FileLoadException("Input file is not a .ply file.");
        if (reader.ReadLine()?.ToLower() is not "format ascii 1.0")
            throw new NotImplementedException("Currently we only support ascii encoding.");
        while (true)
        {
            var currentLine = reader.ReadLine();
            if (currentLine is null) throw new FileLoadException("Broken header info.");
            if (currentLine is "end_header") break;
            yield return currentLine;
        }
    }
    
    private static class PropertyHelper
    {
        private static Dictionary<string, Action<string, CloudPoint>>? _propertyWriterMapping;

        public static Dictionary<string, Action<string, CloudPoint>> PropertyWriterMapping => _propertyWriterMapping ??=
            new()
            {
                {"x", (s, point) => point.Location = (double.Parse(s), point.Location.Y, point.Location.Z)},
                {"y", (s, point) => point.Location = (point.Location.X, double.Parse(s), point.Location.Z)},
                {"z", (s, point) => point.Location = (point.Location.X, point.Location.Y, double.Parse(s))},
                {"nx", (s, point) => point.Normal = (double.Parse(s), point.Normal.Y, point.Normal.Z)},
                {"ny", (s, point) => point.Normal = (point.Normal.X, double.Parse(s), point.Normal.Z)},
                {"nz", (s, point) => point.Normal = (point.Normal.X, point.Normal.Y, double.Parse(s))},
                {
                    "red",
                    (s, point) =>
                        point.Color = Color.FromArgb(point.Color.A, int.Parse(s), point.Color.G, point.Color.B)
                },
                {
                    "green",
                    (s, point) =>
                        point.Color = Color.FromArgb(point.Color.A, point.Color.R, int.Parse(s), point.Color.B)
                },
                {
                    "blue",
                    (s, point) =>
                        point.Color = Color.FromArgb(point.Color.A, point.Color.R, point.Color.G, int.Parse(s))
                },
                {
                    "alpha",
                    (s, point) =>
                        point.Color = Color.FromArgb(int.Parse(s), point.Color.R, point.Color.G, point.Color.B)
                },
                {"material_index", (_, _) => { }}
            };
    }
}