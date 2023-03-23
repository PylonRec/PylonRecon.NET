using HelixToolkit.Wpf;
using PylonRecon.Shared;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PylonRecon.Wpf.BridgeOperations
{
    internal class BrowseFileOperations
    {
        private readonly HelixViewport3D _viewport;

        public BrowseFileOperations(HelixViewport3D viewport)
        {
            _viewport = viewport;
        }

        public void Bind()
        {
            BrowseFileBridge.Instance.FileOpenAction = OpenFile;
        }

        private void OpenFile(string path)
        {
            var cloud = new IO.PlyDocumentReader().ReadFrom(path);
            _viewport.Children.Clear();
            Point3DCollection points = new();
            cloud.ToList().ForEach(point => points.Add(new Point3D(point.Location.X, point.Location.Y, point.Location.Z)));
            _viewport.Children.Add(new PointsVisual3D
            {
                Points = points,
                Color = Colors.Green,
                Size = 2.0
            });
            BrowseFileBridge.Instance.LoadPointCloud(cloud);
        }
    }
}
