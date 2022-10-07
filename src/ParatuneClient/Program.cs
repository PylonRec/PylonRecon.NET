using System.Text;
using Newtonsoft.Json;
using ParatuneClient;
using PylonRecon;
using PylonRecon.IO;

Guid clientId = Guid.Parse("f1d133a4-a600-f0d6-d0f2-32b3906f2dec");

Dictionary<Guid, (string, double, double, double)> models = new Dictionary<Guid, (string, double, double, double)>()
{
    {Guid.Parse("b71e21c7-7937-d2ef-e243-7c6c56ca1fe4"), ("../../../data/pylon1.xyz", 0d, 1d, 0d)},
    {Guid.Parse("4a84ddfb-e5ae-85d2-4d06-72016cb655d2"), ("../../../data/pylon2.xyz", 0d, 1d, 0d)},
    {Guid.Parse("58302488-cb63-3fba-682e-6ddc7cbef04e"), ("../../../data/pyramid1.xyz", 0d, 0d, 1d)},
    {Guid.Parse("ddcae59d-1a49-f21b-5d87-2363b47786f1"), ("../../../data/pyramid2.xyz", 0d, 0d, 1d)}
};

HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
HttpClient client = new(clientHandler);

XyzPointCloudDataFileReader reader = new();

while (true)
{
    var response = await client.GetAsync(
        $"http://10.131.63.171:3615/api/job/request?clientId={clientId.ToString()}");
    var job = JsonConvert.DeserializeObject<ExperimentEntry>(await response.Content.ReadAsStringAsync());
    (string Path, double X, double Y, double Z) model = models[job.ModelId];
    var cloud = reader.ReadFrom(model.Path);
    var centralAxisFinder = new CentralAxisFinder(cloud);
    Console.WriteLine($"Center: {centralAxisFinder.GeometricCenter}");
    int experiment = 1;
    centralAxisFinder.GenerationCalculated += (_, eventArgs) =>
    {
        var optimumIndividual = eventArgs.CurrentPopulation.MaxBy(p => p.Item2);
        double x = Math.Cos(optimumIndividual.Item1.Item1) * Math.Sin(optimumIndividual.Item1.Item2);
        double y = Math.Sin(optimumIndividual.Item1.Item1) * Math.Sin(optimumIndividual.Item1.Item2);
        double z = Math.Cos(optimumIndividual.Item1.Item2);
        Console.WriteLine(
            $"Gen. {eventArgs.Generation}: best fitness = {optimumIndividual.Item2}, x = {x}, y = {y}, z={z}");
        switch (experiment)
        {
            case 1: job.Generation1 = eventArgs.Generation; break;
            case 2: job.Generation2 = eventArgs.Generation; break;
            case 3: job.Generation3 = eventArgs.Generation; break;
            case 4: job.Generation4 = eventArgs.Generation; break;
            case 5: job.Generation5 = eventArgs.Generation; break;
        }
    };
    Predicate<(double, double)> endingPredicate = new(tuple =>
    {
        double x = Math.Cos(tuple.Item1) * Math.Sin(tuple.Item2);
        double y = Math.Sin(tuple.Item1) * Math.Sin(tuple.Item2);
        double z = Math.Cos(tuple.Item2);
        return Math.Abs(x - model.X) < 0.05 && Math.Abs(Math.Abs(y) - model.Y) < 0.05 && Math.Abs(z - model.Z) < 0.05;
    });
    for (experiment = 1; experiment <= 5; experiment++)
    {
        _ = centralAxisFinder.FindCentralAxisInfinite(job.CrossoverProbability, job.MutationProbability,
            job.ParentSelectionRatio, job.InitialPopulation, endingPredicate);
    }
    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(job));
    using StreamContent sc = new(new MemoryStream(bytes));
    sc.Headers.ContentLength = bytes.Length;
    sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
    _ = await client.PostAsync("http://10.131.63.171:3615/api/log/entry", sc);
}