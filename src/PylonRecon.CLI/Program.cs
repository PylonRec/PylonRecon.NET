using PylonRecon;
using PylonRecon.IO;

XyzPointCloudDataFileReader reader = new();

var cloud = reader.ReadFrom("/Users/brandon/Desktop/model.xyz");

CentralAxisFinder centralAxisFinder = new(cloud);

centralAxisFinder.GenerationCalculated += (sender, eventArgs) =>
{
    var best = eventArgs.CurrentPopulation.MaxBy(p => p.Item2);
    double dirX = Math.Cos(best.Item1.Item1) * Math.Sin(best.Item1.Item2);
    double dirY = Math.Sin(best.Item1.Item1) * Math.Sin(best.Item1.Item2);
    double dirZ = Math.Cos(best.Item1.Item2);
    Console.WriteLine($"Gen {eventArgs.Generation}: m.f. = {best.Item2}, b.p. = ({dirX}, {dirY}, {dirZ})");
};

var centralAxis = centralAxisFinder.FindCentralAxis(50, 0.6, 0.2, 0.6, 30);

PylonBodyBottomCornerExtractor bodyBottomCornerExtractor = new(cloud, centralAxis);

var cornerPoints = bodyBottomCornerExtractor.Extract();