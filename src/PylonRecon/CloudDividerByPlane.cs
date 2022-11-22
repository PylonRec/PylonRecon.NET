using PylonRecon.Geometry;

namespace PylonRecon;

public static class CloudDividerByPlane
{
    public static PointCloud SegmentCloud(PointCloud inputCloud, Plane3D targetPlane, double distanceThreshold) =>
        new(inputCloud.Locations.Where(p => p.DistanceTo(targetPlane) < distanceThreshold));
}