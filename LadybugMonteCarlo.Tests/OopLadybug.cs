namespace LadybugMonteCarlo.Tests.OopLadybug;

public record ClockRun(int Rounds, int FinalHour, HashSet<int> VisitedHours, List<int> Log);

public record Ladybug()
{
    public int Position { get; private set; } = 12;

    public void GoRight() => Position = Position % 12 + 1;

    public void GoLeft() => Position = (Position + 10) % 12 + 1;
}

public class BugSimulator(int seed)
{
    private readonly Random random = new(seed);

    public List<ClockRun> Runs { get; } = new();

    public void RunCases(int count)
    {
        for (var i = 0; i < count; i++)
        {
            RunCase();
        }
    }

    public void RunCase()
    {
        var bug = new Ladybug();
        var set = new HashSet<int> { 12 };
        var log = new List<int> { 12 };

        while (set.Count < 12)
        {
            Action move =
                random.Next(2) == 0
                ? bug.GoLeft
                : bug.GoRight;

            move();

            set.Add(bug.Position);
            log.Add(bug.Position);
        }

        Runs.Add(new ClockRun(log.Count, bug.Position, set, log));
    }

    public DistributionDataset GetDistribution()
    {
        return new DistributionDataset(Runs);
    }
}

public record DistributionDataset
{
    public Dictionary<int, int> Distribution { get; }

    public IReadOnlyDictionary<int, double> Frequencies { get; }
    
    public int Cases { get; }
    
    public IReadOnlyCollection<int> Values { get; }
    
    public double AverageFrequency { get; }

    public DistributionDataset(List<ClockRun> runs)
    {
        Cases = runs.Count;

        Distribution = runs
            .GroupBy(x => x.FinalHour)
            .OrderBy(g => g.Key)
            .ToDictionary(x => x.Key, x => x.Count());

        Frequencies = Distribution.ToDictionary(x => x.Key, x => (double)x.Value / Cases).AsReadOnly();
        AverageFrequency = Frequencies.Values.Average();

        Values = Distribution.Values.ToArray();

    }

    public double MaxDifference(double expectedChance)
    {
        return Frequencies.Values.Select(x => x - expectedChance).MaxBy(Math.Abs);
    }
}