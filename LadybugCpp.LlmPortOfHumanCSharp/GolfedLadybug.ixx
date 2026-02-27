module;
#include <vector>
#include <map>
#include <random>

export module GolfedLadybug;

export struct GolfDataset
{
    std::map<int, int> Distribution;
    std::map<int, double> Frequencies;
    double AverageFrequency;

    GolfDataset();
    explicit GolfDataset(const std::vector<int>& results);
};

export std::vector<int> MakeRuns(int seed, int cases);
export GolfDataset FromResults(const std::vector<int>& results);

// --- implementation ---

static int MakeClockRun(std::mt19937& rng)
{
    std::uniform_int_distribution<int> dist(0, 1);
    int location = 0;
    int visitedCount = 1;
    bool visited[12] = { true, false, false, false, false, false, false, false, false, false, false, false };

    while (visitedCount < 12)
    {
        int move = dist(rng) == 0 ? -1 : 1;
        location = (location + move + 12) % 12;
        if (visited[location]) continue;
        visited[location] = true;
        visitedCount++;
    }

    return location;
}

std::vector<int> MakeRuns(int seed, int cases)
{
    std::mt19937 rng(seed);
    std::vector<int> results(cases);
    for (int i = 0; i < cases; i++)
        results[i] = MakeClockRun(rng);
    return results;
}

GolfDataset::GolfDataset() : AverageFrequency(0.0) {}

GolfDataset::GolfDataset(const std::vector<int>& results) : AverageFrequency(0.0)
{
    for (int val : results)
        Distribution[val]++;

    for (const auto& kv : Distribution)
        Frequencies[kv.first] = (double)kv.second / (double)results.size();

    double sum = 0.0;
    for (const auto& kv : Frequencies)
        sum += kv.second;
    AverageFrequency = sum / (double)Frequencies.size();
}

GolfDataset FromResults(const std::vector<int>& results)
{
    return GolfDataset(results);
}
