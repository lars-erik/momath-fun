using Microsoft.Testing.Platform.Logging;

namespace LadybugMonteCarlo.Tests;

public class Monte_Carlo_Ladybug
{
    private Ladybug bug;

    [SetUp]
    public void Setup()
    {
        bug = new Ladybug();
    }

    [Test]
    public void Starts_At_12()
    {
        Assert.That(bug.Position, Is.EqualTo(12));
    }

    [Test]
    public void Goes_Right_To_1_From_12()
    {
        bug.GoRight();
        Assert.That(bug.Position, Is.EqualTo(1));
    }

    [Test]
    public void Goes_Left_To_12_From_1()
    {
        bug.GoRight();
        bug.GoLeft();
        Assert.That(bug.Position, Is.EqualTo(12));
    }

    [Test]
    [TestCase(1, 25, 11)]
    [TestCase(1, 50, 4)]
    [TestCase(2, 25, 5)]
    public void Ends_At_Some_Number_After_N_Moves(int seed, int rounds, int expectedPosition)
    {
        var random = new Random(seed);
        Console.WriteLine($"0: {bug.Position, 2}");
        for (var i = 0; i < rounds; i++)
        {
            if (random.Next(2) == 0)
            {
                bug.GoLeft();
                Console.WriteLine($"0: {bug.Position,2} - Left");
            }
            else
            {
                bug.GoRight();
                Console.WriteLine($"0: {bug.Position,2} - Right");
            }
        }
        Assert.That(bug.Position, Is.EqualTo(expectedPosition));
    }

    [TestCase(1, 41, 6)]
    [TestCase(2, 61, 2)]
    [TestCase(3, 80, 7)]
    public void Simulator_Finds_Final_Hour(int seed, int rounds, int expectedPosition)
    {
        var simulator = new BugSimulator(seed);
        simulator.RunCase();
        simulator.Runs[0].Log.Select((x, i) => (x, i)).ToList().ForEach(x => Console.WriteLine($"{x.i}: {x.x,2}"));
        Assert.That(
            simulator.Runs[0], Has
                .Property(nameof(ClockRun.Rounds)).EqualTo(rounds)
                .And
                .Property(nameof(ClockRun.FinalHour)).EqualTo(expectedPosition));
    }

    [TestCase(1, 10_000, "875, 962, 931, 942, 873, 924, 918, 873, 869, 914, 919")]
    [TestCase(1, 100_000, "9202, 9200, 9027, 9153, 9184, 9062, 8994, 9088, 8930, 9149, 9011")]
    [TestCase(2, 10_000, "935, 937, 892, 884, 872, 909, 924, 915, 916, 933, 883")]
    public void Simulator_Builds_Histogram(int seed, int cases, string expectedDistributionCsv)
    {
        var simulator = new BugSimulator(seed);

        simulator.RunCases(cases);

        var distribution = simulator.GetDistribution();

        foreach (var item in distribution)
        {
            Console.WriteLine($"{item.Key, 2}: {item.Value, 6}");
        }
        Console.WriteLine($"   {distribution.Values.Sum(), 7:# ##0}");

        Assert.Multiple(() =>
        {
            Assert.That(distribution.Values.Sum(), Is.EqualTo(cases));
            Assert.That(distribution.Values, Is.EquivalentTo(expectedDistributionCsv.Split(", ").Select(int.Parse)));
        });
    }

    private const double ExpectedChance = 1.0 / 11.0;

    [TestCase( 1,    10_000, 0.006  )]
    [TestCase( 1,   100_000, 0.002  )]
    [TestCase( 1,   500_000, 0.0007 )]
    [TestCase( 1, 1_000_000, 0.0007 )]
    [TestCase( 2,    10_000, 0.006  )]
    [TestCase( 2,   100_000, 0.002  )]
    [TestCase( 2,   500_000, 0.0012 )]
    [TestCase( 2, 1_000_000, 0.000603 )]
    public void Simulator_Finds_Equal_Chance(int seed, int cases, double tolerance)
    {
        var simulator = new BugSimulator(seed);
        simulator.RunCases(cases);
        var distribution = simulator.GetDistribution();

        Console.WriteLine("    Count  Freq     Dev");
        foreach (var item in distribution)
        {
            var itemFreq = (double)item.Value / cases;
            Console.WriteLine($"{item.Key,2}: {item.Value,6} {itemFreq,6:P4} {itemFreq - ExpectedChance,6:P4}");
        }

        var avg = distribution.Values.Select(x => (double)x / cases).ToArray();

        Console.WriteLine($"Avg/Max:   {avg.Average(),6:P4} {avg.Select(x => x - ExpectedChance).MaxBy(Math.Abs),6:P4}");

        Assert.Multiple(() =>
        {
            Assert.That(avg, Has.All.EqualTo(ExpectedChance).Within(tolerance));
            Assert.That(avg.Average(), Is.EqualTo(ExpectedChance).Within(tolerance));
        });
    }
}

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
        while(set.Count < 12)
        {
            Action move = random.Next(2) == 0 ? bug.GoLeft : bug.GoRight;
            move();
            set.Add(bug.Position);
            log.Add(bug.Position);
        }
        Runs.Add(new ClockRun(log.Count, bug.Position, set, log));
    }

    public Dictionary<int, int> GetDistribution()
    {
        return Runs
            .GroupBy(x => x.FinalHour)
            .OrderBy(g => g.Key)
            .ToDictionary(x => x.Key, x => x.Count());
    }
}