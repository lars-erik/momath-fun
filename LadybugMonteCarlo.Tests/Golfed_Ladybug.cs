// ReSharper disable PossibleMultipleEnumeration
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

    [Test]
    public async Task Multithreaded_Procedural_5Mil()
    {
        var cases = 5_000_000;
        var threads = 16;

        var batchSize = cases / threads;

        var tasks = Enumerable.Range(0, threads)
            .Select(i => Task.Run(() => MakeRuns(i, batchSize)))
            .ToArray();
        var batchResults = await Task.WhenAll(tasks);
        var results = batchResults.SelectMany(x => x);
        var result = new GolfDataset(results);
        
        Assert.That(results.Count(), Is.EqualTo(5_000_000));
        Assert.That(result.Frequencies.Values, Has.All.EqualTo(1.0 / 11).Within(.0055));
        Assert.That(result.MinMaxRounds, Is.EqualTo(new Tuple<int, int>(11, 544)));
    }

    struct GolfDataset
    {
        public IReadOnlyDictionary<int, int> Distribution { get; }
        public IReadOnlyDictionary<int, double> Frequencies { get; }
        public Tuple<int, int> MinMaxRounds { get; }
        public double AverageFrequency { get; }

        public GolfDataset(IEnumerable<(int location, int rounds)> results)
        {
            Distribution = results
                .GroupBy(x => x.location)
                .OrderBy(g => g.Key)
                .ToDictionary(x => x.Key, x => x.Count());
            Frequencies = Distribution.ToDictionary(x => x.Key, x => (double)x.Value / results.Count()).AsReadOnly();
            AverageFrequency = Frequencies.Values.Average();
            MinMaxRounds = new Tuple<int, int>(
                results.Min(x => x.rounds),
                results.Max(x => x.rounds)
            );
        }
    }

    private static GolfDataset FromResults(IEnumerable<(int location, int rounds)> results)
    {
        return new GolfDataset(results);
    }

    private static (int location, int rounds)[] MakeRuns(int seed, int cases)
    {
        var random = new Random(seed);
        return Enumerable.Range(0, cases)
            .Select(_ => MakeClockRun(random))
            .ToArray();
    }

    private static (int location, int rounds) MakeClockRun(Random random)
    {
        var location = 0;
        var visitedCount = 1;
        var visited = new[]{ true, false, false, false, false, false, false, false, false, false, false, false };
        var rounds = 0;
        while (visitedCount < 12)
        {
            int move = random.Next(2) == 0 ? -1 : 1;
            location = (location + move + 12) % 12;
            rounds++;
            if (visited[location]) continue;
            visited[location] = true;
            visitedCount++;
        }

        return (location, rounds);
    }
}
