namespace LadybugMonteCarlo.Tests;

public class Golfed_Ladybug
{
    [Test]
    public void Procedural_ClockRun()
    {
        var results = MakeRuns(1, 10);

        Console.WriteLine(string.Join(", ", results));
        Assert.That(results, Is.EquivalentTo([6, 2, 1, 5, 1, 1, 1, 10, 7, 4]));
    }

    [Test]
    public void Procedural_Frequency_Histogram()
    {
        var results = MakeRuns(1, 10_000);
        var result = FromResults(results);

        Assert.That(result.Frequencies.Values, Has.All.EqualTo(1.0 / 11).Within(.0055));
    }

    struct GolfDataset
    {
        public IReadOnlyDictionary<int, int> Distribution { get; }
        public IReadOnlyDictionary<int, double> Frequencies { get; }
        public double AverageFrequency { get; }

        public GolfDataset(ICollection<int> results)
        {
            Distribution = results
                .GroupBy(x => x)
                .OrderBy(g => g.Key)
                .ToDictionary(x => x.Key, x => x.Count());
            Frequencies = Distribution.ToDictionary(x => x.Key, x => (double)x.Value / results.Count).AsReadOnly();
            AverageFrequency = Frequencies.Values.Average();
        }
    }

    private static GolfDataset FromResults(ICollection<int> results)
    {
        return new GolfDataset(results);
    }

    private static int[] MakeRuns(int seed, int cases)
    {
        var random = new Random(seed);
        return Enumerable.Range(0, cases)
            .Select(_ => MakeClockRun(random))
            .ToArray();
    }

    private static int MakeClockRun(Random random)
    {
        var location = 0;
        var visitedCount = 1;
        var visited = new[]{ true, false, false, false, false, false, false, false, false, false, false, false };
        while (visitedCount < 12)
        {
            int move = random.Next(2) == 0 ? -1 : 1;
            location = (location + move + 12) % 12;
            if (visited[location]) continue;
            visited[location] = true;
            visitedCount++;
        }

        return location;
    }
}
