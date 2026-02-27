namespace Feb.Bacteria.CS.Tests;

public class Bacteria_Life
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Origin_Evolves_To_North_And_East()
    {
        var grid = new bool[2, 2];
        grid[0, 0] = true;
        PrintGrid(0, grid);
        grid = Evolve(grid);
        PrintGrid(1, grid);

        Assert.That(grid, Is.EquivalentTo(new[,] { { false, true }, { true, false } }));
    }

    [Test]
    public void Two_By_Two_Clears_In_Three()
    {
        var grid = new bool[2, 2];
        grid[0, 0] = true;
        PrintGrid(0, grid);
        grid = Evolve(grid);
        PrintGrid(1, grid);
        Assert.That(grid, Has.Some.True, "Should have some true after 1");
        grid = Evolve(grid);
        PrintGrid(2, grid);
        Assert.That(grid, Has.Some.True, "Should have some true after 2");
        grid = Evolve(grid);
        PrintGrid(3, grid);
        Assert.That(grid, Has.All.False, "Should have all false after 3");
    }

    private void PrintGrid(int round, bool[,] grid)
    {
        Console.WriteLine($"{round}:");
        for (var y = grid.GetUpperBound(1); y >= 0 ; y--)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                Console.Write(grid[x, y] ? "X" : " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private bool[,] Evolve(bool[,] grid)
    {
        var newGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
        var width = grid.GetUpperBound(0);
        var height = grid.GetUpperBound(1);
        for (var x = 0; x <= width; x++)
        {
            for (var y = 0; y <= height; y++)
            {
                if (!grid[x, y]) continue;

                var northIsFree = y == height || !grid[x, y + 1];
                var eastIsFree = x == width || !grid[x + 1, y];
                var evolve = northIsFree && eastIsFree;
                if (evolve)
                {
                    if (x < width) newGrid[x + 1, y] = true;
                    if (y < height) newGrid[x, y + 1] = true;
                }
                else
                {
                    newGrid[x, y] = true;
                }
            }
        }
        return newGrid;
    }
}
