using LadybugMonteCarlo.Tests.OopLadybug;

namespace LadybugMonteCarlo.Tests;

[Parallelizable]
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

    [TestCase(1,  10_000,            "875, 962, 931, 942, 873, 924, 918, 873, 869, 914, 919")]
    [TestCase(2,  10_000,            "935, 937, 892, 884, 872, 909, 924, 915, 916, 933, 883")]
    [TestCase(1, 100_000, "9202, 9200, 9027, 9153, 9184, 9062, 8994, 9088, 8930, 9149, 9011")]
    public void Simulator_Builds_Histogram(int seed, int cases, string expectedDistributionCsv)
    {
        var simulator = new BugSimulator(seed);

        simulator.RunCases(cases);

        var dataset = simulator.GetDistribution();

        foreach (var item in dataset.Distribution)
        {
            Console.WriteLine($"{item.Key, 2}: {item.Value, 6}");
        }
        Console.WriteLine($"   {dataset.Cases, 7:# ##0}");

        Assert.Multiple(() =>
        {
            Assert.That(dataset.Cases, Is.EqualTo(cases));
            Assert.That(dataset.Values, Is.EquivalentTo(expectedDistributionCsv.Split(", ").Select(int.Parse)));
        });
    }

    private const double ExpectedChance = 1.0 / 11.0;

    [TestCase( 1,    10_000, 0.006    )]
    [TestCase( 2,    10_000, 0.006    )]
    [TestCase( 1,   100_000, 0.002    )]
    [TestCase( 2,   100_000, 0.002    )]
    [TestCase( 1,   500_000, 0.0007   )]
    [TestCase( 2,   500_000, 0.0012   )]
    [TestCase( 1, 1_000_000, 0.0007   )]
    [TestCase( 2, 1_000_000, 0.000603 )]
    public void Simulator_Finds_Equal_Chance(int seed, int cases, double tolerance)
    {
        var simulator = new BugSimulator(seed);
        simulator.RunCases(cases);
        var dataset = simulator.GetDistribution();

        Console.WriteLine("    Count  Freq     Dev");
        foreach (var item in dataset.Distribution)
        {
            var itemFreq = (double)item.Value / cases;
            Console.WriteLine($"{item.Key,2}: {item.Value,6} {itemFreq,6:P4} {itemFreq - ExpectedChance,6:P4}");
        }

        var maxDifference = dataset.MaxDifference(ExpectedChance);
        Console.WriteLine($"Avg/Max:   {dataset.AverageFrequency,6:P4} {maxDifference,6:P4}");

        Assert.Multiple(() =>
        {
            Assert.That(dataset.Frequencies.Values, Has.All.EqualTo(ExpectedChance).Within(tolerance));
            Assert.That(dataset.AverageFrequency, Is.EqualTo(ExpectedChance).Within(tolerance));
        });
    }
}

