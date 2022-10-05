using PylonRecon;
using PylonRecon.IO;

XyzPointCloudDataFileReader reader = new();
var cloud = reader.ReadFrom("/Users/brandon/Desktop/type1_00.xyz");
var centralAxisFinder = new CentralAxisFinder(cloud);
var axi = centralAxisFinder.FindCentralAxis(500, 0.4, 0.02);