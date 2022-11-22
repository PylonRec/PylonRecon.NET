using PylonRecon.Geometry;

namespace PylonRecon;

public static class HoughTransformLineFitter
{
    public static List<Line2D> FitLines(Point2D centerPoint, IEnumerable<Point2D> neighbors, int targetLineCount, double accuracy)
    {
        Point2D houghCenter = new(0d, centerPoint.Y);
        Line2D houghLine = new(houghCenter, new Vector2D(-1d, centerPoint.X));
        Dictionary<int, List<double>> distribution = new();
        foreach (var neighbor in neighbors)
        {
            Line2D neighborHoughLine = new(new Point2D(0d, neighbor.Y), new Vector2D(-1d, neighbor.X));
            if (houghLine.IntersectionPointWith(neighborHoughLine) is not { } intersection) continue;
            double relative = houghCenter.VectorTo(intersection) * houghLine.DirectionVector;
            int index = (int) Math.Floor(relative / accuracy);
            if (distribution.ContainsKey(index)) distribution[index].Add(relative);
            else distribution[index] = new() {relative};
        }

        return distribution
            .OrderByDescending(p => p.Value.Count)
            .Take(targetLineCount)
            .Select(p => p.Value
                .GroupBy(static r => r)
                .MaxBy(g => g.Count())!.Key)
            .Select(r =>
            {
                var targetLineHoughPoint = houghCenter.MoveBy(r * houghLine.DirectionVector);
                return new Line2D(new Point2D(0d, targetLineHoughPoint.Y), new Vector2D(1d, targetLineHoughPoint.X));
            }).ToList();
    }
}